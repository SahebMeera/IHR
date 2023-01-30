CREATE TABLE [dbo].[EmailJob] (
    [EmailJobID]  INT           NOT NULL,
    [Subject]     VARCHAR (50)  NULL,
    [From]        VARCHAR (255) NOT NULL,
    [To]          VARCHAR (255) NOT NULL,
    [CC]          VARCHAR (255) NULL,
    [BCC]         VARCHAR (255) NULL,
    [Body]        VARCHAR (MAX) NULL,
    [IsSent]      BIT           NOT NULL,
    [CreatedDate] DATETIME      NOT NULL,
    [CreatedBy]   VARCHAR (50)  NOT NULL,
    [SendDate]    DATETIME      NULL
);

