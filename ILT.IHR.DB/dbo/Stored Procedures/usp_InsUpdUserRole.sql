--===============================================================
-- Author : Mihir Hapaliya
-- Created Date : 05/25/2020
-- Description : Insert/Update SP for User Role
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdUserRole]
	@UserRoleID	int = NULL,
	@UserID int = NULL,
	@RoleID	int	=	NULL,
	@IsDefault bit = 0,
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
	
	IF NOT EXISTS(SELECT 1 FROM [UserRole] WHERE UserID = ISNULL(@UserID,0) AND RoleID = ISNULL(@RoleID,0))  
	BEGIN
		INSERT INTO [dbo].[UserRole]
		(
			UserID,
			RoleID,
			IsDefault,
			CreatedBy,
			CreatedDate
		)
	OUTPUT INSERTED.UserRoleID INTO @IDTable 
	VALUES
		(
			@UserID,
			@RoleID,
			@IsDefault,
			@CreatedBy,
			GETDATE()
		)
		SELECT @UserRoleID =(SELECT ID FROM @IDTable);  
	END
	ELSE
	BEGIN
		UPDATE [dbo].[UserRole]
		SET
			@IsDefault = @IsDefault,
			ModifiedBy = @ModifiedBy,
			ModifiedDate = GETDATE()
		WHERE UserID = @UserID AND RoleID = @RoleID
	END
	IF @@ERROR=0   
		SET @ReturnCode  = @UserRoleID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END