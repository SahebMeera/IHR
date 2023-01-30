--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 01/19/2021
-- Description : Insert/Update SP for EmailApproval
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
-- 09/30/2021	Mihir Hapaliya	Email approval only updating isActive 
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_EmailApprovalAction]
	@LinkID	UNIQUEIDENTIFIER	=	NULL,
	@Value	varchar(100)	=	NULL,
	@ReturnCode INT = 0 OUTPUT
AS 
BEGIN
	SET NOCOUNT ON; 
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
 
	DECLARE @IDTable TABLE(ID INT)
 
	IF EXISTS(SELECT 1 FROM EmailApproval WHERE LinkID = @LinkID)
	BEGIN
		
		DECLARE @ID INT, @ModuleID int, @ModuleName varchar(50), @UserID int, @UserName VARCHAR(50), @TSApprovedID int, @TSRejectedID int,@TSVoidID int, @ApproverEmail varchar(50)

		SELECT 
			@ID = EA.ID, 
			@ModuleID = EA.ModuleID, 
			@ApproverEmail = EA.ApproverEmail,
			@ModuleName = M.ModuleName 
		FROM EmailApproval EA
		JOIN [dbo].[Module] M ON EA.ModuleID = M.ModuleID
		WHERE LinkID = @LinkID




		UPDATE [dbo].[EmailApproval]
		SET
			IsActive = 0,
			[Value] = @Value,
			ModifiedDate = GETDATE()
		WHERE LinkID = @LinkID

		SELECT @TSApprovedID  = LV.ListValueID FROM ListValue LV
		JOIN ListType LT ON LV.ListTypeID = LT.ListTypeID WHERE LT.Type = 'TIMESHEETSTATUS'
		AND LV.Value = 'APPROVED'
		SELECT @TSRejectedID  = LV.ListValueID FROM ListValue LV
		JOIN ListType LT ON LV.ListTypeID = LT.ListTypeID WHERE LT.Type = 'TIMESHEETSTATUS'
		AND LV.Value = 'REJECTED'
		SELECT @TSVoidID  = LV.ListValueID FROM ListValue LV
		JOIN ListType LT ON LV.ListTypeID = LT.ListTypeID WHERE LT.Type = 'TIMESHEETSTATUS'
		AND LV.Value = 'VOID'

		IF(@ModuleName = 'TIMESHEET')
		BEGIN
			UPDATE [dbo].[TimeSheet]
			SET StatusID = CASE WHEN @Value = 'APPROVE' THEN @TSApprovedID
			WHEN @Value = 'VOID' THEN @TSVoidID ELSE @TSRejectedID END,
			ApprovedByEmail = @ApproverEmail,
			ApprovedDate = GETDATE()
			WHERE TimeSheetID = @ID 
		END
	
	END

	IF @@ERROR=0
		SET @ReturnCode  = 1;
	ELSE
		SET @ReturnCode = 0
	RETURN

END