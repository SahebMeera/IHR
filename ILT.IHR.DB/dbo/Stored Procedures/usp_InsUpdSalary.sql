--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 07/27/2020
-- Description : Insert/Update SP for Salary
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By			Description

--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdSalary]
@SalaryID int =	NULL,
	@EmployeeID int =	NULL,
	@BasicPay decimal(10, 2) =	NULL,
	@HRA decimal(10, 2) = NULL,
	@LTA decimal(10, 2) = NULL,
	@Bonus decimal(10, 2) = NULL,
	@EducationAllowance decimal(10, 2) = NULL,
	@Conveyance decimal(10, 2) = NULL,
	@MealAllowance decimal(10, 2) = NULL,
	@TelephoneAllowance decimal(10, 2) = NULL,
	@MedicalAllowance decimal(10, 2) = NULL,
	@MedicalInsurance decimal(10,2) = NULL,
	@VariablePay decimal(10, 2) = NULL,
	@Gratuity decimal(10, 2) = NULL,
	@SpecialAllowance decimal(10, 2) = NULL,
	@ProvidentFund decimal(10, 2) = NULL,
	@CostToCompany decimal(10, 2) = NULL,
	@StartDate	date	=	NULL,
	@EndDate	date	=	NULL,
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
	
	IF NOT EXISTS(SELECT 1 FROM Salary WHERE SalaryID = ISNULL(@SalaryID,0))  
	BEGIN
		INSERT INTO [dbo].[Salary]
		(
			EmployeeID,
			BasicPay,
			HRA,
			LTA,
			Bonus,
			EducationAllowance,
			Conveyance,
			MealAllowance,
			TelephoneAllowance,
			MedicalAllowance,
			MedicalInsurance,
			Gratuity,
			VariablePay,
			SpecialAllowance,
			ProvidentFund,
			CostToCompany,
			StartDate,
			EndDate,
			CreatedBy,
			CreatedDate
		)
	OUTPUT INSERTED.SalaryID INTO @IDTable 
	VALUES
		(
			@EmployeeID,
			@BasicPay,
			@HRA,
			@LTA,
			@Bonus,
			@EducationAllowance,
			@Conveyance,
			@MealAllowance,
			@TelephoneAllowance,
			@MedicalAllowance,
			@MedicalInsurance,
			@Gratuity,
			@VariablePay,
			@SpecialAllowance,
			@ProvidentFund,
			@CostToCompany,
			@StartDate,
			@EndDate,
			@CreatedBy,
			GETDATE()
		)
		SELECT @SalaryID =(SELECT ID FROM @IDTable);  
	END
	ELSE
	BEGIN
		UPDATE [dbo].[Salary]
		SET
			EmployeeID = @EmployeeID,
			BasicPay = @BasicPay,
			HRA = @HRA,
			LTA = @LTA,
			Bonus = @Bonus,
			EducationAllowance = @EducationAllowance,
			Conveyance = @Conveyance,
			MealAllowance = @MealAllowance,
			TelephoneAllowance = @TelephoneAllowance,
			MedicalAllowance = @MedicalAllowance,
			MedicalInsurance = @MedicalInsurance,
			Gratuity = @Gratuity,
			VariablePay = @VariablePay,
			SpecialAllowance = @SpecialAllowance,
			ProvidentFund = @ProvidentFund,
			CostToCompany = @CostToCompany,
			StartDate = @StartDate,
			EndDate = @EndDate,
			ModifiedBy = @ModifiedBy,
			ModifiedDate = GETDATE()
		WHERE SalaryID = @SalaryID
	END
	IF @@ERROR=0   
		SET @ReturnCode  = @SalaryID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END