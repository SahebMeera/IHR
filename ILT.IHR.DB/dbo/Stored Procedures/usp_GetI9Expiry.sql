--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 02/24/2021
-- Description : Select SP for FormI9
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetI9Expiry]
	@I94ExpiryDate	date	=	NULL
AS 
BEGIN
 
  --Declare @I94ExpiryDate date = '2022-01-01'  
 
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	;WITH CTE AS 
	(
		SELECT FormI9ID, EmployeeID, ROW_NUMBER() OVER (PARTITION BY EmployeeID ORDER BY CreatedDate DESC) AS RN  FROM FormI9 
	)
 
	SELECT 
		EI.FormI9ID,
		EI.EmployeeID,
		EI.FirstName,
		EI.MiddleName,
		EI.LastName,
		EI.FirstName + ' ' + EI.LastName AS EmployeeName,
		EI.StartDate,
		EI.EndDate,
		EI.Address1,
		EI.Address2,
		EI.City,
		EI.State,
		EI.Country,
		EI.ZipCode,
		EI.BirthDate,
		EI.SSN,
		EI.Phone,
		EI.Email,
		EI.WorkAuthorizationID,
		WA.ValueDesc AS WorkAuthorization,
		EI.ANumber,
		EI.USCISNumber,
		EI.I94Number,
		EI.I94ExpiryDate,
		EI.PassportNumber,
		EI.PassportCountry,
		EI.HireDate,
		EI.ListADocumentTitleID,
		LA.I9DocName AS ListADocumentTitle,
		--EI.ListAIssuingAuthority,
		EI.ListADocumentNumber,
		--EI.ListAStartDate,
		EI.ListAExpirationDate,
		EI.ListBDocumentTitleID,
		LB.I9DocName AS ListBDocumentTitle,
		--EI.ListBIssuingAuthority,
		EI.ListBDocumentNumber,
		--EI.ListBStartDate,
		EI.ListBExpirationDate,
		EI.ListCDocumentTitleID,
		LC.I9DocName AS ListCDocumentTitle,
		--EI.ListCIssuingAuthority,
		EI.ListCDocumentNumber,
		--EI.ListCStartDate,
		EI.ListCExpirationDate,
		EI.IsDeleted,
		EI.CreatedBy,
		EI.CreatedDate,
		EI.ModifiedBy,
		EI.ModifiedDate,
		EI.TimeStamp
	FROM [dbo].[FormI9] EI
	INNER JOIN CTE ON EI.FormI9ID = CTE.FormI9ID
	LEFT JOIN dbo.ListValue WA ON EI.WorkAuthorizationID = WA.ListValueID
	LEFT JOIN dbo.I9Document LA ON EI.ListADocumentTitleID = LA.I9DocumentID
	LEFT JOIN dbo.I9Document LB ON EI.ListBDocumentTitleID = LB.I9DocumentID
	LEFT JOIN dbo.I9Document LC ON EI.ListCDocumentTitleID = LC.I9DocumentID
	WHERE CTE.RN=1 AND
	(EI.I94ExpiryDate BETWEEN GETDATE() AND ISNULL(@I94ExpiryDate, EI.I94ExpiryDate) OR 
	 EI.ListAExpirationDate BETWEEN GETDATE() AND ISNULL(@I94ExpiryDate, EI.ListAExpirationDate) OR
     EI.ListBExpirationDate BETWEEN GETDATE() AND ISNULL(@I94ExpiryDate, EI.ListBExpirationDate) OR
     EI.ListCExpirationDate BETWEEN GETDATE() AND ISNULL(@I94ExpiryDate, EI.ListCExpirationDate))
END