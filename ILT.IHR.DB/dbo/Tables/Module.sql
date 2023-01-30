CREATE TABLE [dbo].[Module] (
    [ModuleID]     INT          IDENTITY (1, 1) NOT NULL,
    [ModuleShort]  VARCHAR (20) NULL,
    [ModuleName]   VARCHAR (50) NULL,
    [CreatedBy]    VARCHAR (50) NULL,
    [CreatedDate]  DATETIME     NULL,
    [ModifiedBy]   VARCHAR (50) NULL,
    [ModifiedDate] DATETIME     NULL,
    [TimeStamp]    ROWVERSION   NULL,
    CONSTRAINT [PK_Module] PRIMARY KEY CLUSTERED ([ModuleID] ASC)
);

