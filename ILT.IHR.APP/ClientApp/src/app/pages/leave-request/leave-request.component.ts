import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { MenuItem } from 'primeng/api';
import { CommonUtils } from '../../common/common-utils';
import { Constants, LeaveStatus, ListTypeConstants, SessionConstants } from '../../constant';
import { IDropDown } from '../../core/interfaces/IDropDown';
import { ILeaveDisplay } from '../../core/interfaces/Leave';
import { IRolePermissionDisplay } from '../../core/interfaces/RolePermission';
import { DataProvider } from '../../core/providers/data.provider';
import { ITableHeaderAction, ITableRowAction } from '../../shared/ihr-table/table-options';
import { LookUpService } from '../lookup/lookup.service';
import { AddEditLeaveComponent } from './add-edit-leave/add-edit-leave.component';
import { LeaveBalanceService } from './leave-balance.service';
import { LeaveRequestService } from './leave-request.service';
import { ApprovalDenyLeaveComponent } from './LeaveApproval/approval-deny-leave/approval-deny-leave.component';

@Component({
  selector: 'app-leave-request',
  templateUrl: './leave-request.component.html',
  styleUrls: ['./leave-request.component.scss']
})
export class LeaveRequestComponent implements OnInit {
    leaveStatus = LeaveStatus;
    @ViewChild('AddEditLeaveModal') AddEditLeaveModal: AddEditLeaveComponent;
    @ViewChild('LeaveApproval') leaveApproval: ApprovalDenyLeaveComponent;
    commonUtils = new CommonUtils()
    cols: any[] = [];
    selectedColumns: any[];
    LeaveRequestcols: any[] = [];
    selectedLeaveRequestColumns: any[];
    isPagination: boolean = true;
    loading: boolean = false;
    showCurrentPageReport: boolean = true;
    Country: string = 'Country';
    rowActions: ITableRowAction[] = [];
    headerActions: ITableHeaderAction[] = [];
    patientLanding: string = 'patientLanding';
    globalFilterFields = ['startDate', 'endDate', 'duration', 'leaveType', 'title', 'approval', 'status']
    globalFilterLeavebalanceFields = ['leaveYear', 'leaveType', 'vacationTotal', 'vacationUsed', 'unpaidLeave', 'vacationBalance', 'encashedLeave']


    index: number;

    EmployeeID: number;
    //table dropdown
    lstStatus: any[] = [];
    status: string;
    DefaultDwnDn1ID: number;

    LeaveBalanceList: any[] = [];
    LeaveRequestList: ILeaveDisplay[] = [];

    RolePermissions: IRolePermissionDisplay[] = []
    user: any;
    constructor(private fb: FormBuilder, private dataProvider: DataProvider,
        private LookupService: LookUpService,
        private router: Router,
        private leaveBalanceService: LeaveBalanceService,
        private leaveRequestService: LeaveRequestService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.user = JSON.parse(localStorage.getItem("User"));
        this.EmployeeID = Number(this.user.employeeID);
        this.LoadDropDown();
        this.loadLeavebalanceTableColumns();
        this.loadLeaveRequestTableColumns();
        this.LoadTableConfig(0);
        this.index = 0;
        var tabindex = this.dataProvider.TabIndex;
        if (tabindex !== undefined && tabindex !== 0 && Object.keys(this.dataProvider).length !== 0) {
            if (this.leaveApproval !== undefined) {
                this.leaveApproval.EmployeeID = this.EmployeeID;
                this.leaveApproval.LoadLeaveRequest();
            }
            this.index = 1;
            this.dataProvider.TabIndex = 0;

        } else {
            this.index = 0;
        }
    }

    ngOnInit(): void {
        this.loadGridData();
        this.loadMobilesItems();
        if (this.dataProvider.storage != null) {
            var tabindex = this.dataProvider.TabIndex;
            if (this.leaveApproval !== undefined) {
                this.leaveApproval.EmployeeID = this.EmployeeID;
                this.leaveApproval.LoadLeaveRequest();
            }
            this.index = 1;
            this.dataProvider.TabIndex = 0
        }
       
    }
    async loadGridData() {
        await this.LoadLeaveBalance();
        await this.LoadLeaveRequest();
    }


