import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { IEmployee, IEmployeeDisplay } from '../../core/interfaces/Employee';
import { ILeave, ILeaveDisplay } from '../../core/interfaces/Leave';
import { ITimesheetDisplay } from '../../core/interfaces/Timesheet';
import { UserNotification, UserNotificationDisplay } from '../../core/interfaces/UserNotification';
import { DataProvider } from '../../core/providers/data.provider';
import { UserNotificationService } from './UserNotificationService';

@Component({
  selector: 'app-user-notification',
  templateUrl: './user-notification.component.html',
  styleUrls: ['./user-notification.component.scss']
})
export class UserNotificationComponent implements OnInit {
   isNotificationEnabled: boolean = true;
    userNotifications: UserNotificationDisplay[] = [];
    timesheetNotifications: UserNotificationDisplay[] = [];
    leaveNotifications: UserNotificationDisplay[] = [];
    ticketNotifications: UserNotificationDisplay[] = [];
    employeeNotifications: UserNotificationDisplay[] = [];
    visibleAnimate: boolean = false;
    right: string = 'top-right';
    clientID: string;
    userDetails: any;
    constructor(private userNotificationService: UserNotificationService, private dataProvider: DataProvider, private router: Router) {
        var UserDetails = JSON.parse(localStorage.getItem('IHR-current-loggedin-user'));
        this.clientID = localStorage.getItem('ClientID');
       // this.userDetails = UserDetails['user'];
        this.userDetails = JSON.parse(localStorage.getItem("User"));
    }

    ngOnInit(): void {
        
    }

    show() {
        if (this.userDetails != null && this.isNotificationEnabled == true) {
            var UserID = Number(this.userDetails.recordID);
            this.userNotificationService.GetUserNotifications(UserID).subscribe(resp => {
                if (resp !== null && resp !== undefined && resp['data'] !== null && resp['data'] !== undefined) {
                    this.userNotifications = resp['data'];
                    if (this.userNotifications.length > 0) {
                        this.notificationFilters();
                        this.visibleAnimate = true;
                    } else {
                        this.visibleAnimate = true;
                    }
                }
            })
        }
        console.log(this.userNotifications)
    }

    notificationFilters() {
        this.timesheetNotifications = []
        this.leaveNotifications = []
        this.ticketNotifications = []
        this.employeeNotifications = []
        if (this.userNotifications.length > 0) {
            if (this.userNotifications.filter(x => x.module == "Timesheet") !== null && this.userNotifications.filter(x => x.module == "Timesheet").length > 0 && this.userNotifications.filter(x => x.module == "Timesheet") !== undefined) {
                this.timesheetNotifications = this.userNotifications.filter(x => x.module == "Timesheet");
            }
            if (this.userNotifications.filter(x => x.module == "Leave") !== null && this.userNotifications.filter(x => x.module == "Leave").length > 0 && this.userNotifications.filter(x => x.module == "Leave") !== undefined) {
                this.leaveNotifications = this.userNotifications.filter(x => x.module == "Leave");
            }
            if (this.userNotifications.filter(x => x.module == "Ticket") !== null && this.userNotifications.filter(x => x.module == "Ticket").length > 0 && this.userNotifications.filter(x => x.module == "Ticket") !== undefined) {
                this.ticketNotifications = this.userNotifications.filter(x => x.module == "Ticket");
            }
            if (this.userNotifications.filter(x => x.module == "Employee") !== null && this.userNotifications.filter(x => x.module == "Employee").length > 0 && this.userNotifications.filter(x => x.module == "Employee") !== undefined) {
                this.employeeNotifications = this.userNotifications.filter(x => x.module == "Employee");
            }
        }
    }

    NavigateToNotifiedRecord(notification: UserNotificationDisplay) {

        if (notification.module == "Employee") {
            var empData = new IEmployeeDisplay();
            empData.employeeID = notification['recordID'];
            // userNotificationService.UpdateUserNotifications(notification.NotificationID, notification);
            this.dataProvider.storage = empData;
            this.router.navigate(['/employees']);
            this.visibleAnimate = false;

        }
        if (notification.module == "Leave") {
            var leave = new ILeaveDisplay();
            leave.leaveID = notification['recordID'];
            this.dataProvider.storage = leave;
            this.dataProvider.TabIndex = 1;
            this.router.navigate(['/leaverequests']);
            this.visibleAnimate = false;
           // navigationManager.NavigateTo("leaverequests");
        }
        if (notification.module == "Timesheet") {
            var timesheet = new ITimesheetDisplay();
            timesheet.timeSheetID = notification['recordID']
            this.dataProvider.storage = timesheet;
            this.router.navigate(['/timesheet']);
            this.visibleAnimate = false;
        }
    }
    DismissNotification(notification: UserNotificationDisplay) {
        if (this.userDetails != null && this.userNotifications != null) {
            var withoutEl = this.userNotifications.filter(x => x.notificationID !== notification.notificationID);
            if (withoutEl.length > 0) {
                this.userNotifications = [];
                this.userNotifications = withoutEl;
            }
            this.notificationFilters();
          //  this.userNotifications.RemoveAll(x => x.notificationID == notification.notificationID);
            let notifications = new UserNotification();
            notifications.NotificationID = notification.notificationID;
            notifications.Module = notification.module;
            notifications.UserID = notification.userID;
            notifications.IsAck = notification.isAck;
            notifications.AckDate = notification.ackDate;
            notifications.CreatedDate = notification.createdDate;
            notifications.CreatedBy = notification.createdBy;
            notifications.ModifiedDate = notification.modifiedDate;
            notifications.ModifiedBy = notification.modifiedBy;
            this.userNotificationService.UpdateUserNotification(notification.notificationID, notifications).subscribe(resp => {

            })
        }
       
    }



}
