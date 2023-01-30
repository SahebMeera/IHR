export class ILeaveBalance {
    LeaveBalanceID: number;
    EmployeeID: number;
    EmployeeCode: string;
    EmployeeName: string;
    LeaveYear: number;
    LeaveTypeID: number;
    LeaveType: string;
    Country: string;
    VacationTotal: string;
    VacationUsed: string;
    UnpaidLeave: string;
    VacationBalance: string;
    EncashedLeave: string;
    LWPAccounted: string;
    LeaveInRange: string;
    LWPPending: string;
    LeaveInNextMonth: string;
    StartDate: Date;
    EndDate: Date;
    TermDate?: Date;
}
export class ILeaveBalanceDisplay {
    leaveBalanceID: number;
    employeeID: number;
    employeeCode: string;
    employeeName: string;
    leaveYear: number;
    leaveTypeID: number;
    leaveType: string;
    country: string;
    vacationTotal: string;
    vacationUsed: string;
    unpaidLeave: string;
    vacationBalance: string;
    encashedLeave: string;
    lWPAccounted: string;
    leaveInRange: string;
    lwpPending: string;
    leaveInNextMonth: string;
    startDate: Date;
    endDate: Date;
    termDate?: Date;
}
