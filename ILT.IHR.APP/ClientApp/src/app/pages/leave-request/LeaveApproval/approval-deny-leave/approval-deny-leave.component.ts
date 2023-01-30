import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { CommonUtils } from '../../../../common/common-utils';
import { ListTypeConstants, SessionConstants } from '../../../../constant';
import { IDropDown } from '../../../../core/interfaces/IDropDown';
import { IRolePermissionDisplay } from '../../../../core/interfaces/RolePermission';
import { DataProvider } from '../../../../core/providers/data.provider';
import { ITableHeaderAction, ITableRowAction } from '../../../../shared/ihr-table/table-options';
import { LookUpService } from '../../../lookup/lookup.service';
import { LeaveRequestService } from '../../leave-request.service';
import { LeaveApproveDenyComponent } from './leave-approve-deny/leave-approve-deny.component';

@Component({
  selector: 'app-approval-deny-leave',
  templateUrl: './approval-deny-leave.component.html',
  styleUrls: ['./approval-deny-leave.component.scss']
})
export class ApprovalDenyLeaveComponent implements OnInit {
    commonUtils = new CommonUtils()
    cols: any[] = [];
    selectedColumns: any[];
    isPagination: boolean = true;
    loading: boolean = false;
    showCurrentPageReport: boolean = true;
    Country: string = 'Country';
    rowActions: ITableRowAction[] = [];
    headerActions: ITableHeaderAction[] = [];
    patientLanding: string = 'patientLanding';
    globalFilterFields = ['employeeName', 'startDate', 'endDate', 'duration', 'leaveType', 'title','status']


    EmployeeID: number;

    //table dropdown
    Statuslst: any[] = [];
    status: string;
    DefaultDwnDn1ID: number;

    RolePermissions: IRolePermissionDisplay[] = []
    user: any;
    constructor(private fb: FormBuilder, private dataProvider: DataProvider,
        private LookupService: LookUpService,
        private router: Router,
        private leaveRequestService: LeaveRequestService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.user = JSON.parse(localStorage.getItem("User"));
        this.EmployeeID = Number(this.user.employeeID);
        this.LoadDropDown();
        this.loadTableColumns();
        this.LoadTableConfig();}

    ngOnInit(): void {
        this.LoadLeaveRequest();
        if (this.dataProvider.storage != null) {
            var leave = this.dataProvider.storage;
            setTimeout(() => this.Edit(leave))
            this.dataProvider.storage = null;
        }
  }

    loadTableColumns() {
        this.cols = [
            { field: 'employeeName', header: 'Employee' },
            { field: 'startDate', header: 'Start Date' },
            { field: 'endDate', header: 'End Date' },
            { field: 'duration', header: 'Duration' },
            { field: 'leaveType', header: 'Leave Type' },
            { field: 'title', header: 'Title' },
            { field: 'status', header: 'Status' },
        ];

        this.selectedColumns = this.cols;
    }
    LoadTableConfig() {
        this.rowActions = [
            {
                actionMethod: this.Edit,
                iconClass: 'pi pi-eye'
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
        this.Statuslst = [];
        if (this.StatusList !== undefined) {
            this.Statuslst.push({ ID: 0, Value: 'All' })
            this.StatusList.forEach(x => {
                this.Statuslst.push({ ID: x.listValueID, Value: x.valueDesc })
            });
        }
        if (this.status === undefined) {
            this.status = "All";
        }
        this.DefaultDwnDn1ID = this.Statuslst.find(x => x.Value.toLowerCase() == this.status.toLowerCase()).ID;
        this.DefaultDwnDn1ID = this.Statuslst.find(x => x.Value.toUpperCase() === "PENDING".toUpperCase()).ID;
    }

    LeaveRequestList: any[] = [];
    lstLeaveRequest: any[] = [];
    async LoadLeaveRequest() {
        this.LeaveRequestList = [];
        this.leaveRequestService.GetLeave("ApproverID", this.EmployeeID).subscribe(respLeaveRequest => {
            if (respLeaveRequest['data'] !== null && respLeaveRequest['messageType'] === 1) {
                respLeaveRequest['data'].forEach((d) => {
                    d.startDate = moment(d.startDate).format("MM/DD/YYYY")
                    if (d.endDate !== null) {
                        d.endDate = moment(d.endDate).format("MM/DD/YYYY")
                    }
                })
                this.LeaveRequestList = respLeaveRequest['data'];
                if (this.Statuslst !== null && this.Statuslst.length > 0) {
                    this.DefaultDwnDn1ID = this.Statuslst.find(x => x.Value.toUpperCase() === "PENDING".toUpperCase()).ID;
                }
                
                this.loadList("PENDING");
            }
            else {
                this.LeaveRequestList = []
                this.lstLeaveRequest = this.LeaveRequestList;
            }
        });
    }

    onStatusChange(event: any) {
        this.DefaultDwnDn1ID = event;
        var StatusId: number = event;
        var status = this.Statuslst.find(x => x.ID === StatusId).Value;
        this.loadList(status);
    }

    loadList(status: string) {
        this.lstLeaveRequest = [];
        if (status.toLowerCase() !== 'All'.toLowerCase()) {
            this.lstLeaveRequest = this.LeaveRequestList.filter(x => x.status.toUpperCase() === status.toUpperCase());
        } else {
            this.lstLeaveRequest = this.LeaveRequestList
        }
    }

    @ViewChild('approveDenyLeave') approveDenyLeave: LeaveApproveDenyComponent;
    Edit = (selected: any) => {
       this.approveDenyLeave.Show(selected.leaveID);
    }

    RefreshList() {
        this.leaveRequestService.GetLeave("ApproverID", this.EmployeeID).subscribe(respLeaveRequest => {
            if (respLeaveRequest['data'] !== null && respLeaveRequest['messageType'] === 1) {
                respLeaveRequest['data'].forEach((d) => {
                    d.startDate = moment(d.startDate).format("MM/DD/YYYY")
                    if (d.endDate !== null) {
                        d.endDate = moment(d.endDate).format("MM/DD/YYYY")
                    }
                })
                this.LeaveRequestList = respLeaveRequest['data'];
                var status: string = this.StatusList.find(x => x.listValueID == this.DefaultDwnDn1ID).valueDesc;
                this.lstLeaveRequest = this.LeaveRequestList.filter(x => x.status.toUpperCase() === status.toUpperCase());
            }
            else {
                this.LeaveRequestList = []
                this.lstLeaveRequest = this.LeaveRequestList;
            }
        });
    }

    searchText: string;
    searchableList: any[] = ['employeeName', 'startDate', 'endDate', 'leaveType', 'status']

    onChangeStatus(event: any) {
        if (event !== undefined && event.value !== undefined) {
            this.onStatusChange(event.value)
        }
    }

}
