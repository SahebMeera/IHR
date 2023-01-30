--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 01/19/2021
-- Description : Insert/Update SP for EmailApproval
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
-- 09/30/2021	Mihir Hapaliya	Email fileds added . 
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdEmailApproval]
	@EmailApprovalID	int	=	NULL,
	@ModuleID	int	=	NULL,
	@ID	int	=	NULL,
	--@UserID	int	=	NULL,
	@ValidTime	datetime	=	NULL,
	@IsActive	bit	=	NULL,
	@Value	varchar(100)	=	NULL,
	@LinkID	uniqueidentifier	=	NULL,
	@ApproverEmail varchar(50) = NULL,
	@EmailSubject varchar(255) = NULL,
	@EmailBody  varchar(MAX) = NULL,
	@EmailFrom varchar(50) = NULL,
	@EmailTo varchar(255) = NULL,
	@EmailCC varchar(255) = NULL,
	@EmailBCC varchar(255) = NULL,
	@SentCount int = 0,
	@SendDate datetime = NULL,
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
 
	IF NOT EXISTS(SELECT 1 FROM EmailApproval WHERE EmailApprovalID = ISNULL(@EmailApprovalID,0))
	BEGIN
		INSERT INTO [dbo].[EmailApproval]
		(
			ModuleID,
			ID,
			--UserID,
			ValidTime,
			IsActive,
			Value,
			LinkID,
			ApproverEmail,
			EmailSubject,
			EmailFrom,
			EmailTo,
			EmailCC,
			EmailBCC,
			EmailBody,
			SendDate,
			SentCount,
			CreatedBy,
			CreatedDate,
			ModifiedBy,
			ModifiedDate
		)
		OUTPUT INSERTED.EmailApprovalID INTO @IDTable
		VALUES
		(
			@ModuleID,
			@ID,
			--@UserID,
			@ValidTime,
			@IsActive,
			@Value,
			@LinkID,
			@ApproverEmail,
			@EmailSubject,
			@EmailFrom,
			@EmailTo,
			@EmailCC,
			@EmailBCC,
			@EmailBody,
			@SendDate,
			@SentCount,
			@CreatedBy,
			GETDATE(),
			@CreatedBy,
			GETDATE()
		)
		SELECT @EmailApprovalID=(SELECT ID FROM @IDTable);
END
	ELSE
		UPDATE [dbo].[EmailApproval]
		SET
			ModuleID = @ModuleID,
			ID = @ID,
			--UserID = @UserID,
			ValidTime = @ValidTime,
			IsActive = @IsActive,
			Value = @Value,
			LinkID = @LinkID,
			ApproverEmail = @ApproverEmail,
			EmailSubject = @EmailSubject,
			EmailBody = @EmailBody,
			EmailFrom = @EmailFrom,
			EmailTo = @EmailTo,
			EmailCC = @EmailCC,
			EmailBCC = @EmailBCC,
			SendDate = @SendDate,
			SentCount = @SentCount,
			ModifiedBy = @ModifiedBy,
			ModifiedDate = GETDATE()
		WHERE EmailApprovalID = @EmailApprovalID
 
	IF @@ERROR=0
		SET @ReturnCode  = @EmailApprovalID;
	ELSE
		SET @ReturnCode = 0
	RETURN
 
END