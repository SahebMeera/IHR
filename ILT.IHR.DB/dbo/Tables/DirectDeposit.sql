CREATE TABLE [dbo].[DirectDeposit] (
    [DirectDepositID] INT          IDENTITY (1, 1) NOT NULL,
    [EmployeeID]      INT          NOT NULL,
    [BankName]        VARCHAR (50) NOT NULL,
    [AccountTypeID]   INT          NOT NULL,
    [RoutingNumber]   VARCHAR (20) NOT NULL,
    [AccountNumber]   VARCHAR (20) NOT NULL,
    [Country]         VARCHAR (50) NULL,
    [State]           VARCHAR (50) NULL,
    [Amount]          INT          NOT NULL,
    [IsPrimary]       BIT          CONSTRAINT [DF_DirectDeposit_IsPrimary] DEFAULT ((0)) NOT NULL,
    [CreatedBy]       VARCHAR (50) NOT NULL,
    [CreatedDate]     DATETIME     NOT NULL,
    [ModifiedBy]      VARCHAR (50) NULL,
    [ModifiedDate]    DATETIME     NULL,
    [TimeStamp]       ROWVERSION   NOT NULL,
    CONSTRAINT [PK_DirectDeposit] PRIMARY KEY CLUSTERED ([DirectDepositID] ASC),
    CONSTRAINT [FK_DirectDeposit_Employee] FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[Employee] ([EmployeeID]),
    CONSTRAINT [FK_DirectDeposit_ListValue_AccountType] FOREIGN KEY ([AccountTypeID]) REFERENCES [dbo].[ListValue] ([ListValueID])
);



