--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 00/30/2021
-- Description : Update SP for EmailApproval sending mail
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
-- 
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_EmailSendAction]
	@EmailApprovalID	int	=	NULL,
	@IsActive bit = 1,
	-- @ReminderCount	varchar(100)	=	NULL,
	@ReturnCode INT = 0 OUTPUT
AS 
BEGIN
	SET NOCOUNT ON; 
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
 
	DECLARE @IDTable TABLE(ID INT)
 
	IF EXISTS(SELECT 1 FROM EmailApproval WHERE EmailApprovalID = @EmailApprovalID)
	BEGIN
		
		UPDATE [dbo].[EmailApproval]
		SET
			SentCount = ISNULL(SentCount, 0) + 1,
			IsActive = @IsActive,
			SendDate = GETDATE()
		WHERE EmailApprovalID = @EmailApprovalID
	
	END

	IF @@ERROR=0
		SET @ReturnCode  = 1;
	ELSE
		SET @ReturnCode = 0
	RETURN

END