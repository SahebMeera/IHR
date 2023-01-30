import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { IRegistrationDisplay } from '../../components/steps/registration';
import { Constants, ListTypeConstants, SessionConstants, TimeSheetStatusConstants, UserRole } from '../../constant';
import { IRolePermissionDisplay } from '../../core/interfaces/RolePermission';
import { DataProvider } from '../../core/providers/data.provider';
import { ExpenseService } from '../../pages/expenses/expense.service';
import { LeaveRequestService } from '../../pages/leave-request/leave-request.service';
import { ProcessDataService } from '../../pages/process-data/process-data.service';
import { TicketService } from '../../pages/ticket/ticket.service';
import { TimesheetService } from '../../pages/timesheet/timesheet.service';
import { User } from '../../_models';
import { WFHService } from '../wfh-request/wfh.service';
import { DashboardService } from './dashboard.service';


class Dashboard {
    dashboardViewPermission: boolean;
    dashboardCardColor: string;
    dashboardListLength: number;
    dashboardHeading: string;
    dashboardSubHeading: string
}
@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
    UserName: string;
    user: any;
    RolePermissions: IRolePermissionDisplay[] = [];
    EmployeeID: number;
    currentLoginUserRole: string;

    TimeSheetRolePermission: IRolePermissionDisplay;
    DashboardRolePermission: IRolePermissionDisplay;
    TicketRolePermission: IRolePermissionDisplay;
    WizardDataRolePermission: IRolePermissionDisplay;
    LeaveRolePermission: IRolePermissionDisplay;
    ExpenseRolePermission: IRolePermissionDisplay;
    WFHRolePermission: IRolePermissionDisplay;
    constructor(private dashboardService: DashboardService,
        private router: Router,
        private dataProvider: DataProvider,
        private LeaveService: LeaveRequestService,
        private timesheetService: TimesheetService,
        private ticketService: TicketService,
        private WizardDataService: ProcessDataService,
        private expenseService: ExpenseService,
        private WorkFromHomeService: WFHService) {
        this.user = JSON.parse(localStorage.getItem("User"));
        if (this.user != null) {
            this.UserName = this.user.firstName + " " + this.user.lastName;
        }
        this.currentLoginUserRole = localStorage.getItem("RoleName");
        this.EmployeeID = Number(this.user.employeeID);
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.TimeSheetRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.TIMESHEET);
        this.DashboardRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.DASHBOARD);
        this.TicketRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.TICKET);
        this.WizardDataRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.PROCESSDATA);
        this.LeaveRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.LEAVEREQUEST);
        this.ExpenseRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.EXPENSES);
        this.WFHRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.WFHREQUEST);
        this.loadmodule();

    }

    async loadmodule() {
        await this.LoadList();
        await this.LoadLeaveRequest();
        await this.LoadWFHRequest();
        await this.LoadTimeSheetRequest();
        await this.LoadExpenses();
        await this.LoadProcessList();
    }
    ngOnInit(): void {

    }
    LeaveRequestList: any[] = [];
    lstLeaveRequest: any[] = [];
    async LoadLeaveRequest() {
        this.LeaveRequestList = [];
        this.LeaveService.GetLeave("ApproverID", this.EmployeeID).subscribe(respLeaveRequest => {
            if (respLeaveRequest['data'] !== null && respLeaveRequest['messageType'] === 1) {
                this.LeaveRequestList = respLeaveRequest['data'];
                this.loadList("PENDING");
            }
            else {
                this.LeaveRequestList = []
                this.lstLeaveRequest = this.LeaveRequestList;
            }
        });
    }
    lstDashboardRequest: Dashboard[] = [];
    //ListItem: Dashboard ;
    async loadList(Status: string) {
        var ListItem: Dashboard = new Dashboard();
        this.lstLeaveRequest = this.LeaveRequestList.filter(x => x.status.toUpperCase() === Status.toUpperCase());
        if (this.LeaveRolePermission !== null && this.LeaveRolePermission.view === true) {
            if (ListItem !== undefined) {
                ListItem.dashboardViewPermission = this.LeaveRolePermission.view;
                ListItem.dashboardCardColor = "#FCB711";
                ListItem.dashboardHeading = "Leaves";
                ListItem.dashboardSubHeading = "Leaves Pending Approval";
                ListItem.dashboardListLength = this.lstLeaveRequest == null ? 0 : this.lstLeaveRequest.length;
                this.lstDashboardRequest.push(ListItem);
            }
        }
    }
    TimeSheetsList: any[] = [];
    async LoadTimeSheetRequest() {
        this.timesheetService.GetTimeSheets(0, this.user.userID, null).subscribe(respTimeSheetRequest => {
            if (respTimeSheetRequest['data'] !== null && respTimeSheetRequest['messageType'] === 1) {
                this.TimeSheetsList = respTimeSheetRequest['data'].filter(x => x.tsApproverEmail === this.user.email);
                this.loadListTimesheet(TimeSheetStatusConstants.SUBMITTED.toUpperCase());
            } else {
                this.TimeSheetsList = [];
            }
        })
    }
    lstTimeSheetRequest: any[] = [];
    async loadListTimesheet(Status: string) {
        var ListItem: Dashboard = new Dashboard();
        this.lstTimeSheetRequest = this.TimeSheetsList.filter(x => x.status.toUpperCase() === Status.toUpperCase());
        if (this.TimeSheetRolePermission != null && this.TimeSheetRolePermission.view == true) {
            ListItem.dashboardViewPermission = this.TimeSheetRolePermission.view;
            ListItem.dashboardCardColor = "#F37021";
            ListItem.dashboardHeading = "Timesheets";
            ListItem.dashboardSubHeading = "Timesheets Pending Approval";
            ListItem.dashboardListLength = this.lstTimeSheetRequest == null ? 0 : this.lstTimeSheetRequest.length;
            if (this.lstDashboardRequest != null) {
                this.lstDashboardRequest.push(ListItem);
            }
            // lstDashboardRequest.Add(ListItem);
        }
    }
    TicketsList: any[] = [];
    lstTicketsList: any[] = [];
    async LoadList() {
        if (this.currentLoginUserRole.toUpperCase() === UserRole.ADMIN) {
            this.ticketService.GetTicket().subscribe(reponses => {
                if (reponses['data'] !== null && reponses['messageType'] === 1) {
                    this.TicketsList = reponses['data'];
                    if (this.TicketsList != null) {
                        this.loadTicketList();
                    }
                    else {
                        this.lstTicketsList = this.TicketsList;
                    }
                } else {
                    //this.me
                }
            })
        }
        else {
            this.ticketService.GetTicketsList(this.EmployeeID, this.EmployeeID).subscribe(reponses => {
                if (reponses['data'] !== null && reponses['messageType'] === 1) {
                    this.TicketsList = reponses['data'];
                    if (this.TicketsList != null) {
                        this.loadTicketList();
                    }
                    else {
                        this.lstTicketsList = this.TicketsList;
                    }
                }
            })
        }
    }

    loadTicketList() {
        if (this.TicketsList != null) {
            var ListItem: Dashboard = new Dashboard();
            this.lstTicketsList = this.TicketsList.filter(x => (x.status.toUpperCase() === "NEW" || x.status.toUpperCase() == "Assigned".toUpperCase()) && x.assignedToID == this.user.employeeID);
            if (this.TicketRolePermission != null && this.TicketRolePermission.view == true) {
                ListItem.dashboardViewPermission = this.TicketRolePermission.view;
                ListItem.dashboardCardColor = "#CC004C";
                ListItem.dashboardHeading = "Tickets";
                ListItem.dashboardSubHeading = "Pending Tickets";
                ListItem.dashboardListLength = this.lstTicketsList == null ? 0 : this.lstTicketsList.length;
                this.lstDashboardRequest.push(ListItem);
            }
        }
        else {
            this.lstTicketsList = this.TicketsList;
        }
    }

    WizardDataList: any[] = [];
    lstWizardDataList: any[] = [];

    LoadProcessList() {
        this.WizardDataService.GetWizardDatas().subscribe(resp => {
            if (resp['data'] !== null && resp['messageType'] === 1) {
                this.WizardDataList = resp['data'];
                var ListItem: Dashboard = new Dashboard();
                this.lstWizardDataList = this.WizardDataList.filter(x => x.statusId == 143 || x.statusId == 142);
                if (this.WizardDataRolePermission != null && this.WizardDataRolePermission.view === true) {
                    ListItem.dashboardViewPermission = this.WizardDataRolePermission.view;
                    ListItem.dashboardCardColor = "#6460AA";
                    ListItem.dashboardHeading = "Process Data";
                    ListItem.dashboardSubHeading = "Process Data Count";
                    ListItem.dashboardListLength = this.lstWizardDataList == null ? 0 : this.lstWizardDataList.length;
                    this.lstDashboardRequest.push(ListItem);
                }
            }
            else {
                this.lstWizardDataList = [];
            }
        })
    }
    ExpensesList: any[] = [];
    lstExpensesList: any[] = [];
    LoadExpenses() {
        var RoleShort: string = localStorage.getItem("RoleShort");
        var ListItem: Dashboard = new Dashboard();
        this.expenseService.GetExpense().subscribe(resp => {
            if (resp['data'] !== null && resp['messageType'] === 1) {
                if (resp['data'] != null && (RoleShort.toUpperCase() == UserRole.EMP || RoleShort.toUpperCase() == UserRole.CONTRACTOR)) {
                    this.ExpensesList = resp['data'].filter(x => x.EmployeeID == this.user.EmployeeID);
                }
                else {
                    this.ExpensesList = resp['data'];
                }
                this.lstExpensesList = this.ExpensesList.filter(x => x.status.toUpperCase() == "Submitted".toUpperCase());
                if (this.ExpenseRolePermission != null && this.ExpenseRolePermission.view == true) {
                    ListItem.dashboardViewPermission = this.ExpenseRolePermission.view;
                    ListItem.dashboardCardColor = "#0089D0";
                    ListItem.dashboardHeading = "Expenses";
                    ListItem.dashboardSubHeading = "Expenses Pending Approval";
                    ListItem.dashboardListLength = this.lstExpensesList == null ? 0 : this.lstExpensesList.length;
                    this.lstDashboardRequest.push(ListItem);
                }
            }
            else {
                //toastService.ShowError(ErrorMsg.ERRORMSG);
            }
        })
    }
    lstOfWFH: any[] = [];
    WFHList: any[] = []
    LoadWFHRequest() {
        this.lstOfWFH = [];
        this.WorkFromHomeService.GetWFH("ApproverID", this.EmployeeID).subscribe(resp => {
            if (resp['data'] !== null && resp['messageType'] === 1) {
                this.WFHList = resp['data']
                this.loadListWFH("PENDING");
            } else {
                this.WFHList = [];
            }
        })
    }
    loadListWFH(Status: string) {
        var ListItem: Dashboard = new Dashboard();
        this.lstOfWFH = this.WFHList.filter(x => x.status.toUpperCase() === Status.toUpperCase());
        if (this.WFHRolePermission != null && this.WFHRolePermission.view == true) {
            ListItem.dashboardViewPermission = this.WFHRolePermission.view;
            ListItem.dashboardCardColor = "#0DB14B";
            ListItem.dashboardHeading = "WFH Requests";
            ListItem.dashboardSubHeading = "WFH Requests Pending Approval";
            ListItem.dashboardListLength = this.lstOfWFH == null ? 0 : this.lstOfWFH.length;
            this.lstDashboardRequest.push(ListItem);
        }
    }

    redirectToPages(pageName: string) {
        if (pageName !== "" && pageName !== null && pageName !== undefined) {
            if (pageName.toUpperCase() === "LEAVES") {
                this.dataProvider.TabIndex = 1;
                this.router.navigate(['/leaverequests']);
            }
            if (pageName.toUpperCase() === "TIMESHEETS") {
                this.dataProvider.TabIndex = 1;
                this.router.navigate(['/timesheet']);
            }
            if (pageName.toUpperCase() === "TICKETS") {
                this.router.navigate(['/ticket']);
            }
            if (pageName.toUpperCase() === "PROCESS DATA") {
                this.router.navigate(['/processdatas']);
            }
            if (pageName.toUpperCase() === "EXPENSES") {
                this.router.navigate(['/expenses']);
            }
            if (pageName.toUpperCase() === "WFH REQUESTS") {
                this.dataProvider.TabIndex = 1;
                this.router.navigate(['/wfhrequests']);
            }
        }
    }

}

