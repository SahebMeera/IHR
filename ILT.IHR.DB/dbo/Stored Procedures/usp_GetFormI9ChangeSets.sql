CREATE PROCEDURE [dbo].[usp_GetFormI9ChangeSets]
	@FormI9ID int = NULL

AS 
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
 
	SELECT 
        FI9CS.FormI9ChangeSetID,
		FI9CS.FormI9ID,
		FI9CS.EmployeeID,
		FI9CS.FirstName,
		FI9CS.MiddleName,
		FI9CS.LastName,
		FI9CS.StartDate,
		FI9CS.EndDate,
		FI9CS.Address1,
		FI9CS.Address2,
		FI9CS.City,
		FI9CS.State,
		FI9CS.Country,
		FI9CS.ZipCode,
		FI9CS.BirthDate,
		FI9CS.SSN,
		FI9CS.Phone,
		FI9CS.Email,
		FI9CS.WorkAuthorizationID,
		FI9CS.ANumber,
		FI9CS.USCISNumber,
		FI9CS.I94Number,
		FI9CS.I94ExpiryDate,
		FI9CS.PassportNumber,
		FI9CS.PassportCountry,
		FI9CS.HireDate,
		FI9CS.ListADocumentTitleID,
		ListA.I9DocName ListADocumentTitle,
		FI9CS.ListAIssuingAuthority,
		FI9CS.ListADocumentNumber,
		FI9CS.ListAStartDate,
		FI9CS.ListAExpirationDate,
		FI9CS.ListBDocumentTitleID,
		ListB.I9DocName ListBDocumentTitle,
		FI9CS.ListBIssuingAuthority,
		FI9CS.ListBDocumentNumber,
		FI9CS.ListBStartDate,
		FI9CS.ListBExpirationDate,
		FI9CS.ListCDocumentTitleID,
		ListC.I9DocName ListCDocumentTitle,
		FI9CS.ListCIssuingAuthority,
		FI9CS.ListCDocumentNumber,
		FI9CS.ListCStartDate,
		FI9CS.ListCExpirationDate,
		FI9CS.CreatedBy,
		FI9CS.CreatedDate,
		FI9CS.ModifiedBy,
		FI9CS.ModifiedDate,
		FI9CS.TimeStamp
	FROM [dbo].[FormI9ChangeSet] FI9CS
        LEFT JOIN [FormI9] F on FI9CS.FormI9ID = F.FormI9ID
		LEFT JOIN [I9Document] ListA on FI9CS.ListADocumentTitleId = ListA.I9DocumentID
		LEFT JOIN [I9Document] ListB on FI9CS.ListADocumentTitleId = ListB.I9DocumentID
		LEFT JOIN [I9Document] ListC on FI9CS.ListADocumentTitleId = ListC.I9DocumentID
	WHERE FI9CS.FormI9ID = @FormI9ID
    ORDER BY FI9CS.ModifiedDate DESC
	
END