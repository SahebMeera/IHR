--===============================================================
-- Author : Mihir Hapaliya
-- Created Date : 12/07/2020
-- Description : Delete SP for TimeSheet
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By			Description
-- 
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_DeleteTimeSheet]
	@TimeSheetID	int
AS 
BEGIN
	DELETE FROM  [dbo].[TimeSheet]
	WHERE TimeSheetID = @TimeSheetID
END