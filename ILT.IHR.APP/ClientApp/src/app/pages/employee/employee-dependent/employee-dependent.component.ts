import { Component, Input, OnInit, ViewChild } from '@angular/core';
import * as moment from 'moment';
import { Constants, SessionConstants } from '../../../constant';
import { IEmployeeDependentDisplay } from '../../../core/interfaces/EmployeeDependent';
import { IDirectDepositDisplay } from '../../../core/interfaces/EmployeeDirectDeposit';
import { IRolePermissionDisplay } from '../../../core/interfaces/RolePermission';
import { ITableHeaderAction, ITableRowAction } from '../../../shared/ihr-table/table-options';
import { EmployeeService } from '../employee.service';
import { AddEditDependentComponent } from './add-edit-dependent/add-edit-dependent.component';

@Component({
  selector: 'app-employee-dependent',
  templateUrl: './employee-dependent.component.html',
  styleUrls: ['./employee-dependent.component.scss']
})
export class EmployeeDependentComponent implements OnInit {
    @ViewChild('addEditDependentModal') addEditDependentModal: AddEditDependentComponent;
    @Input() EmployeeID: number;
    @Input() Name: string;
    cols: any[] = [];
    selectedColumns: any[];
    isPagination: boolean = true;
    loading: boolean = false;
    showCurrentPageReport: boolean = true;
    rowActions: ITableRowAction[] = [];
    headerActions: ITableHeaderAction[] = [];
    globalFilterFields = ['employeeName', 'relation', 'birthDate', 'visaType']
    patientLanding: string = 'patientLanding';

    RolePermissions: IRolePermissionDisplay[] = [];
    EmployeeInfoRolePermission: IRolePermissionDisplay;

    dependentsList: IEmployeeDependentDisplay[] = [];
    lstdependentsList: IEmployeeDependentDisplay[] = [];


    constructor(private employeeService: EmployeeService) {
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
            { field: 'employeeName', header: 'Name' },
            { field: 'relation', header: 'Relation' },
            { field: 'birthDate', header: 'Birth Date' },
            { field: 'visaType', header: 'Visa Type' },
        ];

        this.selectedColumns = this.cols;
    }
    LoadList() {
        this.LoadTableConfig();
        if (this.EmployeeID !== undefined && this.EmployeeID !== 0) {
            this.employeeService.getEmployeeByIdAsync(this.EmployeeID).subscribe(result => {
                if (result !== undefined && result['data'] !== null && result['data'] !== undefined && result['data'].dependents.length > 0) {
                    result['data'].dependents.forEach((d) => {
                        d.birthDate = moment(d.birthDate).format("MM/DD/YYYY")
                        d['employeeName'] = d.firstName + ' ' + d.middleName + ' ' + d.lastName
                    })
                    this.dependentsList = result['data'].dependents;
                    this.lstdependentsList = this.dependentsList;
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
        this.rolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.EMPLOYEEINFO);
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
        this.addEditDependentModal.Show(selectedContact.dependentID, this.EmployeeID)
    }

    add = () => {
        this.addEditDependentModal.Show(0, this.EmployeeID)
    }

    //Mobiledropdown changes method
    searchText: string;
    searchableList: any[] = ['employeeName', 'relation', 'birthDate', 'visaType']

}
