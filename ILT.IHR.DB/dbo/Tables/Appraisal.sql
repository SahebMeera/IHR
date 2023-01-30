CREATE TABLE [dbo].[Appraisal] (
    [AppraisalID]     INT            IDENTITY (1, 1) NOT NULL,
    [ReviewYear]      INT            NOT NULL,
    [EmployeeID]      INT            NOT NULL,
    [ReviewerID]      INT            NULL,
    [FinalReviewerID] INT            NULL,
    [AssignedDate]    DATETIME       NULL,
    [SubmitDate]      DATETIME       NULL,
    [ReviewDate]      DATETIME       NULL,
    [FinalReviewDate] DATETIME       NULL,
    [MgrFeedback]     VARCHAR (1000) NULL,
    [Comment]         VARCHAR (500)  NULL,
    [StatusID]        INT            NOT NULL,
    [CreatedBy]       VARCHAR (50)   NOT NULL,
    [CreatedDate]     DATETIME       NOT NULL,
    [ModifiedBy]      VARCHAR (50)   NULL,
    [ModifiedDate]    DATETIME       NULL,
    [TimeStamp]       ROWVERSION     NOT NULL,
    CONSTRAINT [PK_Appraisal] PRIMARY KEY CLUSTERED ([AppraisalID] ASC)
);







