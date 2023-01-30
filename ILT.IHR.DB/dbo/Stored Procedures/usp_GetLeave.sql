--===============================================================  
-- Author : Sudeep Kukreti  
-- Created Date : 12/05/2020  
-- Description : Select SP for Leave  
--  
-- Revision History:  
-----------------------------------------------------------------  
-- Date            By          Description  
--  
--===============================================================  
   
CREATE PROCEDURE [dbo].[usp_GetLeave]  
 @LeaveID int = NULL,  
 @EmployeeID int = NULL,  
 @Title varchar(100) = NULL,  
 @Detail varchar(500) = NULL,  
 @StartDate date = NULL,  
 @EndDate date = NULL,  
 @LeaveTypeID int = NULL,  
 @RequesterID int = NULL,  
 @ApproverID int = NULL,  
 @StatusID int = NULL,  
 @Comment varchar(500) = NULL,  
 @CreatedBy varchar(50) = NULL,  
 @CreatedDate datetime = NULL,  
 @ModifiedBy varchar(50) = NULL,  
 @ModifiedDate datetime = NULL,  
 @TimeStamp timestamp = NULL  
AS   
BEGIN  
 SET NOCOUNT ON;  
 SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;  
   
 SELECT   
  L.LeaveID,  
  L.EmployeeID,  
  E.FirstName + ' ' + E.LastName as EmployeeName,  
  L.Title,  
  L.Detail,  
  L.StartDate,  
  L.EndDate,  
  L.IncludesHalfDay,  
  L.Duration,  
  L.LeaveTypeID,  
  LT.ValueDesc AS LeaveType,  
  L.RequesterID,  
     R.FirstName + ' ' + R.LastName as Requester,  
  L.ApproverID,  
  A.FirstName + ' ' + A.LastName Approver,  
  L.StatusID,  
  EA.LinkID,  
  LS.Value AS StatusValue,  
  LS.ValueDesc AS Status,  
  L.Comment,  
  E.Country,
  L.CreatedBy,  
  L.CreatedDate,  
  L.ModifiedBy,  
  L.ModifiedDate,  
  L.TimeStamp  
 FROM [dbo].[Leave] L  
 LEFT JOIN [dbo].[ListValue] LT ON L.LeaveTypeID = LT.ListValueID   
 INNER JOIN [dbo].[Employee] E ON L.EmployeeID = E.EmployeeID    
 INNER JOIN [dbo].[Employee] R ON L.RequesterID = R.EmployeeID   
 LEFT JOIN [dbo].[ListValue] LS ON L.StatusID = LS.ListValueID   
 INNER JOIN [dbo].[Employee] A ON L.ApproverID = A.EmployeeID   
 LEFT JOIN dbo.Module M ON M.ModuleShort = 'LEAVEREQUEST'  
 LEFT JOIN dbo.EmailApproval EA ON M.ModuleID = EA.ModuleID AND L.LeaveID = EA.ID AND EA.LinkID <> '00000000-0000-0000-0000-000000000000'  
   
  
 WHERE L.LeaveID = ISNULL(@LeaveID, L.LeaveID)  
 AND L.EmployeeID = ISNULL(@EmployeeId, L.EmployeeID)  
 AND L.ApproverID = ISNULL(@ApproverID, L.ApproverID)  
  
 IF(ISNULL(@EmployeeID,0) <> 0)  
  EXEC [dbo].usp_GetLeaveBalance @EmployeeID = @EmployeeID  
   
END