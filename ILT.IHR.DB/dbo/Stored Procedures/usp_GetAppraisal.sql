--===============================================================
-- Author : Meerasaheb
-- Created Date : 02/22/2021
-- Description : Select SP for Expense
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetAppraisal]
    @AppraisalID	int	=	NULL,	
	@EmployeeID	int	=	NULL,
	@ReviewYear	int	=	NULL,
	@ReviewerID int = NULL,	
	@FinalReviewerID int = NULL,
	@AssignedDate	datetime	=	NULL,
	@SubmitDate	datetime	=	NULL,
	@ReviewDate	datetime =	NULL,
	@FinalReviewDate datetime =	NULL,
	@StatusID int = NULL,
	@MgrFeedback varchar(1000) = NULL,
	@Comment varchar(500) = NULL,
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
		A.AppraisalID,
		A.ReviewYear,
		A.EmployeeID,
	    E.FirstName + ' ' + E.LastName  AS EmployeeName,
		CASE WHEN E.ManagerID IS NULL THEN '' ELSE M.FirstName + ' ' + M.LastName END AS Manager,
		A.ReviewerID,
		CASE WHEN A.ReviewerID IS NULL THEN '' ELSE R.FirstName + ' ' + R.LastName END AS Reviewer,
		A.FinalReviewerID,
		CASE WHEN A.FinalReviewerID IS NULL THEN '' ELSE FR.FirstName + ' ' + FR.LastName   END AS FinalReviewer,
		A.AssignedDate,
		A.SubmitDate,
		A.ReviewDate,
		A.FinalReviewDate,
		A.StatusID,
		LS.Value AS StatusValue,
		LS.ValueDesc AS Status,
		A.MgrFeedback,
		A.Comment,
		A.CreatedBy,
		A.CreatedDate,
		A.ModifiedBy,
		A.ModifiedDate,
		A.TimeStamp
	FROM [dbo].[Appraisal] A
     INNER JOIN [Employee] E on A.EmployeeID = E.EmployeeID
	 INNER JOIN [dbo].[ListValue] LS ON A.StatusID = LS.ListValueID
	 LEFT JOIN [dbo].[Employee] M ON E.ManagerID = M.EmployeeID
	 LEFT JOIN [dbo].[Employee] R ON A.ReviewerID = R.EmployeeID  
	 LEFT JOIN [dbo].[Employee] FR ON A.FinalReviewerID = FR.EmployeeID 
	 WHERE A.AppraisalID = ISNULL(@AppraisalID, A.AppraisalID) 
     AND (A.EmployeeID = ISNULL(@EmployeeID, A.EmployeeID) OR A.ReviewerID = ISNULL(@EmployeeID, A.EmployeeID) OR A.FinalReviewerID = ISNULL(@EmployeeID, A.EmployeeID))
	 
	 --Do not delete below procedure calls
	IF(ISNULL(@AppraisalID,0) <> 0)
	EXEC [dbo].usp_GetAppraisalDetail @AppraisalID = @AppraisalID

	IF(ISNULL(@AppraisalID,0) <> 0)
	EXEC [dbo].usp_GetAppraisalGoal @AppraisalID = @AppraisalID

END