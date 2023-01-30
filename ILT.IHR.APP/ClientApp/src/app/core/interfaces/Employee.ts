import { IEmployeeAddress, IEmployeeAddressDisplay } from "./EmployeeAddress";

export class IEmployee {
    EmployeeID: number;
    EmployeeCode: string;
    FirstName: string;
    MiddleName: string;
    LastName: string;
    Country: string;
    TitleID: number;
    GenderID: number;
    DepartmentID: number;
    Phone: string;
    HomePhone: string;
    WorkPhone: string;
    Email: string;
    WorkEmail: string;
    BirthDate: Date;
    HireDate: Date;
    TermDate: Date;
    WorkAuthorizationID: number;
    SSN: string;
    PAN: string;
    AadharNumber: string;
    Salary: number;
    VariablePay: number;
    MaritalStatusID: number;
    ManagerID: number
    EmploymentTypeID: number;
    IsDeleted: boolean;
    CreatedBy: string;
    CreatedDate: Date;
    ModifiedBy: string;
    ModifiedDate: Date;
    TimeStamp: ['']
    Title?: string;
    Gender?: string;
    Department?: string;
    WorkAuthorization?: string;
    MaritalStatus?: string;
    Manager?: string;
    EmploymentType?: string;
    EmployeeAddresses: IEmployeeAddress[];
}
export class IEmployeeDisplay {
    employeeID: number;
    employeeCode: string;
    firstName: string;
    middleName: string;
    lastName: string;
    employeeName: string;
    country: string;
    titleID: number
    title: string;
    genderID: number;
    gender: string;
    departmentID: number;
    department: string;
    phone: string;
    homePhone: string;
    workPhone: string;
    email: string;
    loginEmail: string;
    workEmail: string;
    birthDate: Date;
    hireDate: Date;
    termDate: Date;
    workAuthorizationID: number;
    workAuthorization: string;
    sSN: string;
    pAN: string;
    aadharNumber: string;
    aalary: number;
    variablePay: number;
    maritalStatusID: number;
    maritalStatus: string;
    managerID: number;
    manager: string;
    managerEmail: string;
    employmentTypeID: number;
    employmentType: string;
    isDeleted: boolean;
    createdBy: string;
    createdDate: Date;
    modifiedBy: string;
    modifiedDate: Date;
    employeeAddresses: IEmployeeAddress[];
}