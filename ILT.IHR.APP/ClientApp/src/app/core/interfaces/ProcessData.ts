import { IProcessDataTicketDisplay } from "./ProcessDataTicket";


export class IProcessData {
    ProcessDataID: number;
    ProcessWizardID: number;
    Process?: string;
    Data?: string;
    DataColumns?: string;
    StatusId?: number;
    Status?: string;
    ChangeNotificationEmailId?: string;
    EmailApprovalValidity?: number;
    ProcessedDate?: Date;
    CreatedBy: string;
    CreatedDate: Date;
    ModifiedBy?: string;
    ModifiedDate?: Date;
}
export class IProcessDataDisplay {
    processDataID: number;
    processWizardID: number;
    process?: string;
    data?: any;
    dataColumns?: string;
    statusId: number;
    status?: string;
    changeNotificationEmailId?: string;
    emailApprovalValidity: number;
    processedDate: Date;
    createdBy: string;
    createdDate: Date;
    modifiedBy: string;
    modifiedDate: Date;
    timeStamp: ['']
    processDataTickets: any[];
}
