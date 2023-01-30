--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 05/24/2020
-- Description : Insert/Update SP for Appraisal
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By				Description
--07/21/2021   Sanjan Madishetti	Altered procedure to insert/update AppraisalDetails
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdAppraisal]
	@AppraisalID	int	=	NULL,
	@EmployeeID	int	=	NULL,
	@ReviewYear	int	=	NULL,
	@ReviewerID	int	=	NULL,
	@FinalReviewerID	int	=	NULL,
	@AssignedDate	datetime	=	NULL,
	@ReviewDate	datetime	=	NULL,
	@SubmitDate	datetime	=	NULL,
	@FinalReviewDate	datetime	=	NULL,
	@StatusID	int	=	NULL,
	@MgrFeedback	varchar(1000)	=	NULL,
	@Comment	varchar(500)	=	NULL,
	@xmlAppraisalDetail xml = NULL,
	@xmlAppraisalGoal xml = NULL,
    @CreatedBy	varchar(50)	=	NULL,
	@CreatedDate datetime =	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate datetime =	NULL,
	@TimeStamp	timestamp	=	NULL,
	@ReturnCode INT = 0 OUTPUT 
AS 
BEGIN
	SET NOCOUNT ON;  
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	DECLARE @IDTable TABLE(ID INT)
	DECLARE @tblAppraisalDetail TypeAppraisalDetail
	DECLARE @tblAppraisalGoal TypeAppraisalGoal

	INSERT INTO @tblAppraisalDetail
	(
		AppraisalDetailID,
		AppraisalID,
		AppraisalQualityID,
		EmpResponse,
		EmpComment,
		MgrResponse,
		MgrComment
	)
	SELECT 
	x.v.value('AppraisalDetailID[1]','int'),
	x.v.value('AppraisalID[1]','int'),
	x.v.value('AppraisalQualityID[1]','int'),
	x.v.value('EmpResponse[1]','int'),
	x.v.value('EmpComment[1]','varchar(200)'),
	x.v.value('MgrResponse[1]','int'),
	x.v.value('MgrComment[1]','varchar(200)')
	FROM @xmlAppraisalDetail.nodes('ArrayOfAppraisalDetail/AppraisalDetail') as x(v)

	INSERT INTO @tblAppraisalGoal
	(
		AppraisalGoalID,
		AppraisalID,
		Goal,
		EmpResponse,
		EmpComment,
		MgrResponse,
		MgrComment
	)
	SELECT 
	x.v.value('AppraisalGoalID[1]','int'),
	x.v.value('AppraisalID[1]','int'),
	x.v.value('Goal[1]','varchar(500)'),
	x.v.value('EmpResponse[1]','int'),
	x.v.value('EmpComment[1]','varchar(200)'),
	x.v.value('MgrResponse[1]','int'),
	x.v.value('MgrComment[1]','varchar(200)')
	FROM @xmlAppraisalGoal.nodes('ArrayOfAppraisalGoal/AppraisalGoal') as x(v)
	
	IF NOT EXISTS(SELECT 1 FROM Appraisal WHERE AppraisalID = ISNULL(@AppraisalID,0))  
	BEGIN
		INSERT INTO [dbo].[Appraisal]
		(
			EmployeeID,
			ReviewYear,
			ReviewerID,
			FinalReviewerID,
			AssignedDate,
			ReviewDate,
			SubmitDate,
			FinalReviewDate,
			StatusID,
			MgrFeedback,
			Comment,
			CreatedBy,
			CreatedDate
		)
		OUTPUT INSERTED.AppraisalID INTO @IDTable 
		VALUES
		(
			@EmployeeID,
			@ReviewYear,
			@ReviewerID,
			@FinalReviewerID,
			@AssignedDate,
			@ReviewDate,
			@SubmitDate,
			@FinalReviewDate,
			@StatusID,
			@MgrFeedback,
			@Comment,
			@CreatedBy,
			GETDATE()
		)
		SELECT @AppraisalID=(SELECT ID FROM @IDTable);  

		INSERT INTO [dbo].[AppraisalDetail]
			(
			AppraisalID,  
			AppraisalQualityID,  
			EmpResponse,  
			EmpComment,  
			MgrResponse,  
			MgrComment  
			)
			SELECT 
			@AppraisalID,
			AppraisalQualityID,  
			EmpResponse,  
			EmpComment,  
			MgrResponse,  
			MgrComment
			FROM @tblAppraisalDetail

		INSERT INTO [dbo].[AppraisalGoal]
			(
			AppraisalID,  
			Goal,  
			EmpResponse,  
			EmpComment,  
			MgrResponse,  
			MgrComment  
			)
			SELECT 
			@AppraisalID,
			Goal,  
			EmpResponse,  
			EmpComment,  
			MgrResponse,  
			MgrComment
			FROM @tblAppraisalGoal
	END
	ELSE
	BEGIN
		UPDATE [dbo].[Appraisal]
		SET
			EmployeeID = @EmployeeID,
			ReviewYear = @ReviewYear,
			ReviewerID = @ReviewerID,
			FinalReviewerID = @FinalReviewerID,
			AssignedDate = @AssignedDate,
			ReviewDate = @ReviewDate,
			SubmitDate = @SubmitDate,
			FinalReviewDate = @FinalReviewDate,
			StatusID = @StatusID,
			MgrFeedback = @MgrFeedback,
			Comment = @Comment,
			ModifiedBy = @ModifiedBy,
			ModifiedDate = GETDATE()
			WHERE AppraisalID = ISNULL(@AppraisalID,0)

			MERGE AppraisalDetail AS TARGET
			USING @tblAppraisalDetail AS SOURCE 
			ON (TARGET.AppraisalDetailID = SOURCE.AppraisalDetailID) 
			WHEN MATCHED 
			THEN UPDATE SET TARGET.EmpResponse = SOURCE.EmpResponse, TARGET.EmpComment = SOURCE.EmpComment, TARGET.MgrResponse = SOURCE.MgrResponse, TARGET.MgrComment = SOURCE.MgrComment 
			WHEN NOT MATCHED BY TARGET 
			THEN INSERT (AppraisalID, AppraisalQualityID, EmpResponse, EmpComment, MgrResponse, MgrComment) VALUES (@AppraisalID, AppraisalQualityID, EmpResponse, EmpComment, MgrResponse, MgrComment)
			WHEN NOT MATCHED BY SOURCE AND TARGET.AppraisalID = @AppraisalID
			THEN DELETE;

			MERGE AppraisalGoal AS TARGET
			USING @tblAppraisalGoal AS SOURCE 
			ON (TARGET.AppraisalGoalID = SOURCE.AppraisalGoalID) 
			WHEN MATCHED 
			THEN UPDATE SET TARGET.Goal = SOURCE.Goal, TARGET.EmpResponse = SOURCE.EmpResponse, TARGET.EmpComment = SOURCE.EmpComment, TARGET.MgrResponse = SOURCE.MgrResponse, TARGET.MgrComment = SOURCE.MgrComment 
			WHEN NOT MATCHED BY TARGET 
			THEN INSERT (AppraisalID, Goal, EmpResponse, EmpComment, MgrResponse, MgrComment) VALUES (@AppraisalID, Goal, EmpResponse, EmpComment, MgrResponse, MgrComment)
			WHEN NOT MATCHED BY SOURCE AND TARGET.AppraisalID = @AppraisalID
			THEN DELETE;
	END

	IF @@ERROR=0   
		SET @ReturnCode  = @AppraisalID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
END