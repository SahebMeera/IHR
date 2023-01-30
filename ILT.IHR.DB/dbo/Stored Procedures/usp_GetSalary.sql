--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 07/27/2021
-- Description : Select SP for Salary
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetSalary]
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
	@MedicalInsurance decimal(10,2) = Null,
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
	@ModifiedDate	datetime	=	NULL
AS 
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
 
	SELECT 
		S.SalaryID,
		S.EmployeeID,
		S.BasicPay,
		S.HRA,
		S.LTA,
		S.Bonus,
		S.EducationAllowance,
		S.Conveyance,
		S.MealAllowance,
		S.TelephoneAllowance,
		S.MedicalAllowance,
		S.MedicalInsurance,
		S.Gratuity,
		S.VariablePay,
		S.SpecialAllowance,
		S.ProvidentFund,
		S.CostToCompany,
		S.StartDate,
		S.EndDate,
		S.CreatedBy,
		S.CreatedDate,
		S.ModifiedBy,
		S.ModifiedDate,
		S.[TimeStamp],
		E.FirstName + ' ' + E.LastName AS EmployeeName
	FROM [dbo].[Salary] S
	INNER JOIN dbo.Employee E ON S.EmployeeID = E.EmployeeID
	WHERE S.SalaryID = ISNULL(@SalaryID, S.SalaryID) 
	AND S.EmployeeID = ISNULL(@EmployeeID, S.EmployeeID)
END