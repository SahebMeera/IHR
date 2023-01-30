import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { IRolePermissionDisplay } from '../../../core/interfaces/RolePermission';
import { ITableHeaderAction, ITableRowAction } from '../../../shared/ihr-table/table-options';
import { IEmployeeW4Display } from '../../../core/interfaces/Employeew4'
import { EmployeeService } from '../employee.service';
import { Constants, SessionConstants } from '../../../constant';
import * as moment from 'moment';
import { EmployeeW4Service } from './w4.service';
import { AddEditEmployeew4Component } from './add-edit-employeew4/add-edit-employeew4.component';

@Component({
  selector: 'app-employee-w4',
  templateUrl: './employee-w4.component.html',
  styleUrls: ['./employee-w4.component.scss']
})
export class EmployeeW4Component implements OnInit {
    @ViewChild('addEditEmployeeW4Modal') addEditEmployeeW4Modal: AddEditEmployeew4Component;
    @Input() EmployeeID: number;
    @Input() SSN: string;
    @Input() Name: string;
    cols: any[] = [];
    selectedColumns: any[];
    isPagination: boolean = true;
    loading: boolean = false;
    showCurrentPageReport: boolean = true;
    rowActions: ITableRowAction[] = [];
    headerActions: ITableHeaderAction[] = [];
    patientLanding: string = 'patientLanding';
    globalFilterFields = ['employeeName', 'w4Type', 'withHoldingStatus', 'ssn', 'allowances', 'startDate','endDate']



    RolePermissions: IRolePermissionDisplay[] = [];
    EmployeeInfoRolePermission: IRolePermissionDisplay;

    //dependentsList: IW4EmployeeDisplay[] = [];
    lstEmployeeW4List: IEmployeeW4Display[] = [];
    employeeW4List: IEmployeeW4Display[] = [];


    constructor(private employeeService: EmployeeService,
        private employeeW4Service: EmployeeW4Service) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.EmployeeInfoRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.EMPLOYEEINFO);
    }
    ngOnInit(): void {
        this.LoadList();
        this.loadTableColumns();
    }
    loadTableColumns() {
        this.cols = [
            { field: 'employeeName', header: 'EmployeeName' },
            { field: 'w4Type', header: 'W4 Type' },
            { field: 'withHoldingStatus', header: 'WithHolding Status' },
            { field: 'ssn', header: 'SSN' },
            { field: 'allowances', header: 'Allowances' },
            { field: 'startDate', header: 'StartDate' },
            { field: 'endDate', header: 'EndDate' },
        ];

        this.selectedColumns = this.cols;
    }
    LoadList() {
        this.LoadTableConfig();
        if (this.EmployeeID !== undefined && this.EmployeeID !== 0) {
            this.employeeW4Service.getEmployeeW4s(this.EmployeeID).subscribe(result => {
                if (result !== undefined && result['data'] !== null && result['data'] !== undefined && result['data'].length > 0) {
                    result['data'].forEach((d) => {
                        d.startDate = moment(d.startDate).format("MM/DD/YYYY")
                        if (d.endDate !== null) {
                            d.endDate = moment(d.endDate).format("MM/DD/YYYY")
                        }
                    })
                    this.employeeW4List = result['data'];
                    this.lstEmployeeW4List = this.employeeW4List;
                }
            }, error => {
                //toastService.ShowError();
            })
        }
    }
    Delete = () => {

    }
    rolePermission: IRolePermissionDisplay;
    LoadTableConfig() {
        this.rowActions = [];
        this.rolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.W4INFO);
        if (this.rolePermission !== null && this.rolePermission !== undefined) {
            if (this.rolePermission.update === true) {
                this.rowActions.push(
                    {
                        actionMethod: this.editContact,
                        iconClass: 'pi pi-pencil'
                    })
            }
            if (this.rolePermission.delete === true) {
                this.rowActions.push(
                    {
                        actionMethod: this.Delete,
                        styleClass: 'p-button-raised p-button-danger',
                        iconClass: 'pi pi-trash'
                    })
               
            }
            if (this.rolePermission.add === true) {
                this.headerActions = [
                    {
                        actionMethod: this.add,
                        hasIcon: false,
                        styleClass: 'btn-width-height btn btn-block btn-sm btn-info',
                        actionText: 'Add',
                        iconClass: 'fas fa-plus'
                    }
                ];
            } else {
                this.headerActions = [];
            }
        } else {
            this.rowActions = [];
            this.headerActions = [];
        }
    }

    editContact = (selectedContact) => {
        this.addEditEmployeeW4Modal.Show(selectedContact.employeeW4ID, this.EmployeeID, this.SSN)
    }

    add = () => {
        this.addEditEmployeeW4Modal.Show(0, this.EmployeeID, this.SSN)
    }

    isSearchRequired: boolean = false;
    isRowExpand: boolean = false;
    defaultPageSize: number = 15;

    isOpen(data: any) {
        setTimeout(() => this.isRowExpand = true);
    }
    //Mobiledropdown changes method
    searchText: string;
    searchableList: any[] = ['contactType', 'firstName', 'lastName', 'phone', 'email']

}
