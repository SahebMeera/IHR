

CREATE PROCEDURE [dbo].[usp_InsAuditLog]
@Action varchar(25),
@TableName varchar(25),
@RecordId int,
@Values nvarchar(max),
@CreatedBy varchar(50)
as
begin

SET NOCOUNT ON;  
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED


INSERT INTO [dbo].[AuditLog]
           ([Action]
           ,[TableName]
		   ,[RecordId]
           ,[Values]
           ,[CreatedDate]
           ,[CreatedBy])
     VALUES
           (@Action
           ,@TableName
		   ,@RecordId
           ,@Values
           ,GetDate()
           ,@CreatedBy)

end