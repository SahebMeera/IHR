CREATE TABLE [dbo].[ListValue] (
    [ListValueID]  INT          IDENTITY (1, 1) NOT NULL,
    [ListTypeID]   INT          NOT NULL,
    [Value]        VARCHAR (20) NOT NULL,
    [ValueDesc]    VARCHAR (50) NOT NULL,
    [IsActive]     BIT          CONSTRAINT [DF_ListValue_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDate]  DATETIME     NOT NULL,
    [CreatedBy]    VARCHAR (50) NOT NULL,
    [ModifiedDate] DATETIME     NULL,
    [ModifiedBy]   VARCHAR (50) NULL,
    [TimeStamp]    ROWVERSION   NOT NULL,
    CONSTRAINT [PK_ListValue] PRIMARY KEY CLUSTERED ([ListValueID] ASC),
    CONSTRAINT [FK_ListValue_ListType] FOREIGN KEY ([ListTypeID]) REFERENCES [dbo].[ListType] ([ListTypeID])
);



