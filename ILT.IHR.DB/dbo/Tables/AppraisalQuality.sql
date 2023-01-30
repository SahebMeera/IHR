CREATE TABLE [dbo].[AppraisalQuality] (
    [AppraisalQualityID] INT           IDENTITY (1, 1) NOT NULL,
    [AppraisalYear]      INT           NOT NULL,
    [Quality]            VARCHAR (100) NOT NULL,
    [ResponseTypeID]     INT           NOT NULL,
    [CreatedBy]          VARCHAR (50)  NOT NULL,
    [CreatedDate]        DATETIME      NOT NULL,
    [ModifiedBy]         VARCHAR (50)  NULL,
    [ModifiedDate]       DATETIME      NULL,
    [TimeStamp]          ROWVERSION    NULL,
    CONSTRAINT [PK_AppraisalQuality] PRIMARY KEY CLUSTERED ([AppraisalQualityID] ASC)
);



