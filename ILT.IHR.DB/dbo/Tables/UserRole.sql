CREATE TABLE [dbo].[UserRole] (
    [UserRoleID]   INT          IDENTITY (1, 1) NOT NULL,
    [UserID]       INT          NOT NULL,
    [RoleID]       INT          NOT NULL,
    [IsDefault]    BIT          CONSTRAINT [DF_UserRole_IsDefault] DEFAULT ((1)) NOT NULL,
    [CreatedBy]    VARCHAR (50) NOT NULL,
    [CreatedDate]  DATETIME     NOT NULL,
    [ModifiedBy]   VARCHAR (50) NULL,
    [ModifiedDate] DATETIME     NULL,
    [TimeStamp]    ROWVERSION   NOT NULL,
    CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED ([UserRoleID] ASC)
);





