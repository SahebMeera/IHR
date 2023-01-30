CREATE TYPE [dbo].[TypeAppraisalDetail] AS TABLE (
    [AppraisalDetailID]  INT           NOT NULL,
    [AppraisalID]        INT           NOT NULL,
    [AppraisalQualityID] INT           NOT NULL,
    [EmpResponse]        INT           NULL,
    [EmpComment]         VARCHAR (200) NULL,
    [MgrResponse]        INT           NULL,
    [MgrComment]         VARCHAR (200) NULL);



