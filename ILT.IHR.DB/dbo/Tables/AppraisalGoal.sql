CREATE TABLE [dbo].[AppraisalGoal] (
    [AppraisalGoalID] INT           IDENTITY (1, 1) NOT NULL,
    [AppraisalID]     INT           NOT NULL,
    [Goal]            VARCHAR (500) NULL,
    [EmpResponse]     INT           NULL,
    [EmpComment]      VARCHAR (200) NULL,
    [MgrResponse]     INT           NULL,
    [MgrComment]      VARCHAR (200) NULL,
    CONSTRAINT [PK_AppraisalGoal] PRIMARY KEY CLUSTERED ([AppraisalGoalID] ASC)
);

