--===============================================================
-- Author : Rama Mohan
-- Created Date : 12/08/2020
-- Description : Insert/Update SP for Holiday
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdAsset]
	@AssetID	int	=	NULL,	
	@AssetTypeID int = NULL,	
	@Tag	varchar(20)	=	NULL,
	@Make  varchar(100)	=	NULL,
    @Model  varchar(100)	=	NULL,
	@Configuration	varchar(200)	=	NULL,
	@PurchaseDate	datetime	=	NULL,
	@WarantyExpDate	datetime	=	NULL,
	@WiFIMAC	varchar(20)	=	NULL,
	@LANMAC	varchar(20)	=	NULL,
	@OS varchar(50)	=	NULL,
	@AssignedToID int = NULL,
	@AssignedTo varchar(50)	=	NULL,
	@StatusID int = NULL,
	@Comment	varchar(100)	=	NULL,
	@CreatedBy	varchar(50)	=	NULL,
	@CreatedDate	datetime	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate	datetime	=	NULL,
	@TimeStamp	timestamp	=	NULL,
	@ReturnCode INT = 0 OUTPUT 
AS 
BEGIN
	SET NOCOUNT ON;  
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	DECLARE @IDTable TABLE(ID INT)
	DECLARE @tabAsset TABLE
	(
		AssetID INT, 
		AssetTypeID INT, 
		AssignedToID INT,
		AssignedTo varchar(50), 
		StatusID INT, 
		CreatedBy varchar(50), 
		CreatedDate DateTime, 
		ModifiedBy varchar(50), 
		ModifiedDate DateTime
	)
	
	IF NOT EXISTS(SELECT 1 FROM Asset WHERE AssetID = ISNULL(@AssetID,0))  
		BEGIN
			INSERT INTO [dbo].[Asset]
			(
				AssetTypeID,
				Tag,
				Make,
				Model,
				Configuration,
				PurchaseDate,
				WarantyExpDate,
				WiFiMAC,
				LANMAC,
				OS,
				AssignedToID,
				AssignedTo,
				StatusID,
				Comment,
				CreatedBy,
				CreatedDate
			)
			OUTPUT INSERTED.AssetID INTO @IDTable 
			VALUES
			(				
				@AssetTypeID,
				@Tag,
				@Make,
				@Model,
				@Configuration,
				@PurchaseDate,
				@WarantyExpDate,
				@WiFIMAC,
				@LANMAC,
				@OS,
				@AssignedToID,
				@AssignedTo,
				@StatusID,
				@Comment,
				@CreatedBy,
				GETDATE()
			)
			SELECT @AssetID=(SELECT ID FROM @IDTable);  
		END
	ELSE
		BEGIN
			UPDATE [dbo].[Asset]
			SET
				AssetTypeID = @AssetTypeID,
				Tag = @Tag,
				Make = @Make,
				Model = @Model,
				Configuration = @Configuration,
				PurchaseDate = @PurchaseDate,
				WarantyExpDate = @WarantyExpDate,
				WiFIMAC = @WiFIMAC,
				LANMAC = @LANMAC,
				OS = @OS,
				AssignedToID = @AssignedToID,
				AssignedTo = @AssignedTo,
				StatusID = @StatusID,
				Comment = @Comment,
				ModifiedBy = @ModifiedBy,
				ModifiedDate = GETDATE()
				OUTPUT DELETED.AssetID,DELETED.AssetTypeID,DELETED.AssignedToID, DELETED.AssignedTo, DELETED.StatusID
					,DELETED.[CreatedBy],DELETED.[CreatedDate], DELETED.[ModifiedBy],DELETED.[ModifiedDate]  
					INTO @tabAsset
                FROM [dbo].[Employee] E
			WHERE AssetID = @AssetID

			INSERT INTO AssetChangeSet
			(
				AssetID, 
				AssetTypeID, 
				AssignedToID,
				AssignedTo,
				StatusID,
				CreatedBy,
				CreatedDate,
				ModifiedBy,
				ModifiedDate
			)
			SELECT 
				TA.AssetID, 
				TA.AssetTypeID,
				TA.AssignedToID,
				TA.AssignedTo,
				TA.StatusID,
				TA.CreatedBy,
				TA.CreatedDate,
				TA.ModifiedBy,
				TA.ModifiedDate
			FROM @tabAsset TA
			JOIN Asset A ON TA.AssetID = A.AssetID
			WHERE 
			TA.AssetTypeID <> A.AssetTypeID
			OR TA.StatusID <> A.StatusID
			OR TA.AssignedToID <> A.AssignedToID
			OR TA.AssignedTo <> A.AssignedTo
			
		END

	IF @@ERROR=0   
		SET @ReturnCode  = @AssetID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END