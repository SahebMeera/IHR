CREATE TABLE [dbo].[EmployeeAddress] (
    [EmployeeAddressID] INT           IDENTITY (1, 1) NOT NULL,
    [EmployeeID]        INT           NOT NULL,
    [AddressTypeID]     INT           NOT NULL,
    [Address1]          VARCHAR (100) NOT NULL,
    [Address2]          VARCHAR (100) NULL,
    [City]              VARCHAR (50)  NOT NULL,
    [State]             VARCHAR (50)  NOT NULL,
    [Country]           VARCHAR (50)  NOT NULL,
    [ZipCode]           VARCHAR (10)  NOT NULL,
    [StartDate]         DATE          NOT NULL,
    [EndDate]           DATE          NULL,
    [CreatedBy]         VARCHAR (50)  NOT NULL,
    [CreatedDate]       DATETIME      NOT NULL,
    [ModifiedBy]        VARCHAR (50)  NULL,
    [ModifiedDate]      DATETIME      NULL,
    [TimeStamp]         ROWVERSION    NOT NULL,
    CONSTRAINT [PK_EmployeeAddress] PRIMARY KEY CLUSTERED ([EmployeeAddressID] ASC),
    CONSTRAINT [FK_EmployeeAddress_Employee] FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[Employee] ([EmployeeID])
);

