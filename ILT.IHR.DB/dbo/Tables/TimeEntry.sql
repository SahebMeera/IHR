CREATE TABLE [dbo].[TimeEntry] (
    [TimeEntryID] INT          IDENTITY (1, 1) NOT NULL,
    [TimeSheetID] INT          NOT NULL,
    [Project]     VARCHAR (50) NULL,
    [Activity]    VARCHAR (50) NULL,
    [WorkDate]    DATE         NOT NULL,
    [Hours]       INT          NOT NULL,
    CONSTRAINT [PK_TimeEntry] PRIMARY KEY CLUSTERED ([TimeEntryID] ASC),
    CONSTRAINT [FK_TimeEntry_TimeSheet] FOREIGN KEY ([TimeSheetID]) REFERENCES [dbo].[TimeSheet] ([TimeSheetID])
);



