CREATE TABLE [dbo].[NLog_Error] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [TimeStamp]  ROWVERSION     NULL,
    [Level]      NVARCHAR (50)  NOT NULL,
    [Host]       NVARCHAR (MAX) NOT NULL,
    [Type]       NVARCHAR (50)  NOT NULL,
    [Logger]     NVARCHAR (50)  NOT NULL,
    [Message]    NVARCHAR (MAX) NOT NULL,
    [stacktrace] NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_NLogError] PRIMARY KEY CLUSTERED ([Id] ASC)
);

