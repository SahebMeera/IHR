CREATE TABLE [dbo].[Notification] (
    [NotificationID] INT          IDENTITY (1, 1) NOT NULL,
    [TableName]      VARCHAR (50) NOT NULL,
    [ChangeSetID]    INT          NOT NULL,
    [RecordID]       INT          NOT NULL,
    [UserID]         INT          NOT NULL,
    [IsAck]          BIT          DEFAULT ((0)) NULL,
    [AckDate]        DATETIME     NULL
);

