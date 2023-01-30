CREATE TABLE [dbo].[Role] (
    [RoleID]       INT          IDENTITY (1, 1) NOT NULL,
    [RoleShort]    VARCHAR (20) NOT NULL,
    [RoleName]     VARCHAR (50) NOT NULL,
    [CreatedBy]    VARCHAR (50) NOT NULL,
    [CreatedDate]  DATETIME     NOT NULL,
    [ModifiedBy]   VARCHAR (50) NULL,
    [ModifiedDate] DATETIME     NULL,
    [TimeStamp]    ROWVERSION   NOT NULL,
    CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([RoleID] ASC),
    CONSTRAINT [UC_Role_RoleName] UNIQUE NONCLUSTERED ([RoleName] ASC),
    CONSTRAINT [UC_Role_RoleShort] UNIQUE NONCLUSTERED ([RoleShort] ASC)
);





