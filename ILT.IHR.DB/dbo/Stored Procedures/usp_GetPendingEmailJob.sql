--===============================================================
-- Author : Hardik S
-- Created Date : 06/23/2021
-- Description : Insert/Update SP for Email Job
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description

--===============================================================
 
CREATE PROCEDURE dbo.usp_GetPendingEmailJob

as
Begin
 
 
	SET NOCOUNT ON;  
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	select * from dbo.EmailJob where IsSent = 0 order by EmailJobID


End