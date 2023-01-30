export class ITicket {
    TicketID: number;
    TicketTypeID: number;
    TicketType: string;
    TicketShort: string;
    RequestedByID: number;
    RequestedBy: string;
    ModuleID: number;
    ModuleName: string;
    ID?: number;
    Description: string;
    AssignedToID: string;
    AssignedTo: string;
    StatusID: number;
    Status: string;
    ResolvedDate?: Date;
    Comment: string;
    Title: string;
    LinkID: string;
    CreatedBy: string;
    CreatedDate: Date;
    ModifiedBy: string;
    ModifiedDate: Date;
    TimeStamp: ['']

}
export class ITicketForDisplay {
    ticketID: number;
    ticketTypeID: number;
    ticketType: string;
    ticketShort: string;
    requestedByID: number;
    requestedBy: string;
    moduleID: number;
    moduleName: string;
    iD?: number;
    description: string;
    assignedToID: number;
    assignedTo: string;
    statusID: number;
    status: string;
    resolvedDate?: Date;
    comment: string;
    title: string;
    linkID: string;
    createdBy: string;
    createdDate: Date;
    modifiedBy: string;
    modifiedDate: Date;
    timeStamp: ['']
}
