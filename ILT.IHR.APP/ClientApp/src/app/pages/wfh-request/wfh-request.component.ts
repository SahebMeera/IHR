import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { MenuItem } from 'primeng/api';
import { CommonUtils } from '../../common/common-utils';
import { Constants, ListTypeConstants, SessionConstants } from '../../constant';
import { IDropDown } from '../../core/interfaces/IDropDown';
import { IRolePermissionDisplay } from '../../core/interfaces/RolePermission';
import { IWFHDisplay } from '../../core/interfaces/WFH';
import { DataProvider } from '../../core/providers/data.provider';
import { ITableHeaderAction, ITableRowAction } from '../../shared/ihr-table/table-options';
import { LookUpService } from '../lookup/lookup.service';
import { AddEditWfhComponent } from './add-edit-wfh/add-edit-wfh.component';
import { WFHService } from './wfh.service';
import { WFHApprovalComponent } from './wfhapproval/wfhapproval.component';

@Component({
  selector: 'app-wfh-request',
  templateUrl: './wfh-request.component.html',
  styleUrls: ['./wfh-request.component.scss']
})
export class WFHRequestComponent implements OnInit {
    @ViewChild('AddEditWFHModal') AddEditWFHModal: AddEditWfhComponent;
    @ViewChild('WFHApproval') wFHApproval: WFHApprovalComponent;
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
    globalFilterFields = ['startDate', 'endDate', 'title', 'approver', 'status']


    EmployeeID: number;

    //table dropdown
    lstStatus: any[] = [];
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
        this.LoadTableConfig(0);
        this.index = 0;
        var tabindex = this.dataProvider.TabIndex;
        if (tabindex !== undefined && tabindex !== 0 && Object.keys(this.dataProvider).length !== 0) {
            if (this.wFHApproval !== undefined) {
                this.wFHApproval.EmployeeID = this.EmployeeID;
                this.wFHApproval.LoadWFHRequest();
            }
            this.index = 1;
            this.dataProvider.TabIndex = 0;

        } else {
            this.index = 0;
        }
    }

    ngOnInit(): void {
        this.LoadWFHRequest();
        this.loadMobilesItems();
    }

    loadTableColumns() {
        this.cols = [
            { field: 'startDate', header: 'Start Date' },
            { field: 'endDate', header: 'End Date' },
            { field: 'title', header: 'Title' },
            { field: 'approver', header: 'Approver' },
            { field: 'status', header: 'Status' }
        ];

        this.selectedColumns = this.cols;
    }
    WFHRolePermission: IRolePermissionDisplay;
    LoadTableConfig(StatusId: number) {
        this.WFHRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.WFHREQUEST);
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
                this.StatusList = result['data'].filter(x => x.type.toUpperCase() === ListTypeConstants.WFHSTATUS);
                this.setStatusList()
            }
        })
    }

    ListItemEmpType: IDropDown;
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


    lstOfWFH: IWFHDisplay[] = [];
    WFHList: IWFHDisplay[] = [];
    statusId: number;
    LoadWFHRequest() {
        this.lstOfWFH = null;
        this.statusId = 0;
        this.LoadTableConfig(this.statusId);
        this.wfhService.GetWFH("EmployeeID", this.EmployeeID).subscribe(result => {
            if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                result['data'].forEach((d) => {
                    d.startDate = moment(d.startDate).format("MM/DD/YYYY")
                    if (d.endDate !== null) {
                        d.endDate = moment(d.endDate).format("MM/DD/YYYY")
                    }
                })

                this.WFHList = result['data'];
                if (this.DefaultDwnDn1ID !== undefined) {
                    this.statusId = this.DefaultDwnDn1ID;
                    this.loadList(this.statusId)
                } else {
                    this.loadList(this.statusId)
                }
                //this.lstOfWFH = this.WFHList;
            } else {
                this.WFHList = [];
                this.lstOfWFH = [];
            }
        })
    }

    loadList(StatusID: number) {
        if (StatusID != 0) {
            status = this.StatusList.find(x => x.listValueID === StatusID).valueDesc;
            this.lstOfWFH = this.WFHList.filter(x => x.status.toUpperCase() == status.toUpperCase());
        }
        else {
            this.lstOfWFH = this.WFHList;
        }
    }
    onStatusChange(event: any) {
        this.DefaultDwnDn1ID = event;
        var StatusId: number = event;
        //this.LoadTableConfig(StatusId);
        this.loadList(StatusId);
    }

    Add = () => {
        this.AddEditWFHModal.Show(0, this.EmployeeID, false);
    }
    Edit = (selected) => {
        this.AddEditWFHModal.Show(selected.wfhid, this.EmployeeID, false);
    }
    Cancel = (selected) => {
        this.AddEditWFHModal.Show(selected.wfhid, this.EmployeeID, true);
    }
    index: number = 0;
    tab(event) {
        if (event.index === 0) {
            this.LoadWFHRequest();
        }
        if (event.index === 1) {
            this.wFHApproval.EmployeeID = this.EmployeeID;
            this.wFHApproval.LoadWFHRequest();
        }
       
    }

    searchText: string;
    searchableList: any[] = ['startDate', 'endDate', 'title', 'approver', 'status']

    onChangeStatus(event: any) {
        if (event !== undefined && event.value !== undefined) {
            this.onStatusChange(event.value)
            this.loadMobilesItems();
        }
    }

    activeItem: IWFHDisplay;
    items: MenuItem[];
    loadMobilesItems() {
        this.items = [
            { label: 'Edit', icon: 'pi pi-pencil', command: (e) => { this.activeItem !== null ? this.Edit(this.activeItem) : "" } },
            { label: 'Cancel', icon: 'pi pi-times-circle', disabled: false, styleClass: this.getClass(), command: (e) => { this.activeItem !== null ? this.Cancel(this.activeItem) : "" } },
        ];
    }

    getClass(): string {
        return 'p-button-raised p-button-danger';
    }

    Editdisabled(): boolean {
        if (this.activeItem !== null && this.activeItem !== undefined) {
            if (this.activeItem.status.toUpperCase() === 'PENDING'.toUpperCase()) {
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
            if (this.activeItem.status.toUpperCase() === 'PENDING'.toUpperCase()) {
                this.items[1].disabled = false;
            } else {
                this.items[1].disabled = true;
            }
          if (this.activeItem.status.toUpperCase() === 'PENDING'.toUpperCase()) {
                this.items[0].disabled = false;
            } else {
                this.items[0].disabled = true;
            }
        }
        menu.toggle(event);
    }
}
