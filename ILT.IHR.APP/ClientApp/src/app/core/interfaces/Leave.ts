import { ILeaveBalanceDisplay } from "./LeaveBalance";

export class ILeave {
    LeaveID: number;
    EmployeeID: number;
    Title: string;
    Detail: string;
    StartDate: Date;
    EndDate: Date;
    IncludesHalfDay: boolean;
    Duration: string;
    LeaveTypeID: number;
    RequesterID: number;
    ApproverID?: number;
    StatusID: number;
    EmployeeName?: string;
    LinkID: string;
    Comment: string;
    CreatedBy: string;
    CreatedDate: Date;
    ModifiedBy: string;
    ModifiedDate: Date;
    TimeStamp: ['']
}
export class ILeaveDisplay {
    leaveID: number;
    employeeID: number;
    employeeName: string;
    requesterID: number;
    requester: string;
    approverID: number;
    approver: string;
    statusID: number;
    statusValue: string;
    status: string;
    leaveTypeID: number;
    leaveType: string;
    title: string;
    comment: string;
    startDate: Date;
    endDate: Date;
    duration: string;
    includesHalfDay: boolean;
    country: string;
    linkID: string;
    detail: string;
    createdBy: string;
    createdDate: Date;
    modifiedBy: string;
    modifiedDate: Date;
    leaveBalances: ILeaveBalanceDisplay[]
}
