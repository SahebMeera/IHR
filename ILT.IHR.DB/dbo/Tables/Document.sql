CREATE TABLE [dbo].[Document] (
    [DocumentID]         INT          IDENTITY (1, 1) NOT NULL,
    [EmployeeID]         INT          NULL,
    [CompanyID]          INT          NULL,
    [DocumentCategoryID] INT          NOT NULL,
    [DocumentTypeID]     INT          NOT NULL,
    [IssuingAuthority]   VARCHAR (50) NULL,
    [DocumentNumber]     VARCHAR (20) NULL,
    [IssueDate]          DATE         NULL,
    [ExpiryDate]         DATE         NULL,
    [Note]               VARCHAR (50) NULL,
    [CreatedBy]          VARCHAR (50) NOT NULL,
    [CreatedDate]        DATETIME     NOT NULL,
    [ModifiedBy]         VARCHAR (50) NULL,
    [ModifiedDate]       DATETIME     NULL,
    [TimeStamp]          ROWVERSION   NOT NULL,
    CONSTRAINT [PK_Document] PRIMARY KEY CLUSTERED ([DocumentID] ASC)
);