    loadLeavebalanceTableColumns() {
        this.cols = [
            { field: 'leaveYear', header: 'Year' },
            { field: 'leaveType', header: 'Leave Type' },
            { field: 'vacationTotal', header: 'Total' },
            { field: 'vacationUsed', header: 'Used' },
            { field: 'unpaidLeave', header: 'Unpaid' },
            { field: 'vacationBalance', header: 'Balance' },
            { field: 'encashedLeave', header: 'Encashed' },
        ];

        this.selectedColumns = this.cols;
    }

     LoadLeaveBalance() {
         if (this.EmployeeID != 0) {
             this.leaveBalanceService.GetLeaveBalance(this.EmployeeID).subscribe(respLeaveBalance => {
                 if (respLeaveBalance['data'] !== null && respLeaveBalance['messageType'] === 1) {
                     this.LeaveBalanceList = respLeaveBalance['data'];
                 } else {
                     this.LeaveBalanceList = []
                 }
             })
        //    if (respLeaveBalance.MessageType == MessageType.Success)
        //        LeaveBalanceList = respLeaveBalance.Data.ToList();
        //    else
        //        LeaveBalanceList = new List < LeaveBalance > {};
        }
    }
    loadLeaveRequestTableColumns() {
        this.LeaveRequestcols = [
            { field: 'startDate', header: 'Start Date' },
            { field: 'endDate', header: 'End Date' },
            { field: 'duration', header: 'Duration' },
            { field: 'leaveType', header: 'Leave Type' },
            { field: 'title', header: 'Title' },
            { field: 'approver', header: 'Approver' },
            { field: 'status', header: 'Status' },
        ];

        this.selectedLeaveRequestColumns = this.LeaveRequestcols;
    }

