export class IAuditLog {
    auditLogID: number;
    action: string;
    tableName: string;
    recordId: number;
    values: string;
    createdDate: Date;
    createdDateForDisplay: string;
    createdBy: string;
}
