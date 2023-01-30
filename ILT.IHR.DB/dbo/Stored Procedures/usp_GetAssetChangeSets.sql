--===============================================================
-- Author : Meerasaheb	
-- Created Date : 30/06/2021
-- Description : Get SP for Asset ChangeSet
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetAssetChangeSets]
	@AssetID int = NULL

AS 
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
	;WITH CTE AS
	(
	SELECT ACS.* FROM [dbo].[AssetChangeSet] ACS WHERE ACS.AssetID = ISNULL(@AssetID, ACS.AssetID)
	UNION 
	SELECT 0 AS AssetChangeSetID, A.AssetID, A.AssetTypeID, A.AssignedToID, A.AssignedTo, A.StatusID, 
	A.CreatedBy, A.CreatedDate, A.ModifiedBy, A.ModifiedDate, A.TimeStamp
	FROM Asset A WHERE A.AssetID = ISNULL(@AssetID, A.AssetID)
	)
	SELECT 
        ACS.AssetChangeSetID,
		ACS.AssetID,
		ACS.AssetTypeID,
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
		ACS.AssignedToID,
		CASE WHEN ACS.AssignedToID IS NULL THEN ACS.AssignedTo ELSE EM.FirstName + ' ' + EM.LastName END AS AssignedTo,
		ACS.StatusID,
		LS.ValueDesc AS Status,
		A.Comment,
		ACS.CreatedBy,
		ACS.CreatedDate,
		ACS.ModifiedBy,
		ACS.ModifiedDate,
		ACS.TimeStamp
	FROM CTE ACS
	--LEFT JOIN [dbo].[AssetChangeSet] ACS ON ACS.AssetID = A.AssetID
    LEFT JOIN  [Asset] A ON ACS.AssetID = A.AssetID
	LEFT JOIN [Employee] EM on ACS.AssignedToID = EM.EmployeeID
	LEFT JOIN [dbo].[ListValue] LS ON ACS.StatusID = LS.ListValueID
	LEFT JOIN [dbo].[ListValue] LT ON ACS.AssetTypeID = LT.ListValueID
	WHERE ACS.AssetID = ISNULL(@AssetID, ACS.AssetID)
    ORDER BY ACS.ModifiedDate DESC
	
END