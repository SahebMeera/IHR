--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 05/25/2020
-- Description : Select SP for State
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetState]
	@StateID	int	=	NULL,
	@CountryID	int	=	NULL,
	@StateShort	varchar(3)	=	NULL,
	@StateDesc	varchar(50)	=	NULL,
	@CreatedBy	varchar(50)	=	NULL,
	@CreatedDate	datetime	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate	datetime	=	NULL
AS 
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
 
	SELECT 
		S.StateID,
		S.CountryID,
		C.CountryDesc AS Country,
		S.StateShort,
		S.StateDesc,
		S.CreatedBy,
		S.CreatedDate,
		S.ModifiedBy,
		S.ModifiedDate,
		S.TimeStamp
	FROM [dbo].[State] S
	INNER JOIN Country C ON S.CountryID = C.CountryID
	WHERE S.StateID = ISNULL(@StateID, S.StateID) 
	AND S.CountryID = ISNULL(@CountryID, S.CountryID)
END
