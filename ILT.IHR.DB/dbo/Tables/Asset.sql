CREATE TABLE [dbo].[Asset] (
    [AssetID]        INT           IDENTITY (1, 1) NOT NULL,
    [AssetTypeID]    INT           NOT NULL,
    [Tag]            VARCHAR (20)  NOT NULL,
    [Make]           VARCHAR (100) NULL,
    [Model]          VARCHAR (100) NULL,
    [Configuration]  VARCHAR (200) NOT NULL,
    [PurchaseDate]   DATE          NOT NULL,
    [WarantyExpDate] DATE          NULL,
    [WiFiMAC]        VARCHAR (20)  NULL,
    [LANMAC]         VARCHAR (20)  NULL,
    [OS]             VARCHAR (50)  NULL,
    [AssignedToID]   INT           NULL,
    [AssignedTo]     VARCHAR (50)  NULL,
    [StatusID]       INT           NOT NULL,
    [Comment]        VARCHAR (100) NULL,
    [CreatedBy]      VARCHAR (50)  NOT NULL,
    [CreatedDate]    DATETIME      NOT NULL,
    [ModifiedBy]     VARCHAR (50)  NULL,
    [ModifiedDate]   DATETIME      NULL,
    [TimeStamp]      ROWVERSION    NOT NULL,
    CONSTRAINT [PK_Asset] PRIMARY KEY CLUSTERED ([AssetID] ASC),
    CONSTRAINT [FK_Asset_Employee] FOREIGN KEY ([AssignedToID]) REFERENCES [dbo].[Employee] ([EmployeeID])
);



