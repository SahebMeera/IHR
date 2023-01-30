--===============================================================
-- Author : Mihir Hapaliya
-- Created Date : 12/18/2020
-- Description : Insert/Update SP for Notification
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdNotification]
	@TableName varchar(50) = NULL,
	@RecordID int = NULL,
	@UserID int = NULL,
	@ReturnCode INT = 0 OUTPUT 
AS 
BEGIN
	SET NOCOUNT ON;  
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	
		UPDATE [dbo].[Notification]
		SET IsAck = 1,
			AckDate = GETDATE()
		WHERE TableName = @TableName AND RecordID = @RecordID AND UserID = @UserID 
	
	IF @@ERROR=0   
		SET @ReturnCode  = @RecordID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
END