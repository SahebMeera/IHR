CREATE TABLE [dbo].[ProcessDataTicket] (
    [ProcessDataTicketID] INT IDENTITY (1, 1) NOT NULL,
    [ProcessDataID]       INT NOT NULL,
    [TicketID]            INT NOT NULL,
    CONSTRAINT [PK_WizardDataTicket] PRIMARY KEY CLUSTERED ([ProcessDataTicketID] ASC)
);

