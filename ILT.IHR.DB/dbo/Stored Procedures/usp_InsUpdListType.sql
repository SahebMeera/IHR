--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 05/24/2020
-- Description : Insert/Update SP for ListType
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdListType]
	@ListTypeID	int	=	NULL,
	@Type	varchar(20)	=	NULL,
	@TypeDesc	varchar(100)	=	NULL,
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
	
	IF NOT EXISTS(SELECT 1 FROM ListType WHERE ListTypeID = ISNULL(@ListTypeID,0))  
		BEGIN
			INSERT INTO [dbo].[ListType]
			(
				ListTypeID,
				Type,
				TypeDesc,
				CreatedBy,
				CreatedDate
			)
			OUTPUT INSERTED.ListTypeID INTO @IDTable 
			VALUES
			(
				@ListTypeID,
				@Type,
				@TypeDesc,
				@CreatedBy,
				GETDATE()
			)
			SELECT @ListTypeID=(SELECT ID FROM @IDTable);  
		END
	ELSE
		BEGIN
			UPDATE [dbo].[ListType]
			SET
				Type = @Type,
				TypeDesc = @TypeDesc,
				ModifiedBy = @ModifiedBy,
				ModifiedDate = GETDATE()
		END

	IF @@ERROR=0   
		SET @ReturnCode  = @ListTypeID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END
