import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
//import { EmployeeService } from './employee.service';
import { IRolePermissionDisplay } from '../../core/interfaces/RolePermission';
import { Constants, ListTypeConstants, SessionConstants, UserRole } from '../../constant';
import { DataProvider } from '../../core/providers/data.provider';
import { ITableHeaderAction, ITableRowAction } from '../../shared/ihr-table/table-options';
import { LookUpService } from '../lookup/lookup.service';
import { IDropDown } from '../../core/interfaces/IDropDown';
import { CommonUtils } from '../../common/common-utils';
import * as moment from 'moment';
import { Router } from '@angular/router';
import { IExpenseForDisplay } from '../../core/interfaces/Expense';
import { TicketService } from '../ticket/ticket.service';
import { ITicketForDisplay } from '../../core/interfaces/Ticket';
import { forkJoin } from 'rxjs';
import { EmployeeService } from '../employee/employee.service';
import { IEmployeeDisplay } from '../../core/interfaces/Employee';
import { AddEditTicketComponent } from './add-edit-ticket/add-edit-ticket.component';

@Component({
  selector: 'app-ticket',
  templateUrl: './ticket.component.html',
  styleUrls: ['./ticket.component.scss']
})
export class TicketComponent implements OnInit {

    commonUtils = new CommonUtils()
    cols: any[] = [];
    selectedColumns: any[];
    isPagination: boolean = true;
    loading: boolean = false;
    showCurrentPageReport: boolean = true;
    rowActions: ITableRowAction[] = [];
    headerActions: ITableHeaderAction[] = [];
    patientLanding: string = 'patientLanding';
    globalFilterFields = ['ticketID', 'ticketType', 'title', 'requestedBy', 'assignedTo', 'status', 'resolvedDate']

    lstTicket: any[] = [];
    //table dropdown
    lstStatus: any[] = [];
    status: string;
    DefaultDwnDn1ID: number;

    RolePermissions: IRolePermissionDisplay[] = []
    user: any;

    EmployeeID: number = 0;
    currentLoginUserRole: string = '';
    UserName: string = '';

    TicketStatusList: any[] = [];
    TicketStatus: string = '';

    DropDown2DefaultID: number = null;

