import { Component, Input, OnInit, ViewChild } from '@angular/core';
import * as moment from 'moment';
import { Constants, SessionConstants } from '../../../constant';
import { ISalaryDisplay } from '../../../core/interfaces/EmployeeSalary';
import { IRolePermissionDisplay } from '../../../core/interfaces/RolePermission';
import { ITableHeaderAction, ITableRowAction } from '../../../shared/ihr-table/table-options';
import { EmployeeService } from '../employee.service';
import { AddEditSalaryComponent } from './add-edit-salary/add-edit-salary.component';
import { EmployeeSalaryService } from './employee-salary.service';

@Component({
  selector: 'app-employee-salary',
  templateUrl: './employee-salary.component.html',
  styleUrls: ['./employee-salary.component.scss']
})
export class EmployeeSalaryComponent implements OnInit {
    @ViewChild('addEditEmpoyeeSalaryModal') addEditEmpoyeeSalaryModal: AddEditSalaryComponent;
    @Input() EmployeeID: number;
    @Input() Name: string;
    cols: any[] = [];
    selectedColumns: any[];
    isPagination: boolean = true;
    loading: boolean = false;
    showCurrentPageReport: boolean = true;
    rowActions: ITableRowAction[] = [];
    headerActions: ITableHeaderAction[] = [];
    globalFilterFields = ['startDate', 'endDate', 'basicPay', 'hra', 'lta', 'educationAllowance', 'specialAllowance', 'bonus', 'variablePay', 'providentFund','costToCompany']
    patientLanding: string = 'patientLanding';

    RolePermissions: IRolePermissionDisplay[] = [];
    EmployeeInfoRolePermission: IRolePermissionDisplay;

    constructor(private employeeService: EmployeeService,
        private skillService: EmployeeSalaryService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.EmployeeInfoRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.SKILL);
    }

    ngOnInit(): void {
        this.LoadList();
        this.loadTableColumns();
    }
    loadTableColumns() {
        this.cols = [
            { field: 'startDate', header: 'Start Date' },
            { field: 'endDate', header: 'End Date' },
            { field: 'basicPay', header: 'Basic Pay' },
            { field: 'hra', header: 'HRA' },
            { field: 'lta', header: 'LTA' },
            { field: 'educationAllowance', header: 'Education Allowance' },
            { field: 'specialAllowance', header: 'Special Allowance' },
            { field: 'bonus', header: 'Bonus' },
            { field: 'variablePay', header: 'Variable Pay' },
            { field: 'providentFund', header: 'Provident Fund' },
            { field: 'costToCompany', header: 'CTC' },
        ];

        this.selectedColumns = this.cols;
    }
    salaryList: ISalaryDisplay[] = [];
    lstSalaryList: ISalaryDisplay[] = [];

    LoadList() {
        this.LoadTableConfig();
        if (this.EmployeeID !== undefined && this.EmployeeID !== 0) {
            this.employeeService.getEmployeeByIdAsync(this.EmployeeID).subscribe(result => {
                if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                    result['data'].salaries.forEach((d) => {
                        d.startDate = moment(d.startDate).format("MM/DD/YYYY")
                        if (d.endDate !== null) {
                            d.endDate = moment(d.endDate).format("MM/DD/YYYY")
                        }
                        d['employeeName'] = d.firstName + ' ' + d.middleName + ' ' + d.lastName
                    })
                    this.salaryList = result['data'].salaries;
                    this.lstSalaryList = this.salaryList
                }
            }, error => {
                //toastService.ShowError();
            })
        }
    }
    Delete = () => {

    }
    SalaryRolePermission: IRolePermissionDisplay;
    LoadTableConfig() {
        this.rowActions = [];
        this.SalaryRolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.SALARY);
        if (this.SalaryRolePermission !== null && this.SalaryRolePermission !== undefined) {
            if (this.SalaryRolePermission.update === true) {
                this.rowActions.push(
                    {
                        actionMethod: this.edit,
                        iconClass: 'pi pi-pencil'
                    })
            }
            if (this.SalaryRolePermission.delete === true) {
                this.rowActions.push(
                    {
                        actionMethod: this.Delete,
                        styleClass: 'p-button-raised p-button-danger',
                        iconClass: 'pi pi-trash'
                    })
            }
            if (this.SalaryRolePermission.add === true) {
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

    edit = (selectedContact) => {
        this.addEditEmpoyeeSalaryModal.Show(selectedContact.salaryID, this.EmployeeID)
    }

    add = () => {
        console.log('Here')
        this.addEditEmpoyeeSalaryModal.Show(0, this.EmployeeID)
    }

    //Mobiledropdown changes method
    searchText: string;
    searchableList: any[] = ['startDate', 'endDate', 'variablePay', 'costToCompany']
}
