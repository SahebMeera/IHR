export class IDirectDeposit {
    DirectDepositID: number;
    EmployeeID: number;
    AccountTypeID: number;
    AccountType?: string;
    EmployeeName: string;
    BankName: string;
    RoutingNumber: string;
    AccountNumber: string;
    State: string;
    Country: string;
    Amount: number;
    IsPrimary: boolean;
    CreatedBy: string;
    CreatedDate: Date;
    ModifiedBy: string;
    ModifiedDate: Date;
    TimeStamp: ['']
}
export class IDirectDepositDisplay {
    directDepositID: number;
    accountTypeID: number;
    accountType: string;
    employeeID: number;
    employeeName: string;
    routingNumber: string;
    bankName: string;
    accountNumber: string;
    state: string;
    country: string;
    amount: number;
    isPrimary: boolean;
    createdBy: string;
    createdDate: Date;
    modifiedBy: string;
    modifiedDate: Date;
}
