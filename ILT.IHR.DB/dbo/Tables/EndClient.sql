CREATE TABLE [dbo].[EndClient] (
    [EndClientID]  INT           IDENTITY (1, 1) NOT NULL,
    [Name]         VARCHAR (50)  NOT NULL,
    [TaxID]        VARCHAR (20)  NULL,
    [CompanyID]    INT           NULL,
    [Address1]     VARCHAR (100) NOT NULL,
    [Address2]     VARCHAR (100) NULL,
    [City]         VARCHAR (50)  NOT NULL,
    [State]        VARCHAR (50)  NOT NULL,
    [Country]      VARCHAR (50)  NOT NULL,
    [ZipCode]      VARCHAR (10)  NOT NULL,
    [CreatedBy]    VARCHAR (50)  NOT NULL,
    [CreatedDate]  DATETIME      NOT NULL,
    [ModifiedBy]   VARCHAR (50)  NULL,
    [ModifiedDate] DATETIME      NULL,
    [TimeStamp]    ROWVERSION    NOT NULL,
    CONSTRAINT [PK_EndClient] PRIMARY KEY CLUSTERED ([EndClientID] ASC)
);





