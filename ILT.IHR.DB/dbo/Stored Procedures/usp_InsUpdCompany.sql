--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 05/25/2020
-- Description : Insert/Update SP for Company
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdCompany]
	@CompanyID	int	=	NULL,
	@Name	varchar(50)	=	NULL,
	@Address1	varchar(100)	=	NULL,
	@Address2	varchar(100)	=	NULL,
	@City	varchar(50)	=	NULL,
	@State	varchar(50)	=	NULL,
	@Country	varchar(50)	=	NULL,
	@ZipCode	varchar(10)	=	NULL,
	@ContactName	varchar(50)	=	NULL,
	@ContactPhone	varchar(10)	=	NULL,
	@ContactEmail	varchar(50)	=	NULL,
	@AlternateContactName	varchar(50)	=	NULL,
	@AlternateContactPhone	varchar(10)	=	NULL,
	@AlternateContactEmail	varchar(50)	=	NULL,
	@InvoiceContactName	varchar(50)	=	NULL,
	@InvoiceContactPhone	varchar(10)	=	NULL,
	@InvoiceContactEmail	varchar(50)	=	NULL,
	@AlternateInvoiceContactName	varchar(50)	=	NULL,
	@AlternateInvoiceContactPhone	varchar(10)	=	NULL,
	@AlternateInvoiceContactEmail	varchar(50)	=	NULL,
	@InvoicingPeriodID	int	=	NULL,
	@PaymentTermID	int	=	NULL,
	@TaxID	varchar(20)	=	NULL,
	@CompanyTypeID int = NULL,
	@IsEndClient bit = 0,
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
	
	IF NOT EXISTS(SELECT 1 FROM Company WHERE CompanyID = ISNULL(@CompanyID,0))  
	BEGIN
		INSERT INTO [dbo].[Company]
		(
			Name,
			Address1,
			Address2,
			City,
			State,
			Country,
			ZipCode,
			ContactName,
			ContactPhone,
			ContactEmail,
			AlternateContactName,
			AlternateContactPhone,
			AlternateContactEmail,
			InvoiceContactName,
			InvoiceContactPhone,
			InvoiceContactEmail,
			AlternateInvoiceContactName,
			AlternateInvoiceContactPhone,
			AlternateInvoiceContactEmail,
			InvoicingPeriodID,
			PaymentTermID,
			TaxID,
			CompanyTypeID,
			IsEndClient,
			CreatedBy,
			CreatedDate
		)
	OUTPUT INSERTED.CompanyID INTO @IDTable 
	VALUES
		(
			@Name,
			@Address1,
			@Address2,
			@City,
			@State,
			@Country,
			@ZipCode,
			@ContactName,
			@ContactPhone,
			@ContactEmail,
			@AlternateContactName,
			@AlternateContactPhone,
			@AlternateContactEmail,
			@InvoiceContactName,
			@InvoiceContactPhone,
			@InvoiceContactEmail,
			@AlternateInvoiceContactName,
			@AlternateInvoiceContactPhone,
			@AlternateInvoiceContactEmail,
			@InvoicingPeriodID,
			@PaymentTermID,
			@TaxID,
			@CompanyTypeID,
			@IsEndClient,
			@CreatedBy,
			GETDATE()
		)
		SELECT @CompanyID =(SELECT ID FROM @IDTable);  
	END
	ELSE
	BEGIN
		UPDATE [dbo].[Company]
		SET
			Name = @Name,
			Address1 = @Address1,
			Address2 = @Address2,
			City = @City,
			State = @State,
			Country = @Country,
			ZipCode = @ZipCode,
			ContactName = @ContactName,
			ContactPhone = @ContactPhone,
			ContactEmail = @ContactEmail,
			AlternateContactName = @AlternateContactName,
			AlternateContactPhone = @AlternateContactPhone,
			AlternateContactEmail = @AlternateContactEmail,
			InvoiceContactName = @InvoiceContactName,
			InvoiceContactPhone = @InvoiceContactPhone,
			InvoiceContactEmail = @InvoiceContactEmail,
			AlternateInvoiceContactName = @AlternateInvoiceContactName,
			AlternateInvoiceContactPhone = @AlternateInvoiceContactPhone,
			AlternateInvoiceContactEmail = @AlternateInvoiceContactEmail,
			InvoicingPeriodID = @InvoicingPeriodID,
			PaymentTermID = @PaymentTermID,
			TaxID = @TaxID,
			CompanyTypeID = @CompanyTypeID,
			IsEndClient = @IsEndClient,
			ModifiedBy = @ModifiedBy,
			ModifiedDate = GETDATE()
		WHERE CompanyID = @CompanyID
	END
	IF @@ERROR=0   
		SET @ReturnCode  = @CompanyID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END
