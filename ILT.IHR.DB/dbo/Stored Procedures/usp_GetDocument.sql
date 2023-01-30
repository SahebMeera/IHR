--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 01/10/2021
-- Description : Select SP for Document
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetDocument]
	@DocumentID	int	=	NULL,
	@EmployeeID	int	=	NULL,
	@CompanyID	int	=	NULL,
	@DocumentCategoryID	int	=	NULL,
	@DocumentTypeID	int	=	NULL,
	@IssuingAuthority varchar(50) = NULL,
	@DocumentNumber	varchar(20)	=	NULL,
	@IssueDate	date	=	NULL,
	@ExpiryDate	date	=	NULL,
	@Note	varchar(50)	=	NULL,
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
		D.DocumentID,
		D.EmployeeID,
		E.EmployeeCode,
		E.FirstName + ' ' + E.LastName AS EmployeeName,
		D.CompanyID,
		C.[Name] AS CompanyName,
		D.DocumentCategoryID,
		LV.ValueDesc AS DocumentCategory,
		D.DocumentTypeID,
		LV1.ValueDesc AS DocumentType,
		D.IssuingAuthority,
		D.DocumentNumber,
		D.IssueDate,
		D.ExpiryDate,
		D.Note,
		D.CreatedBy,
		D.CreatedDate,
		D.ModifiedBy,
		D.ModifiedDate,
		D.TimeStamp
	FROM [dbo].[Document] D
	LEFT JOIN [dbo].[Employee] E ON D.EmployeeID = E.EmployeeID
	LEFT JOIN [dbo].[Company] C ON D.CompanyID = C.CompanyID
	LEFT JOIN [dbo].[ListValue] LV ON D.DocumentCategoryID = LV.ListValueID
	LEFT JOIN [dbo].[ListValue] LV1 ON D.DocumentTypeID = LV1.ListValueID
	WHERE D.DocumentID = ISNULL(@DocumentID, D.DocumentID)
	AND D.EmployeeID = ISNULL(@EmployeeID, D.EmployeeID)
	AND D.CompanyID = ISNULL(@CompanyID, C.CompanyID)
END