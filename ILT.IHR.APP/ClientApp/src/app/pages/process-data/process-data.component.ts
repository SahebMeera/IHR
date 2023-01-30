import { Component, OnInit, ViewChild } from '@angular/core';
import * as moment from 'moment';
import { MenuItem } from 'primeng/api';
import { CommonUtils } from '../../common/common-utils';
import { Constants, ListTypeConstants, SessionConstants } from '../../constant';
import { IDropDown } from '../../core/interfaces/IDropDown';
import { IProcessDataDisplay } from '../../core/interfaces/ProcessData';
import { IRolePermissionDisplay } from '../../core/interfaces/RolePermission';
import { DataProvider } from '../../core/providers/data.provider';
import { IHRTableComponent } from '../../shared/ihr-table/ihr-table.component';
import { ITableHeaderAction, ITableRowAction } from '../../shared/ihr-table/table-options';
import { LookUpService } from '../lookup/lookup.service';
import { ProcessDataService } from './process-data.service';
import { ProcessWizardService } from './process-wizard.service';
import { WizardDataFlowComponent } from './wizard-data-flow/wizard-data-flow.component';
import { WizardFlowComponent } from './wizard-flow/wizard-flow.component';

@Component({
  selector: 'app-process-data',
  templateUrl: './process-data.component.html',
  styleUrls: ['./process-data.component.scss']
})
export class ProcessDataComponent implements OnInit {
    commonUtils = new CommonUtils();
    cols: any[] = [];
    selectedColumns: any[];
    isPagination: boolean = true;
    loading: boolean = false;
    showCurrentPageReport: boolean = true;
    rowActions: ITableRowAction[] = [];
    headerActions: ITableHeaderAction[] = [];
    patientLanding: string = 'patientLanding';
    globalFilterFields = ['process', 'dataColumns', 'status', 'createdDate','processedDate']
    //ViewChild("dt", { static: true }) public dt: IHRTableComponent;
    user: any;
    EmployeeID: number;
    index: number;

    RolePermissions: IRolePermissionDisplay[] = [];

    constructor(private dataProvider: DataProvider,
        private processWizardService: ProcessWizardService,
        private wizardDataService: ProcessDataService,
        private LookupService: LookUpService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.user = JSON.parse(localStorage.getItem("User"));
        this.EmployeeID = Number(this.user.employeeID);
        this.LoadDropDown();
        this.LoadTableConfig();
        this.loadTableColumns();
        this.index = 0;
        var tabindex = this.dataProvider.TabIndex;
        if (tabindex !== undefined && tabindex !== 0 && Object.keys(this.dataProvider).length !== 0) {
            //if (this.leaveApproval !== undefined) {
            //    this.leaveApproval.EmployeeID = this.EmployeeID;
            //    this.leaveApproval.LoadLeaveRequest();
            //}
            this.index = 1;
            this.dataProvider.TabIndex = 0;

        } else {
            this.index = 0;
        }
    }

    ngOnInit(): void {
        this.LoadDropDown();
        this.LoadTableConfig();
        this.loadTableColumns();
        this.LoadProcessList(0);
        this.loadMobilesItems()
    }


    loadTableColumns() {
        this.cols = [
            { field: 'process', header: 'Process' },
            { field: 'dataColumns', header: 'Process Data' },
            { field: 'status', header: 'Status' },
            { field: 'createdDate', header: 'Created Date' },
            { field: 'processedDate', header: 'Processed Date' },
        ];

        this.selectedColumns = this.cols;
    }
    listValuesStatus: any[] = [];
    WizardList: any[] = [];
    lstStatus: any[] = [];
    LoadDropDown() {
        this.LookupService.getListValues().subscribe(result => {
            if (result['data'] !== undefined && result['data'] !== null) {
                this.listValuesStatus = result['data'].filter(x => x.type.toUpperCase() === Constants.WIZARDSTATUS);
                this.setStatusList()
            }
        })
        this.processWizardService.GetWizards().subscribe(result => {
            if (result['data'] !== undefined && result['data'] !== null) {
                this.WizardList = result['data'];
                  this.setWizardList();
            }
        })
        this.LoadProcessList(0)
    }

    ListItemEmpType: IDropDown;
    statusId: number;
    statusFilter: string;
    selectedStatusList: number[] = []
    DefaultDwnDn1ID: number;
    DefaultTypeID: number;
    wizardId: number;
    lstProcesses: any[] = [];
    setWizardList() {
        this.lstProcesses = [];
        if (this.lstProcesses !== undefined) {
            this.lstProcesses.push({ ID: 0, Value: 'All' })
            this.WizardList.forEach(x => {
                this.lstProcesses.push({ ID: x.processWizardID, Value: x.process })
            });
        }
        this.DefaultTypeID = this.lstProcesses[0].ID;
        this.wizardId = this.DefaultTypeID;
        this.DefaultDwnDn1ID = this.DefaultTypeID;
    }
    process: string;
    onProcessChange(event: any) {
        this.DefaultDwnDn1ID = event;
        this.DefaultTypeID = event;
        this.LoadTableConfig();
        this.process = this.lstProcesses.find(x => x.ID == event).Value;
        this.LoadProcessList(event);
    }

