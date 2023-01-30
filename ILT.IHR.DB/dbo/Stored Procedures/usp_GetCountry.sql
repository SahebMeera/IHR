--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 05/25/2020
-- Description : Select SP for Country
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetCountry]
	@CountryID	int	=	NULL,
	@CountryDesc	varchar(50)	=	NULL,
	@CreatedBy	varchar(50)	=	NULL,
	@CreatedDate	datetime	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate	datetime	=	NULL
AS 
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
 
	SELECT 
		CountryID,
		CountryDesc,
		CreatedBy,
		CreatedDate,
		ModifiedBy,
		ModifiedDate,
		TimeStamp
	FROM [dbo].[Country]
	WHERE CountryID = ISNULL(@CountryID, CountryID)

	--Do not delete below procedure calls
	IF(ISNULL(@CountryID,0) <> 0)
	EXEC [dbo].usp_GetState @CountryID = @CountryID

END
