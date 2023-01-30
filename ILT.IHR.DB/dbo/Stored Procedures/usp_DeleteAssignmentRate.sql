--===============================================================
-- Author : Mihir Hapaliya
-- Created Date : 12/02/2020
-- Description : Delete SP for Assignment Rate
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By			Description
-- 
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_DeleteAssignmentRate]
	@AssignmentRateID	int
AS 
BEGIN
	DELETE FROM  [dbo].[AssignmentRate]
	WHERE AssignmentRateID = @AssignmentRateID
END