CREATE TABLE [dbo].[Company] (
    [CompanyID]                    INT           IDENTITY (1, 1) NOT NULL,
    [Name]                         VARCHAR (50)  NOT NULL,
    [CompanyTypeID]                INT           NOT NULL,
    [InvoicingPeriodID]            INT           NOT NULL,
    [PaymentTermID]                INT           NOT NULL,
    [TaxID]                        VARCHAR (20)  NOT NULL,
    [IsEndClient]                  BIT           CONSTRAINT [DF_Company_IsEndClient] DEFAULT ((0)) NOT NULL,
    [Address1]                     VARCHAR (100) NOT NULL,
    [Address2]                     VARCHAR (100) NULL,
    [City]                         VARCHAR (50)  NOT NULL,
    [State]                        VARCHAR (50)  NOT NULL,
    [Country]                      VARCHAR (50)  NOT NULL,
    [ZipCode]                      VARCHAR (10)  NOT NULL,
    [ContactName]                  VARCHAR (50)  NOT NULL,
    [ContactPhone]                 VARCHAR (10)  NOT NULL,
    [ContactEmail]                 VARCHAR (50)  NOT NULL,
    [AlternateContactName]         VARCHAR (50)  NULL,
    [AlternateContactPhone]        VARCHAR (10)  NULL,
    [AlternateContactEmail]        VARCHAR (50)  NULL,
    [InvoiceContactName]           VARCHAR (50)  NOT NULL,
    [InvoiceContactPhone]          VARCHAR (10)  NOT NULL,
    [InvoiceContactEmail]          VARCHAR (50)  NOT NULL,
    [AlternateInvoiceContactName]  VARCHAR (50)  NULL,
    [AlternateInvoiceContactPhone] VARCHAR (10)  NULL,
    [AlternateInvoiceContactEmail] VARCHAR (50)  NULL,
    [CreatedBy]                    VARCHAR (50)  NOT NULL,
    [CreatedDate]                  DATETIME      NOT NULL,
    [ModifiedBy]                   VARCHAR (50)  NULL,
    [ModifiedDate]                 DATETIME      NULL,
    [TimeStamp]                    ROWVERSION    NOT NULL,
    CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED ([CompanyID] ASC),
    CONSTRAINT [FK_Company_ListValue] FOREIGN KEY ([CompanyTypeID]) REFERENCES [dbo].[ListValue] ([ListValueID]),
    CONSTRAINT [FK_Company_ListValue_InvoicingPeriod] FOREIGN KEY ([InvoicingPeriodID]) REFERENCES [dbo].[ListValue] ([ListValueID]),
    CONSTRAINT [FK_Company_ListValue_PaymentTerm] FOREIGN KEY ([PaymentTermID]) REFERENCES [dbo].[ListValue] ([ListValueID])
);







