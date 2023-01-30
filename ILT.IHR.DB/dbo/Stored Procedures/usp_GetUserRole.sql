--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 02/05/2021
-- Description : Select SP for Role
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetUserRole]
	@UserRoleID	int	=	NULL,
	@UserID	int	=	NULL,
	@RoleID	int	=	NULL
AS 
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
 
	SELECT 
		UR.UserRoleID,
		UR.UserID,
		UR.RoleID,
		R.RoleShort,
		R.RoleName,
		UR.IsDefault,
		UR.CreatedBy,
		UR.CreatedDate,
		UR.ModifiedBy,
		UR.ModifiedDate,
		UR.TimeStamp
	FROM [dbo].[UserRole] UR
	JOIN [dbo].[Role] R ON UR.RoleID = R.RoleID
	WHERE UR.UserID = ISNULL(@UserID,UR.UserID) 

END