import { Component, OnInit, Input, SimpleChanges } from '@angular/core';
import { Router } from '@angular/router';
import { Constants, SessionConstants } from './constant';
import { IMenuItem } from './core/interfaces/MenuItem';
import { IRolePermissionDisplay } from './core/interfaces/RolePermission';
import { RolePermissionService } from './pages/role-permission/role-permission.service';
import { Role } from './_models/role';
import { AuthenticationService } from './_services/authentication.service';

@Component({
    selector: 'app-menu',
    template: `
        <div class="menu">
            <ul class="layout-menu p-pl-0 p-pr-2" >
                <li style=" font-size: 15px; font-weight: 600; padding-left: 0px;" app-menuitem  *ngFor="let item of model; let i = index;" [item]="item" [index]="i" [root]="true" [showSideBar]="showSideBars"></li>
            </ul>
        </div>
    `
})
export class AppMenuComponent implements OnInit {
    @Input() showSideBars: boolean = true;
    @Input() RoleID: number = 0;
    model: any[];
    role: string;
    rolePermissions: IRolePermissionDisplay[] = [];
    menuItems: IMenuItem[] = [];


    constructor(private authenticationService: AuthenticationService,
        private rolePermissionService: RolePermissionService,
        private router: Router) {
        this.rolePermissions = [];
        this.rolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
    }

    ngOnChanges(changes: SimpleChanges) {
        if (changes['RoleID'] !== undefined && changes['RoleID'].previousValue != undefined && changes['RoleID'].previousValue != null) {
            if (changes['RoleID'].currentValue != changes['RoleID'].previousValue) {
                this.LoadLeftNav();
            }
        }
    }

    ngOnInit() {
        this.prepareLeftNav();
    }


    LoadLeftNav() {
        this.rolePermissionService.getRolePermission().subscribe(result => {
            this.rolePermissions = result['data'];
            this.rolePermissions = this.rolePermissions.filter(x => x.roleID === this.RoleID);
            this.prepareLeftNav();
        });
    }

    dashBoardRolePermission: IRolePermissionDisplay = null;
    employeeRolePermission: IRolePermissionDisplay = null;
    companyRolePermission: IRolePermissionDisplay = null;
    lookupRolePermission: IRolePermissionDisplay = null;
    roleRolePermission: IRolePermissionDisplay = null;
    userRolePermission: IRolePermissionDisplay = null;
    timesheetRolePermission: IRolePermissionDisplay = null;
    leaveRequestPermission: IRolePermissionDisplay = null;
    holidaysPermission: IRolePermissionDisplay = null;
    manangeLeavePermission: IRolePermissionDisplay = null;
    manangeTimeSheetPermission: IRolePermissionDisplay = null;
    expensesPermission: IRolePermissionDisplay = null;
    wFHRequestPermission: IRolePermissionDisplay = null;
    wizardDataPermission: IRolePermissionDisplay = null;
    assetPermission: IRolePermissionDisplay = null;
    ticketPermission: IRolePermissionDisplay = null;
    reportsPermission: IRolePermissionDisplay = null;
    appraisalPermission: IRolePermissionDisplay = null;

