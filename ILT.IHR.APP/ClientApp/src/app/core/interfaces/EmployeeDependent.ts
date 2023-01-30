export class IEmployeeDependent{ 
    DependentID: number;
    RelationID: number;
    Relation?: string;
    VisaTypeID: number;
    VisaType?: string;
    EmployeeID: number;
    EmployeeName: string;
    FirstName: string;
    LastName: string;
    BirthDate: Date;
    CreatedBy: string;
    CreatedDate: Date;
    ModifiedBy: string;
    ModifiedDate: Date;
    TimeStamp: ['']
}
export class IEmployeeDependentDisplay {
    dependentID: number;
    relationID: number;
    relation: string;
    visaTypeID: number;
    visaType: string;
    employeeID: number;
    employeeName: string;
    firstName: string;
    lastName: string;
    birthDate: Date;
    createdBy: string;
    createdDate: Date;
    modifiedBy: string;
    modifiedDate: Date;
}