    onChangeWizardStatusList(event: any[]) {
        console.log('event', event)
        this.selectedEmpTypeList = [];
        this.selectedStatusList = [];
        this.selectedStatusList = event;
        this.selectedEmpTypeList = event;
        if (this.selectedStatusList.length > 0) {
            if (this.selectedStatusList.length === this.lstMultiStatus.length) {
                this.statusFilter = 'All';
            } else {
                this.statusFilter = 'NotAll';
            }
        }
        this.LoadProcessList(this.DefaultTypeID);
    }
    lstMultiStatus: any[] = [];
    selectedEmpTypeList: number[] = [];
    setStatusList() {
        //this.selectedEmpTypeList = [];
        this.lstMultiStatus = [];
        if (this.listValuesStatus !== undefined) {
           // this.lstMultiStatus.push({ ID: 0, Value: 'All' })
            this.listValuesStatus.forEach(x => {
                this.lstMultiStatus.push({ ID: x.listValueID, Value: x.valueDesc })
            });
        }
        if (this.statusFilter == null || this.statusFilter == undefined) {
            this.statusFilter = "NotAll";
        }
        if (this.lstMultiStatus !== undefined && this.lstMultiStatus.length > 0) {
            this.lstMultiStatus.forEach(x => {
                if (x.Value.toUpperCase() == "PENDING" || x.Value.toUpperCase() == "IN PROCESS") {
                   // x.IsSelected = true;
                    this.selectedEmpTypeList.push(x.ID);
                }
            })
        }
    }

    LeaveRolePermission: IRolePermissionDisplay;
    LoadTableConfig() {
        //this.LeaveRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.LEAVEREQUEST);
        this.rowActions = [
            {
                actionMethod: this.Edit,
                iconClass: 'pi pi-pencil',
            },
            {
                actionMethod: this.Process,
                iconClass: 'pi pi-upload',
            },
            {
                actionMethod: this.Tickets,
                iconClass: 'pi pi-list',
            },
        ];
        this.headerActions = [
            {
                actionMethod: this.Add,
                hasIcon: false,
                styleClass: 'btn-width-height btn btn-block btn-sm btn-info',
                actionText: 'Add',
                iconClass: 'fas fa-plus',
                isDisabled: this.DefaultTypeID !== 0 && this.DefaultTypeID !== undefined ? false : true
            }
        ];
    }

    lstWizardDataList: any[] = [];
    LoadProcessList(processId: number) {
        console.log('processId', processId)
        console.log('processId', this.selectedStatusList)
        console.log('processId', this.statusFilter)
        this.wizardId = processId;
        this.LoadTableConfig();
        this.wizardDataService.GetWizardDatas().subscribe(result => {
            if (result['data'] !== undefined && result['data'] !== null && result['messageType'] === 1) {
                result['data'].forEach((d) => {
                    d.createdDate = moment(d.createdDate).format("MM/DD/YYYY")
                    if (d.processedDate !== null) {
                        d.processedDate = moment(d.processedDate).format("MM/DD/YYYY")
                        }
                })

                if (this.wizardId != 0) {
                    this.lstWizardDataList = result['data'].filter(x => x.processWizardID == this.wizardId);
                }
                else {
                    this.lstWizardDataList = result['data'];
                }
                console.log(this.selectedStatusList)
                if (this.selectedStatusList != null && this.statusFilter != "All") {
                   
                    this.lstWizardDataList = this.lstWizardDataList.filter(x => this.selectedEmpTypeList.includes(x.statusId));
                }
            }
        })
    }
    @ViewChild('WizardFlowBaseModal') WizardFlowBaseModal: WizardFlowComponent;
    @ViewChild('WizardDataFlowModal') WizardDataFlowModal: WizardDataFlowComponent;

    Add = () => {
       // this.WizardFlowBaseModal.loadProcessData(this.DefaultTypeID)
        this.WizardFlowBaseModal.Show(this.DefaultTypeID);
    }

    LoadList(event) {

        this.LoadProcessList(event);
    }


    activeItem: IProcessDataDisplay;
    items: MenuItem[];
    loadMobilesItems() {
        this.items = [
            { label: 'Edit', icon: 'pi pi-pencil', disabled: false, command: (e) => { this.activeItem !== null ? this.Edit(this.activeItem) : "" } },
            { label: 'Process', icon: 'pi pi-upload', disabled: false, command: (e) => { this.activeItem !== null ? this.Process(this.activeItem) : "" } },
            { label: 'Tickets', icon: 'pi pi-list', disabled: false, command: (e) => { this.activeItem !== null ? this.Tickets(this.activeItem) : "" } },
        ];
    }

    toggleMenu(menu, event, rowData) {
        this.activeItem = rowData;
        if (this.activeItem !== null && this.activeItem !== undefined) {
            //if (this.activeItem.reviewStatus.toUpperCase() === this.reviewStatusConstants.SUBMITTED) {
            //    this.items[1].disabled = false;
            //} else {
            //    this.items[1].disabled = true;
            //}
            //if (this.companyID !== 0 && this.activeItem.companyID === this.companyID) {
            //    this.items[0].disabled = false;
            //} else {
            //    this.items[0].disabled = true;
            //}
        }
        menu.toggle(event);
    }

    Edit = (selected) => {
        this.WizardDataFlowModal.Show(selected.processDataID, false, this.DefaultTypeID)
    }

    Process = (selected) => {
        this.WizardDataFlowModal.Show(selected.processDataID, true, this.DefaultTypeID)
    }

    Tickets = (selected) => {
      //  wizardName = selected.Process; //lstProcesses.Find(x => x.ID == selected.ProcessDataID).Value;
        this.WizardDataFlowModal.showTickets(selected.processDataID);
    }


    searchText: string;
    searchableList: any[] = ['process', 'dataColumns', 'status', 'createdDate', 'processedDate']
    onChangeProcess(event: any) {
        if (event !== undefined && event.value !== undefined) {
            this.onProcessChange(event.value)
        }
    }

    OnEmpChange(event: any) {
        if (event !== undefined && event.value !== undefined) {
            this.selectedEmpTypeList = event.value;
            this.onChangeWizardStatusList(event.value);
        }
    }

}
