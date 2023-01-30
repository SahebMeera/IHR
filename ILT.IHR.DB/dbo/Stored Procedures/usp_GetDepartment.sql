--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 05/24/2020
-- Description : Select SP for Department
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetDepartment]
	@DepartmentID	int	=	NULL,
	@DeptCode	varchar(10)	=	NULL,
	@DeptName	varchar(50)	=	NULL,
	@DeptLocationID	int	=	NULL,
	@IsActive	bit	=	NULL,
	@CreatedBy	varchar(50)	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL
AS 
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;       
	SET NOCOUNT ON; 

	SELECT 
		D.DepartmentID,
		D.DeptCode,
		D.DeptName,
		D.DeptLocationID,
		C.CountryDesc AS DeptLocation,
		D.IsActive,
		D.CreatedBy,
		D.CreatedDate,
		D.ModifiedBy,
		D.ModifiedDate,
		D.TimeStamp
		-- E.FirstName + ' ' + E.LastName AS EmployeeName
	FROM [dbo].[Department] D
	JOIN [dbo].[Country] C ON D.DeptLocationID = C.CountryID
	-- INNER JOIN dbo.Employee E ON D.DepartmentID = E.DepartmentID
	WHERE D.DepartmentID = ISNULL(@DepartmentID, D.DepartmentID)
	ORDER BY D.DeptName

END
