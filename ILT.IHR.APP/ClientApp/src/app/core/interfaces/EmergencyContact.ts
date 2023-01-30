export class IEmergnecyContact {
    ContactID: number;
    ContactTypeID: number;
    ContactType?: string;
    EmployeeID: number;
    EmployeeName: string;
    FirstName: string;
    LastName: string;
    Phone: string;
    Email: string;
    Address1: string;
    Address2: string;
    City: string;
    State: string;
    Country: string;
    ZipCode: string;
    IsDeleted: boolean;
    CreatedBy: string;
    CreatedDate: Date;
    ModifiedBy: string;
    ModifiedDate: Date;
    TimeStamp: ['']
}
export class IEmergnecyContactDisplay {
    contactID: number;
    contactTypeID: number;
    contactType: string;
    employeeID: number;
    firstName: string;
    lastName: string;
    phone: string;
    email: string;
    address1: string;
    address2: string;
    city: string;
    state: string;
    country: string;
    zipCode: string;
    isDeleted: boolean;
    createdBy: string;
    createdDate: Date;
    modifiedBy: string;
    modifiedDate: Date;
}


