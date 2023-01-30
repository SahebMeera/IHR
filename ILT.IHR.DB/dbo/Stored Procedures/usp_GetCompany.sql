--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 05/25/2020
-- Description : Select SP for Company
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetCompany]
	@CompanyID	int	=	NULL,
	@Name	varchar(50)	=	NULL,
	@Address1	varchar(100)	=	NULL,
	@Address2	varchar(100)	=	NULL,
	@City	varchar(50)	=	NULL,
	@State	varchar(50)	=	NULL,
	@Country	varchar(50)	=	NULL,
	@ZipCode	varchar(10)	=	NULL,
	@InvoiceContactName	varchar(50)	=	NULL,
	@InvoiceContactPhone	varchar(10)	=	NULL,
	@InvoiceContactEmail	varchar(50)	=	NULL,
	@AlternateContactName	varchar(50)	=	NULL,
	@AlternateContactPhone	varchar(10)	=	NULL,
	@AlternateContactEmail	varchar(50)	=	NULL,
	@InvoicingPeriodID	int	=	NULL,
	@PaymentTermID	int	=	NULL,
	@TaxID	varchar(20)	=	NULL,
	@CreatedBy	varchar(50)	=	NULL,
	@CreatedDate	datetime	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate	datetime	=	NULL
AS 
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
 
	SELECT 
		C.CompanyID,
		C.Name,
		C.Address1,
		C.Address2,
		C.City,
		C.State,
		C.Country,
		C.ZipCode,
		C.ContactName,
		C.ContactPhone,
		C.ContactEmail,
		C.AlternateContactName,
		C.AlternateContactPhone,
		C.AlternateContactEmail,
		C.InvoiceContactName,
		C.InvoiceContactPhone,
		C.InvoiceContactEmail,
		C.AlternateInvoiceContactName,
		C.AlternateInvoiceContactPhone,
		C.AlternateInvoiceContactEmail,		
		C.InvoicingPeriodID,
		IP.ValueDesc AS InvoicingPeriod,
		C.PaymentTermID,
		PT.ValueDesc AS PaymentTerm,
		C.TaxID,
		C.CompanyTypeID,
		CT.ValueDesc AS CompanyType,
		c.IsEndClient,
		C.CreatedBy,
		C.CreatedDate,
		C.ModifiedBy,
		C.ModifiedDate,
		C.TimeStamp
	FROM [dbo].[Company] C
	LEFT JOIN dbo.ListValue IP ON C.InvoicingPeriodID = IP.ListValueID
	LEFT JOIN dbo.ListValue PT ON C.PaymentTermID = PT.ListValueID
	LEFT JOIN dbo.ListValue CT ON C.CompanyTypeID = CT.ListValueID
	WHERE C.CompanyID = ISNULL(@CompanyID, c.CompanyID)
	ORDER BY C.Name
END
