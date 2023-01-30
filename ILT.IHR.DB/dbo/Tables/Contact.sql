CREATE TABLE [dbo].[Contact] (
    [ContactID]     INT           IDENTITY (1, 1) NOT NULL,
    [ContactTypeID] INT           NOT NULL,
    [EmployeeID]    INT           NULL,
    [FirstName]     VARCHAR (50)  NOT NULL,
    [LastName]      VARCHAR (50)  NOT NULL,
    [Phone]         VARCHAR (10)  NOT NULL,
    [Email]         VARCHAR (50)  NOT NULL,
    [Address1]      VARCHAR (100) NOT NULL,
    [Address2]      VARCHAR (100) NULL,
    [City]          VARCHAR (50)  NOT NULL,
    [State]         VARCHAR (50)  NOT NULL,
    [Country]       VARCHAR (50)  NOT NULL,
    [ZipCode]       VARCHAR (10)  NOT NULL,
    [IsDeleted]     BIT           CONSTRAINT [DF_EmergencyContact_IsDeleted] DEFAULT ((0)) NOT NULL,
    [CreatedBy]     VARCHAR (50)  NOT NULL,
    [CreatedDate]   DATETIME      NOT NULL,
    [ModifiedBy]    VARCHAR (50)  NULL,
    [ModifiedDate]  DATETIME      NULL,
    [TimeStamp]     ROWVERSION    NOT NULL,
    CONSTRAINT [FK_ContactType_ListValue] FOREIGN KEY ([ContactTypeID]) REFERENCES [dbo].[ListValue] ([ListValueID])
);







