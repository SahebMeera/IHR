CREATE TABLE [dbo].[Dependent] (
    [DependentID]  INT          IDENTITY (1, 1) NOT NULL,
    [FirstName]    VARCHAR (50) NOT NULL,
    [MiddleName]   VARCHAR (50) NULL,
    [LastName]     VARCHAR (50) NOT NULL,
    [EmployeeID]   INT          NOT NULL,
    [RelationID]   INT          NOT NULL,
    [BirthDate]    DATE         NOT NULL,
    [VisaTypeID]   INT          NOT NULL,
    [CreatedBy]    VARCHAR (50) NOT NULL,
    [CreatedDate]  DATETIME     NOT NULL,
    [ModifiedBy]   VARCHAR (50) NULL,
    [ModifiedDate] DATETIME     NULL,
    [TimeStamp]    ROWVERSION   NOT NULL,
    CONSTRAINT [PK_Dependent] PRIMARY KEY CLUSTERED ([DependentID] ASC),
    CONSTRAINT [FK_Dependent_Employee] FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[Employee] ([EmployeeID]),
    CONSTRAINT [FK_Dependent_ListValue_Relation] FOREIGN KEY ([RelationID]) REFERENCES [dbo].[ListValue] ([ListValueID]),
    CONSTRAINT [FK_Dependent_ListValue_VisaType] FOREIGN KEY ([VisaTypeID]) REFERENCES [dbo].[ListValue] ([ListValueID])
);

