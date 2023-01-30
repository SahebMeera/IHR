--===============================================================
-- Author : Hardik S
-- Created Date : 06/23/2021
-- Description : Insert/Update SP for Email Job
--
-- Revision History:
-----------------------------------------------------------------
-- Date            By          Description

--===============================================================
 
CREATE PROCEDURE dbo.usp_InsUpdEmailJob
@EmailJobID int = null,
@Subject varchar(50),
@From varchar(50),
@To varchar(255),
@CC varchar(255) = null,
@BCC varchar(255) = null,
@Body varchar(max),
@CreatedBy varchar(50),
@IsSent bit = false,
@SendDate datetime = null,
@ReturnCode INT = 0 OUTPUT 
as
Begin
 
 
	SET NOCOUNT ON;  
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	DECLARE @IDTable TABLE(ID INT)
	
if @EmailJobID is null
begin
INSERT INTO [dbo].[EmailJob]
           ([Subject]
           ,[From]
           ,[To]
           ,[CC]
           ,[BCC]
           ,[Body]
           ,[IsSent]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[SendDate])
		OUTPUT INSERTED.EmailJobID INTO @IDTable
     VALUES
           (@Subject,
		   @From,
		   @To,
		   @CC,
		   @BCC,
		   @Body,
		   @IsSent,
		   getdate(),
		   @CreatedBy,
		   @SendDate)

		SELECT @EmailJobID =(SELECT ID FROM @IDTable); 
end
else
begin

	update dbo.EmailJob
	set IsSent = @IsSent,
	SendDate = GETDATE()
	where EmailJobID = @EmailJobID

end
    
IF @@ERROR=0   
		SET @ReturnCode  = @EmailJobID;  
ELSE
		SET @ReturnCode = 0

RETURN

End