CREATE TYPE [dbo].[TypeUserRole] AS TABLE (
    [UserRoleID]   INT          NOT NULL,
    [UserID]       INT          NOT NULL,
    [RoleID]       INT          NOT NULL,
    [IsDefault]    BIT          NOT NULL,
    [CreatedBy]    VARCHAR (50) NOT NULL,
    [CreatedDate]  DATETIME     NOT NULL,
    [ModifiedBy]   VARCHAR (50) NULL,
    [ModifiedDate] DATETIME     NULL);



