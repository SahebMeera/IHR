--===============================================================
-- Author : Rama Mohan
-- Created Date : 02/25/2021
-- Description : Insert/Update SP for EmployeeW4
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdEmployeeW4]
	@EmployeeW4ID	int	=	NULL,
	@EmployeeID int = NULL,
	@W4TypeID int = NULL,
	@SSN varchar(9) = NULL,
	@WithHoldingStatusID int = NULL,
	@Allowances int = NULL,
	@IsMultipleJobsOrSpouseWorks bit = NULL,
	@QualifyingChildren int = NULL,
	@OtherDependents int = NULL,
	@OtherIncome decimal(18,2) = NULL,
	@Deductions decimal(18,2) = NULL,
	@StartDate date = NULL,
	@EndDate date = NULL,	
	@CreatedBy	varchar(50)	=	NULL,
	@CreatedDate	datetime	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate	datetime	=	NULL,
	@ReturnCode INT = 0 OUTPUT 

AS 
BEGIN
	SET NOCOUNT ON;  
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	DECLARE @IDTable TABLE(ID INT)
	
	IF NOT EXISTS(SELECT 1 FROM EmployeeW4 WHERE EmployeeW4ID = ISNULL(@EmployeeW4ID,0))  
	BEGIN
		INSERT INTO [dbo].[EmployeeW4]
		(
			EmployeeID,
			W4TypeID,
			--SSN,
			WithHoldingStatusID,
			Allowances,
			IsMultipleJobsOrSpouseWorks,
			QualifyingChildren,
			OtherDependents,
			OtherIncome,
			Deductions,
			StartDate,
			EndDate,	
			CreatedBy,
			CreatedDate
		)
	OUTPUT INSERTED.EmployeeW4ID INTO @IDTable 
	VALUES
		(
			@EmployeeID,
			@W4TypeID,
			--@SSN,
			@WithHoldingStatusID,
			@Allowances,
			@IsMultipleJobsOrSpouseWorks,
			@QualifyingChildren,
			@OtherDependents,
			@OtherIncome,
			@Deductions,
			@StartDate,
			@EndDate,	
			@CreatedBy,
			GETDATE()
		)
		SELECT @EmployeeW4ID =(SELECT ID FROM @IDTable);  

		IF EXISTS(SELECT 1 FROM EmployeeW4 WHERE EmployeeID=ISNULL(@EmployeeID,0))  
		BEGIN
			UPDATE EW4
			SET EW4.EndDate = DATEADD(day,-1, @StartDate)
			FROM EmployeeW4 EW4
			WHERE EW4.StartDate < @StartDate AND EW4.EndDate IS NULL 
		END
	END
	ELSE
	BEGIN
		UPDATE [dbo].[EmployeeW4]
		SET
			EmployeeID = @EmployeeID,
			W4TypeID = @W4TypeID,
			--SSN = @SSN,
			WithHoldingStatusID = @WithHoldingStatusID,
			Allowances = @Allowances,
			IsMultipleJobsOrSpouseWorks = @IsMultipleJobsOrSpouseWorks,
			QualifyingChildren = @QualifyingChildren,
			OtherDependents = @OtherDependents,
			OtherIncome = @OtherIncome,
			Deductions = @Deductions,
			StartDate = @StartDate,
			EndDate = @EndDate,
			ModifiedBy = @ModifiedBy,
			ModifiedDate = GETDATE()
		WHERE EmployeeW4ID = @EmployeeW4ID
	END
	IF @@ERROR=0   
		SET @ReturnCode  = @EmployeeW4ID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END