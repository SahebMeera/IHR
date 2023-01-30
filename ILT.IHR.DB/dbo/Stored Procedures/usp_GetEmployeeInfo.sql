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
 
CREATE PROCEDURE [dbo].[usp_GetEmployeeInfo]
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
	distinct
	E.EmployeeCode,
	REPLACE(E.FirstName + ' ' + ISNULL(E.MiddleName,'') + ' ' + E.LastName, ' ', ' ') AS 'EmployeeName' ,
	E.FirstName, 
	M.FirstName,
	E.BirthDate,
	E.HireDate,
	STUFF((
         SELECT ',' + Skill
            FROM EmployeeSkill ESK Where ESK.EmployeeID = E.EmployeeID  
            FOR XML PATH('')
         ), 1, 1, '') AS Skill,
	LV.ValueDesc as Title,
	E.Phone, E.Email, WA.ValueDesc AS WorkAuthorization,
	D.DeptName AS Department,
	E.Country,
	REPLACE(M.FirstName + ' ' + ISNULL(M.MiddleName,'') + ' ' + M.LastName,' ', ' ') AS 'Manager',
	E.EmploymentTypeID,
    ET.ValueDesc AS EmploymentType,
	E.TermDate,
	C.Name as Client,
	EC.Name AS EndClient,
	A.StartDate
	FROM Employee E
	LEFT JOIN Employee M on M.EmployeeID = E.ManagerID
	LEFT JOIN Department D on E.DepartmentID = D.DepartmentID
	LEFT JOIN ListValue LV on E.TitleID = LV.ListValueID
	LEFT JOIN ListValue WA ON E.WorkAuthorizationID = WA.ListValueID
	LEFT JOIN ListValue ET ON E.EmploymentTypeID = ET.ListValueID
	LEFT JOIN Assignment A on A.EmployeeID = E.EmployeeID
	LEFT JOIN Company C ON A.ClientID = C.CompanyID
	LEFT JOIN EndClient EC on A.EndClientID = EC.EndClientID
	LEFT JOIN EmployeeSkill ES ON ES.EmployeeID = E.EmployeeID
	WHERE E.IsDeleted = 0
	AND A.EndDate IS NULL
	--AND E.Country <> 'India'
	order by E.Country, D.DeptName,E.FirstName, M.FirstName


END