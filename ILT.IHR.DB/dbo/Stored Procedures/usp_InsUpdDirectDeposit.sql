--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 05/25/2020
-- Description : Insert/Update SP for DirectDeposit
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdDirectDeposit]
	@DirectDepositID	int	=	NULL,
	@EmployeeID	int	=	NULL,
	@BankName	varchar(50)	=	NULL,
	@AccountTypeID	int	=	NULL,
	@RoutingNumber	varchar(20)	=	NULL,
	@AccountNumber	varchar(20)	=	NULL,
	@Country	varchar(50) =	NULL,
	@State	varchar(50) =	NULL,
	@Amount	int	=	NULL,
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
	
	IF NOT EXISTS(SELECT 1 FROM DirectDeposit WHERE DirectDepositID = ISNULL(@DirectDepositID,0))  
	BEGIN
		INSERT INTO [dbo].[DirectDeposit]
		(
			EmployeeID,
			BankName,
			AccountTypeID,
			RoutingNumber,
			AccountNumber,
			Country,
			State,
			Amount,
			CreatedBy,
			CreatedDate
		)
	OUTPUT INSERTED.DirectDepositID INTO @IDTable 
	VALUES
		(
			@EmployeeID,
			@BankName,
			@AccountTypeID,
			@RoutingNumber,
			@AccountNumber,
			@Country,
			@State,
			@Amount,
			@CreatedBy,
			GETDATE()
		)
		SELECT @DirectDepositID =(SELECT ID FROM @IDTable);  
	END
	ELSE
	BEGIN
		UPDATE [dbo].[DirectDeposit]
		SET
			EmployeeID = @EmployeeID,
			BankName = @BankName,
			AccountTypeID = @AccountTypeID,
			RoutingNumber = @RoutingNumber,
			AccountNumber = @AccountNumber,
			Country = @Country,
			State = @State,
			Amount = @Amount,
			ModifiedBy = @ModifiedBy,
			ModifiedDate = GETDATE()
		WHERE DirectDepositID = @DirectDepositID
	END
	IF @@ERROR=0   
		SET @ReturnCode  = @DirectDepositID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END
