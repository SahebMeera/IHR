--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 05/24/2020
-- Description : Insert/Update SP for ListValue
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdListValue]
	@ListValueID	int	=	NULL,
	@Value	varchar(20)	=	NULL,
	@ValueDesc	varchar(50)	=	NULL,
	@ListTypeID	int	=	NULL,
	@IsActive	bit	=	1,
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
	
	IF NOT EXISTS(SELECT 1 FROM ListValue WHERE ListValueID = ISNULL(@ListValueID,0))  
		BEGIN
			INSERT INTO [dbo].[ListValue]
			(
				ListTypeID,
				Value,
				ValueDesc,
				IsActive,
				CreatedBy,
				CreatedDate
			)
			OUTPUT INSERTED.ListValueID INTO @IDTable(ID) 
			VALUES
			(
				@ListTypeID,
				@Value,
				@ValueDesc,
				@IsActive,
				@CreatedBy,
				GETDATE()
			)
			SELECT @ListValueID = (SELECT ID FROM @IDTable);  
		END
	ELSE
		BEGIN
			UPDATE [dbo].[ListValue]
			SET
				Value = @Value,
				ValueDesc = @ValueDesc,
				IsActive = @IsActive,
				ModifiedBy = @ModifiedBy,
				ModifiedDate = GETDATE()
			WHERE
				ListValueID =  @ListValueID
				AND ListTypeID = @ListTypeID
		END

	IF @@ERROR=0   
		 SET @ReturnCode  = @ListValueID;  
	 ELSE
		 SET @ReturnCode = 0

	RETURN @ReturnCode
END
