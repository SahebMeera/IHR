--===============================================================
-- Author : Mihir Hapaliya
-- Created Date : 12/17/2020
-- Description : Select SP for Notification
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetNotification]
		@TableName	varchar(50)	=	NULL,
		@UserID int = NULL
AS 
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;       
	SET NOCOUNT ON; 



	SELECT 
		N.NotificationID,
		N.TableName,
		N.ChangeSetID,
		N.RecordID,
		N.UserID,
		N.IsAck,
		N.AckDate
	FROM [dbo].[Notification]  N
	Where N.TableName = ISNULL(@TableName,N.TableName)  AND N.UserID = @UserID AND N.IsAck = 0

END