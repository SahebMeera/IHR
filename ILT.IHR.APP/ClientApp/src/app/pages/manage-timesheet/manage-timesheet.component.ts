import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { CommonUtils } from '../../common/common-utils';
import { Constants, ListTypeConstants, SessionConstants } from '../../constant';
import { IDropDown } from '../../core/interfaces/IDropDown';
import { IRolePermissionDisplay } from '../../core/interfaces/RolePermission';
import { ITimesheetDisplay } from '../../core/interfaces/Timesheet';
import { DataProvider } from '../../core/providers/data.provider';
import { ITableHeaderAction, ITableRowAction } from '../../shared/ihr-table/table-options';
import { LookUpService } from '../lookup/lookup.service';
import { TimesheetService } from '../timesheet/timesheet.service';
import { AddEditManageTimesheetComponent } from './add-edit-manage-timesheet/add-edit-manage-timesheet.component';

@Component({
  selector: 'app-manage-timesheet',
  templateUrl: './manage-timesheet.component.html',
  styleUrls: ['./manage-timesheet.component.scss']
})
export class ManageTimesheetComponent implements OnInit {

    //
    //@ViewChild('TimeSheetApproval') timeSheetApproval: ApprovalDenyTimesheetComponent;

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


    index: number;

    EmployeeID: number;
    //table dropdown
    lstStatus: any[] = [];
    status: string;
    DefaultDwnDn1ID: number;

    LeaveBalanceList: any[] = [];

    RolePermissions: IRolePermissionDisplay[] = []
    user: any;


    constructor(private fb: FormBuilder, private dataProvider: DataProvider,
        private LookupService: LookUpService,
        private router: Router,
        private timesheetService: TimesheetService
    ) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.user = JSON.parse(localStorage.getItem("User"));
        this.EmployeeID = Number(this.user.employeeID);
        this.LoadDropDown();
        this.loadTableColumns();
        this.LoadTableConfig(0);
       
    }

    ngOnInit(): void {
        this.LoadRequest();
    }

    globalFilterFields = ['employeeName', 'weekEnding', 'totalHours', 'submittedBy', 'submittedDate', 'approvedByEmail', 'approvedDate']

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

    TimesheetRolePermission: IRolePermissionDisplay;
    LoadTableConfig(StatusId: number) {
        this.TimesheetRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.TIMESHEET);
        this.rowActions = [];
       // if (this.TimesheetRolePermission != null && this.TimesheetRolePermission.update == true) {
            var m1 = {
                actionMethod: this.Edit,
                iconClass: 'pi pi-eye',
            }
            this.rowActions.push(m1);
       // }
        this.headerActions = [];
    }

    StatusList: any[] = [];
    LoadDropDown() {
        this.LookupService.getListValues().subscribe(result => {
            if (result['data'] !== undefined && result['data'] !== null) {
                this.StatusList = result['data'].filter(x => x.type.toUpperCase() === ListTypeConstants.TIMESHEETSTATUS);
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
            this.status = "Approved";
        }
        this.DefaultDwnDn1ID = this.lstStatus.find(x => x.Value.toLowerCase() == this.status.toLowerCase()).ID;
        if (this.lstTimeSheetRequest.length > 0) {
            this.loadList(this.DefaultDwnDn1ID);
        }
    }
    TimeSheetsList: ITimesheetDisplay[] = [];
    lstTimeSheetRequest: ITimesheetDisplay[] = [];
    async LoadRequest() {
        this.lstTimeSheetRequest = [];
       // this.statusId = 0;
        this.LoadTableConfig(this.statusId);
        this.timesheetService.GetTimeSheets(0, 0).subscribe(respRequest => {
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
                if (this.lstStatus !== undefined && this.lstStatus.length > 0) {
                   // this.DefaultDwnDn1ID = this.lstStatus.find(x => x.Value.toLowerCase() == this.status.toLowerCase()).ID;
                    this.loadList(this.DefaultDwnDn1ID);
                } else {
                    this.lstTimeSheetRequest = this.TimeSheetsList;
                }
            } else {
                this.TimeSheetsList = []
                this.lstTimeSheetRequest = this.TimeSheetsList;
            }
        })
    }



    loadList(StatusID: number) {
        //LoadTableConfig(StatusID);
        if (StatusID != 0) {
            status = this.StatusList.find(x => x.listValueID === StatusID).valueDesc;
            this.lstTimeSheetRequest = this.TimeSheetsList.filter(x => x.status.toUpperCase() == status.toUpperCase());
        } else {
            this.lstTimeSheetRequest = this.TimeSheetsList;
        }
    }

    onStatusChange(event: any) {
        this.DefaultDwnDn1ID = event;
        var StatusId: number = event;
        //this.LoadTableConfig(StatusId);
        this.loadList(StatusId);
    }
   @ViewChild('AddEditTimesheetModal') AddEditTimesheetModal: AddEditManageTimesheetComponent;



    Edit = (selected: ITimesheetDisplay) => {
        this.AddEditTimesheetModal.Show(selected.timeSheetID)
    }

    Cancel = () => {

    }

    searchableList: any[] = ['employeeName', 'weekEnding', 'totalHours', 'submittedBy', 'submittedDate', 'approvedByEmail', 'approvedDate']

    onChangeStatus(event: any) {
        if (event !== undefined && event.value !== undefined) {
            this.onStatusChange(event.value)
        }
    }

}
