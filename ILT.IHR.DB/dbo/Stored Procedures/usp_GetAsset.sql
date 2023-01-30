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
 
CREATE PROCEDURE [dbo].[usp_GetAsset]
	@AssetID	int	=	NULL,	
	@AssetTypeID int = NULL,	
	@Tag	varchar(20)	=	NULL,
	@Make  varchar(100)	=	NULL,
    @Model  varchar(100)	=	NULL,
	@Configuration	varchar(200)	=	NULL,
	@PurchaseDate	datetime	=	NULL,
	@WarantyExpDate	datetime	=	NULL,
	@WiFiMAC	varchar(20)	=	NULL,
	@LANMAC	varchar(20)	=	NULL,
	@OS	varchar(50)	=	NULL,
	@AssignedToID int = NULL,
	@AssignedTo varchar(50) = NULL,
	@StatusID int = NULL,
	@Comment	varchar(100)	=	NULL,
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
		A.AssetID,
		A.AssetTypeID,
		LT.ValueDesc AS AssetType,
		A.Tag,
		A.Make,
		A.Model,
		A.Configuration,
		A.PurchaseDate,
		A.WarantyExpDate,
		A.WiFiMAC,
		A.LANMAC,
		A.OS,
		A.AssignedToID,
		CASE WHEN A.AssignedToID IS NULL THEN A.AssignedTo ELSE EM.FirstName + ' ' + EM.LastName END AS AssignedTo,
		A.StatusID,
		LS.ValueDesc AS Status,
		A.Comment,
		A.CreatedBy,
		A.CreatedDate,
		A.ModifiedBy,
		A.ModifiedDate,
		A.TimeStamp
	FROM [dbo].[Asset] A
	LEFT JOIN [Employee] EM on A.AssignedToID = EM.EmployeeID
	LEFT JOIN [dbo].[ListValue] LS ON A.StatusID = LS.ListValueID
	LEFT JOIN [dbo].[ListValue] LT ON A.AssetTypeID = LT.ListValueID
	WHERE AssetID = ISNULL(@AssetID, A.AssetID) 
	AND ISNULL(A.AssignedToID, 0) = COALESCE(@AssignedToID, A.AssignedToID, 0)
END