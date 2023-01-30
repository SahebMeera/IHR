CREATE TABLE [dbo].[State] (
    [StateID]      INT          IDENTITY (1, 1) NOT NULL,
    [CountryID]    INT          NOT NULL,
    [StateShort]   VARCHAR (3)  NOT NULL,
    [StateDesc]    VARCHAR (50) NOT NULL,
    [CreatedBy]    VARCHAR (50) NOT NULL,
    [CreatedDate]  DATETIME     NOT NULL,
    [ModifiedBy]   VARCHAR (50) NULL,
    [ModifiedDate] DATETIME     NULL,
    [TimeStamp]    ROWVERSION   NOT NULL,
    CONSTRAINT [PK_State] PRIMARY KEY CLUSTERED ([StateID] ASC),
    CONSTRAINT [FK_State_Country] FOREIGN KEY ([CountryID]) REFERENCES [dbo].[Country] ([CountryID])
);

