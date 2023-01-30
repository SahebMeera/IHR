CREATE TABLE [dbo].[RoleNotification] (
    [RoleNotificationID] INT          IDENTITY (1, 1) NOT NULL,
    [RoleID]             INT          NOT NULL,
    [TableName]          VARCHAR (50) NOT NULL,
    [IsActive]           BIT          CONSTRAINT [DF__RoleNotif__IsAct__2E3BD7D3] DEFAULT ((0)) NOT NULL,
    [CreatedBy]          VARCHAR (50) NOT NULL,
    [CreatedDate]        DATETIME     NOT NULL,
    [ModifiedBy]         VARCHAR (50) NULL,
    [ModifiedDate]       DATETIME     NULL,
    [TimeStamp]          ROWVERSION   NOT NULL
);



