--===============================================================
-- Author : Rama Mohan
-- Created Date : 12/08/2020
-- Description : Insert/Update SP for Holiday
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdHoliday]
	@HolidayID	int	=	NULL,	
	@Name varchar(50) = NULL,	
	@StartDate	datetime	=	NULL,
	@Country	varchar(50)	=	NULL,
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
	
	IF NOT EXISTS(SELECT 1 FROM Holiday WHERE HolidayID = ISNULL(@HolidayID,0))  
		BEGIN
			INSERT INTO [dbo].[Holiday]
			(
				Name,
				StartDate,
				Country,
				CreatedBy,
				CreatedDate
			)
			OUTPUT INSERTED.HolidayID INTO @IDTable 
			VALUES
			(				
				@Name,
				@StartDate,
				@Country,
				@CreatedBy,
				GETDATE()
			)
			SELECT @HolidayID=(SELECT ID FROM @IDTable);  
		END
	ELSE
		BEGIN
			UPDATE [dbo].[Holiday]
			SET
				Name = @Name,
				StartDate = @StartDate,
				Country = @Country,
				ModifiedBy = @ModifiedBy,
				ModifiedDate = GETDATE()
			WHERE HolidayID = @HolidayID
		END

	IF @@ERROR=0   
		SET @ReturnCode  = @HolidayID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END
