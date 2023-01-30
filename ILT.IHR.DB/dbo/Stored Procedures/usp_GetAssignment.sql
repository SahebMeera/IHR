--===============================================================
-- Author : Sudeep Kukreti
-- Created Date : 05/24/2020
-- Description : Select SP for Assignment
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By			Description
-- 11/16/2020	Sudeep Kukreti	Commented Role field reference
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetAssignment]
	@AssignmentID	int	=	NULL,
	@EmployeeID	int	=	NULL,
	@StartDate	date	=	NULL,
	@EndDate	date	=	NULL,
	@VendorID	int	=	NULL,
	@ClientManager	varchar(50)	=	NULL,
	--@Role	varchar(20)	=	NULL,
	@Address1	varchar(100)	=	NULL,
	@Address2	varchar(100)	=	NULL,
	@City	varchar(50)	=	NULL,
	@State	varchar(50)	=	NULL,
	@Country	varchar(50)	=	NULL,
	@ZipCode	varchar(10)	=	NULL,
	@ClientID	int	=	NULL,
	@EndClientID	int	=	NULL,
	@SubClient	int	=	NULL,
	@Comments	varchar(100)	=	NULL,
	@PaymentTypeID	int	=	NULL,
	@CreatedBy	varchar(50)	=	NULL,
	@CreatedDate	datetime	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate	datetime	=	NULL
AS 
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
 
	SELECT 
		A.AssignmentID,
		A.EmployeeID,
		E.FirstName,
		E.MiddleName,
		E.LastName,
		A.StartDate,
		A.EndDate,
		A.VendorID,
		C.Name AS Vendor,
		A.ClientManager,
		A.Title,
		--A.Role,
		A.Address1,
		A.Address2,
		A.City,
		A.State,
		A.Country,
		A.ZipCode,
		A.ClientID,
		A.EndClientID,
		A.SubClient,
		C1.Name AS Client,
		A.Comments,
		A.PaymentTypeID,
		A.TimesheetTypeID,
		LV1.ValueDesc as TimesheetType,
		--A.TimesheetApproverID,
		A.TSApproverEmail,
		A.ApprovedEmailTo,
		--U.FirstName + ' ' + U.LastName AS TimesheetApprover,
		LV.ValueDesc AS PaymentType,
		A.CreatedBy,
		A.CreatedDate,
		A.ModifiedBy,
		A.ModifiedDate,
		A.TimeStamp,
		E.FirstName + ' ' + E.LastName AS EmployeeName
	FROM [dbo].[Assignment] A
	INNER JOIN [dbo].[Employee] E ON A.EmployeeID = E.EmployeeID
	LEFT JOIN [dbo].[Company] C ON A.VendorID = C.CompanyID
	LEFT JOIN [dbo].[Company] C1 ON A.ClientID = C1.CompanyID
	LEFT JOIN [dbo].[ListValue] LV ON A.PaymentTypeID = LV.ListValueID
	LEFT JOIN [dbo].[ListValue] LV1 ON A.TimesheetTypeID = LV1.ListValueID
	--LEFT JOIN [dbo].[User] U ON A.TimesheetApproverID = U.UserID
	WHERE A.AssignmentID = ISNULL(@AssignmentID,A.AssignmentID) 
	AND A.EmployeeID = ISNULL(@EmployeeID,A.EmployeeID)  

	--Do not delete below procedure calls
	IF(ISNULL(@AssignmentID,0) <> 0) 
	EXEC [dbo].usp_GetAssignmentRate @AssignmentID = @AssignmentID

END
GO

