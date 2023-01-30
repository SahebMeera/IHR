CREATE TABLE [dbo].[AuditLog] (
    [AuditLogID]  INT            IDENTITY (1, 1) NOT NULL,
    [Action]      VARCHAR (25)   NULL,
    [TableName]   VARCHAR (25)   NULL,
    [RecordId]    INT            NULL,
    [Values]      NVARCHAR (MAX) NULL,
    [CreatedDate] DATETIME       NOT NULL,
    [CreatedBy]   VARCHAR (50)   NULL,
    CONSTRAINT [PK_dbo.AuditLogs] PRIMARY KEY CLUSTERED ([AuditLogID] ASC)
);



