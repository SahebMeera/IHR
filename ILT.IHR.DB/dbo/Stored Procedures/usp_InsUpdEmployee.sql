--===============================================================  
-- Author : Sudeep Kukreti  
-- Created Date : 05/23/2020  
-- Description : Insert/Update SP for Employee  
--  
-- Revision History:  
-----------------------------------------------------------------  
-- Date            By          Description  
-- 02/11/2021 Mihir Hapaliya updated sp to add or update employee addresses   
--===============================================================  
  
CREATE PROCEDURE [dbo].[usp_InsUpdEmployee]  
       @EmployeeID   int    =      NULL,  
       @EmployeeCode varchar(12)   =      NULL,  
       @FirstName    varchar(50)   =      NULL,  
       @MiddleName   varchar(50)   =      NULL,  
       @LastName     varchar(50)   =      NULL,  
       @Country      varchar(50)   =      NULL,  
    @xmlEmployeeAddress xml = NULL,  
       @TitleID      int    =      NULL,  
       @GenderID     int    =      NULL,  
       @DepartmentID int    =      NULL,  
       @Phone varchar(10)   =      NULL,  
       @HomePhone    varchar(10)   =      NULL,  
       @WorkPhone    varchar(10)   =      NULL,  
       @Email varchar(50)   =      NULL,  
       @WorkEmail    varchar(50)   =      NULL,  
       @BirthDate    date   =      NULL,  
       @HireDate     date   =      NULL,  
       @TermDate     date   =      NULL,  
       @WorkAuthorizationID int    =      NULL,  
    @SSN varchar(9)    =      NULL,  
    @PAN varchar(10)    =      NULL,  
    @AadharNumber varchar(12)    =      NULL,  
       @Salary       int    =      NULL,  
       @MaritalStatusID     int    =      NULL,  
       @ManagerID int       =      NULL,  
       @EmploymentTypeID int      =      NULL, 
	   @VariablePay int = 0, 
       @IsDeleted    bit    =      NULL,  
       @CreatedBy    varchar(50)   =      NULL,  
       @ModifiedBy   varchar(50)   =      NULL,  
       @ReturnCode INT = 0 OUTPUT   
