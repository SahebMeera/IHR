--===============================================================
-- Author : Nimesh
-- Created Date : 11/15/2021
-- Description : Insert/Update SP for Wizard Ticket
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
Create PROCEDURE [dbo].[usp_Termination]
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
	
	Exec [usp_InsUpdTicket] 
				@TicketID,
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
				@CreatedDate,
				@Title,
				@ReturnCode
	
	IF @@ERROR=0   
		SET @ReturnCode  = @ReturnCode;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END