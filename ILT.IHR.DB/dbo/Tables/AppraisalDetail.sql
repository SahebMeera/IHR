CREATE TABLE [dbo].[AppraisalDetail] (
    [AppraisalDetailID]  INT           IDENTITY (1, 1) NOT NULL,
    [AppraisalID]        INT           NOT NULL,
    [AppraisalQualityID] INT           NOT NULL,
    [EmpResponse]        INT           NULL,
    [EmpComment]         VARCHAR (200) NULL,
    [MgrResponse]        INT           NULL,
    [MgrComment]         VARCHAR (200) NULL,
    CONSTRAINT [PK_AppraisalDetail] PRIMARY KEY CLUSTERED ([AppraisalDetailID] ASC)
);



