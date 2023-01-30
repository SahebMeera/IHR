--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 12/05/2020
-- Description : Select SP for Holiday
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetHoliday]
	@HolidayID	int	=	NULL,
	@Name	varchar(100)	=	NULL,
	@StartDate	date	=	NULL,
	@Country	varchar(50)	=	NULL,
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
		HolidayID,
		Name,
		StartDate,
		Country,
		CreatedBy,
		CreatedDate,
		ModifiedBy,
		ModifiedDate,
		TimeStamp		
	FROM [dbo].[Holiday]
	WHERE HolidayID = ISNULL(@HolidayID, HolidayID)
	Order By StartDate
END