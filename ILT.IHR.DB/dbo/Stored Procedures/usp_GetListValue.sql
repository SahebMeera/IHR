--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 05/24/2020
-- Description : Select SP for ListValue
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetListValue]
	@ListValueID	int	=	NULL,
	@Value	varchar(20)	=	NULL,
	@ValueDesc	varchar(50)	=	NULL,
	@ListTypeID	int	=	NULL,
	@IsActive	bit	=	NULL
AS 
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;       
	SET NOCOUNT ON; 

	SELECT 
		LV.ListValueID,
		LV.ListTypeID,
		LT.Type,
		LT.TypeDesc,
		LV.Value,
		LV.ValueDesc,
		LV.IsActive,
		LV.CreatedBy,
		LV.CreatedDate,
		LV.ModifiedBy,
		LV.ModifiedDate,
		LV.TimeStamp
	FROM [dbo].[ListValue] LV
	JOIN [dbo].[ListType] LT ON LV.ListTypeID = LT.ListTypeID
	WHERE ListValueID = ISNULL(@ListValueID, ListValueID) 
	AND LV.ListTypeID = ISNULL(@ListTypeID, LV.ListTypeID)
	ORDER BY LV.ValueDesc

END
