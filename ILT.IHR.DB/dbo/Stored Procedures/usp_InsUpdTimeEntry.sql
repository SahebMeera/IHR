--===============================================================
-- Author : Mihir Hapaliya
-- Created Date : 12/07/2020
-- Description : Insert/Update SP for TimeEntry
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdTimeEntry]
	@TimeEntryID	int = NULL,
	@TimeSheetID	int	=	NULL,
	@WorkDate datetime = NULL,
	@Project varchar(50) = NULL,
	@Activity varchar(50) = NULL,
	@Hours int = NULL,
	-- @CreatedBy	varchar(50)	=	NULL,
	-- @CreatedDate	datetime =	NULL,
	-- @ModifiedBy	varchar(50)	=	NULL,
	-- @ModifiedDate	datetime =	NULL,
	-- @TimeStamp	timestamp	=	NULL,
	@ReturnCode INT = 0 OUTPUT 
AS 
BEGIN
	SET NOCOUNT ON;  
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	DECLARE @IDTable TABLE(ID INT)
	
	IF NOT EXISTS(SELECT 1 FROM TimeEntry WHERE TimeEntryID = ISNULL(@TimeEntryID,0))  
		BEGIN
			INSERT INTO [dbo].[TimeEntry]
			(
				TimeSheetID,
				WorkDate,
				Project,
				Activity,
				[Hours]
				--CreatedBy,
				--CreatedDate
			)
			OUTPUT INSERTED.TimeSheetID INTO @IDTable 
			VALUES
			(
				@TimeSheetID,
				@WorkDate,
				@Project,
				@Activity,
				@Hours
				-- @CreatedBy,
				-- GETDATE()
			)
			SELECT @TimeEntryID=(SELECT ID FROM @IDTable);  
		END
	ELSE
		BEGIN
			UPDATE [dbo].[TimeEntry]
			SET
				TimeSheetID = @TimeSheetID,
				WorkDate = @WorkDate,
				Project = @Project,
				Activity = @Activity,
				Hours = @Hours
				-- ModifiedBy = @ModifiedBy,
				-- ModifiedDate = GETDATE()
			WHERE TimeEntryID = @TimeEntryID
		END

	IF @@ERROR=0   
		SET @ReturnCode  = @TimeEntryID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END