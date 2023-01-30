--===============================================================
-- Author :Nimesh Patel
-- Created Date : 11/23/2021
-- Description : Select SP for ProcessWizard
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetProcessWizard]
	@ProcessWizardID	int	=	NULL,
	@Process	varchar(50)	=	NULL,	
	@Elements XML = NULL,
	@CreatedBy	varchar(50)	=	NULL,
	@CreatedDate	datetime	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate	datetime	=	NULL
AS 
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
 
	SELECT 
		ProcessWizardID,
		Process,		
		[Elements],
		CreatedBy,
		CreatedDate,
		ModifiedBy,
		ModifiedDate,
		[TimeStamp]
	FROM [dbo].[ProcessWizard]
	WHERE ProcessWizardID = ISNULL(@ProcessWizardID, ProcessWizardID)	
	ORDER by Process
END