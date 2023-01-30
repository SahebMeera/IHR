--===============================================================
-- Author : Sanjan Madishetti
-- Created Date : 04/05/2021	
-- Description : Insert/Update SP for WFH
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdWFH]
	@WFHID	int	=	NULL,
	@EmployeeID	int	=	NULL,
	@Title	varchar(100)	=	NULL,
	@StartDate	date	=	NULL,
	@EndDate	date	=	NULL,
	@RequesterID	int	=	NULL,
	@ApproverID	int	=	NULL,
	@StatusID	int	=	NULL,
	@Comment	varchar(500)	=	NULL,
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


	IF NOT EXISTS(SELECT 1 FROM WFH WHERE WFHID = ISNULL(@WFHID,0))  
	BEGIN
		INSERT INTO [dbo].[WFH]
		(
			EmployeeID,
			Title,
			StartDate,
			EndDate,
			RequesterID,
			ApproverID,
			StatusID,
			Comment,
			CreatedBy,
			CreatedDate
		)
		OUTPUT INSERTED.WFHID INTO @IDTable 
		VALUES
		(
			@EmployeeID,
			@Title,
			@StartDate,
			@EndDate,
			@RequesterID,
			@ApproverID,
			@StatusID,
			@Comment,
			@CreatedBy,
			GETDATE()
		)
		SELECT @WFHID=(SELECT ID FROM @IDTable); 
	END
	ELSE
	BEGIN
		UPDATE [dbo].[WFH]
		SET
			EmployeeID = @EmployeeID,
			Title = @Title,
			StartDate = @StartDate,
			EndDate = @EndDate,
			RequesterID = @RequesterID,
			ApproverID = @ApproverID,
			StatusID = @StatusID,
			Comment = @Comment,
			ModifiedBy = @ModifiedBy,
			ModifiedDate =GETDATE()
		WHERE WFHID = @WFHID 
		END

	IF @@ERROR=0   
		SET @ReturnCode  = @WFHID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
END