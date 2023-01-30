--===============================================================
-- Author : Sanjan Madishetti
-- Created Date : 01/22/2021
-- Description : Select SP for LeaveAccrual
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetLeaveAccrual]
	@LeaveAccrualID	int	=	NULL,
	@Country varchar(50) = NULL,
	@AccruedDate	date	=	NULL,
	@CreatedBy	varchar(50)	=	NULL,
	@CreatedDate	datetime	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate	datetime	=	NULL,
	@TimeStamp	timestamp	=	NULL
AS 
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
 
	DECLARE @StartDate DATETIME, @EndDate DATETIME;  
  
	SELECT @StartDate = DATEADD(mm,1,DATEADD(mm, DATEDIFF(mm,1,MAX(AccruedDate)),0)) FROM LeaveAccrual WHERE Country = @Country;

	SELECT @StartDate = ISNULL(@StartDate, DATEADD(mm,-1,DATEADD(mm, DATEDIFF(mm,1,GETDATE()),0))),
		   @EndDate   = GETDATE()  --TO be replaced with GETDATE() post testing 

	;WITH Numbers (Number) as
	(SELECT row_number() OVER (ORDER BY object_id) FROM sys.all_objects)
	SELECT 
		LA.LeaveAccrualID,
		LA.Country,
		LA.AccruedDate,
		LA.CreatedBy,
		LA.CreatedDate,
		LA.ModifiedBy,
		LA.ModifiedDate,
		LA.TimeStamp
	FROM [dbo].[LeaveAccrual] LA
	WHERE LeaveAccrualID = ISNULL(@LeaveAccrualID, LeaveAccrualID)
	AND LA.Country = @Country
	UNION
	SELECT
		0 AS LeaveAccrualID,
		@Country AS Country,
		DATEADD(s,-1,DATEADD(mm, DATEDIFF(m,0,@StartDate)+number,0)),
		NULL AS CreatedBy, 
		NULL AS CreatedDate,
		NULL AS ModifiedBy,
		NULL AS ModifiedDate,
		NULL AS TimeStamp  
	FROM Numbers
	WHERE Number <=datediff(month, @StartDate, @EndDate)+1
	

END