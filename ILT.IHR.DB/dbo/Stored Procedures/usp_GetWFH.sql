--===============================================================
-- Author : Sanjan Madishetti
-- Created Date : 05/04/2021
-- Description : Select SP for Leave
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetWFH]
	@WFHID	int	=	NULL,
	@EmployeeID	int	=	NULL,
	@Title	varchar(100)	=	NULL,
	@StartDate	date	=	NULL,
	@EndDate	date	=	NULL,
	@RequesterID	int	=	NULL,
	@ApproverID	int	=	NULL,
	@StatusID	int	=	NULL,
	@Comment	varchar(500)	=	NULL,
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
		W.WFHID,
		W.EmployeeID,
		E.FirstName + ' ' + E.LastName as EmployeeName,
		W.Title,
		W.StartDate,
		W.EndDate,
		W.RequesterID,
	    R.FirstName + ' ' + R.LastName as Requester,
		W.ApproverID,
		A.FirstName + ' ' + A.LastName Approver,
		W.StatusID,
		EA.LinkID,
		LS.Value AS StatusValue,
		LS.ValueDesc AS Status,
		W.Comment,
		W.CreatedBy,
		W.CreatedDate,
		W.ModifiedBy,
		W.ModifiedDate,
		W.TimeStamp
	FROM [dbo].[WFH] W
	INNER JOIN [dbo].[Employee] E ON W.EmployeeID = E.EmployeeID  
	INNER JOIN [dbo].[Employee] R ON W.RequesterID = R.EmployeeID 
	LEFT JOIN [dbo].[ListValue] LS ON W.StatusID = LS.ListValueID 
	INNER JOIN [dbo].[Employee] A ON W.ApproverID = A.EmployeeID 
	LEFT JOIN dbo.Module M ON M.ModuleShort = 'WFHREQUEST'
	LEFT JOIN dbo.EmailApproval EA ON M.ModuleID = EA.ModuleID AND W.WFHID = EA.ID AND EA.LinkID <> '00000000-0000-0000-0000-000000000000'
	WHERE W.WFHID = ISNULL(@WFHID, W.WFHID)
	AND W.EmployeeID = ISNULL(@EmployeeId, W.EmployeeID)
	AND W.ApproverID = ISNULL(@ApproverID, W.ApproverID)

END