--===============================================================
-- Author : Mihir Hapaliya
-- Created Date : 12/07/2020
-- Description : Delete SP for TimeEntry
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By			Description
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_DeleteTimeEntry]
	@TimeEntryID	int
AS 
BEGIN
	DELETE FROM  [dbo].[TimeEntry]
	WHERE TimeEntryID = @TimeEntryID
END