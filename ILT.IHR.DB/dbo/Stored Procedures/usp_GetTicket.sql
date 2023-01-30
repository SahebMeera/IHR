--===============================================================
-- Author : Meerasaheb
-- Created Date : 02/22/2021
-- Description : Select SP for Expense
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
-- 11/17/2021 Mihir Hapaliya   Title added in dataset 
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetTicket]
	@TicketID int	=	NULL,	
	@TicketTypeID int = NULL,
    @RequestedByID int = NULL,
    @ModuleID int = NULL,	
	@ID int = NULL,
	@Description	varchar(200)	=	NULL,
	@ResolvedDate	datetime	=	NULL,
	@AssignedToID int = NULL,
	@StatusID int = NULL,
	@Comment	varchar(200)	=	NULL,
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
		T.TicketID,
		T.TicketTypeID,
		LT.ValueDesc AS TicketType,
		LT.Value AS TicketShort,
		T.ModuleID,
		M.ModuleName,
		T.ID,
		T.Description,
		T.Title,
		T.RequestedByID,
		R.FirstName + ' ' + R.LastName AS RequestedBy,
        T.ResolvedDate,
        T.AssignedToID,
		EM.FirstName + ' ' + EM.LastName AS AssignedTo,
		T.StatusID,
		LS.ValueDesc AS Status,
		T.Comment,
		EA.LinkID,
		T.CreatedBy,
		T.CreatedDate,
		T.ModifiedBy,
		T.ModifiedDate,
		T.TimeStamp
	FROM [dbo].[Ticket] T
	LEFT JOIN [Employee] EM on T.AssignedToID =  EM.EmployeeID
    LEFT JOIN [Employee] R on T.RequestedByID = R.EmployeeID
	LEFT JOIN [dbo].[ListValue] LS ON T.StatusID = LS.ListValueID
	LEFT JOIN [dbo].[ListValue] LT ON T.TicketTypeID = LT.ListValueID
	LEFT JOIN dbo.Module M ON M.ModuleShort = 'TICKET'
	LEFT JOIN dbo.EmailApproval EA ON M.ModuleID = EA.ModuleID AND T.TicketID = EA.ID AND EA.LinkID <> '00000000-0000-0000-0000-000000000000'
	WHERE TicketID = ISNULL(@TicketID, T.TicketID) 
	AND 
	(
	ISNULL(T.AssignedToID, 0) = COALESCE(@AssignedToID, T.AssignedToID, 0)
	OR ISNULL(T.RequestedByID, 0) = COALESCE(@AssignedToID, T.RequestedByID, 0)
	OR ISNULL(EM.ManagerID, 0) = COALESCE(@AssignedToID, EM.ManagerID, 0)
	OR (T.AssignedToID IS NULL AND EXISTS(SELECT  1 FROM Employee WHERE ManagerID = ISNULL(@AssignedToID,0)))
	)
END