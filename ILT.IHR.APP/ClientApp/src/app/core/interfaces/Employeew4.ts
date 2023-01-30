export class IEmployeeW4 {
    EmployeeW4ID: number;
    W4TypeID: number;
    W4Type?: string;
    WithHoldingStatusID: number;
    WithHoldingStatus?: string;
    EmployeeID: number;
    EmployeeName: string;
    SSN: string;
    Allowances: number;
    IsMultipleJobsOrSpouseWorks: boolean;
    QualifyingChildren: number;
    OtherDependents: number;
    OtherIncome: string;
    Deductions: string;
    StartDate: Date;
    EndDate: Date;
    CreatedBy: string;
    CreatedDate: Date;
    ModifiedBy: string;
    ModifiedDate: Date;
    TimeStamp: ['']
}
export class IEmployeeW4Display {
    employeeW4ID: number;
    w4TypeID: number;
    w4Type: string;
    WithHoldingStatusID: number;
    withHoldingStatus: string;
    employeeID: number;
    employeeName: string;
    ssn: string;
    allowances: number;
    isMultipleJobsOrSpouseWorks: boolean;
    qualifyingChildren: number;
    otherDependents: number;
    otherIncome: string;
    deductions: string;
    startDate: Date;
    endDate: Date;
    createdBy: string;
    createdDate: Date;
    modifiedBy: string;
    modifiedDate: Date;
}
