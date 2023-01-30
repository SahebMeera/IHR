--===============================================================
-- Author : Mihir Hapaliya
-- Created Date : 11/30/2020
-- Description : Insert/Update SP for Contact
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdContact]
	@ContactID	int	=	NULL,
	@ContactTypeID int = NULL,
	@EmployeeID int = NULL,
	@FirstName varchar(50) = NULL,
	@LastName varchar(50) = NULL,
	@Phone varchar(10) = NULL,
	@Email varchar(50) = NULL,
	@Address1 varchar(100) = NULL,
	@Address2 varchar(100) = NULL,
	@City varchar(50) = NULL,
	@State varchar(50) = NULL,
	@Country varchar(50) = NULL,
	@ZipCode varchar(10) = NULL,
	@IsDeleted	bit	=	NULL,
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
	
	IF NOT EXISTS(SELECT 1 FROM Contact WHERE ContactID = ISNULL(@ContactID,0))  
		BEGIN
			INSERT INTO [dbo].[Contact]
			(
				ContactTypeID,
				EmployeeID,
				FirstName,
				LastName,
				Phone,
				Email,
				Address1,
				Address2,
				City,
				[State],
				Country,
				ZipCode,
				IsDeleted,
				CreatedBy,
				CreatedDate
			)
			OUTPUT INSERTED.ContactID INTO @IDTable 
			VALUES
			(
				@ContactTypeID, 
				@EmployeeID,
				@FirstName,
				@LastName,
				@Phone,
				@Email,
				@Address1,
				@Address2,
				@City,
				@State,
				@Country,
				@ZipCode,
				@IsDeleted,
				@CreatedBy,
				GETDATE()
			)
			SELECT @ContactID=(SELECT ID FROM @IDTable);  
		END
	ELSE
		BEGIN
			UPDATE [dbo].[Contact]
			SET
				ContactTypeID = @ContactTypeID,
				EmployeeID = @EmployeeID,
				FirstName = @FirstName,
				LastName = @LastName,
				Phone = @Phone,
				Email = @Email,
				Address1 = @Address1,
				Address2 = @Address2,
				City = @City,
				[State] = @State,
				Country = @Country,
				ZipCode = @ZipCode,
				IsDeleted = @IsDeleted,
				ModifiedBy = @ModifiedBy,
				ModifiedDate = GETDATE()
			WHERE ContactID = @ContactID
		END

	IF @@ERROR=0   
		SET @ReturnCode  = @ContactID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END