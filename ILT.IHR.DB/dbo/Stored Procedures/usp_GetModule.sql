--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 11/18/2020
-- Description : Select SP for Module
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetModule]
	@ModuleID	int	=	NULL,
	@ModuleShort	varchar(20)	=	NULL,
	@ModuleName	varchar(50)	=	NULL,
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
		ModuleID,
		ModuleShort,
		ModuleName,
		CreatedBy,
		CreatedDate,
		ModifiedBy,
		ModifiedDate,
		TimeStamp
	FROM [dbo].[Module]
	WHERE ModuleID = ISNULL(@ModuleID, ModuleID)
END