    constructor(private fb: FormBuilder, private dataProvider: DataProvider,
        private ticketService: TicketService,
        private lookupService: LookUpService,
        private employeeService: EmployeeService,
        private router: Router) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.user = JSON.parse(localStorage.getItem("User"));
        this.EmployeeID = Number(this.user.employeeID);
        this.UserName = this.user.firstName + " " + this.user.lastName;
        this.currentLoginUserRole = localStorage.getItem("RoleName");
        this.LoadDropDown();
        this.loadTableColumns();
        this.LoadTableConfig();
    }

    ngOnInit(): void {
        this.LoadTickets();
        this.LoadDropDown();
        if (this.dataProvider.storage != null) {
            var ticket = this.dataProvider.storage;
            if (ticket.ticketID != 0) {
                setTimeout(() => this.edit(ticket))
            }
            this.dataProvider.storage = null;
        }
    }

    loadTableColumns() {
        this.cols = [
            { field: 'ticketID', header: 'Ticket ID' },
            { field: 'ticketType', header: 'Ticket Type' },
            { field: 'title', header: 'Title' },
            { field: 'requestedBy', header: 'Requested By' },
            { field: 'assignedTo', header: 'Assigned To' },
            { field: 'status', header: 'Status' },
            { field: 'resolvedDate', header: 'Resolved Date' },
        ];

        this.selectedColumns = this.cols;
    }

    LoadTableConfig() {
        var TicketRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.TICKET);
        if (TicketRolePermission != null) {
            this.rowActions = []
            if (TicketRolePermission.update == true) {
                this.rowActions.push(
                    {
                        actionMethod: this.edit,
                        iconClass: 'pi pi-pencil'
                    })
                
            }
            if (TicketRolePermission.delete == true) {
                this.rowActions.push(
                    {
                        actionMethod: this.Delete,
                        styleClass: 'p-button-raised p-button-danger',
                        iconClass: 'pi pi-trash'
                    }
                )
            }
        }
        if (TicketRolePermission != null) {
            if (TicketRolePermission.add == true) {
                this.headerActions = [
                    {
                        actionMethod: this.add,
                        hasIcon: false,
                        styleClass: 'btn btn-block btn-sm btn-info',
                        actionText: 'Add',
                        iconClass: 'fas fa-plus'
                    }
                ];
            }
        } else {
            this.headerActions = []
        }
    }
    Delete = () => {

    }
    lstTicketStatus: any[] = [];
    respEmployees: IEmployeeDisplay[] = [];
    Employees: IEmployeeDisplay[] = [];
    reponses: any[] = [];
    TicketReponses: any[] = [];
    TicketsList: any[] = [];
    lstTicketsList: any[] = [];
    LoadDropDown() {
        forkJoin(
            this.lookupService.getListValues(),
            this.employeeService.GetEmployees(),
            this.ticketService.GetTicket(),
            this.ticketService.GetTicketsList(this.EmployeeID, this.EmployeeID)
        ).subscribe(async resultSet => {
            if (resultSet !== undefined && resultSet !== null) {
                 this.TicketStatusList = resultSet[0]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.TICKETSTATUS);
                if (this.TicketStatus === null || this.TicketStatus === '') {
                    this.setEmployeeTypeList();
                }
                else {
                     //this.lstTicketStatus = this.selectedStatusList;
                }
                 this.respEmployees = resultSet[1]['data'];
                if (this.currentLoginUserRole.toUpperCase() === UserRole.ADMIN) {
                    this.reponses = resultSet[2]['data']
                    this.reponses.forEach((d) => {
                        if (d.resolvedDate !== null) {
                            d.resolvedDate = moment(d.resolvedDate).format("MM/DD/YYYY")
                        }
                    })
                } else {
                    this.reponses = resultSet[3]['data']
                    this.reponses.forEach((d) => {
                        if (d.resolvedDate !== null) {
                            d.resolvedDate = moment(d.resolvedDate).format("MM/DD/YYYY")
                        }
                    })
                }
                 this.TicketReponses = this.reponses;
                if (this.reponses !== null && this.reponses !== undefined && this.reponses.length > 0 && this.respEmployees !== null && this.respEmployees !== undefined && this.respEmployees.length > 0) {
                    if (this.currentLoginUserRole.toUpperCase() === UserRole.ADMIN) {
                        this.Employees = this.respEmployees.filter(x => this.reponses.some(s => s.assignedToID === x.employeeID));
                    } else {
                        this.Employees = this.respEmployees.filter(x => this.user.employeeID === x.employeeID);
                    }
                    if (this.Employees !== null) {
                        this.loadAssignedTo();
                    }
                } else {
                    this.Employees = this.respEmployees;
                }

                this.TicketsList = this.TicketReponses;
                if (this.TicketsList != null) {
                    this.loadTicketList();
                } else {
                    this.lstTicketsList = this.TicketsList;
                }
            }
        });
    }



   
    lstTicketAssignedTo: any[] = [];
    TicketAssignedTo: string = '';
    selectedEmpTypeList: number[] = [];
    loadAssignedTo() {
        this.lstTicketAssignedTo = [];
        this.selectedEmpTypeList = [];
        if (this.Employees !== undefined) {
            //this.lstTicketAssignedTo.push({ ID: null, Value: 'All' })
            this.Employees.forEach(x => {
                this.lstTicketAssignedTo.push({ ID: x.employeeID, Value: x.employeeName })
            });
        }
        if (this.TicketAssignedTo == undefined) {
            this.TicketAssignedTo = "All";
        }
        if (this.lstTicketAssignedTo !== undefined && this.lstTicketAssignedTo.length > 0) {
            this.lstTicketAssignedTo.forEach(x => {
                if (x.ID === this.user.employeeID) {
                    this.selectedEmpTypeList.push(x.ID);
                }
            })
        }
    }
    selectedStatusList: number[] = [];
    setEmployeeTypeList() {
        this.selectedStatusList = [];
        this.lstTicketStatus = [];
        if (this.TicketStatusList !== undefined) {
            // this.lstEmployeeType.push({ ID: 0, Value: 'All' })
            this.TicketStatusList.forEach(x => {
                this.lstTicketStatus.push({ ID: x.listValueID, Value: x.valueDesc })
            });
        }
        if (this.TicketStatus === '') {
            this.TicketStatus = "New";
        }
        if (this.lstTicketStatus !== undefined && this.lstTicketStatus.length > 0) {
            this.lstTicketStatus.forEach(x => {
                if (x.Value.toLowerCase() === 'New'.toLowerCase()) {
                    this.selectedStatusList.push(x.ID);
                }
                if (x.Value.toLowerCase() == "Assigned".toLowerCase()) {
                    this.selectedStatusList.push(x.ID);
                }
            })
        }
    }
    OnAssignedToChange(event: any[]) {
        this.selectedEmpTypeList = [];
        this.selectedEmpTypeList = event;
        if (this.selectedEmpTypeList.length > 0) {
            if (this.selectedEmpTypeList.length === this.lstTicketAssignedTo.length) {
                this.TicketAssignedTo = 'All';
            } else {
                this.TicketAssignedTo = 'NotAll';
            }
        }
        this.LoadList();
    }

    onStatusChange(event: any[]) {
        this.selectedStatusList = [];
        this.selectedStatusList = event;
        if (this.selectedStatusList.length > 0) {
            if (this.selectedStatusList.length === this.lstTicketAssignedTo.length) {
                this.TicketStatus = 'All';
            } else {
                this.TicketStatus = 'NotAll';
            }
        }
        this.LoadList();
    }


    async LoadList() {
        await this.LoadTableConfig();
        forkJoin(
            this.ticketService.GetTicket(),
            this.ticketService.GetTicketsList(this.EmployeeID, this.EmployeeID)
        ).subscribe(async resultSet => {
            if (resultSet !== undefined && resultSet !== null) {
                if (this.currentLoginUserRole.toUpperCase() === UserRole.ADMIN) {
                    this.reponses = [];
                    this.reponses = resultSet[0]['data']
                    this.reponses.forEach((d) => {
                        if (d.resolvedDate !== null) {
                            d.resolvedDate = moment(d.resolvedDate).format("MM/DD/YYYY")
                        }
                    })
                } else {
                    this.reponses = [];
                    this.reponses = resultSet[1]['data']
                    this.reponses.forEach((d) => {
                        if (d.resolvedDate !== null) {
                            d.resolvedDate = moment(d.resolvedDate).format("MM/DD/YYYY")
                        }
                    })
                }
                
                this.TicketsList = this.reponses;
                if (this.TicketsList != null) {
                    this.loadTicketList();
                } else {
                    this.lstTicketsList = this.TicketsList;
                }
            }
        })

    }

    loadTicketList() {
        if (this.TicketStatus !== "All" && this.selectedStatusList !== null && this.TicketAssignedTo !== "All" && this.selectedEmpTypeList != null ) {
            if (this.currentLoginUserRole.toUpperCase() === UserRole.ADMIN || this.currentLoginUserRole.toUpperCase() === UserRole.FINADMIN || this.currentLoginUserRole.toUpperCase() === UserRole.ITADMIN || this.currentLoginUserRole.toUpperCase() === UserRole.OPSADMIN) {
                this.lstTicketsList = this.TicketsList.filter(x => this.selectedStatusList.includes(x.statusID) && (this.selectedEmpTypeList.includes(x.assignedToID) || x.assignedToID == null));
            }
            else {
                this.lstTicketsList = this.TicketsList.filter(x => this.selectedStatusList.includes(x.statusID) && this.selectedEmpTypeList.includes(x.assignedToID));
            }
        }
        else if (this.TicketStatus == "All" && this.TicketAssignedTo != "All") {
            if (this.currentLoginUserRole.toUpperCase() == UserRole.ADMIN || this.currentLoginUserRole.toUpperCase() == UserRole.FINADMIN || this.currentLoginUserRole.toUpperCase() == UserRole.ITADMIN || this.currentLoginUserRole.toUpperCase() == UserRole.OPSADMIN) {
                this.lstTicketsList = this.TicketsList.filter(x => this.selectedEmpTypeList.includes(x.assignedToID) || x.assignedToID == null);
            }
            else {
                this.lstTicketsList = this.TicketsList.filter(x => this.selectedEmpTypeList.includes(x.assignedToID));
            }
        }
        else if (this.TicketStatus != "All" && this.TicketAssignedTo == "All") {
            this.lstTicketsList = this.TicketsList.filter(x => this.selectedStatusList.includes(x.statusID))
        } else {
            this.lstTicketsList = this.TicketsList;
        }
        //StateHasChanged();
    }

    LoadTickets() {
        this.ticketService.GetTicket().subscribe(respTicket => {
            if (respTicket['messageType'] !== null && respTicket['messageType'] === 1) {
                this.lstTicket = respTicket['data'];
            } else {
                this.lstTicket = [];
            }
        })
    }

    @ViewChild('AddEditTicketModal') AddEditTicketModal: AddEditTicketComponent;


    add = () => {
        this.AddEditTicketModal.Show(0, this.EmployeeID, 0);
    }


    edit = (selected: ITicketForDisplay) => {
        this.AddEditTicketModal.Show(selected.ticketID, this.EmployeeID, 0);
    }


    searchText: string;
    searchableList: any[] = ['ticketID', 'ticketType', 'requestedBy', 'assignedTo', 'status']

    OnChangeStatus(event: any) {
        if (event !== undefined && event.value !== undefined) {
            this.selectedStatusList = event.value;
            this.onStatusChange(event.value);
        }
    }

    OnChangeAssignedTo(event: any) {
        if (event !== undefined && event.value !== undefined) {
            this.selectedEmpTypeList = event.value;
            this.OnAssignedToChange(event.value);
        }
    }



}
