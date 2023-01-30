--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 05/25/2020
-- Description : Insert/Update SP for Role
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdRole]
	@RoleID	int	=	NULL,
	@RoleShort	varchar(20)	=	NULL,
	@RoleName	varchar(50)	=	NULL,
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
	
	IF NOT EXISTS(SELECT 1 FROM [Role] WHERE RoleID = ISNULL(@RoleID,0))  
	BEGIN
		INSERT INTO [dbo].[Role]
		(
			RoleShort,
			RoleName,
			CreatedBy,
			CreatedDate
		)
	OUTPUT INSERTED.RoleID INTO @IDTable 
	VALUES
		(
			@RoleShort,
			@RoleName,
			@CreatedBy,
			GETDATE()
		)
		SELECT @RoleID =(SELECT ID FROM @IDTable);  
	END
	ELSE
	BEGIN
		UPDATE [dbo].[Role]
		SET
			RoleShort = @RoleShort,
			RoleName = @RoleName,
			ModifiedBy = @ModifiedBy,
			ModifiedDate = GETDATE()
		WHERE RoleID = @RoleID
	END
	IF @@ERROR=0   
		SET @ReturnCode  = @RoleID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END
