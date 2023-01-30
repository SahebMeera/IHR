--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 07/25/2021
-- Description : Select SP for Expense
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetAppraisalGoal]
    @AppraisalGoalID	int	=	NULL,	
	@AppraisalID	int	=	NULL
AS 
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	DECLARE @tblAppraisalGoal TypeAppraisalGoal
	DECLARE @EmployeeID int, @ReviewYear int
	SELECT  @EmployeeID =  EmployeeID, @ReviewYear = ReviewYear
	FROM Appraisal WHERE AppraisalID = @AppraisalID

	IF NOT EXISTS(SELECT 1 FROM AppraisalGoal AG WHERE AG.AppraisalID = @AppraisalID )
	BEGIN
		INSERT INTO @tblAppraisalGoal
		(
			AppraisalGoalID,
			AppraisalID,
			Goal
		)
		VALUES
		(0, @AppraisalID, ''),
		(0, @AppraisalID, ''),
		(0, @AppraisalID, '')
		
		SELECT 
			AppraisalGoalID,
			AppraisalID,
			@ReviewYear AS ReviewYear,
			Goal,
			EmpResponse,
			EmpComment,
			MgrResponse,
			MgrComment
		FROM @tblAppraisalGoal
		UNION ALL
		SELECT 
			AG.AppraisalGoalID,
			AG.AppraisalID,
			A.ReviewYear,
			AG.Goal,
			AG.EmpResponse,
			AG.EmpComment,
			AG.MgrResponse,
			AG.MgrComment
		FROM AppraisalGoal AG
		JOIN Appraisal A ON A.AppraisalID = AG.AppraisalID
		WHERE AG.AppraisalGoalID = ISNULL(@AppraisalGoalID, AG.AppraisalGoalID) 
		AND A.EmployeeID = @EmployeeID AND A.ReviewYear IN (@ReviewYear, @ReviewYear-1)
		
	END
	ELSE
		SELECT 
			AG.AppraisalGoalID,
			AG.AppraisalID,
			A.ReviewYear,
			AG.Goal,
			AG.EmpResponse,
			AG.EmpComment,
			AG.MgrResponse,
			AG.MgrComment
		FROM [dbo].[AppraisalGoal] AG
		JOIN Appraisal A ON A.AppraisalID = AG.AppraisalID
		WHERE AG.AppraisalGoalID = ISNULL(@AppraisalGoalID, AG.AppraisalGoalID) 
		AND A.EmployeeID = @EmployeeID AND A.ReviewYear IN (@ReviewYear, @ReviewYear-1)
		ORDER BY A.ReviewYear, AG.AppraisalGoalID	
END