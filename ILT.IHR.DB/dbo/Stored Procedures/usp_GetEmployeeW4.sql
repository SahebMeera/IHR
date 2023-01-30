--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 02/22/2021
-- Description : Select SP for EmployeeW4
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetEmployeeW4]
	@EmployeeW4ID	int	=	NULL,
	@EmployeeID	int	=	NULL,
	@W4TypeID	int	=	NULL,
	@WithHoldingStatusID	int	=	NULL,
	@Allowances	int	=	NULL,
	@IsMultipleJobsOrSpouseWorks	bit	=	NULL,
	@QualifyingChildren	int	=	NULL,
	@OtherDependents	int	=	NULL,
	@OtherIncome	decimal	=	NULL,
	@Deductions	decimal	=	NULL,
	@StartDate	date	=	NULL,
	@EndDate	date	=	NULL,
	@SSN	varchar(9)	=	NULL,
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
		EW.EmployeeW4ID,
		EW.EmployeeID,
		E.FirstName + ' ' + E.LastName AS EmployeeName,
		E.SSN,
		EW.W4TypeID,
		W4T.ValueDesc AS W4Type,
		EW.WithHoldingStatusID,
		WIT.ValueDesc AS WithHoldingStatus,
		EW.Allowances,
		EW.IsMultipleJobsOrSpouseWorks,
		EW.QualifyingChildren,
		EW.OtherDependents,
		EW.OtherIncome,
		EW.Deductions,
		EW.StartDate,
		EW.EndDate,
		EW.CreatedBy,
		EW.CreatedDate,
		EW.ModifiedBy,
		EW.ModifiedDate,
		EW.TimeStamp
	FROM [dbo].[EmployeeW4] EW
	INNER JOIN [dbo].[Employee] E ON E.EmployeeID = EW.EmployeeID
	LEFT JOIN dbo.ListValue W4T ON EW.W4TypeID = W4T.ListValueID
	LEFT JOIN dbo.ListValue WIT ON EW.WithHoldingStatusID = WIT.ListValueID 
	WHERE EW.EmployeeW4ID = ISNULL(@EmployeeW4ID, EW.EmployeeW4ID)
	AND EW.EmployeeID = ISNULL(@EmployeeID, EW.EmployeeID)
END