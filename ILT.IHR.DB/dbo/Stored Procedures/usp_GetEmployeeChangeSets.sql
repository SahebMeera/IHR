--===============================================================
-- Author : Mihir Hapaliya	
-- Created Date : 12/18/2020
-- Description : Get SP for Employee ChangeSet
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetEmployeeChangeSets]
	@EmployeeID int = NULL,
	@UserID int = NULL
AS 
BEGIN
	SET NOCOUNT ON;  
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	;WITH CTE AS
	(
	SELECT  EC.*, null as TimeStamp from [dbo].[EmployeeChangeSet] EC LEFT JOIN
	[dbo].[Notification] N on EC.EmployeeID = N.RecordID AND N.TableName = 'Employee' AND N.IsAck = 0 AND N.UserID = @UserID
	AND EC.EmployeeChangeSetID = N.ChangeSetID
	WHERE EmployeeID = @EmployeeID 
	UNION 
	SELECT 0 AS EmployeeChangeSetID, E.* FROM Employee E WHERE E.EmployeeID = @EmployeeID
	)
	SELECT 
		E.EmployeeChangeSetID,
		E.EmployeeID,
		E.EmployeeCode,
		E.FirstName,
		E.MiddleName,
		E.LastName,
		E.FirstName + ' ' + E.LastName AS EmployeeName,
		E.Country,
		E.TitleID,
		TIT.ValueDesc AS Title,
		E.GenderID,
		GEN.ValueDesc AS Gender,
		E.DepartmentID,
		D.DeptName AS Department,
		E.Phone,
		E.HomePhone,
		E.WorkPhone,
		E.Email,
		E.WorkEmail,
		E.BirthDate,
		E.HireDate,
		E.TermDate,
		E.WorkAuthorizationID,
		WAT.ValueDesc AS WorkAuthorization,
		E.SSN,
		E.PAN,
		E.AadharNumber,
		E.Salary,
		E.VariablePay,
		E.MaritalStatusID,
		MST.ValueDesc AS MaritalStatus,
		E.ManagerID,
		M.FirstName + ' ' + M.LastName AS	Manager,
		E.EmploymentTypeID,
		ET.ValueDesc AS EmploymentType,
		E.IsDeleted,
		E.CreatedBy,
		E.CreatedDate,
		E.ModifiedBy,
		E.ModifiedDate,
		E.TimeStamp
	FROM CTE E
	LEFT JOIN dbo.Department D ON E.DepartmentID = D.DepartmentID
	LEFT JOIN dbo.ListValue TIT ON E.TitleID = TIT.ListValueID 
	LEFT JOIN dbo.ListValue GEN ON E.GenderID = GEN.ListValueID
	LEFT JOIN dbo.ListValue WAT ON E.WorkAuthorizationID = WAT.ListValueID
	LEFT JOIN dbo.ListValue MST ON E.MaritalStatusID = MST.ListValueID
	LEFT JOIN dbo.ListValue ET ON E.EmploymentTypeID = ET.ListValueID 
	LEFT JOIN dbo.Employee M ON E.ManagerID = M.EmployeeID 
	WHERE E.EmployeeID = @EmployeeID
	ORDER BY E.ModifiedDate DESC
END