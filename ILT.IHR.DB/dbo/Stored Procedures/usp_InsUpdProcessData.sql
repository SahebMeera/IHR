--===============================================================
-- Author : Rama Mohan
-- Created Date : 11/23/2021
-- Description : Insert/Update SP for Process Data
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
-- 
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdProcessData]
	@ProcessDataID	int	=	NULL,
	@ProcessWizardID	int	=	NULL,
	@Data	XML	=	NULL,
	@CreatedBy	varchar(50)	=	NULL,
	@CreatedDate	datetime	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate	datetime	=	NULL,
	@StatusID int = NULL,
	@ChangeNotificationEmailId Varchar(50) = NULL,
	@EmailApprovalValidity int = NULL,
	@ReturnCode INT = 0 OUTPUT 

AS 
BEGIN
	SET NOCOUNT ON;  
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	DECLARE @IDTable TABLE(ID INT)
	
	IF NOT EXISTS(SELECT 1 FROM [dbo].[ProcessData] WHERE ProcessDataID = ISNULL(@ProcessDataID,0))  
	BEGIN
		INSERT INTO [dbo].[ProcessData]
		(
			ProcessWizardID,
			[Data],
			CreatedBy,
			CreatedDate,
			StatusID
		)
	OUTPUT INSERTED.ProcessDataID INTO @IDTable 
	VALUES
		(
			@ProcessWizardID,
			@Data,
			@CreatedBy,
			GETDATE(),
			@StatusID
		)
		SELECT @ProcessDataID =(SELECT ID FROM @IDTable);  
	END
	ELSE
	BEGIN
		UPDATE [dbo].[ProcessData]
		SET
			ProcessWizardID = @ProcessWizardID,
			[Data] = @Data,
			ModifiedBy = @ModifiedBy,
			ModifiedDate = GETDATE(),
			StatusID = @StatusID
		WHERE ProcessDataID = @ProcessDataID

	
		EXEC [dbo].[usp_InsertProcessTicket] @ProcessDataID = @ProcessDataID, @ChangeNotificationEmailId = @ChangeNotificationEmailId,
		@EmailApprovalValidity = @EmailApprovalValidity
		
	END
	IF @@ERROR=0   
		SET @ReturnCode  = @ProcessDataID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END