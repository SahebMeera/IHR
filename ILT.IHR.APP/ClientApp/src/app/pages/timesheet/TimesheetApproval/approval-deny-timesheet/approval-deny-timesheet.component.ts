import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { CommonUtils } from '../../../../common/common-utils';
import { ListTypeConstants, SessionConstants, TimeSheetStatusConstants } from '../../../../constant';
import { IDropDown } from '../../../../core/interfaces/IDropDown';
import { IRolePermissionDisplay } from '../../../../core/interfaces/RolePermission';
import { ITimesheetDisplay } from '../../../../core/interfaces/Timesheet';
import { DataProvider } from '../../../../core/providers/data.provider';
import { ITableHeaderAction, ITableRowAction } from '../../../../shared/ihr-table/table-options';
import { LookUpService } from '../../../lookup/lookup.service';
import { TimesheetService } from '../../timesheet.service';
import { TimesheetApprovalDenyComponent } from './timesheet-approval-deny/timesheet-approval-deny.component';

@Component({
  selector: 'app-approval-deny-timesheet',
  templateUrl: './approval-deny-timesheet.component.html',
  styleUrls: ['./approval-deny-timesheet.component.scss']
})
export class ApprovalDenyTimesheetComponent implements OnInit {
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
    globalFilterFields = ['employeeName', 'weekEnding', 'totalHours', 'status', 'submittedBy', 'submittedDate', 'approvedByEmail', 'approvedDate']


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
        private timesheetService: TimesheetService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.user = JSON.parse(localStorage.getItem("User"));
        this.EmployeeID = Number(this.user.employeeID);
        this.LoadDropDown();
        this.loadTableColumns();
        this.LoadTableConfig();
    }

    ngOnInit(): void {
        this.LoadTimeSheetRequest();
        if (this.dataProvider.storage != null) {
            var timesheet = this.dataProvider.storage;
            setTimeout(() => this.Edit(timesheet))
            this.dataProvider.storage = null;
        }
    }


    loadTableColumns() {
        this.cols = [
            { field: 'employeeName', header: 'Emp Name' },
            { field: 'weekEnding', header: 'Week Ending' },
            { field: 'totalHours', header: 'Total Hours' },
            { field: 'status', header: 'Status' },
            { field: 'submittedBy', header: 'Submitted By' },
            { field: 'submittedDate', header: 'Submitted Date' },
            { field: 'approvedByEmail', header: 'Approved By Email' },
            { field: 'approvedDate', header: 'Approved Date' },
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
                this.StatusList = result['data'].filter(x => x.type.toUpperCase() === ListTypeConstants.TIMESHEETSTATUS);
                this.DefaultDwnDn1ID = this.StatusList !== undefined && this.StatusList.find(x => x.value.toUpperCase() == TimeSheetStatusConstants.SUBMITTED.toUpperCase()).listValueID !== undefined ? this.StatusList.find(x => x.value.toUpperCase() == TimeSheetStatusConstants.SUBMITTED.toUpperCase()).listValueID : 0;
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
        this.DefaultDwnDn1ID = this.Statuslst.find(x => x.Value.toUpperCase() === "SUBMITTED".toUpperCase()).ID;
    }

   
    TimeSheetsList: ITimesheetDisplay[] = [];
    lstTimeSheetRequest: ITimesheetDisplay[] = [];
    async LoadTimeSheetRequest() {
        this.lstTimeSheetRequest = [];
        this.statusId = 0;
        this.LoadDropDown();
       //this.LoadTableConfig(this.statusId);
        this.timesheetService.GetTimeSheets(0, this.user.userID).subscribe(respRequest => {
            if (respRequest['data'] !== null && respRequest['messageType'] === 1) {
                respRequest['data'].forEach((d) => {
                    d.weekEnding = moment(d.weekEnding).format("MM/DD/YYYY")
                    if (d.submittedDate !== null) {
                        d.submittedDate = moment(d.submittedDate).format("MM/DD/YYYY")
                    }
                    if (d.approvedDate !== null) {
                        d.approvedDate = moment(d.approvedDate).format("MM/DD/YYYY")
                    }
                })
                this.TimeSheetsList = respRequest['data']
                this.lstTimeSheetRequest = this.TimeSheetsList;
                this.loadList('SUBMITTED');
            } else {
                this.TimeSheetsList = []
                this.lstTimeSheetRequest = this.TimeSheetsList;
            }
        })
    }

    onStatusChange(event: any) {
        this.DefaultDwnDn1ID = event;
        var StatusId: number = event;
        var status = this.Statuslst.find(x => x.ID === StatusId).Value;
        this.loadList(status);
    }

    loadList(status: string) {
        this.lstTimeSheetRequest = [];
        if (status.toLowerCase() !== 'All'.toLowerCase()) {
            this.lstTimeSheetRequest = this.TimeSheetsList.filter(x => x.status.toUpperCase() == status.toUpperCase());
        } else {
            this.lstTimeSheetRequest = this.TimeSheetsList;
        }
    }

    RefreshList() {
        this.timesheetService.GetTimeSheets(0, this.user.userID).subscribe(respLeaveRequest => {
            if (respLeaveRequest['data'] !== null && respLeaveRequest['messageType'] === 1) {
                respLeaveRequest['data'].forEach((d) => {
                    d.weekEnding = moment(d.weekEnding).format("MM/DD/YYYY")
                    if (d.submittedDate !== null) {
                        d.submittedDate = moment(d.submittedDate).format("MM/DD/YYYY")
                    }
                    if (d.approvedDate !== null) {
                        d.approvedDate = moment(d.approvedDate).format("MM/DD/YYYY")
                    }
                })
                this.TimeSheetsList = respLeaveRequest['data'];
                var status: string = this.StatusList.find(x => x.listValueID == this.DefaultDwnDn1ID).valueDesc;
                this.lstTimeSheetRequest = this.TimeSheetsList.filter(x => x.status.toUpperCase() === status.toUpperCase());
            }
            else {
                this.lstTimeSheetRequest = []
                this.lstTimeSheetRequest = this.TimeSheetsList;
            }
        });
    }

    @ViewChild('approveDenyTimesheet') approveDenyTimesheet: TimesheetApprovalDenyComponent;
    Edit = (selected: ITimesheetDisplay) => {
        this.approveDenyTimesheet.Show(selected.timeSheetID);
    }


    searchText: string;
    searchableList: any[] = ['employeeName', 'weekEnding', 'status', 'submittedDate', 'totalHours']

    onChangeStatus(event: any) {
        if (event !== undefined && event.value !== undefined) {
            this.onStatusChange(event.value)
        }
    }
 

}
