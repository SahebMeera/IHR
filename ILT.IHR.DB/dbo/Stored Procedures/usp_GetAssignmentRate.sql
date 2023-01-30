--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 05/25/2020
-- Description : Select SP for AssignmentRate
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetAssignmentRate]
	@AssignmentRateID	int	=	NULL,
	@AssignmentID	int	=	NULL,
	@BillingRate	int	=	NULL,
	@PaymentRate	int	=	NULL,
	@StartDate	date	=	NULL,
	@EndDate	date	=	NULL,
	@CreatedBy	varchar(50)	=	NULL,
	@CreatedDate	datetime	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate	datetime	=	NULL
AS 
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
 
	SELECT 
		AR.AssignmentRateID,
		AR.AssignmentID,
		AR.BillingRate,
		AR.PaymentRate,
		AR.StartDate,
		AR.EndDate,
		AR.CreatedBy,
		AR.CreatedDate,
		AR.ModifiedBy,
		AR.ModifiedDate,
		AR.TimeStamp
	FROM [dbo].[AssignmentRate] AR
	INNER JOIN dbo.Assignment A ON AR.AssignmentID = A.AssignmentID
	WHERE AR.AssignmentRateID = ISNULL(@AssignmentRateID, AR.AssignmentRateID) 
	AND AR.AssignmentID = ISNULL(@AssignmentID, AR.AssignmentID)
END

