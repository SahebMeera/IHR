--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 05/23/2020
-- Description : Select SP for Employee
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--11/30/2020	Mihir Hapaliya	returning contact dataset related to employee
--02/11/2020	Mihir Hapaliya	returning employee address dataset related to employee
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetEmployee]
	@EmployeeID	int	=	NULL,
	@EmployeeCode	varchar(12)	=	NULL,
	@FirstName	varchar(50)	=	NULL,
	@MiddleName	varchar(50)	=	NULL,
	@LastName	varchar(50)	=	NULL,
	@Country	varchar(50)	=	NULL,
	@TitleID	int	=	NULL,
	@GenderID	int	=	NULL,
	@DepartmentID	int	=	NULL,
	@Phone	varchar(10)	=	NULL,
	@HomePhone	varchar(10)	=	NULL,
	@WorkPhone	varchar(10)	=	NULL,
	@Email	varchar(50)	=	NULL,
	@WorkEmail	varchar(50)	=	NULL,
	@BirthDate	date	=	NULL,
	@HireDate	date	=	NULL,
	@TermDate	date	=	NULL,
	@WorkAuthorizationID	int	=	NULL,
	@SSN varchar(9)    =      NULL,
	@PAN varchar(10)    =      NULL,
	@AadharNumber varchar(12)    =      NULL,
	@Salary	int	=	NULL,
	@Allowances	int	=	NULL,
	@MaritalStatusID	int	=	NULL,
	@WithHoldingStatusID	int	=	NULL,
	@ManagerID int =	NULL,
	@IsDeleted	bit	=	NULL,
	@CreatedBy	varchar(50)	=	NULL,
	@CreatedDate	datetime	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate	datetime	=	NULL,
	@TimeStamp	timestamp	=	NULL
AS 
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;       
	SET NOCOUNT ON; 
	
	SELECT 
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
		U.Email AS LoginEmail,
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
		ISNULL(UM.Email,M.Email) AS ManagerEmail,
		E.EmploymentTypeID,
		ET.ValueDesc AS EmploymentType,
		E.IsDeleted,
		E.CreatedBy,
		E.CreatedDate,
		E.ModifiedBy,
		E.ModifiedDate,
		E.TimeStamp
	FROM dbo.Employee E
	LEFT JOIN dbo.Department D ON E.DepartmentID = D.DepartmentID
	LEFT JOIN dbo.ListValue TIT ON E.TitleID = TIT.ListValueID 
	LEFT JOIN dbo.ListValue GEN ON E.GenderID = GEN.ListValueID
	LEFT JOIN dbo.ListValue WAT ON E.WorkAuthorizationID = WAT.ListValueID
	LEFT JOIN dbo.ListValue MST ON E.MaritalStatusID = MST.ListValueID
	LEFT JOIN dbo.ListValue ET ON E.EmploymentTypeID = ET.ListValueID 
	LEFT JOIN dbo.Employee M ON E.ManagerID = M.EmployeeID
	LEFT JOIN dbo.[User] UM ON E.ManagerID = UM.EmployeeID
	LEFT JOIN dbo.[User] U ON E.EmployeeID = U.EmployeeID
	WHERE E.EmployeeID = ISNULL(@EmployeeID, E.EmployeeID)
	AND E.IsDeleted = 0
	ORDER BY E.FirstName, E.LastName

	--Do not delete below 4 procedure calls
	IF(ISNULL(@EmployeeID,0) <> 0)
	EXEC [dbo].usp_GetDependent @EmployeeID = @EmployeeID

	IF(ISNULL(@EmployeeID,0) <> 0)
	EXEC [dbo].usp_GetDirectDeposit @EmployeeID = @EmployeeID

	IF(ISNULL(@EmployeeID,0) <> 0)
	EXEC [dbo].usp_GetAssignment @EmployeeID = @EmployeeID

	IF(ISNULL(@EmployeeID,0) <> 0)
	EXEC [dbo].usp_GetContact  @EmployeeID = @EmployeeID

	IF(ISNULL(@EmployeeID,0) <> 0)
	EXEC [dbo].usp_GetEmployeeAddress @EmployeeID = @EmployeeID

	IF(ISNULL(@EmployeeID,0) <> 0)
	EXEC [dbo].usp_GetSalary @EmployeeID = @EmployeeID


END
