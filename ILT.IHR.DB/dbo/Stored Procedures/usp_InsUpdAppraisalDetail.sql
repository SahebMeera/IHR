--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 05/25/2020
-- Description : Insert/Update SP for AssignmentRate
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By			Description
--12/02/2020	Mihir Hapaliya	explicit insert of AssignmentID column removed due to identity coulmn
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdAppraisalDetail]
	@AppraisalDetailID	int	=	NULL,
	@AppraisalID	int	=	NULL,
	@AppraisalQualityID	int	=	NULL,
	@EmpResponse varchar(500)	=	NULL,
	@EmpComment varchar(200)	=	NULL,
	@MgrResponse varchar(500)	=	NULL,
	@MgrComment varchar(200)	=	NULL,
	--@CreatedBy	varchar(50)	=	NULL,
	--@CreatedDate	datetime	=	NULL,
	--@ModifiedBy	varchar(50)	=	NULL,
	--@ModifiedDate	datetime	=	NULL,
	--@TimeStamp	timestamp	=	NULL,
	@ReturnCode INT = 0 OUTPUT 

AS 
BEGIN
	SET NOCOUNT ON;  
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	DECLARE @IDTable TABLE(ID INT)
	
	IF NOT EXISTS(SELECT 1 FROM AppraisalDetailID WHERE AppraisalDetailID = ISNULL(@AppraisalDetailID,0))  
	BEGIN
		INSERT INTO [dbo].[AppraisalDetail]
		(
			AppraisalID,
			AppraisalQualityID,
			EmpResponse,
			EmpComment,
			MgrResponse,
			MgrComment
			-- CreatedBy,
			-- CreatedDate
		)
	OUTPUT INSERTED.AppraisalDetailID INTO @IDTable 
	VALUES
		(
			@AppraisalID,
			@AppraisalQualityID,
			@EmpResponse,
			@EmpComment,
			@MgrResponse,
			@MgrComment
			-- @CreatedBy,
			-- GETDATE()
		)
		SELECT @AppraisalDetailID =(SELECT ID FROM @IDTable);  
	END
	ELSE
	BEGIN
		UPDATE [dbo].[AppraisalDetail]
		SET
			AppraisalID = @AppraisalID,
			AppraisalQualityID = @AppraisalQualityID,
			EmpResponse = @EmpResponse,
			EmpComment = @EmpComment,
			MgrResponse = @MgrResponse,
			MgrComment = @MgrComment
			-- ModifiedBy = @ModifiedBy
			-- ModifiedDate = GETDATE()
		WHERE AppraisalDetailID = @AppraisalDetailID
	END
	IF @@ERROR=0   
		SET @ReturnCode  = @AppraisalDetailID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END