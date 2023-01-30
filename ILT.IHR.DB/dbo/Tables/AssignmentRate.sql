CREATE TABLE [dbo].[AssignmentRate] (
    [AssignmentRateID] INT             IDENTITY (1, 1) NOT NULL,
    [AssignmentID]     INT             NOT NULL,
    [BillingRate]      DECIMAL (10, 2) NOT NULL,
    [PaymentRate]      DECIMAL (10, 2) NOT NULL,
    [StartDate]        DATE            NOT NULL,
    [EndDate]          DATE            NULL,
    [CreatedBy]        VARCHAR (50)    NOT NULL,
    [CreatedDate]      DATETIME        NOT NULL,
    [ModifiedBy]       VARCHAR (50)    NULL,
    [ModifiedDate]     DATETIME        NULL,
    [TimeStamp]        ROWVERSION      NOT NULL,
    CONSTRAINT [PK_AssignmentRate] PRIMARY KEY CLUSTERED ([AssignmentRateID] ASC),
    CONSTRAINT [FK_AssignmentRate_Assignment] FOREIGN KEY ([AssignmentID]) REFERENCES [dbo].[Assignment] ([AssignmentID])
);



