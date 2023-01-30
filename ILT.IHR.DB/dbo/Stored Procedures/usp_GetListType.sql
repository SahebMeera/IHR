--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 05/24/2020
-- Description : Select SP for ListType
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetListType]
	@ListTypeID	int	=	NULL,
	@Type	varchar(20)	=	NULL,
	@TypeDesc	varchar(100)	=	NULL
AS 
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;       
	SET NOCOUNT ON; 

	SELECT 
		LT.ListTypeID,
		LT.Type,
		LT.TypeDesc,
		LT.CreatedBy,
		LT.CreatedDate,
		LT.ModifiedBy,
		LT.ModifiedDate,
		LT.TimeStamp
	FROM [dbo].[ListType] LT
	WHERE ListTypeID = ISNULL(@ListTypeID, ListTypeID)
	ORDER BY LT.TypeDesc

	--Do not delete below procedure calls
	IF(ISNULL(@ListTypeID,0) <> 0)
	EXEC [dbo].usp_GetListValue @ListTypeID = @ListTypeID

END
