--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 05/23/2020
-- Description : Delete SP for Employee
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_DeleteEmployee]
	@EmployeeID	int,
	@ModifiedBy	varchar(50)
AS 
BEGIN
	UPDATE [dbo].[Employee]
	SET IsDeleted = 1,
	ModifiedBy = @ModifiedBy,
	ModifiedDate = GETDATE()
	WHERE EmployeeID = @EmployeeID
END
