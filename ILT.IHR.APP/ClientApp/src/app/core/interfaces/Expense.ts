export class IExpense {
    ExpenseID: number;
    EmployeeID: number;
    EmployeeName: String;
    ExpenseTypeID: number;
    ExpenseType: string;
    FileName: string;
    Amount?: number;
    SubmissionDate: Date;
    SubmissionComment: string;
    StatusID: number;
    LinkID: string;
    Status: string;
    AmountPaid?: number;
    PaymentDate?: Date;
    PaymentComment: string;
    CreatedBy: string;
    CreatedDate: Date;
    ModifiedBy: string;
    ModifiedDate: Date;
    TimeStamp: ['']
}

export class IExpenseForDisplay {
    expenseID: number;
    employeeID: number;
    employeeName: String;
    expenseTypeID: number;
    expenseType: string;
    fileName: string;
    amount?: number;
    submissionDate: Date;
    submissionComment: string;
    statusID: number;
    linkID: string;
    status: string;
    amountPaid?: number;
    paymentDate?: Date;
    paymentComment: string;
    createdBy: string;
    createdDate: Date;
    modifiedBy: string;
    modifiedDate: Date;
    timeStamp: ['']
}
