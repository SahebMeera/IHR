--===============================================================
-- Author : Nimesh Patel
-- Created Date : 04/30/2021
-- Description : Select SP for LWP
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
CREATE Procedure [dbo].[usp_GetLeavesCount]
@startDate smalldatetime,
@endDate smalldatetime,
@country varchar(50)
AS 
BEGIN
		--Declare @startDate date = '2021-01-01'
		--Declare @endDate date = '2022-06-22'
		--Declare @country varchar(50) = 'India'
		
		DECLARE @EmploymentTypeID int

		SELECT @EmploymentTypeID = ListValueID FROM ListValue LV
		INNER JOIN ListType LT ON LV.ListTypeID = Lt.ListTypeID
		WHERE LT.Type = 'EmploymentType' AND LV.Value = 'FTE'

		IF OBJECT_ID(N'tempdb..#tmpLB') IS NOT NULL
                     BEGIN
                     DROP TABLE #tmpLB
                     END

              CREATE TABLE #tmpLB
              (
                     EmployeeId int,
                     VacationTotal numeric(18,2),
                     VacationUsed numeric(18,2),
                     LWPAccounted numeric(18,2),
                     LWPPending numeric(18,2),
                     LeaveInRange numeric(18,2),
					 LeaveInNextMonth numeric(18,2),
					 VacationBalance numeric(18,2),
					 EncashedLeave numeric(18,2)
              )
              INSERT INTO #tmpLB 
              SELECT LB.EmployeeID, LB.VacationTotal, LB.VacationUsed, 0,0,0,0,0, EncashedLeave from LeaveBalance LB
              INNER JOIN Employee E ON LB.EmployeeID = E.EmployeeID
              WHERE E.Country = @country AND LB.LeaveYear = YEAR(@startDate)
			  AND E.EmploymentTypeID = @EmploymentTypeID
			  
              ;WITH CTE AS
              (
				SELECT L.EmployeeID, Sum(L.Duration) AS LWPCount FROM Leave L 
				INNER JOIN dbo.ListValue LV ON L.LeaveTypeID = LV.ListValueID
				INNER JOIN Employee E ON L.EmployeeID = E.EmployeeID
				WHERE LV.[Value] = 'LWP' AND E.Country = @country 
				AND YEAR(L.StartDate) = YEAR(@startDate)
				GROUP BY L.EmployeeID
              )
              UPDATE TLB
              SET TLB.LWPAccounted = LWPCount
              FROM #tmpLB TLB JOIN CTE ON CTE.EmployeeID = TLB.EmployeeId

              ;WITH CTE AS
              (
				  SELECT L.EmployeeId, L.Duration,L.IncludesHalfDay,
				  CASE 
				  WHEN (L.StartDate >= @startDate AND L.EndDate <= @endDate) THEN
					dbo.LeaveDays(L.StartDate, L.EndDate, @country)
				  WHEN (L.StartDate >= @startDate AND L.EndDate > @endDate) THEN
					dbo.LeaveDays(L.StartDate, @endDate, @country)
				  WHEN (L.StartDate < @startDate AND L.EndDate <= @endDate) THEN
					dbo.LeaveDays(@startDate, L.EndDate, @country)
				  WHEN (L.StartDate < @startDate AND L.EndDate > @endDate) THEN 
					dbo.LeaveDays(@startDate, @endDate, @country)
				  ELSE
					0
				  END AS LeavesDays, 
				  CASE WHEN L.IncludesHalfDay = 1 AND L.StartDate >= @startDate THEN 0.5 ELSE 0 END AS HalfDays, 
				  0 AS TotalTakenLeaves
				  FROM Leave L 
				  INNER JOIN dbo.ListValue LV1 ON L.StatusID = LV1.ListValueID
				  INNER JOIN dbo.Employee E ON E.EmployeeID = L.EmployeeID
				  LEFT OUTER JOIN dbo.ListValue lV2 ON L.LeaveTypeID = LV2.ListValueID
				  WHERE 
				  LV1.[Value] IN ('PENDING','APPROVED') and LV2.[Value] <> 'LWP' and E.Country = @country
				  AND ((L.StartDate <= @endDate and @startDate <= L.EndDate) 
				  OR  (L.StartDate >= @startDate and L.EndDate <= @endDate))
              )
              UPDATE TLB 
              SET TLB.LeaveInRange = ISNULL(B.Duration,0) - ISNULL(B.HalfDays,0),
			  TLB.VacationBalance = ISNULL(TLB.VacationTotal,0) - ISNULL(TLB.VacationUsed,0) + ISNULL(TLB.LWPAccounted,0) - ISNULL(TLB. EncashedLeave,0)
              FROM #tmpLB TLB LEFT JOIN
			  (Select CTE.EmployeeID, Sum(CTE.LeavesDays) as Duration, SUM(CTE.HalfDays) AS HalfDays from CTE Group by CTE.EmployeeID) B
			  on TLB.EmployeeId = B.EmployeeID

			  ;WITH CTE AS
              (
				  SELECT L.EmployeeId, L.Duration,L.IncludesHalfDay,
				  CASE WHEN (L.StartDate <= @endDate AND @startDate <= L.EndDate AND L.EndDate > @endDate) THEN
					dbo.LeaveDays(DATEADD(day, 1, @endDate), L.EndDate, @country)
				  WHEN L.StartDate > @endDate THEN
					dbo.LeaveDays(L.StartDate, L.EndDate, @country) 
				  END AS LeavesDays,
				  CASE WHEN L.IncludesHalfDay = 1 AND L.StartDate >= @endDate THEN 0.5 ELSE 0 END AS HalfDays, 
				  0 AS TotalTakenLeaves
				  FROM Leave L 
				  INNER JOIN dbo.ListValue LV1 ON L.StatusID = LV1.ListValueID
				  INNER JOIN dbo.Employee E ON E.EmployeeID = L.EmployeeID
				  LEFT OUTER JOIN dbo.ListValue lV2 ON L.LeaveTypeID = LV2.ListValueID
				  WHERE 
				  LV1.[Value] IN ('PENDING','APPROVED') and LV2.[Value] <> 'LWP' and E.Country = @country
				  --AND ((L.StartDate <= @endDate and @startDate <= L.EndDate AND L.EndDate > @endDate)
				  --OR (L.StartDate > @endDate))
				  AND L.EndDate > @endDate
              )
              UPDATE TLB 
              SET TLB.LeaveInNextMonth = ISNULL(B.Duration,0) - ISNULL(B.HalfDays,0)
              FROM #tmpLB TLB JOIN
			  (Select CTE.EmployeeID, Sum(CTE.LeavesDays) as Duration, SUM(CTE.HalfDays) AS HalfDays from CTE Group by CTE.EmployeeID) B
			  on TLB.EmployeeId = B.EmployeeID

              UPDATE #tmpLB SET LWPPending = CASE WHEN VacationTotal > (VacationUsed - LWPAccounted - LeaveInNextMonth) THEN 0 
              ELSE ABS(VacationTotal + LWPAccounted - (VacationUsed - LeaveInNextMonth)) END

              SELECT T.EmployeeId,E.EmployeeCode, E.FirstName, E.LastName, E.FirstName + ' ' + E.LastName AS EmployeeName,E.TermDate,T.VacationTotal, T.VacationUsed,
			  T.LWPAccounted, T.LeaveInRange, T.LWPPending, LeaveInNextMonth, VacationBalance FROM #tmpLB T
			  inner join dbo.Employee E on E.EmployeeID = T.EmployeeId
			  --WHERE LeaveInRange>0 OR LeaveInNextMonth >0
			  ORDER BY E.FirstName, E.LastName

              IF OBJECT_ID(N'tempdb..#tmpLB') IS NOT NULL
                     BEGIN
                     DROP TABLE #tmpLB
                     END
              
END