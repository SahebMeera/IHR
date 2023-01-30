CREATE TABLE [dbo].[Ticket] (
    [TicketID]         INT           IDENTITY (1, 1) NOT NULL,
    [TicketTypeID]     INT           NOT NULL,
    [RequestedByID]    INT           NOT NULL,
    [ModuleID]         INT           NULL,
    [ID]               INT           NULL,
    [Title]            VARCHAR (100) NOT NULL,
    [Description]      VARCHAR (500) NOT NULL,
    [AssignedToID]     INT           NULL,
    [StatusID]         INT           NOT NULL,
    [ResolvedDate]     DATETIME      NULL,
    [Comment]          VARCHAR (500) NULL,
    [ReminderDuration] INT           NULL,
    [CreatedBy]        VARCHAR (50)  NOT NULL,
    [CreatedDate]      DATETIME      NOT NULL,
    [ModifiedBy]       VARCHAR (50)  NULL,
    [ModifiedDate]     DATETIME      NULL,
    [TimeStamp]        ROWVERSION    NOT NULL,
    CONSTRAINT [PK_Ticket] PRIMARY KEY CLUSTERED ([TicketID] ASC)
);















