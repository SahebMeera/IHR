CREATE PROCEDURE [dbo].[usp_onBoarding]
	@WizardDataID	int	=	NULL,	
	@ReturnCode INT = 0 OUTPUT 

AS 
BEGIN
	SET NOCOUNT ON;  
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	
	DECLARE @DataField XML;
	DECLARE @IDTable1 TABLE(ID INT);
	DECLARE @IDTable2 TABLE(ID INT);
	DECLARE @IDTable3 TABLE(ID INT);
	DECLARE @CompanyID int = NULL;
	DECLARE @EmployeeID int = NULL;
	DECLARE @AssignmentID int = NULL;

	SELECT @DataField = Data FROM WizardData WHERE WizardDataID = @WizardDataID  
	
	INSERT INTO Company(Name,CompanyTypeID,InvoicingPeriodID,PaymentTermID,TaxID,Address1,Address2,City,State,Country,ZipCode,ContactName,ContactPhone,
			ContactEmail,InvoiceContactName,InvoiceContactPhone,InvoiceContactEmail,CreatedBy,CreatedDate)
	OUTPUT INSERTED.CompanyID INTO @IDTable1
	SELECT
		[Table].[Column].value('ClientName[1]', 'varchar(50)') as 'ClientName', 44 as 'CompanyType',
		[Table].[Column].value('InvoicingPeriod[1]', 'varchar(50)') as 'InvoicingPeriod',
		[Table].[Column].value('PaymentTerm[1]', 'varchar(50)') as 'PaymentTerm',
		[Table].[Column].value('Tax[1]', 'varchar(50)') as 'Tax',
		[Table].[Column].value('Address1[1]', 'varchar(50)') as 'Address1',
		[Table].[Column].value('Address2[1]', 'varchar(50)') as 'Address2',
		[Table].[Column].value('City[1]', 'varchar(50)') as 'City',
		[Table].[Column].value('State[1]', 'varchar(50)') as 'State',
		[Table].[Column].value('Country[1]', 'varchar(50)') as 'Country',
		[Table].[Column].value('ZipCode[1]', 'varchar(50)') as 'ZipCode',
		[Table].[Column].value('ContactName[1]', 'varchar(50)') as 'ContactName',
		[Table].[Column].value('ContactPhone[1]', 'varchar(50)') as 'ContactPhone',
		[Table].[Column].value('ContactEmail[1]', 'varchar(50)') as 'ContactEmail',
		[Table].[Column].value('InvoiceContactName[1]', 'varchar(50)') as 'InvoiceContactName',
		[Table].[Column].value('InvoiceContactPhone[1]', 'varchar(50)') as 'InvoiceContactPhone',
		[Table].[Column].value('InvoiceContactEmail[1]', 'varchar(50)') as 'InvoiceContactEmail',
		'Admin' as 'CreatedBy', GETDATE() as 'CreatedDate'
		 FROM @DataField.nodes('/WizardData/ClientInfo') as [Table]([Column])      

	INSERT INTO Employee(EmployeeCode,FirstName,MiddleName,LastName,GenderID,DepartmentID,Phone,Email,BirthDate,HireDate,WorkAuthorizationID,
			Salary,MaritalStatusID,EmploymentTypeID,IsDeleted,CreatedBy,CreatedDate)
	OUTPUT INSERTED.EmployeeID INTO @IDTable2
	SELECT
		'EMP' as 'EmployeeCode',
		[Table].[Column].value('CandidateFirstName[1]', 'varchar(50)') as 'CandidateFirstName',
		[Table].[Column].value('CandidateMiddleName[1]', 'varchar(50)') as 'CandidateMiddleName',
		[Table].[Column].value('CandidateLastName[1]', 'varchar(50)') as 'CandidateLastName',
		[Table].[Column].value('Gender[1]', 'varchar(50)') as 'Gender',
		1 as 'DepartmentID',
		[Table].[Column].value('Phone[1]', 'varchar(50)') as 'Phone',
		[Table].[Column].value('Email[1]', 'varchar(50)') as 'Email',
		[Table].[Column].value('BirthDate[1]', 'varchar(50)') as 'BirthDate',
		[Table].[Column].value('HireDate[1]', 'varchar(50)') as 'HireDate',
		[Table].[Column].value('WorkAuthorization[1]', 'varchar(50)') as 'WorkAuthorization',
		0 as 'Salary',
		[Table].[Column].value('MaritalStatus[1]', 'varchar(50)') as 'MaritalStatus',
		[Table].[Column].value('EmploymentType[1]', 'varchar(50)') as 'EmploymentType',	
		0 as 'Department',
		'Admin' as 'CreatedBy', GETDATE() as 'CreatedDate'
		 FROM @DataField.nodes('/WizardData/CandidateInfo') as [Table]([Column])

	INSERT INTO Company(Name,CompanyTypeID,InvoicingPeriodID,PaymentTermID,TaxID,Address1,Address2,City,State,Country,ZipCode,ContactName,ContactPhone,
			ContactEmail,InvoiceContactName,InvoiceContactPhone,InvoiceContactEmail,CreatedBy,CreatedDate)
	SELECT
		[Table].[Column].value('CompanyName[1]', 'varchar(50)') as 'CompanyName', 45 as 'CompanyType',
		[Table].[Column].value('InvoicingPeriod[1]', 'varchar(50)') as 'InvoicingPeriod',
		[Table].[Column].value('PaymentTerm[1]', 'varchar(50)') as 'PaymentTerm',
		[Table].[Column].value('Tax[1]', 'varchar(50)') as 'Tax',
		[Table].[Column].value('Address1[1]', 'varchar(50)') as 'Address1',
		[Table].[Column].value('Address2[1]', 'varchar(50)') as 'Address2',
		[Table].[Column].value('City[1]', 'varchar(50)') as 'City',
		[Table].[Column].value('State[1]', 'varchar(50)') as 'State',
		[Table].[Column].value('Country[1]', 'varchar(50)') as 'Country',
		[Table].[Column].value('ZipCode[1]', 'varchar(50)') as 'ZipCode',
		[Table].[Column].value('ContactName[1]', 'varchar(50)') as 'ContactName',
		[Table].[Column].value('ContactPhone[1]', 'varchar(50)') as 'ContactPhone',
		[Table].[Column].value('ContactEmail[1]', 'varchar(50)') as 'ContactEmail',
		[Table].[Column].value('InvoiceContactName[1]', 'varchar(50)') as 'InvoiceContactName',
		[Table].[Column].value('InvoiceContactPhone[1]', 'varchar(50)') as 'InvoiceContactPhone',
		[Table].[Column].value('InvoiceContactEmail[1]', 'varchar(50)') as 'InvoiceContactEmail',
		'Admin' as 'CreatedBy', GETDATE() as 'CreatedDate'
		 FROM @DataField.nodes('/WizardData/VendorInfo') as [Table]([Column]) 
	
	SELECT @CompanyID=(SELECT ID FROM @IDTable1);
	SELECT @EmployeeID=(SELECT ID FROM @IDTable2);

	INSERT INTO Assignment(EmployeeID,StartDate,Address1,Address2,City,State,Country,ZipCode,ClientID,PaymentTypeID,CreatedBy,CreatedDate)
	OUTPUT INSERTED.AssignmentID INTO @IDTable3
	SELECT
		@EmployeeID as 'EmployeeID',
		[Table].[Column].value('StartDate[1]', 'varchar(50)') as 'StartDate',		
		[Table].[Column].value('Address1[1]', 'varchar(50)') as 'Address1',
		[Table].[Column].value('Address2[1]', 'varchar(50)') as 'Address2',
		[Table].[Column].value('City[1]', 'varchar(50)') as 'City',
		[Table].[Column].value('State[1]', 'varchar(50)') as 'State',
		[Table].[Column].value('Country[1]', 'varchar(50)') as 'Country',
		[Table].[Column].value('ZipCode[1]', 'varchar(50)') as 'ZipCode',
		@CompanyID as 'CompanyID',
		[Table].[Column].value('PaymentType[1]', 'varchar(50)') as 'PaymentType',		
		'Admin' as 'CreatedBy', GETDATE() as 'CreatedDate'
		 FROM @DataField.nodes('/WizardData/AssignmentInfo') as [Table]([Column]) 
	
	SELECT @AssignmentID=(SELECT ID FROM @IDTable3);

	INSERT INTO AssignmentRate(AssignmentID,BillingRate,PaymentRate,StartDate,CreatedBy,CreatedDate)
	OUTPUT INSERTED.AssignmentID INTO @IDTable3
	SELECT
		@AssignmentID as 'AssignmentID',
		[Table].[Column].value('BillRate[1]', 'varchar(50)') as 'BillRate',		
		[Table].[Column].value('PayRate[1]', 'varchar(50)') as 'PayRate',
		[Table].[Column].value('StartDate[1]', 'varchar(50)') as 'StartDate',			
		'Admin' as 'CreatedBy', GETDATE() as 'CreatedDate'
		 FROM @DataField.nodes('/WizardData/AssignmentInfo') as [Table]([Column]) 

	IF @@ERROR=0   
		SET @ReturnCode  = @WizardDataID;  
	ELSE
		SET @ReturnCode = 0
	RETURN
	
END