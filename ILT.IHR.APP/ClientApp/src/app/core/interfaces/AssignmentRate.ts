export class IAssignmentRate {
    AssignmentRateID: number;
    AssignmentID: number;
    BillingRate: string;
    PaymentRate: string;
    IsFLSAExempt: boolean;
    StartDate: Date;
    EndDate: Date;
    CreatedBy: string;
    CreatedDate: Date;
    ModifiedBy: string;
    ModifiedDate: Date;
    TimeStamp: ['']
}
export class IAssignmentRateDisplay {
    assignmentRateID: number;
    assignmentID: number;
    billingRate: string;
    paymentRate: string;
    isFLSAExempt: boolean;
    startDate: string;
    endDate: string;
    createdBy: string;
    createdDate: Date;
    modifiedBy: string;
    modifiedDate: Date;
}
