--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 05/25/2020
-- Description : Select SP for DirectDeposit
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetDirectDeposit]
	@DirectDepositID	int	=	NULL,
	@EmployeeID	int	=	NULL,
	@BankName	varchar(50)	=	NULL,
	@AccountTypeID	int	=	NULL,
	@RoutingNumber	varchar(20)	=	NULL,
	@AccountNumber	varchar(20)	=	NULL,
	@Country	varchar(50)	=	NULL,
	@State	varchar(50)	=	NULL,
	@Amount	int	=	NULL,
	@CreatedBy	varchar(50)	=	NULL,
	@CreatedDate	datetime	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate	datetime	=	NULL
AS 
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
 
	SELECT 
		DD.DirectDepositID,
		DD.EmployeeID,
		DD.BankName,
		DD.AccountTypeID,
		LV.ValueDesc AS AccountType,
		DD.RoutingNumber,
		DD.AccountNumber,
		DD.Country,
		DD.[State],
		DD.Amount,
		DD.CreatedBy,
		DD.CreatedDate,
		DD.ModifiedBy,
		DD.ModifiedDate,
		DD.TimeStamp
	FROM [dbo].[DirectDeposit] DD
	LEFT JOIN dbo.ListValue LV ON DD.AccountTypeID = LV.ListValueID
	WHERE DD.DirectDepositID = ISNULL(@DirectDepositID, DD.DirectDepositID) 
	AND DD.EmployeeID = ISNULL(@EmployeeID, DD.EmployeeID)
END
