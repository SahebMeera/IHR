--===============================================================
-- Author : Rama Mohan
-- Created Date : 02/08/2021
-- Description : Insert/Update SP for End Client
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdEndClient]
	@EndClientID	int	=	NULL,
	@Name	varchar(50)	=	NULL,
	@CompanyID int = NULL,
	@Address1	varchar(100)	=	NULL,
	@Address2	varchar(100)	=	NULL,
	@City	varchar(50)	=	NULL,
	@State	varchar(50)	=	NULL,
	@Country	varchar(50)	=	NULL,
	@ZipCode	varchar(10)	=	NULL,	
	@TaxID	varchar(20)	=	NULL,	
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
	
	IF NOT EXISTS(SELECT 1 FROM EndClient WHERE EndClientID = ISNULL(@EndClientID,0))  
	BEGIN
		INSERT INTO [dbo].[EndClient]
		(
			Name,
			CompanyID,
			Address1,
			Address2,
			City,
			State,
			Country,
			ZipCode,			
			TaxID,			
			CreatedBy,
			CreatedDate
		)
	OUTPUT INSERTED.EndClientID INTO @IDTable 
	VALUES
		(
			@Name,
			@CompanyID,
			@Address1,
			@Address2,
			@City,
			@State,
			@Country,
			@ZipCode,			
			@TaxID,			
			@CreatedBy,
			GETDATE()
		)
		SELECT @EndClientID =(SELECT ID FROM @IDTable);  
	END
	ELSE
	BEGIN
		UPDATE [dbo].[EndClient]
		SET
			Name = @Name,
			CompanyID = @CompanyID,
			Address1 = @Address1,
			Address2 = @Address2,
			City = @City,
			State = @State,
			Country = @Country,
			ZipCode = @ZipCode,			
			TaxID = @TaxID,			
			ModifiedBy = @ModifiedBy,
			ModifiedDate = GETDATE()
		WHERE EndClientID = @EndClientID
	END
	IF @@ERROR=0   
		SET @ReturnCode  = @EndClientID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END