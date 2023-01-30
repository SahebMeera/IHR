--===============================================================
-- Author : Rama Mohan
-- Created Date : 12/08/2020
-- Description : Delete SP for Holiday
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_DeleteHoliday]
	@HolidayID	int
AS 
BEGIN
	DELETE FROM  [dbo].[Holiday]
	WHERE HolidayID = @HolidayID
END