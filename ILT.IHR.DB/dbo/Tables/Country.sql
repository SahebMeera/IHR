CREATE TABLE [dbo].[Country] (
    [CountryID]    INT          IDENTITY (1, 1) NOT NULL,
    [CountryDesc]  VARCHAR (50) NULL,
    [CreatedBy]    VARCHAR (50) NOT NULL,
    [CreatedDate]  DATETIME     NOT NULL,
    [ModifiedBy]   VARCHAR (50) NULL,
    [ModifiedDate] DATETIME     NULL,
    [TimeStamp]    ROWVERSION   NOT NULL,
    CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED ([CountryID] ASC)
);

