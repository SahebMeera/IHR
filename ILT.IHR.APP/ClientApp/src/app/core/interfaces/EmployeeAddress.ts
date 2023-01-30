export class IEmployeeAddress {
    EmployeeName: string;
    EmployeeAddressID: number;
    EmployeeID: number;
    AddressTypeID: number;
    Address1: string;
    Address2: string;
    City: string;
    State: string;
    Country: string;
    ZipCode: string;
    StartDate: Date;
    EndDate: Date;
    CreatedBy: string;
    CreatedDate: Date;
    ModifiedBy: string;
    ModifiedDate: Date;
    TimeStamp: string;
}
export class IEmployeeAddressDisplay {
    employeeAddressID: number;
    employeeID: number;
    employeeName: string;
    addressTypeID: number;
    address1: string;
    address2: string;
    city: string;
    state: string;
    country: string;
    zipCode: string;
    startDate: Date;
    endDate: Date;
}
