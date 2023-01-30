export class IEmailApproval {
    EmailApprovalID: number;
    ModuleID: number;
    ID: number;
    ValidTime: Date;
    IsActive: boolean;
    Value: string;
    LinkID: string;
    ApproverEmail: string;
    EmailSubject: string;
    EmailFrom: string;
    EmailTo: string;
    EmailCC: string;
    EmailBCC: string;
    EmailBody: string;
    SendDate: Date;
    SentCount: number;
    ReminderDuration: number;
    CreatedBy: string;
    CreatedDate: Date;
    ModifiedBy: string;
    ModifiedDate: Date;
    TimeStamp: ['']
}
export class IEmailApprovalDisplay {
    emailApprovalID: number;
    moduleID: number;
    id: number;
    validTime: Date;
    isActive: boolean;
    value: string;
    linkID: string;
    approverEmail: string;
    emailSubject: string;
    emailFrom: string;
    emailTo: string;
    emailCC: string;
    emailBCC: string;
    emailBody: string;
    sendDate: Date;
    sentCount: number;
    reminderDuration: number;
    createdBy: string;
    createdDate: Date;
    modifiedBy: string;
    modifiedDate: Date;
    timeStamp: ['']
}
