--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 05/25/2020
-- Description : Select SP for Role
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetRole]
	@RoleID	int	=	NULL,
	@RoleShort	varchar(20)	=	NULL,
	@RoleName	varchar(50)	=	NULL,
	@CreatedBy	varchar(50)	=	NULL,
	@CreatedDate	datetime	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate	datetime	=	NULL
AS 
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
 
	SELECT 
		RoleID,
		RoleShort,
		RoleName,
		CreatedBy,
		CreatedDate,
		ModifiedBy,
		ModifiedDate,
		TimeStamp
	FROM [dbo].[Role] R
	WHERE R.RoleID = ISNULL(@RoleID,RoleID)
	ORDER BY RoleName

	 IF(ISNULL(@RoleID,0) <> 0)
	EXEC [dbo].usp_GetRolePermission @RoleID = @RoleID
END
