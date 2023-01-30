CREATE TABLE [dbo].[Expense] (
    [ExpenseID]         INT             IDENTITY (1, 1) NOT NULL,
    [EmployeeID]        INT             NOT NULL,
    [ExpenseTypeID]     INT             NOT NULL,
    [FileName]          VARCHAR (100)   NULL,
    [Amount]            DECIMAL (18, 2) NULL,
    [SubmissionDate]    DATE            NOT NULL,
    [SubmissionComment] VARCHAR (100)   NULL,
    [StatusID]          INT             NOT NULL,
    [AmountPaid]        INT             NULL,
    [PaymentDate]       DATE            NULL,
    [PaymentComment]    VARCHAR (100)   NULL,
    [CreatedBy]         VARCHAR (50)    NOT NULL,
    [CreatedDate]       DATETIME        NOT NULL,
    [ModifiedBy]        VARCHAR (50)    NULL,
    [ModifiedDate]      DATETIME        NULL,
    [TimeStamp]         ROWVERSION      NOT NULL,
    CONSTRAINT [PK_Expense] PRIMARY KEY CLUSTERED ([ExpenseID] ASC)
);



