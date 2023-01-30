CREATE TABLE [dbo].[WFH] (
    [WFHID]        INT           IDENTITY (1, 1) NOT NULL,
    [EmployeeID]   INT           NOT NULL,
    [Title]        VARCHAR (100) NOT NULL,
    [StartDate]    DATE          NOT NULL,
    [EndDate]      DATE          NOT NULL,
    [RequesterID]  INT           NOT NULL,
    [ApproverID]   INT           NOT NULL,
    [StatusID]     INT           NOT NULL,
    [Comment]      VARCHAR (500) NULL,
    [CreatedBy]    VARCHAR (50)  NOT NULL,
    [CreatedDate]  DATETIME      NOT NULL,
    [ModifiedBy]   VARCHAR (50)  NULL,
    [ModifiedDate] DATETIME      NULL,
    [TimeStamp]    ROWVERSION    NOT NULL,
    CONSTRAINT [PK_WFH] PRIMARY KEY CLUSTERED ([WFHID] ASC)
);

