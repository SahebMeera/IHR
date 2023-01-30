--===============================================================
-- Author : Mihir Hapaliya
-- Created Date : 02/11/2021
-- Description : Select SP for EmployeeAddress
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetEmployeeAddress]
	@EmployeeAddressID int = NULL,
	@EmployeeID int = NULL,
	@AddressTypeID int = NULL,
	@Address1 varchar(100) = NULL,
	@Address2 varchar(100) = NULL,
	@City varchar(50) = NULL,
	@State varchar(50) = NULL,
	@Country varchar(50) = NULL,
	@ZipCode varchar(10) = NULL,
	@StartDate datetime = NULL,
	@EndDate datetime = NULL,
	@CreatedBy	varchar(50)	=	NULL,
	@CreatedDate	datetime	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate	datetime	=	NULL
AS 
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	SELECT 
		EA.EmployeeAddressID,
		EA.EmployeeID,
		EA.AddressTypeID,
		EA.Address1,
		EA.Address2,
		EA.City,
		EA.[State],
		EA.Country,
		EA.ZipCode,
		EA.StartDate,
		EA.EndDate,
		EA.CreatedBy,
		EA.CreatedDate,
		EA.ModifiedBy,
		EA.ModifiedDate,
		E.FirstName + ' ' + E.LastName AS EmployeeName
	FROM EmployeeAddress EA
	JOIN Employee E ON EA.EmployeeID = E.EmployeeID
	WHERE EA.EmployeeID = @EmployeeID
	ORDER BY EA.EndDate DESC, EA.StartDate DESC

 END