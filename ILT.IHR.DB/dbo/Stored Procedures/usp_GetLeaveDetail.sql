--===============================================================  
-- Author : Sanjan Madishetti  
-- Created Date : 09/09/2021  
-- Description : Select SP for LeaveDetail
--  
-- Revision History:  
-----------------------------------------------------------------  
-- Date            By          Description  
--  exec usp_GetLeaveDetail '2022-01-01','2022-01-21','India'
--===============================================================  
CREATE Procedure [dbo].[usp_GetLeaveDetail]  
@startDate smalldatetime,  
@endDate smalldatetime,  
@country varchar(50)  
AS   
BEGIN  
  --Declare @startDate date = '2020-01-01'  
  --Declare @endDate date = '2022-06-20'
  --Declare @country varchar(50) = 'United States'  
    
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
					 StartDate date,
					 EndDate date,
					 IncludesHalfDay bit,
					 LeaveTypeID int,
					 Duration numeric(5,1),
					 LeaveInRange numeric(18,2),  
                     VacationTotal numeric(18,2),  
                     VacationUsed numeric(18,2),  
                     LWPAccounted numeric(18,2),  
                     LWPPending numeric(18,2),  
					 LeaveInNextMonth numeric(18,2),  
					 VacationBalance numeric(18,2),
					 LeaveDays numeric(18,2),
					 EncashedLeave numeric(18,2)
              )  
              INSERT INTO #tmpLB   
              SELECT L.EmployeeID, L.StartDate, L.EndDate,L.IncludesHalfDay,L.LeaveTypeID,L.Duration, 0,LB.VacationTotal,LB.VacationUsed,0,0,0,0,0, LB.EncashedLeave from Leave L
              INNER JOIN Employee E ON L.EmployeeID = E.EmployeeID 
			  INNER JOIN LeaveBalance LB ON L.EmployeeID = LB.EmployeeID AND LB.LeaveYear = YEAR(@endDate)
			  INNER JOIN dbo.ListValue LV1 ON L.StatusID = LV1.ListValueID
              WHERE E.Country = @country AND ((L.StartDate <= @endDate and @startDate <= L.EndDate) 
				  OR  (L.StartDate >= @startDate and L.EndDate <= @endDate))
			  AND LV1.[Value] IN ('PENDING','APPROVED')
			  
     --AND E.EmploymentTypeID = @EmploymentTypeID  
       
              ;WITH CTE AS  
              (  
					SELECT L.EmployeeID, Sum(L.Duration) AS LWPCount FROM Leave L   
					INNER JOIN dbo.ListValue LV ON L.LeaveTypeID = LV.ListValueID  
					INNER JOIN Employee E ON L.EmployeeID = E.EmployeeID  
					WHERE LV.[Value] = 'LWP' AND E.Country = @country
					--WHERE  E.Country = @country   
					AND YEAR(L.StartDate) = YEAR(@startDate)  
					GROUP BY L.EmployeeID  
              )  
              UPDATE TLB  
              SET TLB.LWPAccounted = LWPCount  
              FROM #tmpLB TLB JOIN CTE ON CTE.EmployeeID = TLB.EmployeeId
              ;WITH CTE AS  
              (  
      SELECT L.EmployeeId, L.Duration,L.IncludesHalfDay,L.LeaveTypeID,  
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
      0 AS TotalTakenLeaves  ,
	  L.StartDate
      FROM Leave L   
      INNER JOIN dbo.ListValue LV1 ON L.StatusID = LV1.ListValueID  
      INNER JOIN dbo.Employee E ON E.EmployeeID = L.EmployeeID  
      LEFT OUTER JOIN dbo.ListValue lV2 ON L.LeaveTypeID = LV2.ListValueID  
      WHERE 
	  --LV1.[Value] IN ('PENDING','APPROVED') and LV2.[Value] <> 'LWP' and E.Country = @country  
      LV1.[Value] IN ('PENDING','APPROVED')  and E.Country = @country  
      AND ((L.StartDate <= @endDate and @startDate <= L.EndDate)   
      OR  (L.StartDate >= @startDate and L.EndDate <= @endDate))  
              )  
              UPDATE TLB   
              SET TLB.LeaveInRange = ISNULL(B.Duration,0) - ISNULL(B.HalfDays,0),  
			  TLB.VacationBalance = ISNULL(TLB.VacationTotal,0) - ISNULL(TLB.VacationUsed,0) + ISNULL(TLB.LWPAccounted,0) - ISNULL(TLB.EncashedLeave,0) 
              FROM #tmpLB TLB LEFT JOIN  
			  (SELECT CTE.EmployeeID, CTE.LeavesDays as Duration, CTE.HalfDays AS HalfDays, CTE.StartDate,CTE.LeaveTypeID FROM CTE ) B  
			  ON TLB.EmployeeId = B.EmployeeID 
			  WHERE TLB.StartDate = B.StartDate  AND TLB.LeaveTypeID = B.LeaveTypeID

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
			   UPDATE #tmpLB SET StartDate = CASE WHEN @StartDate >= StartDate THEN @startDate 
			   ELSE	 StartDate END  
			  UPDATE #tmpLB SET EndDate = CASE WHEN LeaveInNextMonth > 0 AND EndDate >= @endDate THEN @endDate 
			   ELSE	 EndDate END  
			  UPDATE #tmpLB SET VacationBalance = CASE WHEN LeaveInNextMonth > 0 THEN VacationBalance + LeaveInNextMonth
				ELSE VacationBalance END
			
              SELECT  ROW_NUMBER() OVER(PARTITION BY E.EmployeeCode ORDER BY T.StartDate) AS RowNumber,E.EmployeeCode, E.FirstName + ' ' + ISNull(E.MiddleName,'') + ' ' + E.LastName AS EmployeeName,E.TermDate, T.StartDate, T.EndDate,T.Duration,T.VacationTotal,  
      LV.ValueDesc AS LeaveType, T.LeaveInRange , T.VacationBalance FROM #tmpLB T  
     inner join dbo.Employee E on E.EmployeeID = T.EmployeeId 
	 LEFT JOIN dbo.ListValue LV on LV.ListValueID = T.LeaveTypeID
     WHERE LeaveInRange >0 --OR LeaveInNextMonth >0  
     ORDER BY E.FirstName, E.LastName , StartDate
  
              IF OBJECT_ID(N'tempdb..#tmpLB') IS NOT NULL  
                     BEGIN  
                     DROP TABLE #tmpLB  
                     END  
                 
END