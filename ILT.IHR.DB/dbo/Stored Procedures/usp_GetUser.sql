--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 05/25/2020
-- Description : Select SP for User
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetUser]
	@UserID	int	=	NULL,
	@EmployeeID	int	=	NULL,
	@FirstName	varchar(50)	=	NULL,
	@LastName	varchar(50)	=	NULL,
	@RoleID	int	=	NULL,
	@Email	varchar(50)	=	NULL,
	@Password	varchar(50)	=	NULL,
	@IsOAuth	bit	=	NULL,
	@IsActive	bit	=	NULL,
	@CompanyID	int =	NULL,
	@CreatedBy	varchar(50)	=	NULL,
	@CreatedDate	datetime	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate	datetime	=	NULL,
	@TimeStamp	timestamp	=	NULL
AS 
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
 
	SELECT 
		U.UserID,
		U.EmployeeID,
		E.EmployeeCode,
		ISNULL(E.FirstName, U.FirstName) AS FirstName,
		ISNULL(E.LastName, U.LastName) AS LastName,
		U.Email,	
		STUFF(
		(SELECT ', ' + R.RoleShort FROM dbo.UserRole UR
		JOIN dbo.Role R ON UR.RoleID = R.RoleID
		WHERE UR.UserID = U.UserID
		ORDER BY R.RoleName
        FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 1, '') AS RoleShort,
		STUFF(
		(SELECT ', ' + R.RoleName FROM dbo.UserRole UR
		JOIN dbo.Role R ON UR.RoleID = R.RoleID
		WHERE UR.UserID = U.UserID
		ORDER BY R.RoleName
        FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 1, '') AS RoleName,
		U.Password,
		U.IsOAuth,
		U.IsActive,
		U.CompanyID,
		C.Name AS CompanyName,
		U.CreatedBy,
		U.CreatedDate,
		U.ModifiedBy,
		U.ModifiedDate,
		U.TimeStamp
	FROM [dbo].[User] U
	LEFT JOIN dbo.Employee E ON U.EmployeeID = E.EmployeeID
	LEFT JOIN dbo.Company C ON U.CompanyID = C.CompanyID
	WHERE U.UserID = ISNULL(@UserID,U.UserID) AND ISNULL(E.EmployeeID,0) = COALESCE(@EmployeeID, U.EmployeeID,0)
	AND U.Email = ISNULL(@Email,U.Email)			
	
	IF (ISNULL(@UserID, 0) = 0 AND @Email IS NOT NULL)
		SELECT @UserID = U.UserID FROM [dbo].[User] U
		WHERE U.Email = ISNULL(@Email,U.Email)

	IF ISNULL(@UserID, 0) <> 0
		EXEC [dbo].usp_GetUserRole @UserID = @UserID

	IF ISNULL(@UserID, 0) <> 0
		SELECT 
		RP.RolePermissionID,
		RP.RoleID,
		R.RoleShort,
		R.RoleName,
		RP.ModuleID,
		M.ModuleShort,
		M.ModuleName,
		RP.[View],
		RP.[Add],
		RP.[Update],
		RP.[Delete],
		RP.CreatedBy,
		RP.CreatedDate,
		RP.ModifiedBy,
		RP.ModifiedDate,
		RP.TimeStamp
	FROM [dbo].[RolePermission] RP
	INNER JOIN [UserRole] UR on UR.UserID = @UserID AND UR.RoleID = RP.RoleId
	INNER JOIN Module M on M.ModuleID = RP.ModuleID 
	INNER JOIN [Role] R on R.RoleID = UR.RoleID	
END
