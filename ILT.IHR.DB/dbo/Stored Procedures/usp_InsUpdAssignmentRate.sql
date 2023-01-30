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
 
CREATE PROCEDURE [dbo].[usp_InsUpdAssignmentRate]
	@AssignmentRateID	int	=	NULL,
	@AssignmentID	int	=	NULL,
	@BillingRate	decimal(10,2)	=	NULL,
	@PaymentRate	decimal(10,2)	=	NULL,
	@StartDate	date	=	NULL,
	@EndDate	date	=	NULL,
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
	
	IF NOT EXISTS(SELECT 1 FROM AssignmentRate WHERE AssignmentRateID = ISNULL(@AssignmentRateID,0))  
	BEGIN
		INSERT INTO [dbo].[AssignmentRate]
		(
			AssignmentID,
			BillingRate,
			PaymentRate,
			StartDate,
			EndDate,
			CreatedBy,
			CreatedDate
		)
	OUTPUT INSERTED.AssignmentRateID INTO @IDTable 
	VALUES
		(
			@AssignmentID,
			@BillingRate,
			@PaymentRate,
			@StartDate,
			@EndDate,
			@CreatedBy,
			GETDATE()
		)
		SELECT @AssignmentRateID =(SELECT ID FROM @IDTable);  
	END
	ELSE
	BEGIN
		UPDATE [dbo].[AssignmentRate]
		SET
			AssignmentID = @AssignmentID,
			BillingRate = @BillingRate,
			PaymentRate = @PaymentRate,
			StartDate = @StartDate,
			EndDate = @EndDate,
			ModifiedBy = @ModifiedBy,
			ModifiedDate = GETDATE()
		WHERE AssignmentRateID = @AssignmentRateID
	END
	IF @@ERROR=0   
		SET @ReturnCode  = @AssignmentRateID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END
