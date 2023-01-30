export class INotification {
    NotificationID: number;
    TableName: string;
    ChangeSetID: number;
    UserID: number;
    IsAck: boolean;
    AckDate?: Date;
    RecordID: number
}
