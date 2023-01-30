CREATE TABLE [dbo].[Assignment] (
    [AssignmentID]    INT           IDENTITY (1, 1) NOT NULL,
    [EmployeeID]      INT           NOT NULL,
    [StartDate]       DATE          NOT NULL,
    [EndDate]         DATE          NULL,
    [VendorID]        INT           NULL,
    [ClientManager]   VARCHAR (50)  NULL,
    [Title]           VARCHAR (50)  NULL,
    [Address1]        VARCHAR (100) NOT NULL,
    [Address2]        VARCHAR (100) NULL,
    [City]            VARCHAR (50)  NOT NULL,
    [State]           VARCHAR (50)  NOT NULL,
    [Country]         VARCHAR (50)  NOT NULL,
    [ZipCode]         VARCHAR (10)  NOT NULL,
    [ClientID]        INT           NOT NULL,
    [EndClientID]     INT           NULL,
    [SubClient]       VARCHAR (500) NULL,
    [Comments]        VARCHAR (100) NULL,
    [PaymentTypeID]   INT           NOT NULL,
    [TimesheetTypeID] INT           NULL,
    [TSApproverEmail] VARCHAR (50)  NULL,
    [ApprovedEmailTo] VARCHAR (100) NULL,
    [CreatedBy]       VARCHAR (50)  NOT NULL,
    [CreatedDate]     DATETIME      NOT NULL,
    [ModifiedBy]      VARCHAR (50)  NULL,
    [ModifiedDate]    DATETIME      NULL,
    [TimeStamp]       ROWVERSION    NOT NULL,
    CONSTRAINT [PK_Assignment] PRIMARY KEY CLUSTERED ([AssignmentID] ASC),
    CONSTRAINT [FK_Assignment_Company] FOREIGN KEY ([ClientID]) REFERENCES [dbo].[Company] ([CompanyID]),
    CONSTRAINT [FK_Assignment_Company1] FOREIGN KEY ([VendorID]) REFERENCES [dbo].[Company] ([CompanyID]),
    CONSTRAINT [FK_Assignment_Employee] FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[Employee] ([EmployeeID]),
    CONSTRAINT [FK_Assignment_ListValue] FOREIGN KEY ([PaymentTypeID]) REFERENCES [dbo].[ListValue] ([ListValueID]),
    CONSTRAINT [FK_Assignment_ListValue_TimesheetType] FOREIGN KEY ([TimesheetTypeID]) REFERENCES [dbo].[ListValue] ([ListValueID])
);















