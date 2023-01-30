--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 01/10/2021
-- Description : Insert/Update SP for Document
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdDocument]
	@DocumentID	int	=	NULL,
	@EmployeeID	int	=	NULL,
	@CompanyID	int	=	NULL,
	@DocumentCategoryID	int	=	NULL,
	@DocumentTypeID	int	=	NULL,
	@IssuingAuthority   varchar(50)	=	NULL,
	@DocumentNumber	varchar(20)	=	NULL,
	@IssueDate	date	=	NULL,
	@ExpiryDate	date	=	NULL,
	@Note	varchar(50)	=	NULL,
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
 
	IF NOT EXISTS(SELECT 1 FROM Document WHERE DocumentID = ISNULL(@DocumentID,0))
	BEGIN
		INSERT INTO [dbo].[Document]
		(
			EmployeeID,
			CompanyID,
			DocumentCategoryID,
			DocumentTypeID,
			IssuingAuthority,
			DocumentNumber,
			IssueDate,
			ExpiryDate,
			Note,
			CreatedBy,
			CreatedDate,
			ModifiedBy,
			ModifiedDate
		)
		OUTPUT INSERTED.DocumentID INTO @IDTable
		VALUES
		(
			@EmployeeID,
			@CompanyID,
			@DocumentCategoryID,
			@DocumentTypeID,
			@IssuingAuthority,
			@DocumentNumber,
			@IssueDate,
			@ExpiryDate,
			@Note,
			@CreatedBy,
			GETDATE(),
			@ModifiedBy,
			GETDATE()
		)
		SELECT @DocumentID=(SELECT ID FROM @IDTable);
END
	ELSE
		UPDATE [dbo].[Document]
		SET
			EmployeeID = @EmployeeID,
			CompanyID = @CompanyID,
			DocumentCategoryID = @DocumentCategoryID,
			DocumentTypeID = @DocumentTypeID,
			IssuingAuthority = @IssuingAuthority,
			DocumentNumber = @DocumentNumber,
			IssueDate = @IssueDate,
			ExpiryDate = @ExpiryDate,
			Note = @Note,
			ModifiedBy = @ModifiedBy,
			ModifiedDate = GETDATE()
		WHERE DocumentID = @DocumentID
 
	IF @@ERROR=0
		SET @ReturnCode  = @DocumentID;
	ELSE
		SET @ReturnCode = 0
	RETURN
 
END