CREATE TABLE [dbo].[ListType] (
    [ListTypeID]   INT           IDENTITY (1, 1) NOT NULL,
    [Type]         VARCHAR (20)  NOT NULL,
    [TypeDesc]     VARCHAR (100) NOT NULL,
    [CreatedDate]  DATETIME      NOT NULL,
    [CreatedBy]    VARCHAR (50)  NOT NULL,
    [ModifiedDate] DATETIME      NULL,
    [ModifiedBy]   VARCHAR (50)  NULL,
    [TimeStamp]    ROWVERSION    NOT NULL,
    CONSTRAINT [PK_ListType] PRIMARY KEY CLUSTERED ([ListTypeID] ASC)
);



