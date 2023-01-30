--===============================================================
-- Author : Mihir Hapaliya
-- Created Date : 12/07/2020
-- Description : Select SP for TimeSheet
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[USP_GetTimeSheet]
	@TimeSheetID int = NULL,
	@EmployeeID	int	= NULL,
	@WeekEnding date = NULL,
	@ClientID int = NULL,
	@AssignmentID int = NULL,
	@StatusID int = NULL,
	@SubmittedByID	int	= NULL,
	@DocGuid uniqueidentifier = NULL,
	@SubmittedDate datetime = NULL,
	--@ApprovedByID int = NULL,
	@ApprovedDate datetime = NULL,
	@ApprovedByEmail varchar(50) = NULL,
	@ClosedByID	int	= NULL,
	@ClosedDate	datetime = NULL,
	@Comment varchar(500) = NULL,
	@CreatedBy varchar(50) = NULL,
	@CreatedDate datetime = NULL,
	@ModifiedBy	varchar(50)	= NULL,
	@ModifiedDate datetime = NULL
AS 
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;       
	SET NOCOUNT ON; 

	SELECT 
		TS.TimeSheetID,
		TS.EmployeeID,
		E.EmployeeCode,
		E.FirstName + ' ' + E.LastName AS EmployeeName, 
		TS.WeekEnding,
		TS.ClientID,
		CM.[Name] as ClientName,
		TS.AssignmentID,
		--AM.TimesheetApproverID,
		--AU.FirstName + ' ' + AU.LastName AS TSApproverName,
		AM.TSApproverEmail TSApproverEmail,
		AM.ApprovedEmailTo,
		TS.TotalHours,
		TS.FileName,
		TS.DocGuid,
		TS.StatusID,
		TSS.Value AS StatusValue,
		TSS.ValueDesc AS Status,
		TS.SubmittedByID,
		S.FirstName + ' ' + S.LastName AS SubmittedBy,
		TS.SubmittedDate,
		--TS.ApprovedByID,
		TS.ApprovedByEmail,
		--A.FirstName + ' ' + A.LastName AS ApprovedBy,
		TS.ApprovedDate,
		TS.ClosedByID,
		C.FirstName + ' ' + C.LastName AS ClosedBy,
		AM.TimesheetTypeID,
		AM.ClientManager,
		TST.ValueDesc AS TimesheetType,
		TS.ClosedDate,
		EA.EmailApprovalID,
		EA.LinkID,
		EA.ValidTime,
		TS.Comment,
		TS.CreatedBy,
		TS.CreatedDate,
		TS.ModifiedBy,
		TS.ModifiedDate
	FROM [dbo].[TimeSheet] TS
	JOIN [dbo].[Employee] E ON TS.EmployeeID = E.EmployeeID
	LEFT JOIN [dbo].[Company] CM ON TS.ClientID = CM.CompanyID
	LEFT JOIN [dbo].[User] S ON TS.SubmittedByID = S.UserID
	--LEFT JOIN [dbo].[User] A ON TS.ApprovedByID = A.UserID
	LEFT JOIN [dbo].[User] C ON TS.ClosedByID = C.UserID
	LEFT JOIN dbo.ListValue TSS ON TS.StatusID = TSS.ListValueID
	LEFT JOIN dbo.Assignment AM ON TS.AssignmentID = AM.AssignmentID
	LEFT JOIN dbo.ListValue TST ON AM.TimesheetTypeID = TST.ListValueID
	LEFT JOIN dbo.Module M ON  M.ModuleShort = 'TIMESHEET'
	LEFT JOIN dbo.EmailApproval EA ON M.ModuleID = EA.ModuleID AND TS.TimeSheetID = EA.ID AND EA.LinkID <>  CAST(0x0 AS UNIQUEIDENTIFIER)
	--LEFT JOIN [dbo].[User] AU ON AM.TSApproverEmail = AU.Email
	WHERE TS.TimeSheetID = ISNULL(@TimeSheetID, TS.TimeSheetID)
	AND ((TS.EmployeeID = ISNULL(@EmployeeID, TS.EmployeeID) OR TS.SubmittedByID = ISNULL(@SubmittedByID, TS.SubmittedByID))) 
	AND (@DocGuid = '00000000-0000-0000-0000-000000000000' OR TS.DocGuid = ISNULL(@DocGuid, TS.DocGuid))
	AND (TS.StatusID = CASE WHEN @StatusID = 0 THEN TS.StatusID ELSE @StatusID END OR TS.StatusID = ISNULL(@StatusID, TS.StatusID))
	ORDER BY Ts.CreatedDate

	--for reference
	--OR (ISNULL(@EmployeeID,0) = 0 AND AM.TSApproverEmail = @ApprovedByEmail)
	--AND (@DocGuid = '00000000-0000-0000-0000-000000000000' OR TS.DocGuid = ISNULL(@DocGuid, TS.DocGuid))
	--AND (@StatusID = 0 OR TS.StatusID = ISNULL(@StatusID, TS.StatusID))
	--AND EA.LinkID <> '00000000-0000-0000-0000-000000000000'


	IF ISNULL(@TimeSheetID, 0) <> 0
	EXEC [dbo].usp_GetTimeEntry @TimeSheetID = @TimeSheetID
END