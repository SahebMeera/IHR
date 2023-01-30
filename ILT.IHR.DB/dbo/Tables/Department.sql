CREATE TABLE [dbo].[Department] (
    [DepartmentID]   INT          IDENTITY (1, 1) NOT NULL,
    [DeptCode]       VARCHAR (10) NOT NULL,
    [DeptName]       VARCHAR (50) NOT NULL,
    [DeptLocationID] INT          NOT NULL,
    [IsActive]       BIT          CONSTRAINT [DF__Departmen__IsAct__440B1D61] DEFAULT ((1)) NOT NULL,
    [CreatedBy]      VARCHAR (50) NOT NULL,
    [CreatedDate]    DATETIME     NOT NULL,
    [ModifiedBy]     VARCHAR (50) NULL,
    [ModifiedDate]   DATETIME     NULL,
    [TimeStamp]      ROWVERSION   NOT NULL,
    CONSTRAINT [PK_Department] PRIMARY KEY CLUSTERED ([DepartmentID] ASC),
    CONSTRAINT [FK_Department_Country] FOREIGN KEY ([DeptLocationID]) REFERENCES [dbo].[Country] ([CountryID])
);



