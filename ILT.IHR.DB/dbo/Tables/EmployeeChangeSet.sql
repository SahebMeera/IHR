CREATE TABLE [dbo].[EmployeeChangeSet] (
    [EmployeeChangeSetID] INT          IDENTITY (1, 1) NOT NULL,
    [EmployeeID]          INT          NOT NULL,
    [EmployeeCode]        VARCHAR (12) NOT NULL,
    [FirstName]           VARCHAR (50) NOT NULL,
    [MiddleName]          VARCHAR (50) NULL,
    [LastName]            VARCHAR (50) NOT NULL,
    [Country]             VARCHAR (50) NULL,
    [TitleID]             INT          NULL,
    [GenderID]            INT          NOT NULL,
    [DepartmentID]        INT          NULL,
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
    [EmploymentTypeID]    INT          NULL,
    [IsDeleted]           BIT          NOT NULL,
    [CreatedBy]           VARCHAR (50) NOT NULL,
    [CreatedDate]         DATETIME     NOT NULL,
    [ModifiedBy]          VARCHAR (50) NULL,
    [ModifiedDate]        DATETIME     NULL,
    CONSTRAINT [PK_EmployeeChangeSetID] PRIMARY KEY CLUSTERED ([EmployeeChangeSetID] ASC)
);









