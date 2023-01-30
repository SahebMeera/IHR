--===============================================================
-- Author : Mihir Hapaliya
-- Created Date : 11/30/2021
-- Description : Select SP for User Notification
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetUserNotification]
		@UserID int = NULL
AS 
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;       
	SET NOCOUNT ON; 

	SELECT 
		N.NotificationID,
		N.TableName as Module,
		N.RecordID,
		E.FirstName + ' ' + E.LastName as EmployeeName, 
		N.UserID,
		N.IsAck
	FROM [dbo].[Notification]  N
	left join Employee E on N.RecordID = E.EmployeeID
	Where  N.UserID = @UserID AND N.IsAck = 0
	UNION
	SELECT 0 as NotificationID, 
		'Leave' as Module, 
		LeaveID as RecordID,
		RE.FirstName + ' ' + RE.LastName as EmployeeName,
		L.ApproverID as UserID,
		0 as IsAck
	FROM Leave L
	LEFT JOIN Employee E on L.ApproverID = E.EmployeeID 
	LEFT JOIN [User] U on U.EmployeeID = E.EmployeeID
	LEFT JOIN Employee RE on RE.EmployeeID = L.RequesterID
	WHERE U.UserID = @UserID and statusID = 56
	UNION
	SELECT 0 as NotificationID, 
		'Timesheet' as Module, 
		TS.TimeSheetID as RecordID,
		E.FirstName + ' ' + E.LastName AS EmployeeName,
		TS.SubmittedByID as UserID,
		0 as IsAck
		FROM [dbo].[TimeSheet] TS
	JOIN [dbo].[Employee] E ON TS.EmployeeID = E.EmployeeID
	LEFT JOIN [dbo].[Company] CM ON TS.ClientID = CM.CompanyID
	LEFT JOIN [dbo].[User] S ON TS.SubmittedByID = S.UserID
	LEFT JOIN dbo.Assignment AM ON TS.AssignmentID = AM.AssignmentID
	JOIN [dbo].[User] U ON U.UserID = @UserID AND u.Email = AM.TSApproverEmail
	WHERE (TS.EmployeeID = TS.EmployeeID OR TS.SubmittedByID = @UserID)
	AND  TS.StatusID = 62 
END