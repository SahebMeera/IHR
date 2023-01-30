CREATE TABLE [dbo].[LeaveBalance] (
    [LeaveBalanceID] INT            IDENTITY (1, 1) NOT NULL,
    [EmployeeID]     INT            NOT NULL,
    [LeaveYear]      INT            NOT NULL,
    [LeaveTypeID]    INT            NOT NULL,
    [VacationTotal]  NUMERIC (5, 1) NOT NULL,
    [VacationUsed]   NUMERIC (5, 1) NOT NULL,
    [UnpaidLeave]    NUMERIC (5, 1) CONSTRAINT [DF_LeaveBalance_LWPTotal] DEFAULT ((0)) NOT NULL,
    [EncashedLeave]  NUMERIC (5, 1) CONSTRAINT [DF__LeaveBala__Encas__0618D7E0] DEFAULT ((0)) NULL,
    [CreatedBy]      VARCHAR (50)   NOT NULL,
    [CreatedDate]    DATETIME       NOT NULL,
    [ModifiedBy]     VARCHAR (50)   NULL,
    [ModifiedDate]   DATETIME       NULL,
    [TimeStamp]      ROWVERSION     NOT NULL,
    CONSTRAINT [PK_LeaveBalance] PRIMARY KEY CLUSTERED ([LeaveBalanceID] ASC),
    CONSTRAINT [FK_LeaveBalance_Employee] FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[Employee] ([EmployeeID]),
    CONSTRAINT [FK_LeaveBalance_ListValue_LeaveType] FOREIGN KEY ([LeaveTypeID]) REFERENCES [dbo].[ListValue] ([ListValueID])
);











