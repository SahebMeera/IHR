--===============================================================
-- Author : Mihir Hapaliya
-- Created Date : 02/08/2020
-- Description : Delete SP for User Role
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
 CREATE PROCEDURE [dbo].[usp_DeleteUserRole]
	@UserRoleID	int = NULL,
	@UserID	int = NULL,
	@RoleID int = NULL
AS 
BEGIN
	DELETE FROM  [dbo].[UserRole]
	WHERE UserID = @UserID AND RoleID = @RoleID
END