CREATE TABLE [dbo].[EmailApproval] (
    [EmailApprovalID]  INT              IDENTITY (1, 1) NOT NULL,
    [ModuleID]         INT              NOT NULL,
    [ID]               INT              NOT NULL,
    [ValidTime]        DATETIME         NOT NULL,
    [IsActive]         BIT              CONSTRAINT [DF__EmailAppr__IsAct__093F5D4E] DEFAULT ((1)) NOT NULL,
    [Value]            VARCHAR (100)    NULL,
    [LinkID]           UNIQUEIDENTIFIER NULL,
    [ApproverEmail]    VARCHAR (50)     NULL,
    [EmailSubject]     VARCHAR (255)    NULL,
    [EmailFrom]        VARCHAR (50)     NULL,
    [EmailTo]          VARCHAR (255)    NULL,
    [EmailCC]          VARCHAR (255)    NULL,
    [EmailBCC]         VARCHAR (255)    NULL,
    [EmailBody]        VARCHAR (MAX)    NULL,
    [SendDate]         DATETIME         NULL,
    [SentCount]        SMALLINT         NULL,
    [ReminderDuration] INT              NULL,
    [CreatedBy]        VARCHAR (50)     NOT NULL,
    [CreatedDate]      DATETIME         NOT NULL,
    [ModifiedBy]       VARCHAR (50)     NULL,
    [ModifiedDate]     DATETIME         NULL,
    [TimeStamp]        ROWVERSION       NOT NULL,
    CONSTRAINT [PK_EmailApproval] PRIMARY KEY CLUSTERED ([EmailApprovalID] ASC)
);







