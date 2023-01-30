CREATE TABLE [dbo].[Salary] (
    [SalaryID]           INT             IDENTITY (1, 1) NOT NULL,
    [EmployeeID]         INT             NOT NULL,
    [BasicPay]           DECIMAL (10, 2) NOT NULL,
    [HRA]                DECIMAL (10, 2) NOT NULL,
    [LTA]                DECIMAL (10, 2) NOT NULL,
    [Bonus]              DECIMAL (10, 2) NOT NULL,
    [EducationAllowance] DECIMAL (10, 2) NOT NULL,
    [Conveyance]         DECIMAL (10, 2) NOT NULL,
    [MealAllowance]      DECIMAL (10, 2) NOT NULL,
    [TelephoneAllowance] DECIMAL (10, 2) NOT NULL,
    [MedicalAllowance]   DECIMAL (10, 2) NOT NULL,
    [MedicalInsurance]   DECIMAL (10, 2) NOT NULL,
    [VariablePay]        DECIMAL (10, 2) NOT NULL,
    [SpecialAllowance]   DECIMAL (10, 2) NOT NULL,
    [ProvidentFund]      DECIMAL (10, 2) NOT NULL,
    [Gratuity]           DECIMAL (10, 2) NOT NULL,
    [CostToCompany]      DECIMAL (10, 2) NOT NULL,
    [StartDate]          DATE            NOT NULL,
    [EndDate]            DATE            NULL,
    [CreatedBy]          VARCHAR (50)    NOT NULL,
    [CreatedDate]        DATETIME        NOT NULL,
    [ModifiedBy]         VARCHAR (50)    NULL,
    [ModifiedDate]       DATETIME        NULL,
    [TimeStamp]          ROWVERSION      NOT NULL,
    CONSTRAINT [PK_Salary] PRIMARY KEY CLUSTERED ([SalaryID] ASC)
);





