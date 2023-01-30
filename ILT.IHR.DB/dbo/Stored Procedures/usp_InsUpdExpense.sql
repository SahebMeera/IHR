--===============================================================
-- Author : Sanjan Madishetti
-- Created Date : 02/23/2021
-- Description : Insert/Update SP for Expense
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdExpense]
	@ExpenseID	int	=	NULL,
	@EmployeeID	int	=	NULL,
	@ExpenseTypeID	int	=	NULL,
	@FileName varchar(100) = NULL,
	@Amount	int	=	NULL,
	@SubmissionDate	date	=	NULL,
	@SubmissionComment	varchar(100)	=	NULL,
	@StatusID int = NULL,
	@AmountPaid int = NULL,
	@PaymentDate date = NULL,
	@PaymentComment varchar(100) = NULL,
	@CreatedBy	varchar(50)	=	NULL,
	@CreatedDate	datetime	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate	datetime	=	NULL,
	@ReturnCode INT = 0 OUTPUT 

AS 
BEGIN
	SET NOCOUNT ON;  
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	DECLARE @IDTable TABLE(ID INT)
	
	IF NOT EXISTS(SELECT 1 FROM Expense WHERE ExpenseID = ISNULL(@ExpenseID,0))  
	BEGIN
		INSERT INTO [dbo].[Expense]
		(
			EmployeeID,
			ExpenseTypeID,
			[FileName],
			Amount,
			SubmissionDate,
			SubmissionComment,
			StatusID,
			AmountPaid,
			PaymentDate,
			PaymentComment,
			CreatedBy,
			CreatedDate
		)
	OUTPUT INSERTED.ExpenseID INTO @IDTable 
	VALUES
		(
			@EmployeeID,
			@ExpenseTypeID,
			@FileName,
			@Amount,
			@SubmissionDate,
			@SubmissionComment,
			@StatusID,
			@AmountPaid,
			@PaymentDate,
			@PaymentComment,
			@CreatedBy,
			GETDATE()
		)
		SELECT @ExpenseID =(SELECT ID FROM @IDTable);  
	END
	ELSE
	BEGIN
		UPDATE [dbo].[Expense]
		SET
			EmployeeID = @EmployeeID,
			ExpenseTypeID = @ExpenseTypeID,
			[FileName] = @FileName,
			Amount = @Amount,
			SubmissionDate = @SubmissionDate,
			SubmissionComment = @SubmissionComment,
			StatusID = @StatusID,
			AmountPaid = @AmountPaid,
			PaymentDate = @PaymentDate,
			PaymentComment = @PaymentComment,
			ModifiedBy = @ModifiedBy,
			ModifiedDate = GETDATE()
		WHERE ExpenseID = @ExpenseID
	END
	IF @@ERROR=0   
		SET @ReturnCode  = @ExpenseID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END