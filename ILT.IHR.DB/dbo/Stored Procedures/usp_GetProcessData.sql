--===============================================================
-- Author :Nimesh Patel
-- Created Date : 11/23/2021
-- Description : Select SP for Process Data
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description

--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetProcessData]
	@ProcessDataID	int	=	NULL,
	@ProcessWizardID	int	=	NULL,	
	@Data XML = NULL,
	@CreatedBy	varchar(50)	=	NULL,
	@CreatedDate	datetime	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate	datetime	=	NULL
AS 
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
 
 SELECT   
  PD.ProcessDataID,  
  PD.ProcessWizardID,  
  PD.[Data],  
  PD.CreatedBy,  
  PD.CreatedDate,  
  PD.ModifiedBy,  
  PD.ModifiedDate,  
  PD.[TimeStamp],
  P.Process,
  PD.StatusId,
  LV.[ValueDesc] AS [Status],
  PD.ProcessedDate
 FROM [dbo].[ProcessData]  PD
 INNER JOIN [dbo].[ProcessWizard] P on P.ProcessWizardID=PD.ProcessWizardID
 INNER JOIN [dbo].[ListValue] LV on PD.StatusId = LV.ListValueID
 WHERE ProcessDataID = ISNULL(@ProcessDataID, ProcessDataID)   	

 IF(ISNULL(@ProcessDataID,0) <> 0)
 EXEC [dbo].[usp_GetProcessDataTicket] @ProcessDataID

END