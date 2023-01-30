--===============================================================
-- Author : Rama Mohan
-- Created Date : 02/08/2021
-- Description : Select SP for End Client
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetEndClient]
	@EndClientID	int	=	NULL,
	@Name	varchar(50)	=	NULL,
	@CompanyID int = NULL,
	@Address1	varchar(100)	=	NULL,
	@Address2	varchar(100)	=	NULL,
	@City	varchar(50)	=	NULL,
	@State	varchar(50)	=	NULL,
	@Country	varchar(50)	=	NULL,
	@ZipCode	varchar(10)	=	NULL,	
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
		EndClientID,
		Name,
		CompanyID,
		Address1,
		Address2,
		City,
		State,
		Country,
		ZipCode,		
		TaxID,		
		CreatedBy,
		CreatedDate,
		ModifiedBy,
		ModifiedDate,
		TimeStamp
	FROM [dbo].[EndClient]
	WHERE EndClientID = ISNULL(@EndClientID, EndClientID) 
END