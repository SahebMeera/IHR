CREATE TABLE [dbo].[Holiday] (
    [HolidayID]    INT           IDENTITY (1, 1) NOT NULL,
    [Name]         VARCHAR (100) NOT NULL,
    [StartDate]    DATE          NOT NULL,
    [Country]      VARCHAR (50)  NULL,
    [CreatedBy]    VARCHAR (50)  NOT NULL,
    [CreatedDate]  DATETIME      NOT NULL,
    [ModifiedBy]   VARCHAR (50)  NULL,
    [ModifiedDate] DATETIME      NULL,
    [TimeStamp]    ROWVERSION    NOT NULL,
    CONSTRAINT [PK_Holiday] PRIMARY KEY CLUSTERED ([HolidayID] ASC)
);



