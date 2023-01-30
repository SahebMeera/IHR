CREATE TABLE [dbo].[Leave] (
    [LeaveID]         INT            IDENTITY (1, 1) NOT NULL,
    [EmployeeID]      INT            NOT NULL,
    [Title]           VARCHAR (100)  NOT NULL,
    [Detail]          VARCHAR (500)  NULL,
    [StartDate]       DATE           NOT NULL,
    [EndDate]         DATE           NOT NULL,
    [IncludesHalfDay] BIT            CONSTRAINT [DF_Leave_IncludesHalfDay] DEFAULT ((0)) NULL,
    [Duration]        NUMERIC (5, 1) NULL,
    [LeaveTypeID]     INT            NOT NULL,
    [RequesterID]     INT            NOT NULL,
    [ApproverID]      INT            NOT NULL,
    [StatusID]        INT            NOT NULL,
    [Comment]         VARCHAR (500)  NULL,
    [CreatedBy]       VARCHAR (50)   NOT NULL,
    [CreatedDate]     DATETIME       NOT NULL,
    [ModifiedBy]      VARCHAR (50)   NULL,
    [ModifiedDate]    DATETIME       NULL,
    [TimeStamp]       ROWVERSION     NOT NULL,
    CONSTRAINT [PK_Leave] PRIMARY KEY CLUSTERED ([LeaveID] ASC),
    CONSTRAINT [FK_Leave_Employee] FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[Employee] ([EmployeeID]),
    CONSTRAINT [FK_Leave_Employee_Approver] FOREIGN KEY ([ApproverID]) REFERENCES [dbo].[Employee] ([EmployeeID]),
    CONSTRAINT [FK_Leave_Employee_Requester] FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[Employee] ([EmployeeID]),
    CONSTRAINT [FK_Leave_ListValue_LeaveType] FOREIGN KEY ([LeaveTypeID]) REFERENCES [dbo].[ListValue] ([ListValueID])
);