    LeaveRolePermission: IRolePermissionDisplay;
    LoadTableConfig(StatusId: number) {
        this.LeaveRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.LEAVEREQUEST);
        this.rowActions = [
            {
                actionMethod: this.Edit,
                iconClass: 'pi pi-pencil',
            },
            {
                actionMethod: this.Cancel,
                styleClass: 'p-button-raised p-button-danger',
                iconClass: 'pi pi-times-circle',
            },
        ];
        this.headerActions = [
            {
                actionMethod: this.Add,
                hasIcon: false,
                styleClass: 'btn-width-height btn btn-block btn-sm btn-info',
                actionText: 'Add',
                iconClass: 'fas fa-plus',

            }
        ];
    }
    StatusList: any[] = [];
    LoadDropDown() {
        this.LookupService.getListValues().subscribe(result => {
            if (result['data'] !== undefined && result['data'] !== null) {
                this.StatusList = result['data'].filter(x => x.type.toUpperCase() === ListTypeConstants.LEAVEREQUESTSTATUS);
               this.setStatusList()
            }
        })
    }


    ListItemEmpType: IDropDown;
    statusId: number;
    setStatusList() {
        this.lstStatus = [];
        if (this.StatusList !== undefined) {
            this.lstStatus.push({ ID: 0, Value: 'All' })
            this.StatusList.forEach(x => {
                this.lstStatus.push({ ID: x.listValueID, Value: x.valueDesc })
            });
        }
        if (this.status === undefined) {
            this.status = "All";
        }
        this.DefaultDwnDn1ID = this.lstStatus.find(x => x.Value.toLowerCase() == this.status.toLowerCase()).ID;
    }
    lstLeaveRequest: ILeaveDisplay[] = [];
    async  LoadLeaveRequest() {
        await this.LoadLeaveBalance();
        this.lstLeaveRequest = null;
        console.log(this.statusId)
        //this.statusId = 0;
        this.LoadTableConfig(this.statusId);
        this.leaveRequestService.GetLeave("EmployeeID", this.EmployeeID).subscribe(respLeaveRequest => {
            if (respLeaveRequest['data'] !== null && respLeaveRequest['messageType'] === 1) {
                respLeaveRequest['data'].forEach((d) => {
                    d.startDate = moment(d.startDate).format("MM/DD/YYYY")
                    if (d.endDate !== null) {
                        d.endDate = moment(d.endDate).format("MM/DD/YYYY")
                    }
                })
                this.LeaveRequestList = respLeaveRequest['data']
                this.lstLeaveRequest = this.LeaveRequestList;
            } else {
                this.LeaveRequestList = []
                this.lstLeaveRequest = this.LeaveRequestList;
            }
        })
    }


   loadList(StatusID: number) {
        //LoadTableConfig(StatusID);
        if (StatusID != 0) {
            status = this.StatusList.find(x => x.listValueID === StatusID).valueDesc;
            this.lstLeaveRequest = this.LeaveRequestList.filter(x => x.status.toUpperCase() == status.toUpperCase());
        } else {
            this.lstLeaveRequest = this.LeaveRequestList;
        }
    }

    onStatusChange(event: any) {
        this.DefaultDwnDn1ID = event;
        var StatusId: number = event;
        //this.LoadTableConfig(StatusId);
        this.loadList(StatusId);
    }

    tab(event) {
        if (event.index === 0) {
            this.LoadLeaveRequest();
        }
        if (event.index === 1) {
            this.leaveApproval.EmployeeID = this.EmployeeID;
            this.leaveApproval.LoadLeaveRequest();
        }

    }
    

    Add = () => {
        this.AddEditLeaveModal.Show(0, this.EmployeeID, false, this.LeaveRequestList);

    }

    Edit = (selected: ILeaveDisplay) => {
       this.AddEditLeaveModal.Show(selected.leaveID, this.EmployeeID, false, this.LeaveRequestList);
    }

    Cancel = (selected: ILeaveDisplay) => {
       this.AddEditLeaveModal.Show(selected.leaveID, this.EmployeeID, true, this.LeaveRequestList);

    }

    activeItem: ILeaveDisplay;
    items: MenuItem[];
    loadMobilesItems() {
        this.items = [
            { label: 'Edit', icon: 'pi pi-pencil',  command: (e) => { this.activeItem !== null ? this.Edit(this.activeItem) : "" } },
            { label: 'Cancel', icon: 'pi pi-times-circle', disabled: false, styleClass: this.getClass(), command: (e) => { this.activeItem !== null ? this.Cancel(this.activeItem) : "" } },
        ];
    }

    getClass(): string {
        return 'p-button-raised p-button-danger';
    }

    Editdisabled(): boolean {
        if (this.activeItem !== null && this.activeItem !== undefined) {
            if (this.activeItem.status.toUpperCase() === LeaveStatus.PENDING) {
                return false;
            } else {
                return true;
            }
        } else {
            return false;
        }
       // return false;
    }

    toggleMenu(menu, event, rowData) {
        this.activeItem = rowData;
        if (this.activeItem !== null && this.activeItem !== undefined) {
            if (this.activeItem.status.toUpperCase() === LeaveStatus.PENDING) {
                this.items[1].disabled = false;
            } else {
                this.items[1].disabled = true;
            }
            if (this.activeItem.status.toUpperCase() === LeaveStatus.PENDING) {
                this.items[0].disabled = false;
            } else {
                this.items[0].disabled = true;
            }
        }
        menu.toggle(event);
    }

    searchText: string;
    searchableList: any[] = ['startDate', 'endDate', 'leaveType', 'approver', 'status']

    onChangeStatus(event: any) {
        if (event !== undefined && event.value !== undefined) {
            this.onStatusChange(event.value)
        }
    }

    async refreshLeaveData() {
        await this.LoadLeaveBalance();
        this.lstLeaveRequest = null;
        console.log(this.statusId)
        this.statusId = this.DefaultDwnDn1ID;
        this.LoadTableConfig(this.statusId);
        this.leaveRequestService.GetLeave("EmployeeID", this.EmployeeID).subscribe(respLeaveRequest => {
            if (respLeaveRequest['data'] !== null && respLeaveRequest['messageType'] === 1) {
                respLeaveRequest['data'].forEach((d) => {
                    d.startDate = moment(d.startDate).format("MM/DD/YYYY")
                    if (d.endDate !== null) {
                        d.endDate = moment(d.endDate).format("MM/DD/YYYY")
                    }
                })
                this.LeaveRequestList = respLeaveRequest['data']
                this.lstLeaveRequest = this.LeaveRequestList;
            } else {
                this.LeaveRequestList = []
                this.lstLeaveRequest = this.LeaveRequestList;
            }

            this.loadList(this.statusId)
        })
    }


}
