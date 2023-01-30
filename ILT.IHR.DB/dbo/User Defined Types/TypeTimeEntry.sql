CREATE TYPE [dbo].[TypeTimeEntry] AS TABLE (
    [TimeEntryID] INT          NOT NULL,
    [TimeSheetID] INT          NOT NULL,
    [Project]     VARCHAR (50) NULL,
    [Activity]    VARCHAR (50) NULL,
    [WorkDate]    DATE         NOT NULL,
    [Hours]       INT          NOT NULL);

