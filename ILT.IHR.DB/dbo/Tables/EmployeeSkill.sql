CREATE TABLE [dbo].[EmployeeSkill] (
    [EmployeeSkillID] INT           IDENTITY (1, 1) NOT NULL,
    [EmployeeID]      INT           NOT NULL,
    [Skill]           VARCHAR (100) NOT NULL,
    [SkillTypeID]     INT           NOT NULL,
    [Experience]      INT           NOT NULL,
    [CreatedBy]       VARCHAR (50)  NOT NULL,
    [CreatedDate]     DATETIME      NOT NULL,
    [ModifiedBy]      VARCHAR (50)  NULL,
    [ModifiedDate]    DATETIME      NULL,
    [TimeStamp]       ROWVERSION    NOT NULL,
    CONSTRAINT [PK_EmployeeSkill] PRIMARY KEY CLUSTERED ([EmployeeSkillID] ASC),
    CONSTRAINT [FK_EmployeeID_Employee] FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[Employee] ([EmployeeID]),
    CONSTRAINT [FK_EmployeeSkill_ListValue] FOREIGN KEY ([SkillTypeID]) REFERENCES [dbo].[ListValue] ([ListValueID])
);