AS   
BEGIN  
       SET NOCOUNT ON;    
       SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED  
       DECLARE @IDTable TABLE(ID INT)  
    DECLARE @IDTable1 TABLE(ID INT, EmployeeID INT)  
       DECLARE @TypeEmployee TypeEmployee  
    DECLARE @tblEmployeeAddress TypeEmployeeAddress  
  
  INSERT INTO @tblEmployeeAddress  
  (  
   EmployeeAddressID,  
   EmployeeID,   
   AddressTypeID,  
   Address1,  
   Address2,  
   City,  
   [State],  
   Country,  
   ZipCode,  
   StartDate,   
   EndDate  
  )  
  SELECT   
  x.v.value('EmployeeAddressID[1]','int'),  
  x.v.value('EmployeeID[1]','int'),  
  x.v.value('AddressTypeID[1]','int'),  
  x.v.value('Address1[1]','varchar(100)'),   
  x.v.value('Address2[1]','varchar(100)'),  
  x.v.value('City[1]','varchar(50)'),   
  x.v.value('State[1]','varchar(50)'),  
  x.v.value('Country[1]','varchar(50)'),   
  x.v.value('ZipCode[1]','varchar(10)'),  
  x.v.value('StartDate[1]','date'),  
  CASE WHEN x.v.value('EndDate[1]','date') = '1/1/1900' THEN NULL ELSE x.v.value('EndDate[1]','date') END  
  FROM @xmlEmployeeAddress.nodes('ArrayOfEmployeeAddress/EmployeeAddress') as x(v)  
  
       IF NOT EXISTS(SELECT 1 FROM Employee WHERE EmployeeID=ISNULL(@EmployeeID,0))    
              BEGIN   
     DECLARE @TempEmpCode VARCHAR(12);  
      SELECT @TempEmpCode = dbo.GenerateEmployeeCode(@EmploymentTypeID, @Country)  
      IF(ISNULL(@TempEmpCode,'') <> '')  
      SET @EmployeeCode = @TempEmpCode  
  
                     INSERT INTO [dbo].[Employee]  
                     (  
                           EmployeeCode,  
                           FirstName,  
                           MiddleName,  
                           LastName,  
                           Country,  
                           TitleID,  
                           GenderID,  
                           DepartmentID,  
                           Phone,  
                           HomePhone,  
                           WorkPhone,  
                           Email,  
                           WorkEmail,  
                           BirthDate,  
                           HireDate,  
                           TermDate,  
      WorkAuthorizationID,  
         SSN,  
         PAN,  
         AadharNumber,  
                           Salary,  
         VariablePay,  
                           MaritalStatusID,  
                           ManagerID,  
                           EmploymentTypeID,  
                           IsDeleted,  
                           CreatedBy,  
                           CreatedDate,  
         ModifiedBy,  
                           ModifiedDate  
                     )  
                     OUTPUT INSERTED.EmployeeID INTO @IDTable   
                     VALUES  
                     (  
                           @EmployeeCode,  
                           @FirstName,  
                           @MiddleName,  
                           @LastName,  
                           @Country,  
                           @TitleID,  
                           @GenderID,  
                           @DepartmentID,  
                           @Phone,  
                           @HomePhone,  
                           @WorkPhone,  
                           @Email,  
                           @WorkEmail,  
                           @BirthDate,  
                           @HireDate,  
                           @TermDate,  
                           @WorkAuthorizationID,  
         @SSN,  
         @PAN,  
         @AadharNumber,  
                           @Salary,  
         @VariablePay,  
                           @MaritalStatusID,  
                           @ManagerID,  
                           @EmploymentTypeID,  
                           @IsDeleted,  
                           @CreatedBy,  
                           GETDATE(),  
         @CreatedBy,  
                           GETDATE()  
                     )  
                     SELECT @EmployeeID=(SELECT ID FROM @IDTable);  
        
     INSERT INTO [dbo].[EmployeeAddress]  
     (  
      EmployeeID,   
      AddressTypeID,  
      Address1,  
      Address2,  
      City,  
      [State],  
      Country,  
      ZipCode,  
      StartDate,   
      EndDate,  
      CreatedBy,  
      CreatedDate  
     )  
     SELECT   
      @EmployeeID,  
      AddressTypeID,  
      Address1,  
      Address2,  
      City,  
      [State],  
      Country,  
      ZipCode,  
      StartDate,   
      EndDate,  
      @CreatedBy,  
      GETDATE()  
     FROM @tblEmployeeAddress  
  
              END  
       ELSE  
              BEGIN  
                     UPDATE E  
                     SET  
                     EmployeeCode = @EmployeeCode,  
                     FirstName = @FirstName,  
                     MiddleName = @MiddleName,  
                     LastName = @LastName,  
                     Country = @Country,  
                     TitleID = @TitleID,  
                     GenderID = @GenderID,  
                     DepartmentID = @DepartmentID,  
                     Phone = @Phone,  
                     HomePhone = @HomePhone,  
                     WorkPhone = @WorkPhone,  
                     Email = @Email,  
                     WorkEmail = @WorkEmail,  
                     BirthDate = @BirthDate,  
                     HireDate = @HireDate,  
                     TermDate = @TermDate,  
                     WorkAuthorizationID = @WorkAuthorizationID,  
      SSN = @SSN,  
      PAN = @PAN,  
      AadharNumber = @AadharNumber,  
                     Salary = @Salary,  
      VariablePay = @VariablePay,  
                     MaritalStatusID = @MaritalStatusID,  
                     ManagerID = @ManagerID,  
                     EmploymentTypeID = @EmploymentTypeID,  
                     IsDeleted = @IsDeleted,  
                     ModifiedBy = @ModifiedBy,  
                     ModifiedDate = GETDATE()  
                     OUTPUT DELETED.[EmployeeID],DELETED.[EmployeeCode],DELETED.[FirstName],DELETED.[MiddleName]  
          ,DELETED.[LastName],DELETED.[Country]  
          ,DELETED.[TitleID],DELETED.[GenderID],DELETED.[DepartmentID],DELETED.[Phone]  
       ,DELETED.[HomePhone],DELETED.[WorkPhone],DELETED.[Email],DELETED.[WorkEmail],DELETED.[BirthDate]  
       ,DELETED.[HireDate],DELETED.[TermDate],DELETED.[WorkAuthorizationID]  
       ,DELETED.[SSN],DELETED.[PAN],DELETED.[AadharNumber]  
       ,DELETED.[Salary], DELETED.[VariablePay], DELETED.[MaritalStatusID],DELETED.[ManagerID]  
       ,DELETED.[EmploymentTypeID],DELETED.[IsDeleted],DELETED.[CreatedBy],DELETED.[CreatedDate]  
       ,DELETED.[ModifiedBy],DELETED.[ModifiedDate]  INTO @TypeEmployee  
                     FROM [dbo].[Employee] E  
                     WHERE EmployeeID = @EmployeeID  
  
      INSERT INTO EmployeeChangeSet  
      OUTPUT INSERTED.EmployeeChangeSetID, INSERTED.EmployeeID INTO @IDTable1      
      SELECT TE.* FROM @TypeEmployee TE  
      JOIN Employee E ON TE.EmployeeID = E.EmployeeID  
      WHERE   
      TE.EmployeeCode <> E.EmployeeCode  
      OR TE.FirstName <> E.FirstName  
      OR TE.MiddleNAme <> E.MiddleName  
      OR TE.LastName <> E.LastName  
      OR TE.Country <> E.Country  
      OR TE.TitleID <> E.TitleID  
      OR TE.DepartmentID <> E.DepartmentID  
      OR TE.Email <> E.Email  
      OR TE.BirthDate <> E.BirthDate  
      OR TE.HireDate <> E.HireDate  
      OR TE.TermDate <> E.TermDate  
      OR TE.WorkAuthorizationID <> E.WorkAuthorizationID  
      OR TE.SSN <> E.SSN  
      OR TE.PAN <> E.PAN  
      OR TE.AadharNumber <> E.AadharNumber  
      OR TE.Salary <> E.Salary  
      OR TE.MaritalStatusID <> E.MaritalStatusID  
      OR TE.ManagerID <> E.ManagerID  
      OR TE.EmploymentTypeID <> E.EmploymentTypeID  
      OR TE.VariablePay <> E.VariablePay  
  
      INSERT INTO Notification   
      SELECT DISTINCT 'Employee', T.ID, T.EmployeeID, U.UserID, 0, NULL  
      FROM @IDTable1 T  
      JOIN [USERROLE] U ON 1=1
      JOIN [RoleNotification] RN ON U.RoleID = RN.RoleID AND TableName = 'Employee' and RN.IsActive = 1  
	  JOIN [User] U1 ON U.UserID = U1.UserID
	  WHERE U1.FirstName + ' ' + U1.LastName <> @ModifiedBy
  
     MERGE EmployeeAddress AS TARGET  
     USING @tblEmployeeAddress AS SOURCE   
     ON (TARGET.EmployeeAddressID = SOURCE.EmployeeAddressID)   
     WHEN MATCHED   
     THEN UPDATE SET TARGET.AddressTypeID = SOURCE.AddressTypeID,   
         TARGET.Address1 = SOURCE.Address1, TARGET.[Address2] = SOURCE.[Address2],  
         TARGET.City = SOURCE.City, TARGET.[State] = SOURCE.[State], TARGET.Country = SOURCE.Country,  
         TARGET.ZipCode = SOURCE.ZipCode,TARGET.StartDate = SOURCE.StartDate, TARGET.EndDate = SOURCE.EndDate, TARGET.ModifiedBy = @ModifiedBy,  
         TARGET.ModifiedDate = GetDate()  
     WHEN NOT MATCHED BY TARGET   
     THEN INSERT (EmployeeID, AddressTypeID, Address1, Address2, City, [State], Country, ZipCode, StartDate, EndDate,   
        CreatedBy, CreatedDate)   
      VALUES (@EmployeeID,SOURCE.AddressTypeID, SOURCE.Address1, SOURCE.Address2, SOURCE.City, SOURCE.[State],SOURCE.Country,  
        SOURCE.ZipCode, SOURCE.StartDate,SOURCE.EndDate, @CreatedBy, GetDate())  
     WHEN NOT MATCHED BY SOURCE AND TARGET.EmployeeID = @EmployeeID  
     THEN DELETE;  
  
     UPDATE EA  
     SET EA.EndDate = DATEADD(day,-1, EA1.StartDate)  
     FROM EmployeeAddress EA  
     INNER JOIN @tblEmployeeAddress EA1 ON EA.EmployeeID = EA1.EmployeeID AND EA.AddressTypeID = EA1.AddressTypeID  
     AND EA.StartDate < EA1.StartDate --AND EA.EndDate IS NULL   
  
              END  
       IF @@ERROR=0     
              SET @ReturnCode  = @EmployeeID;    
       ELSE  
              SET @ReturnCode = 0  
       RETURN      
END
