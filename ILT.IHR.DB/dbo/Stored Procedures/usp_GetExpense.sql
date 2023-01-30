--===============================================================
-- Author : Meerasaheb
-- Created Date : 02/22/2021
-- Description : Select SP for Expense
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetExpense]
	@ExpenseID	int	=	NULL,
	@EmployeeID	int	=	NULL,
	@ExpenseTypeID	int	=	NULL,
	@FileName varchar(100) = NULL,
	@Amount	int	=	NULL,
	@SubmissionDate	date	=	NULL,
	@SubmissionComment	varchar(100)	=	NULL,
	@StatusID int = NULL,
	@AmountPaid int = NULL,
	@PaymentDate date = NULL,
	@PaymentComment varchar(100) = NULL,
	@CreatedBy	varchar(50)	=	NULL,
	@CreatedDate	datetime	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate	datetime	=	NULL,
	@TimeStamp	timestamp	=	NULL
AS 
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
 
	SELECT 
		EX.ExpenseID,
		EX.EmployeeID,
		EM.FirstName + ' ' + EM.LastName AS EmployeeName,
		EX.ExpenseTypeID,
		LT.ValueDesc AS ExpenseType,
		EX.FileName,
		EX.Amount,
		EX.SubmissionDate,
		EX.SubmissionComment,
		EX.StatusID,
		EA.LinkID,
		LS.ValueDesc AS Status,
		EX.AmountPaid,
		EX.PaymentDate,
        EX.PaymentComment,
		EX.CreatedBy,
		EX.CreatedDate,
		EX.ModifiedBy,
		EX.ModifiedDate,
		EX.TimeStamp
	FROM [dbo].[Expense] EX
	INNER JOIN [Employee] EM on EM.EmployeeID = EX.EmployeeID
	LEFT JOIN [dbo].[ListValue] LS ON EX.StatusID = LS.ListValueID
	LEFT JOIN [dbo].[ListValue] LT ON EX.ExpenseTypeID = LT.ListValueID
	LEFT JOIN dbo.Module M ON M.ModuleShort = 'EXPENSES'
	LEFT JOIN dbo.EmailApproval EA ON M.ModuleID = EA.ModuleID AND EX.ExpenseID = EA.ID AND EA.LinkID <> '00000000-0000-0000-0000-000000000000'
	WHERE ExpenseID = ISNULL(@ExpenseID, EX.ExpenseID) 
	AND EX.EmployeeID = ISNULL(@EmployeeID, EX.EmployeeID)
END