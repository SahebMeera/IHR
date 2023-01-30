import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IRolePermission } from '../../core/interfaces/RolePermission';
import { UserNotification } from '../../core/interfaces/UserNotification';

@Injectable()

@Injectable({
    providedIn: 'root'
})
export class UserNotificationService {
    getUserNotificationsApiMethod = "api/UserNotification";
    UpdateUserNotificationApiMethod = "api/UserNotification";

    constructor(private http: HttpClient) { }

    GetUserNotifications(UserID: number) {
        return this.http.get<any[]>(`${this.getUserNotificationsApiMethod}/${UserID}`);
    }

    UpdateUserNotification(notificationID: number, client: UserNotification): Observable<any[]> {
        return this.http.put<any>(`${this.UpdateUserNotificationApiMethod}/${notificationID}`, client);
    }

}
