CREATE TABLE [dbo].[ProcessWizard] (
    [ProcessWizardID] INT          IDENTITY (1, 1) NOT NULL,
    [Process]         VARCHAR (50) NOT NULL,
    [Elements]        XML          NOT NULL,
    [StoredProc]      VARCHAR (50) NULL,
    [CreatedBy]       VARCHAR (50) NOT NULL,
    [CreatedDate]     DATETIME     NOT NULL,
    [ModifiedBy]      VARCHAR (50) NULL,
    [ModifiedDate]    DATETIME     NULL,
    [TimeStamp]       ROWVERSION   NOT NULL,
    CONSTRAINT [PK_Wizard] PRIMARY KEY CLUSTERED ([ProcessWizardID] ASC)
);

