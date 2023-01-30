export class UserNotification {
    NotificationID: number;
    Module?: string;
    UserID: number;
    IsAck: boolean;
    AckDate?: Date;
    CreatedDate: Date;
    CreatedBy: string;
    ModifiedDate: Date;
    ModifiedBy: string;
}
export class UserNotificationDisplay {
    notificationID: number;
    module?: string;
    userID: number;
    isAck: boolean;
    ackDate?: Date;
    createdDate: Date;
    createdBy: string;
    modifiedDate: Date;
    modifiedBy: string;
}
