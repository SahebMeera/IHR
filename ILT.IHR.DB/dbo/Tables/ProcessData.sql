CREATE TABLE [dbo].[ProcessData] (
    [ProcessDataID]   INT          IDENTITY (1, 1) NOT NULL,
    [ProcessWizardID] INT          NOT NULL,
    [Data]            XML          NOT NULL,
    [CreatedBy]       VARCHAR (50) NOT NULL,
    [CreatedDate]     DATETIME     NOT NULL,
    [ModifiedBy]      VARCHAR (50) NULL,
    [ModifiedDate]    DATETIME     NULL,
    [TimeStamp]       ROWVERSION   NOT NULL,
    [StatusID]        INT          DEFAULT ((0)) NOT NULL,
    [ProcessedDate]   DATETIME     NULL,
    CONSTRAINT [PK_WizardData] PRIMARY KEY CLUSTERED ([ProcessDataID] ASC),
    CONSTRAINT [FK_WizardData_Wizard] FOREIGN KEY ([ProcessWizardID]) REFERENCES [dbo].[ProcessWizard] ([ProcessWizardID])
);

