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
CREATE Procedure dbo.usp_GetDuration
@employeeID int,
@startDate Date,
@endDate Date,
@IncludesHalfDay bit
as
BEGIN
Declare @Duration numeric(18,2)
SELECT @Duration = dbo.LeaveDays(@startDate, @endDate, E.Country)  FROM Employee E
WHERE E.EmployeeID = @employeeID
	IF (@IncludesHalfDay = 1 AND @Duration > 0) 
		SET @Duration = @Duration - 0.5


Select @Duration as Duraction
END