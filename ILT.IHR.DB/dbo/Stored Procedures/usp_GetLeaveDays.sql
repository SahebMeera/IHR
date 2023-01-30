--===============================================================
-- Author : Nimesh Patel
-- Created Date : 05/30/2020
-- Description : Select SP for Unpaid leave duration
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
CREATE Procedure [dbo].[usp_GetLeaveDays]
@employeeID int,
@startDate Date,
@endDate Date,
@IncludesHalfDay bit = 0
as
BEGIN
Declare @Duration numeric(18,2)
Declare @EmpName varchar(100)
SELECT @Duration = dbo.LeaveDays(@startDate, @endDate, E.Country),
@EmpName = E.FirstName + ' ' + E.LastName FROM Employee E
WHERE E.EmployeeID = @employeeID
	IF (@IncludesHalfDay = 1 AND @Duration > 0) 
		SET @Duration = @Duration - 0.5

Select @employeeID as EmployeeId, @EmpName as EmployeeName, 
@startDate as StartDate, @endDate as EndDate,
@Duration as Duration

END