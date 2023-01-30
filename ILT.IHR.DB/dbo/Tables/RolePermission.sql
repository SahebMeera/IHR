CREATE TABLE [dbo].[RolePermission] (
    [RolePermissionID] INT          IDENTITY (1, 1) NOT NULL,
    [RoleId]           INT          NOT NULL,
    [ModuleID]         INT          NOT NULL,
    [View]             BIT          NOT NULL,
    [Add]              BIT          NOT NULL,
    [Update]           BIT          NOT NULL,
    [Delete]           BIT          NOT NULL,
    [CreatedBy]        VARCHAR (50) NULL,
    [CreatedDate]      DATETIME     NULL,
    [ModifiedBy]       VARCHAR (50) NULL,
    [ModifiedDate]     DATETIME     NULL,
    [TimeStamp]        ROWVERSION   NULL,
    CONSTRAINT [PK_RolePermission] PRIMARY KEY CLUSTERED ([RolePermissionID] ASC),
    CONSTRAINT [FK_RolePermission_Module] FOREIGN KEY ([ModuleID]) REFERENCES [dbo].[Module] ([ModuleID]),
    CONSTRAINT [FK_RolePermission_Role] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([RoleID])
);

