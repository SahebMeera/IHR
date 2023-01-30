CREATE TABLE [dbo].[LeaveAccrual] (
    [LeaveAccrualID] INT            IDENTITY (1, 1) NOT NULL,
    [Country]        VARCHAR (50)   NULL,
    [AccruedDate]    DATE           NOT NULL,
    [AccruedValue]   NUMERIC (5, 1) NOT NULL,
    [CreatedBy]      VARCHAR (50)   NOT NULL,
    [CreatedDate]    DATETIME       NOT NULL,
    [ModifiedBy]     VARCHAR (50)   NULL,
    [ModifiedDate]   DATETIME       NULL,
    [TimeStamp]      ROWVERSION     NOT NULL,
    CONSTRAINT [PK_LeaveAccrualID] PRIMARY KEY CLUSTERED ([LeaveAccrualID] ASC)
);







