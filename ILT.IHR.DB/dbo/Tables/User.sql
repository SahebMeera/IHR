CREATE TABLE [dbo].[User] (
    [UserID]       INT          IDENTITY (1, 1) NOT NULL,
    [EmployeeID]   INT          NULL,
    [FirstName]    VARCHAR (50) NULL,
    [LastName]     VARCHAR (50) NULL,
    [Email]        VARCHAR (50) NULL,
    [Password]     VARCHAR (50) NULL,
    [IsOAuth]      BIT          CONSTRAINT [DF_User_UseOAuth] DEFAULT ((0)) NULL,
    [IsActive]     BIT          NULL,
    [CompanyID]    INT          NULL,
    [CreatedBy]    VARCHAR (50) NOT NULL,
    [CreatedDate]  DATETIME     NOT NULL,
    [ModifiedBy]   VARCHAR (50) NULL,
    [ModifiedDate] DATETIME     NULL,
    [TimeStamp]    ROWVERSION   NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([UserID] ASC),
    CONSTRAINT [FK_User_Company] FOREIGN KEY ([CompanyID]) REFERENCES [dbo].[Company] ([CompanyID]),
    CONSTRAINT [FK_User_Employee] FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[Employee] ([EmployeeID])
);















