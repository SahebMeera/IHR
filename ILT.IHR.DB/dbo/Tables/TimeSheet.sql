CREATE TABLE [dbo].[TimeSheet] (
    [TimeSheetID]     INT              IDENTITY (1, 1) NOT NULL,
    [EmployeeID]      INT              NOT NULL,
    [WeekEnding]      DATE             NOT NULL,
    [ClientID]        INT              NOT NULL,
    [AssignmentID]    INT              NULL,
    [TotalHours]      INT              NOT NULL,
    [FileName]        VARCHAR (100)    NULL,
    [DocGuid]         UNIQUEIDENTIFIER NULL,
    [StatusID]        INT              NOT NULL,
    [SubmittedByID]   INT              NULL,
    [SubmittedDate]   DATETIME         NULL,
    [ApprovedDate]    DATETIME         NULL,
    [ApprovedByEmail] VARCHAR (50)     NULL,
    [ClosedByID]      INT              NULL,
    [ClosedDate]      DATETIME         NULL,
    [Comment]         VARCHAR (500)    NULL,
    [CreatedBy]       VARCHAR (50)     NOT NULL,
    [CreatedDate]     DATETIME         CONSTRAINT [DF_TimeSheet_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [ModifiedBy]      VARCHAR (50)     NULL,
    [ModifiedDate]    DATETIME         NULL,
    CONSTRAINT [PK_TimeSheet] PRIMARY KEY CLUSTERED ([TimeSheetID] ASC),
    CONSTRAINT [FK_TimeSheet_Assignment] FOREIGN KEY ([AssignmentID]) REFERENCES [dbo].[Assignment] ([AssignmentID]),
    CONSTRAINT [FK_TimeSheet_Company] FOREIGN KEY ([ClientID]) REFERENCES [dbo].[Company] ([CompanyID]),
    CONSTRAINT [FK_TimeSheet_Employee] FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[Employee] ([EmployeeID]),
    CONSTRAINT [FK_TimeSheet_ListValue_StatusID] FOREIGN KEY ([StatusID]) REFERENCES [dbo].[ListValue] ([ListValueID]),
    CONSTRAINT [FK_TimeSheet_User_ClosedBy] FOREIGN KEY ([ClosedByID]) REFERENCES [dbo].[User] ([UserID]),
    CONSTRAINT [FK_TimeSheet_User_SubmittedBy] FOREIGN KEY ([SubmittedByID]) REFERENCES [dbo].[User] ([UserID])
);

















