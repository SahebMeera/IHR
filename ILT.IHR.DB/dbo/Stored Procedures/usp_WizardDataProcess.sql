CREATE PROCEDURE [dbo].[usp_WizardDataProcess]
	@ProcessWizardID	int	=	NULL,
	@ProcessDataID	int	=	NULL,	
	@ReturnCode INT = 0 OUTPUT 

AS 
BEGIN
	SET NOCOUNT ON;  
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	
	DECLARE @StoredProc varchar(50);
	
	SELECT @StoredProc = StoredProc FROM [dbo].[ProcessWizard] WHERE ProcessWizardID = @ProcessWizardID  	
	
	exec @StoredProc @ProcessDataID = @ProcessDataID

	IF @@ERROR=0   
		SET @ReturnCode  = @ProcessDataID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END