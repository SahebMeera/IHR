import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { CommonUtils } from '../../../common/common-utils';
import { ListTypeConstants, SessionConstants } from '../../../constant';
import { IDropDown } from '../../../core/interfaces/IDropDown';
import { IRolePermissionDisplay } from '../../../core/interfaces/RolePermission';
import { IWFHDisplay } from '../../../core/interfaces/WFH';
import { DataProvider } from '../../../core/providers/data.provider';
import { ITableHeaderAction, ITableRowAction } from '../../../shared/ihr-table/table-options';
import { LookUpService } from '../../lookup/lookup.service';
import { WFHService } from '../wfh.service';
import { ApproveDenyWfhComponent } from './approve-deny-wfh/approve-deny-wfh.component';

@Component({
  selector: 'app-wfhapproval',
  templateUrl: './wfhapproval.component.html',
  styleUrls: ['./wfhapproval.component.scss']
})
export class WFHApprovalComponent implements OnInit {
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
    globalFilterFields = ['employeeName', 'startDate', 'endDate', 'title', 'status']


    EmployeeID: number = 0;

    //table dropdown
    Statuslst: any[] = [];
    status: string;
    DefaultDwnDn1ID: number;

    RolePermissions: IRolePermissionDisplay[] = []
    user: any;
    constructor(private fb: FormBuilder, private dataProvider: DataProvider,
        private LookupService: LookUpService,
        private router: Router,
        private wfhService: WFHService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.user = JSON.parse(localStorage.getItem("User"));
        this.EmployeeID = Number(this.user.employeeID);
        this.LoadDropDown();
        this.loadTableColumns();
        this.LoadTableConfig();
    }

    ngOnInit(): void {
        this.LoadWFHRequest();
    }

    loadTableColumns() {
        this.cols = [
            { field: 'employeeName', header: 'Employee' },
            { field: 'startDate', header: 'Start Date' },
            { field: 'endDate', header: 'End Date' },
            { field: 'title', header: 'Title' },
            { field: 'status', header: 'Status' }
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
                this.StatusList = result['data'].filter(x => x.type.toUpperCase() === ListTypeConstants.WFHSTATUS);
                this.setStatusList()
            }
        })
    }

    ListItemEmpType: IDropDown;
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
      //  this.DefaultDwnDn1ID = this.Statuslst.find(x => x.Value.toLowerCase() == this.status.toLowerCase()).ID;
        this.DefaultDwnDn1ID = this.Statuslst.find(x => x.Value.toUpperCase() === "PENDING".toUpperCase()).ID;
    }

    lstOfWFHApproval: IWFHDisplay[] = [];
    WFHList: IWFHDisplay[] = [];
    statusId: number;
    LoadWFHRequest() {
        this.lstOfWFHApproval = [];
        this.statusId = 0;
        this.wfhService.GetWFH("ApproverID", this.EmployeeID).subscribe(result => {
            if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                result['data'].forEach((d) => {
                    d.startDate = moment(d.startDate).format("MM/DD/YYYY")
                    if (d.endDate !== null) {
                        d.endDate = moment(d.endDate).format("MM/DD/YYYY")
                    }
                })
                this.WFHList = result['data'];
                if (this.Statuslst !== null && this.Statuslst.length > 0) {
                    this.DefaultDwnDn1ID = this.Statuslst.find(x => x.Value.toUpperCase() === "PENDING".toUpperCase()).ID;
                }
                
                this.loadList("PENDING");
                //this.loadList(this.statusId)
                //this.loadList(this.statusId)
            } else {
                this.WFHList = [];
                this.lstOfWFHApproval = [];
            }
        })
    }

    loadList(status: string) {
        this.lstOfWFHApproval = [];
        if (status.toLowerCase() !== 'All'.toLowerCase()) {
            this.lstOfWFHApproval = this.WFHList.filter(x => x.status.toUpperCase() === status.toUpperCase());
        } else {
            this.lstOfWFHApproval = this.WFHList
        }
    }

    onStatusChange(event: any) {
        this.DefaultDwnDn1ID = event;
        var StatusId: number = event;
        var status = this.Statuslst.find(x => x.ID === StatusId).Value;
        this.loadList(status);
    }
    @ViewChild('approveDenyWFH') approveDenyWFH: ApproveDenyWfhComponent;
    Edit = (selected: any) => {
        this.approveDenyWFH.Show(selected.wfhid);
    }

    RefreshList() {
        this.wfhService.GetWFH("ApproverID", this.EmployeeID).subscribe(result => {
            if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                result['data'].forEach((d) => {
                    d.startDate = moment(d.startDate).format("MM/DD/YYYY")
                    if (d.endDate !== null) {
                        d.endDate = moment(d.endDate).format("MM/DD/YYYY")
                    }
                })
                this.WFHList = result['data'];
                var status: string = this.StatusList.find(x => x.listValueID == this.DefaultDwnDn1ID).valueDesc;
                this.lstOfWFHApproval = this.WFHList.filter(x => x.status.toUpperCase() == status.toUpperCase());
            }
        })
       // this.UpdateWFHList.InvokeAsync(true);
    }

    searchText: string;
    searchableList: any[] = ['employeeName', 'startDate', 'endDate', 'leaveType', 'status']

    onChangeStatus(event: any) {
        if (event !== undefined && event.value !== undefined) {
            this.onStatusChange(event.value)
        }
    }



}
