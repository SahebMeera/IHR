--===============================================================
-- Author : Mihir Hapaliya
-- Created Date : 12/10/2021
-- Description : Insert/Update SP for User Notification
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdUserNotification]
	@NotificationID int = NULL,
	@IsAck bit = 1,
	@ReturnCode INT = 0 OUTPUT 
AS 
BEGIN
	SET NOCOUNT ON;  
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	
		UPDATE [dbo].[Notification]
		SET IsAck = @IsAck,
			AckDate = GETDATE()
		WHERE NotificationID = @NotificationID 
	
	IF @@ERROR=0   
		SET @ReturnCode  = @NotificationID  
	ELSE
		SET @ReturnCode = 0
	RETURN
END