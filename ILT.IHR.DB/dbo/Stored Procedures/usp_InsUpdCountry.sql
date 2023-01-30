--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 05/25/2020
-- Description : Insert/Update SP for Country
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdCountry]
	@CountryID	int	=	NULL,
	@CountryDesc	varchar(50)	=	NULL,
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
	
	IF NOT EXISTS(SELECT 1 FROM Country WHERE CountryID = ISNULL(@CountryID,0))  
	BEGIN
		INSERT INTO [dbo].[Country]
		(
			CountryDesc,
			CreatedBy,
			CreatedDate
		)
	OUTPUT INSERTED.CountryID INTO @IDTable 
	VALUES
		(
			@CountryDesc,
			@CreatedBy,
			GETDATE()
		)
		SELECT @CountryID =(SELECT ID FROM @IDTable);  
	END
	ELSE
	BEGIN
		UPDATE [dbo].[Country]
		SET
			CountryDesc = @CountryDesc,
			ModifiedBy = @ModifiedBy,
			ModifiedDate = GETDATE()
		WHERE CountryID = @CountryID
	END
	IF @@ERROR=0   
		SET @ReturnCode  = @CountryID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END
