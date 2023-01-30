CREATE TABLE [dbo].[Employee] (
    [EmployeeID]          INT          IDENTITY (1, 1) NOT NULL,
    [EmployeeCode]        VARCHAR (12) NOT NULL,
    [FirstName]           VARCHAR (50) NOT NULL,
    [MiddleName]          VARCHAR (50) NULL,
    [LastName]            VARCHAR (50) NOT NULL,
    [Country]             VARCHAR (50) NULL,
    [TitleID]             INT          NULL,
    [GenderID]            INT          NOT NULL,
    [DepartmentID]        INT          NOT NULL,
    [Phone]               VARCHAR (10) NOT NULL,
    [HomePhone]           VARCHAR (10) NULL,
    [WorkPhone]           VARCHAR (10) NULL,
    [Email]               VARCHAR (50) NOT NULL,
    [WorkEmail]           VARCHAR (50) NULL,
    [BirthDate]           DATE         NOT NULL,
    [HireDate]            DATE         NOT NULL,
    [TermDate]            DATE         NULL,
    [WorkAuthorizationID] INT          NOT NULL,
    [SSN]                 VARCHAR (9)  NULL,
    [PAN]                 VARCHAR (10) NULL,
    [AadharNumber]        VARCHAR (12) NULL,
    [Salary]              INT          NOT NULL,
    [VariablePay]         INT          NULL,
    [MaritalStatusID]     INT          NOT NULL,
    [ManagerID]           INT          NULL,
    [EmploymentTypeID]    INT          NOT NULL,
    [IsDeleted]           BIT          CONSTRAINT [DF__Employee__IsDele__36B12243] DEFAULT ((0)) NOT NULL,
    [CreatedBy]           VARCHAR (50) NOT NULL,
    [CreatedDate]         DATETIME     NOT NULL,
    [ModifiedBy]          VARCHAR (50) NULL,
    [ModifiedDate]        DATETIME     NULL,
    [TimeStamp]           ROWVERSION   NOT NULL,
    CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED ([EmployeeID] ASC),
    CONSTRAINT [FK_EmpGender_ListValue] FOREIGN KEY ([GenderID]) REFERENCES [dbo].[ListValue] ([ListValueID]),
    CONSTRAINT [FK_Employee_Department] FOREIGN KEY ([DepartmentID]) REFERENCES [dbo].[Department] ([DepartmentID]),
    CONSTRAINT [FK_EmploymentType_ListValue] FOREIGN KEY ([EmploymentTypeID]) REFERENCES [dbo].[ListValue] ([ListValueID]),
    CONSTRAINT [FK_EmpMaritalStatus_ListValue] FOREIGN KEY ([MaritalStatusID]) REFERENCES [dbo].[ListValue] ([ListValueID]),
    CONSTRAINT [FK_EmpTitle_ListValue] FOREIGN KEY ([TitleID]) REFERENCES [dbo].[ListValue] ([ListValueID]),
    CONSTRAINT [FK_EmpWorkAutorization_ListValue] FOREIGN KEY ([WorkAuthorizationID]) REFERENCES [dbo].[ListValue] ([ListValueID]),
    CONSTRAINT [FK_Manager_Employee] FOREIGN KEY ([ManagerID]) REFERENCES [dbo].[Employee] ([EmployeeID])
);















