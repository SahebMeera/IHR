--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 05/25/2020
-- Description : Insert/Update SP for State
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdState]
	@StateID	int	=	NULL,
	@CountryID	int	=	NULL,
	@StateShort	varchar(3)	=	NULL,
	@StateDesc	varchar(50)	=	NULL,
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
	
	IF NOT EXISTS(SELECT 1 FROM State WHERE StateID = ISNULL(@StateID,0))  
	BEGIN
		INSERT INTO [dbo].[State]
		(
			CountryID,
			StateShort,
			StateDesc,
			CreatedBy,
			CreatedDate
		)
	OUTPUT INSERTED.StateID INTO @IDTable 
	VALUES
		(
			@CountryID,
			@StateShort,
			@StateDesc,
			@CreatedBy,
			GETDATE()
		)
		SELECT @StateID =(SELECT ID FROM @IDTable);  
	END
	ELSE
	BEGIN
		UPDATE [dbo].[State]
		SET
			CountryID = @CountryID,
			StateShort = @StateShort,
			StateDesc = @StateDesc,	
			ModifiedBy = @ModifiedBy,
			ModifiedDate = GETDATE()
		WHERE StateID = @StateID
	END
	IF @@ERROR=0   
		SET @ReturnCode  = @StateID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END
