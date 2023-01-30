export class ITimeEntry {
    TimeEntryID: number;
    TimeSheetID: number;
    Project: string;
    Activity: string;
    WorkDate: Date;
    Hours: number;
    CreatedBy: string;
    CreatedDate: Date;
    ModifiedBy: string;
    ModifiedDate: Date;
    TimeStamp: ['']
}
export class ITimeEntryDisplay {
    timeEntryID: number;
    timeSheetID: number;
    project: string;
    activity: string;
    workDate: Date;
    hours: number;
    createdBy: string;
    createdDate: Date;
    modifiedBy: string;
    modifiedDate: Date;
}
