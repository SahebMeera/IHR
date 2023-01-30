-- =============================================
-- Author:      Shubh Dhar
-- Create Date: 16/09/2021
-- Description: Select SP for audit
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetAuditLog]
	@StartDate	date	=	NULL,
	@EndDate	date	=	NULL
AS
BEGIN
     
    SET NOCOUNT ON;
	

        SELECT 
		AuditLogID,
		[Action],
		TableName,
		RecordId,
		[Values],
		CreatedDate,
		CreatedBy
		FROM [dbo].[AuditLog] 
		WHERE CreatedDate >= coalesce(@StartDate,CreatedDate) 
		AND CreatedDate <= DATEADD(day, 1, coalesce(@endDate,CreatedDate))
		ORDER BY 1 DESC
		
END