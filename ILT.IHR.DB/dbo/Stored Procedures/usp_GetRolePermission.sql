--===============================================================
-- Author : Mihir Hapaliya
-- Created Date : 05/25/2020
-- Description : Select SP for Role PErmission
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetRolePermission]
	@RolePermissionID int =  NULL,
	@RoleID	int	=	NULL,
	@ModuleID	int	=	NULL,
	@View	bit	=	0,
	@Add	bit	=	0,
	@Update	bit	=	0,
	@Delete	bit	=	0,
	@CreatedBy	varchar(50)	=	NULL,
	@CreatedDate	datetime	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate	datetime	=	NULL
AS 
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;       
	SET NOCOUNT ON; 

	INSERT [dbo].[RolePermission] ([RoleID], [ModuleID], [View], [Add],[Update],[Delete], [CreatedBy], [CreatedDate]) 
	SELECT R.RoleID, M.ModuleID, 0, 0, 0, 0, 'Admin', GETDATE() 
	FROM Role R CROSS JOIN Module M
	LEFT JOIN RolePermission RP ON M.ModuleID = RP.ModuleID AND RP.RoleID = R.RoleID
	WHERE RP.RolePermissionID IS NULL
	ORDER BY 1,2
 
	SELECT 
		RP.RolePermissionID,
		RP.RoleID,
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
	INNER JOIN [Role] R on R.RoleID = RP.RoleId
	INNER JOIN Module M on M.ModuleID = RP.ModuleID 
	WHERE RP.RolePermissionID = ISNULL(@RolePermissionID, RolePermissionID)
	AND	RP.RoleID = ISNULL(@RoleID,R.RoleID)
END
GO