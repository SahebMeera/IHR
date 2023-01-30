--===============================================================
-- Author : Mihir Hapaliya
-- Created Date : 05/23/2020
-- Description : Delete SP for Role Permission
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By			Description
-- 11/19/2020	Mihir Hapaliya	updated to delete query from update query
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_DeleteRolePermission]
	@RolePermissionID	int
AS 
BEGIN
	DELETE FROM  [dbo].[RolePermission]
	WHERE RolePermissionID = @RolePermissionID
END