    prepareLeftNav() {
        if (this.rolePermissions != null && this.rolePermissions.length > 0) {

            this.dashBoardRolePermission = this.rolePermissions.find(x => x.moduleShort == Constants.DASHBOARD);
            this.employeeRolePermission = this.rolePermissions.find(x => x.moduleShort == Constants.EMPLOYEE);
            this.companyRolePermission = this.rolePermissions.find(x => x.moduleShort == Constants.COMPANY);
            this.lookupRolePermission = this.rolePermissions.find(x => x.moduleShort == Constants.LOOKUP);
            this.roleRolePermission = this.rolePermissions.find(x => x.moduleShort == Constants.PERMISSION);
            this.userRolePermission = this.rolePermissions.find(x => x.moduleShort == Constants.USER);
            this.timesheetRolePermission = this.rolePermissions.find(x => x.moduleShort == Constants.TIMESHEET);
            this.leaveRequestPermission = this.rolePermissions.find(x => x.moduleShort == Constants.LEAVEREQUEST);
            this.holidaysPermission = this.rolePermissions.find(x => x.moduleShort == Constants.HOLIDAY);
            this.manangeLeavePermission = this.rolePermissions.find(x => x.moduleShort == Constants.MANAGELEAVE);
            this.manangeTimeSheetPermission = this.rolePermissions.find(x => x.moduleShort == Constants.MANAGETIMESHEET);
            this.expensesPermission = this.rolePermissions.find(x => x.moduleShort == Constants.EXPENSES);
            this.wFHRequestPermission = this.rolePermissions.find(x => x.moduleShort == Constants.WFHREQUEST);
            this.wizardDataPermission = this.rolePermissions.find(x => x.moduleShort == Constants.PROCESSDATA);
            this.assetPermission = this.rolePermissions.find(x => x.moduleShort == Constants.ASSET);
            this.ticketPermission = this.rolePermissions.find(x => x.moduleShort == Constants.TICKET);
            this.appraisalPermission = this.rolePermissions.find(x => x.moduleShort == Constants.APPRAISAL);
            this.reportsPermission = this.rolePermissions.find(x => x.moduleShort == Constants.REPORTS);

            this.model = [];

            if (this.dashBoardRolePermission != null && this.dashBoardRolePermission.view === true) {
                this.model.push({ label: 'Dashboard', icon: 'fas fa-home', routerLink: ['/dashboard'] });
            }

            if (this.employeeRolePermission != null && this.employeeRolePermission.view === true) {
                this.model.push({ label: 'Employee', icon: 'fas fa-user', routerLink: ['/employees'] });
            }
            if (this.leaveRequestPermission != null && this.leaveRequestPermission.view === true) {
                this.model.push({ label: 'Leave Request', icon: 'fas fa-calendar-plus', routerLink: ['/leaverequests'] });
            }
            if (this.wFHRequestPermission != null && this.wFHRequestPermission.view === true) {
                this.model.push({ label: 'WFH Request', icon: 'fas fa-calendar-plus', routerLink: ['/wfhrequests'] });
            }
            if (this.timesheetRolePermission != null && this.timesheetRolePermission.view === true) {
                this.model.push({ label: 'Timesheet', icon: 'fas fa-business-time', routerLink: ['/timesheet'] });
            }
            if (this.holidaysPermission != null && this.holidaysPermission.view === true) {
                this.model.push({ label: 'Holidays', icon: 'fas fa-plane-departure', routerLink: ['/holidays'] });
            }
            if (this.companyRolePermission != null && this.companyRolePermission.view === true) {
                this.model.push({ label: 'Company', icon: 'fas fa-building', routerLink: ['/company'] });
            }
            if (this.manangeLeavePermission != null && this.manangeLeavePermission.view === true) {
                this.model.push({ label: 'Manage Leave', icon: 'fas fa-calendar-alt', routerLink: ['/manageLeave'] });
            }
            if (this.manangeTimeSheetPermission != null && this.manangeTimeSheetPermission.view === true) {
                this.model.push({ label: 'Manage Timesheet', icon: 'fas fa-business-time', routerLink: ['/managetimesheet'] });
            }
            if (this.lookupRolePermission != null && this.lookupRolePermission.view === true) {
                this.model.push({ label: 'Lookup', icon: 'fas fa-list', routerLink: ['/lookupTables'] });
            }
            if (this.roleRolePermission != null && this.roleRolePermission.view === true) {
                this.model.push({ label: 'Role Permission', icon: 'fas fa-user-lock', routerLink: ['/rolepermission'] });
            }
            if (this.userRolePermission != null && this.userRolePermission.view === true) {
                this.model.push({ label: 'User', icon: 'fas fa-users', routerLink: ['/users'] });
            }
            if (this.expensesPermission != null && this.expensesPermission.view === true) {
                this.model.push({ label: 'Expenses', icon: 'fas fa-file-invoice-dollar', routerLink: ['/expenses'] });
            }
            if (this.wizardDataPermission != null && this.wizardDataPermission.view === true) {
                this.model.push({ label: 'Process Data', icon: 'fa fa-calendar-plus-o', routerLink: ['/processdatas'] });
            }
            if (this.assetPermission != null && this.assetPermission.view === true) {
                this.model.push({ label: 'Asset', icon: 'fa fa-calendar-plus-o', routerLink: ['/asset'] });
            }
            if (this.ticketPermission != null && this.ticketPermission.view === true) {
                this.model.push({ label: 'Ticket', icon: 'fas fa-ticket-alt', routerLink: ['/ticket'] });
            }
            if (this.reportsPermission != null && this.reportsPermission.view === true) {
                this.model.push({ label: 'Reports', icon: 'fas fa-file-medical-alt', routerLink: ['/reports'] });
            }
            if (this.appraisalPermission != null && this.appraisalPermission.view === true) {
                this.model.push({ label: 'Appraisal', icon: 'fas fa-file-medical-alt', routerLink: ['/appraisal'] });
            }
            this.model.push({ label: 'Sign out', icon: 'fas fa-sign-out-alt', routerLink: ['/login'] });

            //this.router.navigate(['/dashboard']);

        }
    }


}
