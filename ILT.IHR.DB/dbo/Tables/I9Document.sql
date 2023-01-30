CREATE TABLE [dbo].[I9Document] (
    [I9DocumentID] INT           IDENTITY (1, 1) NOT NULL,
    [I9DocName]    VARCHAR (100) NOT NULL,
    [I9DocTypeID]  INT           NOT NULL,
    [WorkAuthID]   INT           NOT NULL,
    [CreatedBy]    VARCHAR (50)  NOT NULL,
    [CreatedDate]  DATETIME      NOT NULL,
    [ModifiedBy]   VARCHAR (50)  NULL,
    [ModifiedDate] DATETIME      NULL,
    [TimeStamp]    ROWVERSION    NOT NULL
);

