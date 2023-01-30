--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 12/05/2020
-- Description : Select SP for LeaveBalance
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetLeaveBalance]
	@LeaveBalanceID	int	=	NULL,
	@EmployeeID	int	=	NULL,
	@LeaveYear	int	=	NULL,
	@LeaveTypeID	int	=	NULL,
	@VacationTotal	numeric(5,1)	=	NULL,
	@VacationUsed	numeric(5,1) =	NULL,
	@CreatedBy	varchar(50)	=	NULL,
	@CreatedDate	datetime	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate	datetime	=	NULL,
	@TimeStamp	timestamp	=	NULL
AS 
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	DECLARE @EmploymentTypeID int
	DECLARE @IDTable TABLE(ID INT)

	SELECT @EmploymentTypeID = ListValueID FROM ListValue LV
		INNER JOIN ListType LT ON LV.ListTypeID = Lt.ListTypeID
		WHERE LT.Type = 'EmploymentType' AND LV.Value = 'FTE'

	INSERT INTO [dbo].[LeaveBalance]
	(
		EmployeeID,
		LeaveYear,
		LeaveTypeID,
		VacationTotal,
		VacationUsed,
		UnpaidLeave,
		EncashedLeave,
		CreatedBy,
		CreatedDate
	)
	OUTPUT INSERTED.LeaveBalanceID INTO @IDTable
	SELECT
		E.EmployeeID,
		YEAR(GETDATE()),
		LV.ListValueID,
		0,
		0,
		0,
		0,
		'Admin',
		GETDATE()
	FROM Employee E 
	LEFT JOIN [dbo].[LeaveBalance] LB ON E.EmployeeId = LB.EmployeeID AND LB.LeaveYear = YEAR(GETDATE())
	JOIN ListType LT ON 1 =1 AND [Type] = 'VACATIONTYPE'
	JOIN ListValue LV ON LT.ListTypeID = LV.ListTypeID AND LV.Value <> 'LWP'
	WHERE LB.EmployeeID IS NULL AND E.IsDeleted = 0 AND E.TermDate IS NULL
	
	-- LB means current year
	UPDATE LB
	--SET LB.VacationTotal =  CASE WHEN (LB1.VacationTotal - LB1.VacationUsed + LB1.UnpaidLeave) > 5 THEN	5  
	--ELSE (LB1.VacationTotal - LB1.VacationUsed + LB1.UnpaidLeave) END, 
	SET LB.VacationTotal = LB1.VacationTotal - LB1.VacationUsed + LB1.UnpaidLeave
	FROM [dbo].[LeaveBalance] LB
	INNER JOIN [dbo].Employee E ON LB.EmployeeID = E.EmployeeID
	INNER JOIN [dbo].[LeaveBalance] LB1 ON LB1.EmployeeID = LB.EmployeeID AND LB1.LeaveYear = YEAR(GETDATE())-1 AND LB1.LeaveTypeID = LB.LeaveTypeID
	INNER JOIN @IDTable TMP ON TMP.ID = LB.LeaveBalanceID
	WHERE LB.LeaveYear = YEAR(GETDATE())
	
	-- LB1 means previous year
	--UPDATE LB1
	--SET EncashedLeave = CASE WHEN (LB1.VacationTotal - LB1.VacationUsed + LB1.UnpaidLeave) > 5 THEN (LB1.VacationTotal - LB1.VacationUsed + LB1.UnpaidLeave) - 5 
	--ELSE 0 END
	--FROM [dbo].[LeaveBalance] LB
	--INNER JOIN [dbo].Employee E ON LB.EmployeeID = E.EmployeeID
	--INNER JOIN [dbo].[LeaveBalance] LB1 ON LB1.EmployeeID = LB.EmployeeID AND LB1.LeaveYear = YEAR(GETDATE())-1 AND LB1.LeaveTypeID = LB.LeaveTypeID
	--INNER JOIN @IDTable TMP ON TMP.ID = LB.LeaveBalanceID
	--WHERE LB.LeaveYear = YEAR(GETDATE())



	SELECT 
		LB.LeaveBalanceID,
		LB.EmployeeID,
		E.FirstName + ' ' + E.LastName AS EmployeeName,
		LB.LeaveYear,
		LB.LeaveTypeID,
		VT.ValueDesc AS LeaveType,
		LB.VacationTotal,
		LB.VacationUsed,
		LB.UnpaidLeave,
		LB.EncashedLeave,
		ISNULL(LB.VacationTotal,0) - ISNULL(LB.VacationUsed,0) + ISNULL(LB.UnpaidLeave,0)-ISNULL(LB.EncashedLeave,0) AS VacationBalance,
		E.Country,
		LB.CreatedBy,
		LB.CreatedDate,	
		LB.ModifiedBy,
		LB.ModifiedDate,
		LB.TimeStamp
	FROM [dbo].[LeaveBalance] LB
	LEFT JOIN [dbo].[Employee] E ON LB.EmployeeID = E.EmployeeID
	LEFT JOIN [dbo].[ListValue] VT ON LB.LeaveTypeID = VT.ListValueID
	WHERE LB.LeaveBalanceID = ISNULL(@LeaveBalanceID, LB.LeaveBalanceID)
	AND LB.EmployeeID =  ISNULL(@EmployeeID, LB.EmployeeID)
	AND LB.LeaveYear >= YEAR(GETDATE()) - 1
	AND E.EmploymentTypeID = @EmploymentTypeID;
END