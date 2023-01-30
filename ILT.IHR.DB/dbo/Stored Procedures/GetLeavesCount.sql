CREATE Procedure dbo.GetLeavesCount
@startDate smalldatetime,
@endDate smalldatetime,
@country varchar(50)
as 
BEGIN
		SET NOCOUNT ON;
		SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
		IF OBJECT_ID(N'tempdb..#tmpLeaves') IS NOT NULL
		
		BEGIN
		DROP TABLE #tmpLeaves
		END
		Create Table #tmpLeaves
		(
			LeaveId int,
			EmployeeId int,
			Duration numeric,
			LeavesDays numeric,
			TotalTakenLeaves numeric,
		)

		IF OBJECT_ID(N'tempdb..#tmpFromStartLeaves') IS NOT NULL
		BEGIN
		DROP TABLE #tmpFromStartLeaves
		END
		Create Table #tmpFromStartLeaves
		(
			LeaveId int,
			EmployeeId int,
			Duration numeric,
			TotalLeaves numeric,
		)

		--Declare @startDate date = '2021-04-29'
		--Declare @endDate date = '2021-05-29'
		--Declare @country varchar(50) = 'India'
		Declare @initialDate date = DATEFROMPARTS(YEAR(@startDate), 1, 1) 
		
		insert into #tmpLeaves
		select LeaveId, EmployeeId, Duration,
		dbo.LeaveDays(startDate, endDate, @country) as LeavesDays, 0 as TotalTakenLeaves
		from Leave L
		INNER JOIN dbo.ListValue LV1 on L.StatusID = LV1.ListValueID
		where 
		--EmployeeId in (23) and
		LV1.Value='Approved' 
		and (StartDate >= @startDate and StartDate <= @endDate) and EndDate <= @endDate
		union
		select LeaveId,EmployeeId,Duration,
		dbo.LeaveDays(startDate, @endDate, @country) as LeavesDays , 0 as TotalTakenLeaves
		from Leave L
		INNER JOIN dbo.ListValue LV1 on L.StatusID = LV1.ListValueID
		where 
		-- EmployeeId in (2) and
		 LV1.Value='Approved' 
		 and (StartDate >= @startDate and StartDate <= @endDate) and EndDate > @endDate

		 --- From InitialDaysofYear

		insert into #tmpFromStartLeaves
		select LeaveId, EmployeeId, Duration,
		dbo.LeaveDays(startDate, endDate, @country) as TotalLeaves
		from Leave L
		INNER JOIN dbo.ListValue LV1 on L.StatusID = LV1.ListValueID
		where 
		--EmployeeId in (23) and
		LV1.Value='Approved' 
		and (StartDate >= @initialDate and StartDate <= @endDate) and EndDate <= @endDate
		union
		select LeaveId,EmployeeId,Duration,
		dbo.LeaveDays(startDate, @endDate, @country) as TotalLeaves
		from Leave L
		INNER JOIN dbo.ListValue LV1 on L.StatusID = LV1.ListValueID
		where 
		-- EmployeeId in (2) and
		 LV1.Value='Approved' 
		 and (StartDate >= @initialDate and StartDate <= @endDate) and EndDate > @endDate

		
		 Select t.EmployeeId, E.FirstName, E.LastName, SUM(t.LeavesDays) as RangeLeaves,
		 LB.VacationTotal, LB.VacationUsed,  0 as TotalLeaves
		 into #tmpLeavesSummary  from #tmpLeaves t
		 INNER JOIN dbo.Employee E on t.EmployeeID = E.EmployeeID
		 Inner Join dbo.LeaveBalance LB on t.EmployeeId = LB.EmployeeID
		 Group by t.EmployeeId, E.FirstName, E.LastName, LB.VacationTotal, LB.VacationUsed

		 Select t.EmployeeId, SUM(t.TotalLeaves) as TotalLeaves
		 into #tmpFromStartLeavesSummary
		 from #tmpFromStartLeaves t
		 Group by t.EmployeeId

		 Update s set TotalLeaves = t.TotalLeaves
		 From #tmpFromStartLeavesSummary t 
		 INNER JOIN #tmpLeavesSummary s on s.EmployeeId= t.EmployeeId

		 select EmployeeId,FirstName, LastName, VacationTotal, VacationUsed, TotalLeaves,
		 case when  (VacationTotal - TotalLeaves) > 0 
		 then 0 
		 Else (VacationTotal - TotalLeaves) * -1 End as LWP  from #tmpLeavesSummary
		 

		 IF OBJECT_ID(N'tempdb..#tmpLeavesSummary') IS NOT NULL
			BEGIN
			DROP TABLE #tmpLeavesSummary
			END

		 IF OBJECT_ID(N'tempdb..#tmpFromStartLeavesSummary') IS NOT NULL
			BEGIN
			DROP TABLE #tmpFromStartLeavesSummary
			END
END