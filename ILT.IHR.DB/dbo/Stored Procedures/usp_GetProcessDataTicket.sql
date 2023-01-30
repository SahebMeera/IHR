--===============================================================
-- Author :Nimesh Patel
-- Created Date : 11/23/2021
-- Description : Select SP for ProcessDataTicket
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description
--
--===============================================================
 
CREATE PROCEDURE [dbo].[usp_GetProcessDataTicket]
	@ProcessDataID	int	=	NULL
	
AS 
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
 
	SELECT
		PDT.ProcessDataTicketID,
		PDT.ProcessDataID,
		PDT.TicketID,
		T.Title,
		T.[Description] AS TicketDescription,
		T.StatusID,
		V.ValueDesc AS [Status],
		T.CreatedBy,
		T.CreatedDate,
		T.ResolvedDate,
		T.AssignedToID,
		U.FirstName + ' ' + U.LastName AS AssignedToUser
	FROM [dbo].[ProcessDataTicket] PDT
	INNER JOIN [dbo].[Ticket] T ON PDT.TicketID = T.TicketID
	INNER JOIN [dbo].[ListValue] V ON V.ListValueID = T.StatusID
	LEFT OUTER JOIN [dbo].[User] U ON U.EmployeeID = T.AssignedToID
	WHERE PDT.ProcessDataID = ISNULL(@ProcessDataID, ProcessDataID)	

END