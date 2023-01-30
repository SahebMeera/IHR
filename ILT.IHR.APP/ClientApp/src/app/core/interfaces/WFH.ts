export class IWFH {
    WFHID: number;
    EmployeeID: number;
    Title: string;
    StartDate: Date;
    EndDate: Date;
    RequesterID: number;
    ApproverID?: number;
    StatusID: number;
    EmployeeName: string;
    LinkID: string;
    Comment: string;
    CreatedBy: string;
    CreatedDate: Date;
    ModifiedBy: string;
    ModifiedDate: Date;
    TimeStamp: ['']
}
export class IWFHDisplay {
    wfhid: number;
    employeeID: number;
    employeeName: string;
    requesterID: number;
    requester: string;
    approverID: number;
    approver: string;
    statusID: number;
    statusValue: string;
    status: string;
    title: string;
    comment: string;
    startDate: Date;
    endDate: Date;
    linkID: string;
    createdBy: string;
    createdDate: Date;
    modifiedBy: string;
    modifiedDate: Date;
}
