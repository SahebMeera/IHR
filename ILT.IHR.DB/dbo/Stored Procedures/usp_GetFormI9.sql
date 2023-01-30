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
 
CREATE PROCEDURE [dbo].[usp_GetFormI9]
	@FormI9ID	int	=	NULL,
	@EmployeeID	int	=	NULL,
	@FirstName	varchar(50)	=	NULL,
	@MiddleName	varchar(50)	=	NULL,
	@LastName	varchar(50)	=	NULL,
	@StartDate	date	=	NULL,
	@EndDate	date	=	NULL,
	@Address1	varchar(100)	=	NULL,
	@Address2	varchar(100)	=	NULL,
	@City	varchar(50)	=	NULL,
	@State	varchar(50)	=	NULL,
	@Country	varchar(50)	=	NULL,
	@ZipCode	varchar(10)	=	NULL,
	@BirthDate	date	=	NULL,
	@SSN	varchar(9)	=	NULL,
	@Phone	varchar(10)	=	NULL,
	@Email	varchar(50)	=	NULL,
	@WorkAuthorizationID	int	=	NULL,
	@USCISNumber	varchar(10)	=	NULL,
	@ANumber	varchar(9)	=	NULL,
	@I94Number	varchar(11)	=	NULL,
	@I94ExpiryDate	date	=	NULL,
	@PassportNumber	varchar(20)	=	NULL,
	@PassportCountry	varchar(50)	=	NULL,
	@HireDate	date	=	NULL,
	@ListADocumentTitleID	int	=	NULL,
	@ListAIssuingAuthority	varchar(50)	=	NULL,
	@ListADocumentNumber	varchar(50)	=	NULL,
	@ListAStartDate	date	=	NULL,
	@ListAExpirationDate	date	=	NULL,
	@ListBDocumentTitleID	int	=	NULL,
	@ListBIssuingAuthority	varchar(50)	=	NULL,
	@ListBDocumentNumber	varchar(50)	=	NULL,
	@ListBStartDate	date	=	NULL,
	@ListBExpirationDate	date	=	NULL,
	@ListCDocumentTitleID	int	=	NULL,
	@ListCIssuingAuthority	varchar(50)	=	NULL,
	@ListCDocumentNumber	varchar(50)	=	NULL,
	@ListCStartDate	date	=	NULL,
	@ListCExpirationDate	date	=	NULL,
	@IsDeleted	bit	=	NULL,
	@CreatedBy	varchar(50)	=	NULL,
	@CreatedDate	datetime	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate	datetime	=	NULL,
	@TimeStamp	timestamp	=	NULL
AS 
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
 
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
		EI.ListAIssuingAuthority,
		EI.ListADocumentNumber,
		EI.ListAStartDate,
		EI.ListAExpirationDate,
		EI.ListBDocumentTitleID,
		LB.I9DocName AS ListBDocumentTitle,
		EI.ListBIssuingAuthority,
		EI.ListBDocumentNumber,
		EI.ListBStartDate,
		EI.ListBExpirationDate,
		EI.ListCDocumentTitleID,
		LC.I9DocName AS ListCDocumentTitle,
		EI.ListCIssuingAuthority,
		EI.ListCDocumentNumber,
		EI.ListCStartDate,
		EI.ListCExpirationDate,
		EI.IsDeleted,
		EI.CreatedBy,
		EI.CreatedDate,
		EI.ModifiedBy,
		EI.ModifiedDate,
		EI.TimeStamp
	FROM [dbo].[FormI9] EI
	LEFT JOIN dbo.ListValue WA ON EI.WorkAuthorizationID = WA.ListValueID
	LEFT JOIN dbo.I9Document LA ON EI.ListADocumentTitleID = LA.I9DocumentID
	LEFT JOIN dbo.I9Document LB ON EI.ListBDocumentTitleID = LB.I9DocumentID
	LEFT JOIN dbo.I9Document LC ON EI.ListCDocumentTitleID = LC.I9DocumentID
	WHERE EI.FormI9ID = ISNULL(@FormI9ID, EI.FormI9ID)
	AND EI.EmployeeID = ISNULL(@EmployeeID, EI.EmployeeID)
 
END