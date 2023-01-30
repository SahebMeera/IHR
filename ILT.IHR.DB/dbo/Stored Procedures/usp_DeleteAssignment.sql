--===============================================================
-- Author : Mihir Hapaliya
-- Created Date : 12/02/2020
-- Description : Delete SP for Assignment
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By			Description
-- 
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_DeleteAssignment]
	@AssignmentID	int
AS 
BEGIN
	DELETE FROM  [dbo].[Assignment]
	WHERE AssignmentID = @AssignmentID
END