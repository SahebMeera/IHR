--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 05/25/2020
-- Description : Insert/Update SP for Dependent
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdDependent]
	@DependentID	int	=	NULL,
	@FirstName	varchar(50)	=	NULL,
	@MiddleName	varchar(50)	=	NULL,
	@LastName	varchar(50)	=	NULL,
	@EmployeeID	int	=	NULL,
	@RelationID	int	=	NULL,
	@BirthDate	date	=	NULL,
	@VisaTypeID	int	=	NULL,
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
	
	IF NOT EXISTS(SELECT 1 FROM Dependent WHERE DependentID = ISNULL(@DependentID,0))  
	BEGIN
		INSERT INTO [dbo].[Dependent]
		(
			FirstName,
			MiddleName,
			LastName,
			EmployeeID,
			RelationID,
			BirthDate,
			VisaTypeID,
			CreatedBy,
			CreatedDate
		)
	OUTPUT INSERTED.DependentID INTO @IDTable 
	VALUES
		(
			@FirstName,
			@MiddleName,
			@LastName,
			@EmployeeID,
			@RelationID,
			@BirthDate,
			@VisaTypeID,
			@CreatedBy,
			GETDATE()
		)
		SELECT @DependentID =(SELECT ID FROM @IDTable);  
	END
	ELSE
	BEGIN
		UPDATE [dbo].[Dependent]
		SET
			FirstName = @FirstName,
			MiddleName = @MiddleName,
			LastName = @LastName,
			EmployeeID = @EmployeeID,
			RelationID = @RelationID,
			BirthDate = @BirthDate,
			VisaTypeID = @VisaTypeID,
			ModifiedBy = @ModifiedBy,
			ModifiedDate = GETDATE()
		WHERE DependentID = @DependentID
	END
	IF @@ERROR=0   
		SET @ReturnCode  = @DependentID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END
