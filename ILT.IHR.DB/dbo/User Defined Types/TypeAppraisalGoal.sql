CREATE TYPE [dbo].[TypeAppraisalGoal] AS TABLE (
    [AppraisalGoalID] INT           NOT NULL,
    [AppraisalID]     INT           NOT NULL,
    [Goal]            VARCHAR (500) NOT NULL,
    [EmpResponse]     INT           NULL,
    [EmpComment]      VARCHAR (200) NULL,
    [MgrResponse]     INT           NULL,
    [MgrComment]      VARCHAR (200) NULL);

