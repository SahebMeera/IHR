CREATE TABLE [dbo].[ProcessConfig] (
    [ProcessConfigID]  INT           IDENTITY (1, 1) NOT NULL,
    [ProcessWizardID]  INT           NOT NULL,
    [TicketTypeID]     INT           NOT NULL,
    [Task]             VARCHAR (100) NULL,
    [TaskDescription]  VARCHAR (500) NULL,
    [AssignTo]         INT           NULL,
    [ReminderDuration] INT           NULL,
    CONSTRAINT [PK_WizardConfig] PRIMARY KEY CLUSTERED ([ProcessConfigID] ASC)
);







