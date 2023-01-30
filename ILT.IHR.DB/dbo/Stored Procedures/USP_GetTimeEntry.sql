--===============================================================
-- Author : Mihir Hapaliya
-- Created Date : 12/07/2020
-- Description : Select SP for TimeEntry
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[USP_GetTimeEntry]
	@TimeEntryID int = NULL,
	@TimeSheetID	int	=	NULL
AS 
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;       
	SET NOCOUNT ON; 

	SELECT 
		T.TimeEntryID,
		T.TimeSheetID,
		T.WorkDate,
		T.Project,
		T.Activity,
		T.[Hours]
		-- T.CreatedBy,
		-- T.CreatedDate,
		-- T.ModifiedBy,
		-- T.ModifiedDate
	FROM [dbo].[TimeEntry] T
	JOIN [dbo].[TimeSheet] TS ON T.TimeSheetID = TS.TimeSheetID
	WHERE T.TimeEntryID = ISNULL(@TimeEntryID, T.TimeEntryID) 
	AND TS.TimeSheetID = ISNULL(@TimeSheetID, TS.TimeSheetID)
	ORDER BY T.WorkDate
END