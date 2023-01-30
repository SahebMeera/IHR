export class IProcessDataTicket {
    ProcessDataTicketID: number;
    ProcessDataID: number;
    TicketID: number;
    Title?: string;
    TicketDescription?: string;
    StatusId: number;
    Status?: string;
    ResolvedDate?: Date;
    AssignedToId: number;
    AssignedToUser: string;
    CreatedBy: string;
    CreatedDate: Date;
    ModifiedBy: string;
    ModifiedDate: Date;
    TimeStamp: ['']

}
export class IProcessDataTicketDisplay {
    processDataTicketID: number;
    processDataID: number;
    ticketID: number;
    title?: string;
    ticketDescription?: string;
    statusId: number;
    status?: string;
    resolvedDate?: Date;
    assignedToId: number;
    assignedToUser: string;
    createdBy: string;
    createdDate: Date;
    modifiedBy: string;
    modifiedDate: Date;
    timeStamp: ['']
}
