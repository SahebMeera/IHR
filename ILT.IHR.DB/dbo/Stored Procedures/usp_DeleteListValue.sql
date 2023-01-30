--===============================================================
-- Author : Mihir Hapaliya
-- Created Date : 11/18/2020
-- Description : Delete SP for List Value
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_DeleteListValue]
	@ListValueId	int,
	@ModifiedBy	varchar(50)
AS 
BEGIN
	UPDATE [dbo].[ListValue]
	SET IsActive = 0,
	ModifiedBy = @ModifiedBy,
	ModifiedDate = GETDATE()
	WHERE ListValueID = @ListValueId
END