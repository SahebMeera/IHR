--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 05/25/2020
-- Description : Select SP for Dependent
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetDependent]
	@DependentID	int	=	NULL,
	@FirstName	varchar(50)	=	NULL,
	@MiddleName	varchar(50)	=	NULL,
	@LastName	varchar(50)	=	NULL,
	@EmployeeID	int	=	NULL,
	@RelationTypeID	int	=	NULL,
	@BirthDate	date	=	NULL,
	@VisaTypeID	int	=	NULL,
	@CreatedBy	varchar(50)	=	NULL,
	@CreatedDate	datetime	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate	datetime	=	NULL
AS 
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
 
	SELECT 
		D.DependentID,
		D.FirstName,
		D.MiddleName,
		D.LastName,
		D.EmployeeID,
		D.RelationID,
		LV.ValueDesc AS Relation,
		D.BirthDate,
		D.VisaTypeID,
		LV1.ValueDesc AS VisaType,
		D.CreatedBy,
		D.CreatedDate,
		D.ModifiedBy,
		D.ModifiedDate,
		D.TimeStamp,
		E.FirstName + ' ' + E.LastName AS EmployeeName
	FROM [dbo].[Dependent] D
	LEFT JOIN dbo.ListValue LV ON D.RelationID = LV.ListValueID
	LEFT JOIN dbo.ListValue LV1 ON D.VisaTypeID = LV1.ListValueID
	JOIN Employee E ON D.EmployeeID = E.EmployeeID
	WHERE D.DependentID = ISNULL(@DependentID, D.DependentID) 
	AND D.EmployeeID = ISNULL(@EmployeeID, D.EmployeeID)

END
