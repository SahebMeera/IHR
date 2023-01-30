CREATE TABLE [dbo].[AssetChangeSet] (
    [AssetChangeSetID] INT          IDENTITY (1, 1) NOT NULL,
    [AssetID]          INT          NOT NULL,
    [AssetTypeID]      INT          NOT NULL,
    [AssignedToID]     INT          NULL,
    [AssignedTo]       VARCHAR (50) NULL,
    [StatusID]         INT          NOT NULL,
    [CreatedBy]        VARCHAR (50) NOT NULL,
    [CreatedDate]      DATETIME     NULL,
    [ModifiedBy]       VARCHAR (50) NULL,
    [ModifiedDate]     DATETIME     NULL,
    [TimeStamp]        ROWVERSION   NOT NULL
);



