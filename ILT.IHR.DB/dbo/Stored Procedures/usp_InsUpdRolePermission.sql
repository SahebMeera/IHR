--===============================================================
-- Author : Mihir Hapaliya
-- Created Date : 05/25/2020
-- Description : Insert/Update SP for Role Permission
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdRolePermission]

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
	@ModifiedDate	datetime	=	NULL,
	@TimeStamp	timestamp	=	NULL,
	@ReturnCode INT = 0 OUTPUT 
AS 
BEGIN
	SET NOCOUNT ON;  
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	DECLARE @IDTable TABLE(ID INT)
	
	IF NOT EXISTS(SELECT 1 FROM [RolePermission] WHERE RolePermissionID = ISNULL(@RolePermissionID,0))  
	BEGIN
		INSERT INTO [dbo].[RolePermission]
		(
			RoleID,
			ModuleID,
			[View],
			[Add],
			[Update],
			[Delete],
			CreatedBy,
			CreatedDate
		)
	OUTPUT INSERTED.RolePermissionID INTO @IDTable 
	VALUES
		(
			@RoleID,
			@ModuleID,
			@View,
			@Add,
			@Update,
			@Delete,
			@CreatedBy,
			GETDATE()
		)
		SELECT @RolePermissionID =(SELECT ID FROM @IDTable);  
	END
	ELSE
	BEGIN
		UPDATE [dbo].[RolePermission]
		SET
			[View] = @View,
			[Add] = @Add,
			[Update] = @Update,
			[Delete] = @Delete,
			ModifiedDate = GETDATE()
		WHERE RolePermissionID = @RolePermissionID
	END
	IF @@ERROR=0   
		SET @ReturnCode  = @RolePermissionID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END
