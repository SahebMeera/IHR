--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 01/19/2021
-- Description : Select SP for EmailApproval
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetEmailApproval]
	@EmailApprovalID	int	=	NULL,
	@ModuleID	int	=	NULL,
	@ID	int	=	NULL,
	--@UserID	int	=	NULL,
	@ValidTime	datetime	=	NULL,
	@IsActive	bit	=	NULL,
	@Value	varchar(100)	=	NULL,
	@LinkID	uniqueidentifier	=	NULL,
	@ApproverEmail varchar(50) = NULL,
	@CreatedBy	varchar(50)	=	NULL,
	@CreatedDate	datetime	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate	datetime	=	NULL,
	@TimeStamp	timestamp	=	NULL
AS 
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
 
	SELECT 
		E.EmailApprovalID,
		E.ModuleID,
		E.ID,
		--E.UserID,
		E.ValidTime,
		E.IsActive,
		E.Value,
		E.LinkID,
		E.ApproverEmail,
		E.EmailSubject,
		E.EmailBody,
		E.EmailFrom,
		E.EmailTo,
		E.EmailCC,
		E.EmailBCC,
		E.SentCount,
		E.ReminderDuration,
		E.SendDate,
		E.CreatedBy,
		E.CreatedDate,
		E.ModifiedBy,
		E.ModifiedDate,
		E.TimeStamp
	FROM [dbo].[EmailApproval] E
	WHERE E.EmailApprovalID = ISNULL(@EmailApprovalID, E.EmailApprovalID)
	AND E.LinkID = ISNULL(@LinkID, E.LinkID)
END