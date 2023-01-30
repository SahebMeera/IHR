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
 
CREATE PROCEDURE [dbo].[usp_GetAppraisalDetail]
    @AppraisalDetailID	int	=	NULL,	
	@AppraisalID	int	=	NULL,	
	@ppraisalQualityID int = NULL,	
	@FinalReviewerID int = NULL,
	@EmpResponse	varchar(500)	=	NULL,
	@EmpComment	varchar(500)	=	NULL,
	@MgrResponse	varchar(500)	=	NULL,
	@MgrComment	varchar(500)	=	NULL
AS 
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

		SELECT 
		AD.AppraisalDetailID,
		AD.AppraisalID,
		AD.AppraisalQualityID,
		AQ.Quality,
		AQ.ResponseTypeID,
		LS.Value As ResponseType,
		LS.ValueDesc AS ResponseTypeDescription,
		AD.EmpResponse,
		AD.EmpComment,
		AD.MgrResponse,
		AD.MgrComment
	FROM [dbo].[AppraisalDetail] AD
	 INNER JOIN dbo.AppraisalQuality AQ On AD.AppraisalQualityID = AQ.AppraisalQualityID
	  INNER JOIN [dbo].[ListValue] LS ON AQ.ResponseTypeID = LS.ListValueID
	 WHERE AD.AppraisalDetailID = ISNULL(@AppraisalDetailID, AD.AppraisalDetailID) 
	AND AD.AppraisalID = ISNULL(@AppraisalID, AD.AppraisalID) 
END