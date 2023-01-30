--===============================================================
-- Author : Rama Mohan
-- Created Date : 02/25/2021
-- Description : Insert/Update SP for FormI9
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdFormI9]
	@FormI9ID	int	=	NULL,
	@EmployeeID int = NULL,
	@FirstName varchar(50) = NULL,
	@MiddleName varchar(50) = NULL,
	@LastName varchar(50) = NULL,
	@StartDate date = NULL,
	@EndDate date = NULL,	
	@Address1 varchar(100) = NULL,
	@Address2 varchar(100) = NULL,
	@City varchar(50) = NULL,
	@State varchar(50) = NULL,
	@Country varchar(50) = NULL,
	@ZipCode varchar(10) = NULL,
	@BirthDate date = NULL,
	@SSN varchar(9) = NULL,
	@Phone varchar(10) = NULL,
	@Email varchar(50) = NULL,
	@WorkAuthorizationID int = NULL,
	@ANumber varchar(10) = NULL,
	@USCISNumber varchar(10) = NULL,
	@I94Number varchar(11) = NULL,
	@I94ExpiryDate	date	=	NULL,
	@PassportNumber varchar(20) = NULL,
	@PassportCountry varchar(50) = NULL,
	@HireDate date = NULL,
	@ListADocumentTitleID int = NULL,
	@ListAIssuingAuthority varchar(50) = NULL,
	@ListADocumentNumber varchar(50) = NULL,
	@ListAStartDate date = NULL,
	@ListAExpirationDate date = NULL,
	@ListBDocumentTitleID int = NULL,
	@ListBIssuingAuthority varchar(50) = NULL,
	@ListBDocumentNumber varchar(50) = NULL,
	@ListBStartDate date = NULL,
	@ListBExpirationDate date = NULL,
	@ListCDocumentTitleID int = NULL,
	@ListCIssuingAuthority varchar(50) = NULL,
	@ListCDocumentNumber varchar(50) = NULL,
	@ListCStartDate date = NULL,
	@ListCExpirationDate date = NULL,
	@IsDeleted bit = NULL,
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
	DECLARE @tabFormI9 TABLE
	(
		FormI9ID	int,
		EmployeeID int,
		FirstName varchar(50),
		MiddleName varchar(50),
		LastName varchar(50),
		StartDate date,
		EndDate date,	
		Address1 varchar(100),
		Address2 varchar(100),
		City varchar(50),
		State varchar(50),
		Country varchar(50),
		ZipCode varchar(10),
		BirthDate date,
		SSN varchar(9),
		Phone varchar(10),
		Email varchar(50),
		WorkAuthorizationID int,
		ANumber varchar(10),
		USCISNumber varchar(10),
		I94Number varchar(11),
		I94ExpiryDate	date,
		PassportNumber varchar(20),
		PassportCountry varchar(50),
		HireDate date,
		ListADocumentTitleID int,
		ListAIssuingAuthority varchar(50),
		ListADocumentNumber varchar(50),
		ListAStartDate date,
		ListAExpirationDate date,
		ListBDocumentTitleID int,
		ListBIssuingAuthority varchar(50),
		ListBDocumentNumber varchar(50),
		ListBStartDate date,
		ListBExpirationDate date,
		ListCDocumentTitleID int,
		ListCIssuingAuthority varchar(50),
		ListCDocumentNumber varchar(50),
		ListCStartDate date,
		ListCExpirationDate date,
		IsDeleted bit,
		CreatedBy	varchar(50),
		CreatedDate	datetime,
		ModifiedBy	varchar(50),
		ModifiedDate	datetime
	)
	
	IF NOT EXISTS(SELECT 1 FROM FormI9 WHERE FormI9ID = ISNULL(@FormI9ID,0))  
	BEGIN
		INSERT INTO [dbo].[FormI9]
		(
			EmployeeID,
			FirstName,
			MiddleName,
			LastName,
			StartDate,
			EndDate,	
			Address1,
			Address2,
			City,
			State,
			Country,
			ZipCode,
			BirthDate,
			SSN,
			Phone,
			Email,
			WorkAuthorizationID,
			ANumber,
			USCISNumber,
			I94Number,
			I94ExpiryDate,
			PassportNumber,
			PassportCountry,
			HireDate,
			ListADocumentTitleID,
			ListAIssuingAuthority,
			ListADocumentNumber,
			ListAStartDate,
			ListAExpirationDate,
			ListBDocumentTitleID,
			ListBIssuingAuthority,
			ListBDocumentNumber,
			ListBStartDate,
			ListBExpirationDate,
			ListCDocumentTitleID,
			ListCIssuingAuthority,
			ListCDocumentNumber,
			ListCStartDate,
			ListCExpirationDate,
			IsDeleted,
			CreatedBy,
			CreatedDate
		)
	OUTPUT INSERTED.FormI9ID INTO @IDTable 
	VALUES
		(
			@EmployeeID,
			@FirstName,
			@MiddleName,
			@LastName,
			@StartDate,
			@EndDate,	
			@Address1,
			@Address2,
			@City,
			@State,
			@Country,
			@ZipCode,
			@BirthDate,
			@SSN,
			@Phone,
			@Email,
			@WorkAuthorizationID,
			@ANumber,
			@USCISNumber,
			@I94Number,
			@I94ExpiryDate,
			@PassportNumber,
			@PassportCountry,
			@HireDate,
			@ListADocumentTitleID,
			@ListAIssuingAuthority,
			@ListADocumentNumber,
			@ListAStartDate,
			@ListAExpirationDate,
			@ListBDocumentTitleID,
			@ListBIssuingAuthority,
			@ListBDocumentNumber,
			@ListBStartDate,
			@ListBExpirationDate,
			@ListCDocumentTitleID,
			@ListCIssuingAuthority,
			@ListCDocumentNumber,
			@ListCStartDate,
			@ListCExpirationDate,
			@IsDeleted,
			@CreatedBy,
			GETDATE()
		)
		SELECT @FormI9ID =(SELECT ID FROM @IDTable);  

		--IF EXISTS(SELECT 1 FROM FormI9 WHERE EmployeeID=ISNULL(@EmployeeID,0))  
		--BEGIN
		--	UPDATE EI9
		--	SET EI9.EndDate = DATEADD(day,-1, @StartDate)
		--	FROM FormI9 EI9
		--	WHERE EI9.StartDate < @StartDate AND EI9.EndDate IS NULL 
		--END
	END
	ELSE
	BEGIN
		UPDATE [dbo].[FormI9]
		SET
			EmployeeID = @EmployeeID,
			FirstName = @FirstName,
			MiddleName = @MiddleName,
			LastName = @LastName,
			StartDate = @StartDate,
			EndDate = @EndDate,	
			Address1 = @Address1,
			Address2 = @Address2,
			City = @City,
			State = @State,
			Country = @Country,
			ZipCode = @ZipCode,
			BirthDate = @BirthDate,
			SSN = @SSN,
			Phone = @Phone,
			Email = @Email,
			WorkAuthorizationID = @WorkAuthorizationID,
			ANumber = @ANumber,
			USCISNumber = @USCISNumber,
			I94Number = @I94Number,
			I94ExpiryDate = @I94ExpiryDate,
			PassportNumber = @PassportNumber,
			PassportCountry = @PassportCountry,
			HireDate = @HireDate,
			ListADocumentTitleID = @ListADocumentTitleID,
			ListAIssuingAuthority = @ListAIssuingAuthority,
			ListADocumentNumber = @ListADocumentNumber,
			ListAStartDate = @ListAStartDate,
			ListAExpirationDate = @ListAExpirationDate,
			ListBDocumentTitleID = @ListBDocumentTitleID,
			ListBIssuingAuthority = @ListBIssuingAuthority,
			ListBDocumentNumber = @ListBDocumentNumber,
			ListBStartDate = @ListBStartDate,
			ListBExpirationDate = @ListBExpirationDate,
			ListCDocumentTitleID = @ListCDocumentTitleID,
			ListCIssuingAuthority = @ListCIssuingAuthority,
			ListCDocumentNumber = @ListCDocumentNumber,
			ListCStartDate = @ListCStartDate,
			ListCExpirationDate = @ListCExpirationDate,
			IsDeleted = @IsDeleted,
			ModifiedBy = @ModifiedBy,
			ModifiedDate = GETDATE()
			OUTPUT DELETED.FormI9ID,DELETED.EmployeeID,DELETED.FirstName, DELETED.MiddleName, DELETED.LastName,
					DELETED.StartDate,DELETED.EndDate,DELETED.Address1, DELETED.Address2, DELETED.City,
					DELETED.State,DELETED.Country,DELETED.ZipCode, DELETED.BirthDate, DELETED.SSN,
					DELETED.Phone,DELETED.Email,DELETED.WorkAuthorizationID, DELETED.ANumber, DELETED.USCISNumber,
					DELETED.I94Number,DELETED.I94ExpiryDate,DELETED.PassportNumber, DELETED.PassportCountry, DELETED.HireDate,
					DELETED.ListADocumentTitleID,DELETED.ListAIssuingAuthority,DELETED.ListADocumentNumber, DELETED.ListAStartDate, DELETED.ListAExpirationDate,
					DELETED.ListBDocumentTitleID,DELETED.ListBIssuingAuthority,DELETED.ListBDocumentNumber, DELETED.ListBStartDate, DELETED.ListBExpirationDate,
					DELETED.ListCDocumentTitleID,DELETED.ListCIssuingAuthority,DELETED.ListCDocumentNumber, DELETED.ListCStartDate, DELETED.ListCExpirationDate,
					DELETED.IsDeleted,DELETED.[CreatedBy],DELETED.[CreatedDate], DELETED.[ModifiedBy],DELETED.[ModifiedDate]  
					INTO @tabFormI9
                FROM [dbo].[FormI9] F
		WHERE FormI9ID = @FormI9ID


		INSERT INTO FormI9ChangeSet
			(
				FormI9ID,
				EmployeeID,
				FirstName,
				MiddleName,
				LastName,
				StartDate,
				EndDate,	
				Address1,
				Address2,
				City,
				State,
				Country,
				ZipCode,
				BirthDate,
				SSN,
				Phone,
				Email,
				WorkAuthorizationID,
				ANumber,
				USCISNumber,
				I94Number,
				I94ExpiryDate,
				PassportNumber,
				PassportCountry,
				HireDate,
				ListADocumentTitleID,
				ListAIssuingAuthority,
				ListADocumentNumber,
				ListAStartDate,
				ListAExpirationDate,
				ListBDocumentTitleID,
				ListBIssuingAuthority,
				ListBDocumentNumber,
				ListBStartDate,
				ListBExpirationDate,
				ListCDocumentTitleID,
				ListCIssuingAuthority,
				ListCDocumentNumber,
				ListCStartDate,
				ListCExpirationDate,
				IsDeleted,
				CreatedBy,
				CreatedDate,
				ModifiedBy,
				ModifiedDate
			)
			SELECT 
				TF.FormI9ID,
				TF.EmployeeID,
				TF.FirstName,
				TF.MiddleName,
				TF.LastName,
				TF.StartDate,
				TF.EndDate,	
				TF.Address1,
				TF.Address2,
				TF.City,
				TF.State,
				TF.Country,
				TF.ZipCode,
				TF.BirthDate,
				TF.SSN,
				TF.Phone,
				TF.Email,
				TF.WorkAuthorizationID,
				TF.ANumber,
				TF.USCISNumber,
				TF.I94Number,
				TF.I94ExpiryDate,
				TF.PassportNumber,
				TF.PassportCountry,
				TF.HireDate,
				TF.ListADocumentTitleID,
				TF.ListAIssuingAuthority,
				TF.ListADocumentNumber,
				TF.ListAStartDate,
				TF.ListAExpirationDate,
				TF.ListBDocumentTitleID,
				TF.ListBIssuingAuthority,
				TF.ListBDocumentNumber,
				TF.ListBStartDate,
				TF.ListBExpirationDate,
				TF.ListCDocumentTitleID,
				TF.ListCIssuingAuthority,
				TF.ListCDocumentNumber,
				TF.ListCStartDate,
				TF.ListCExpirationDate,
				TF.IsDeleted,			
				TF.CreatedBy,
				TF.CreatedDate,
				TF.ModifiedBy,
				TF.ModifiedDate
			FROM @tabFormI9 TF
			JOIN FormI9 F ON TF.FormI9ID = F.FormI9ID
			WHERE 
				TF.FirstName <> F.FirstName
				OR TF.MiddleName <> F.MiddleName
				OR TF.LastName <> F.LastName
				OR TF.StartDate <> F.StartDate
				OR TF.EndDate <> F.EndDate	
				OR TF.Address1 <> F.Address1
				OR TF.Address2 <> F.Address2
				OR TF.City <> F.City
				OR TF.State <> F.State
				OR TF.Country <> F.Country
				OR TF.ZipCode <> F.ZipCode
				OR TF.BirthDate <> F.BirthDate
				OR TF.SSN <> F.SSN
				OR TF.Phone <> F.Phone
				OR TF.Email <> F.Email
				OR TF.WorkAuthorizationID <> F.WorkAuthorizationID
				OR TF.ANumber <> F.ANumber
				OR TF.USCISNumber <> F.USCISNumber
				OR TF.I94Number <> F.I94Number
				OR TF.I94ExpiryDate <> F.I94ExpiryDate
				OR TF.PassportNumber <> F.PassportNumber
				OR TF.PassportCountry <> F.PassportCountry
				OR TF.HireDate <> F.HireDate
				OR TF.ListADocumentTitleID <> F.ListADocumentTitleID
				OR TF.ListAIssuingAuthority <> F.ListAIssuingAuthority
				OR TF.ListADocumentNumber <> F.ListADocumentNumber
				OR TF.ListAStartDate <> F.ListAStartDate
				OR TF.ListAExpirationDate <> F.ListAExpirationDate
				OR TF.ListBDocumentTitleID <> F.ListBDocumentTitleID
				OR TF.ListBIssuingAuthority <> F.ListBIssuingAuthority
				OR TF.ListBDocumentNumber <> F.ListBDocumentNumber
				OR TF.ListBStartDate <> F.ListBStartDate
				OR TF.ListBExpirationDate <> F.ListBExpirationDate
				OR TF.ListCDocumentTitleID <> F.ListCDocumentTitleID
				OR TF.ListCIssuingAuthority <> F.ListCIssuingAuthority
				OR TF.ListCDocumentNumber <> F.ListCDocumentNumber
				OR TF.ListCStartDate <> F.ListCStartDate
				OR TF.ListCExpirationDate <> F.ListCExpirationDate
				OR TF.IsDeleted <> F.IsDeleted
	END
	
	IF @@ERROR=0   
		SET @ReturnCode  = @FormI9ID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END