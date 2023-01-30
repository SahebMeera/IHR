--===============================================================
-- Author : Meerasaheb
-- Created Date : 12/08/2020
-- Description : Insert/Update SP for Holiday
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
-- 11/18/2021	Nimesh Patel	Added logic for the update status of the wizardData
-- 11/23/2021	Nimesh Patel	Rename the table WizardData to ProcessData
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_InsUpdTicket]
	@TicketID	int	=	NULL,	
	@TicketTypeID int = NULL,	
	@RequestedByID	int	=	NULL,
	@ModuleID	int =	NULL,
	@ID	int =	NULL,
	@Description	varchar(500) =	NULL,
    @AssignedToID int = NULL,
	@StatusID int = NULL,
    @ResolvedDate datetime	=	NULL,
	@Comment	varchar(500)	=	NULL,
	@CreatedBy	varchar(50)	=	NULL,
	@CreatedDate	datetime	=	NULL,
	@ModifiedBy	varchar(50)	=	NULL,
	@ModifiedDate	datetime	=	NULL,
	@TimeStamp	timestamp	=	NULL,
	@Title varchar(100) = NULL,
	@ReturnCode INT = 0 OUTPUT 
AS 
BEGIN
	SET NOCOUNT ON;  
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	DECLARE @IDTable TABLE(ID INT)
	Declare @ListTypeID INT
	Declare @WizardStatusID INT

	IF NOT EXISTS(SELECT 1 FROM Ticket WHERE TicketID = ISNULL(@TicketID,0))  
		BEGIN
			INSERT INTO [dbo].[Ticket]
			(
				TicketTypeID,
				RequestedByID,
                ModuleID,
				ID,
                Description,
				ResolvedDate,
				AssignedToID,
				StatusID,
				Comment,
				CreatedBy,
				CreatedDate,
				Title
			)
			OUTPUT INSERTED.TicketID INTO @IDTable 
			VALUES
			(				
				@TicketTypeID,
				@RequestedByID,
				@ModuleID,
				@ID,
				@Description,
				@ResolvedDate,
				@AssignedToID,
				@StatusID,
				@Comment,
				@CreatedBy,
				GETDATE(),
				@Title
			)
			SELECT @TicketID=(SELECT ID FROM @IDTable);  
		END
	ELSE
		BEGIN
			UPDATE [dbo].[Ticket]
			SET
				TicketTypeID = @TicketTypeID,
				RequestedByID = @RequestedByID,
				ModuleID = @ModuleID,
				ID = @ID,
				Description = @Description,
				ResolvedDate = @ResolvedDate,
				AssignedToID = @AssignedToID,
				StatusID = @StatusID,
				Comment = @Comment,
				ModifiedBy = @ModifiedBy,
				ModifiedDate = GETDATE(),
				Title = @Title
			WHERE TicketID = @TicketID
		
			IF NOT EXISTS(SELECT 1 FROM ProcessDataTicket PDT 
						 INNER JOIN Ticket T ON PDT.TicketID = T.TicketID
						 INNER JOIN ListValue LV ON LV.ListValueID = T.StatusID
						 WHERE LV.[VALUE] <>'RESOLVED' AND PDT.ProcessDataID in 
						 (SELECT ProcessDataID FROM [dbo].[ProcessDataTicket] WHERE TicketID=@TicketID))
			BEGIN

			
					SELECT @ListTypeID = LV.ListTypeID FROM ProcessData PD
					INNER JOIN ListValue LV ON LV.ListValueID = PD.StatusID
					WHERE ProcessDataID in ((SELECT ProcessDataID FROM ProcessDataTicket WHERE TicketID=@TicketID))
					
					SELECT @WizardStatusID = ListValueID FROM ListValue WHERE ListTypeID=@ListTypeID AND [Value]='PROCESSED'

					UPDATE PD
					SET  PD.StatusID = @WizardStatusID,
						 PD.ProcessedDate = GETDATE()
					FROM [dbo].[ProcessData] PD
					INNER JOIN [dbo].[ProcessDataTicket] PDT ON PD.ProcessDataID = PDT.ProcessDataID
					WHERE PDT.ProcessDataID IN ((SELECT ProcessDataID FROM [dbo].[ProcessDataTicket] WHERE TicketID=@TicketID))

			END

			
		END

	IF @@ERROR=0   
		SET @ReturnCode  = @TicketID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END