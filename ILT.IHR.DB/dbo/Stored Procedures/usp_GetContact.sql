--===============================================================
-- Author : Mihir Hapaliya
-- Created Date : 11/30/2020
-- Description : Select SP for Contact
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetContact]
	@ContactID	int	=	NULL,
	@ContactTypeID int = NULL,
	@EmployeeID int = NULL,
	@FirstName varchar(50) = NULL,
	@LastName varchar(50) = NULL,
	@Phone varchar(10) = NULL,
	@Email varchar(50) = NULL,
	@Address1 varchar(100) = NULL,
	@Address2 varchar(100) = NULL,
	@City varchar(50) = NULL,
	@State varchar(50) = NULL,
	@Country varchar(50) = NULL,
	@ZipCode varchar(10) = NULL,
	@IsDeleted	bit	=	NULL,
	@CreatedBy	varchar(50)	=	NULL,
	@CreatedDate	datetime	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate	datetime	=	NULL
AS 
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
 
	SELECT 
		C.ContactID,
		C.ContactTypeID,
		CT.ValueDesc AS ContactType,
		C.EmployeeID,
		C.FirstName,
		C.LastName,
		C.Phone,
		C.Email,
		C.Address1,
		C.Address2,
		C.City,
		C.State,
		C.Country,
		C.ZipCode,
		C.IsDeleted,
		C.CreatedBy,
		C.CreatedDate,
		C.ModifiedDate,
		C.ModifiedDate,
		E.FirstName + ' ' + E.LastName AS EmployeeName
	FROM [dbo].[Contact] C
	INNER JOIN dbo.Employee E ON C.EmployeeID = E.EmployeeID
	LEFT JOIN dbo.ListValue CT ON C.ContactTypeID = CT.ListValueID
	WHERE C.ContactID = ISNULL(@ContactID, C.ContactID) 
	-- AND C.ContactType = ISNULL(@ContactType, C.ContactType)
	AND C.EmployeeID = ISNULL(@EmployeeID, C.EmployeeID)
	
END