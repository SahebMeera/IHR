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
import { ExpenseService } from './expense.service';
import { AddEditExpensesComponent } from './add-edit-expenses/add-edit-expenses.component';

@Component({
  selector: 'app-expenses',
  templateUrl: './expenses.component.html',
  styleUrls: ['./expenses.component.scss']
})
export class ExpensesComponent implements OnInit {
    @ViewChild('addEditExpenseModal') AddEditExpenseModal: AddEditExpensesComponent;
    commonUtils = new CommonUtils();
    cols: any[] = [];
    selectedColumns: any[];
    isPagination: boolean = true;
    loading: boolean = false;
    showCurrentPageReport: boolean = true;
    rowActions: ITableRowAction[] = [];
    headerActions: ITableHeaderAction[] = [];
    patientLanding: string = 'patientLanding';
    globalFilterFields = ['employeeName', 'expenseType', 'amount', 'submissionDate', 'amountPaid', 'paymentDate', 'status']

    lstExpense: any[] = [];
    lstExpensesList: any[] = [];
    //table dropdown
    lstStatus: any[] = [];
    status: string;
    DefaultDwnDn1ID: number;
    RoleShort: string;

    RolePermissions: IRolePermissionDisplay[] = []
    user: any;

    constructor(private fb: FormBuilder, private dataProvider: DataProvider,
        private expenseService: ExpenseService,
        private LookupService: LookUpService,
        private router: Router) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.user = JSON.parse(localStorage.getItem("User"));
        this.RoleShort = localStorage.getItem("RoleShort");
        this.LoadDropDown();
        this.loadTableColumns();
        this.LoadTableConfig();
    }

    ngOnInit(): void {
        this.LoadExpenses();
    }

    loadTableColumns() {
        this.cols = [
            { field: 'employeeName', header: 'Employee Name' },
            { field: 'expenseType', header: 'Expense Type' },
            { field: 'amount', header: 'Amount' },
            { field: 'submissionDate', header: 'Submission Date' },
            { field: 'amountPaid', header: 'Amount Paid' },
            { field: 'paymentDate', header: 'Payment Date' },
            { field: 'status', header: 'Staus' },
        ];

        this.selectedColumns = this.cols;
    }

    Delete = () => {

    }
    rolePermission: IRolePermissionDisplay;
    LoadTableConfig() {
        this.rowActions = [];
        this.rolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.EXPENSES);
        if (this.rolePermission !== null && this.rolePermission !== undefined) {
            if (this.rolePermission.update === true) {
                this.rowActions.push(
                    {
                        actionMethod: this.editExpense,
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

            this.headerActions = [
                {
                    actionMethod: this.add,
                    hasIcon: false,
                    styleClass: 'btn btn-block btn-sm btn-info',
                    actionText: 'Add',
                    iconClass: 'fas fa-plus'

                }
            ];
        } else {
            this.rowActions = [];
            this.headerActions = [];
        }
    }


    //LoadTableConfig() {
    //    this.rowActions = [
    //        {
    //            actionMethod: this.editExpense,
    //            iconClass: 'pi pi-pencil'
    //        }
    //    ];
    //    this.headerActions = [
    //        {
    //            actionMethod: this.add,
    //            hasIcon: false,
    //            styleClass: 'btn btn-block btn-sm btn-info',
    //            actionText: 'Add',
    //            iconClass: 'fas fa-plus'

    //        }
    //    ];
    //}
  

    DefaultStatusID: number;
    LoadDropDown() {
        this.LookupService.getListValues().subscribe(result => {
            if (result['data'] !== undefined && result['data'] !== null) {
                this.lstStatus = result['data'].filter(x => x.type.toUpperCase() === ListTypeConstants.ExpenseStatus);
                var submittedStatusID = this.lstStatus.find(x => x.value.toUpperCase() === "Submitted".toUpperCase()).listValueID;
               // this.DefaultStatusID = submittedStatusID;
                this.setStatusList();
            }
        })
    }
    ListItem: IDropDown;
    statusList: any[] = [];
    setStatusList() {
        this.statusList = [];
        if (this.lstStatus != null) {
            this.statusList.push({ ID: 0, Value: 'All' })
            this.lstStatus.forEach(x => {
                this.statusList.push({ ID: x.listValueID, Value: x.valueDesc });
            })
            var submittedStatusID = this.lstStatus.find(x => x.value.toUpperCase() === "Submitted".toUpperCase()).listValueID;
            this.DefaultStatusID = submittedStatusID;
            this.DefaultDwnDn1ID = submittedStatusID;
            this.LoadExpenses();
        }
    }
    OnStatusChange(ID: any) {
        this.DefaultDwnDn1ID = ID;
        this.LoadExpensesByStatus(ID);
    }

    LoadExpenses() {
        this.expenseService.GetExpense().subscribe(respExpense => {
            if (respExpense['messageType'] !== null && respExpense['messageType'] === 1) {
                if (respExpense['data'] != null && (this.RoleShort.toUpperCase() == UserRole.EMP || this.RoleShort.toUpperCase() == UserRole.CONTRACTOR)) {
                    respExpense['data'].forEach(d => {
                        if (d.submissionDate !== null) {
                            d.submissionDate = moment(d.submissionDate).format("MM/DD/YYYY")
                        }
                        if (d.paymentDate !== null) {
                            d.paymentDate = moment(d.paymentDate).format("MM/DD/YYYY")
                        }
                        if (d.amount !== null) {
                            d.amount = Number(d.amount).toFixed(2)
                        }
                    })
                    this.lstExpense = respExpense['data'].filter(x => x.employeeID == this.user.employeeID);
                   // this.lstExpense = respExpense['data'];
                    this.LoadExpensesByStatus(this.DefaultStatusID);
                } else {
                    respExpense['data'].forEach(d => {
                        if (d.submissionDate !== null) {
                            d.submissionDate = moment(d.submissionDate).format("MM/DD/YYYY")
                        }
                        if (d.paymentDate !== null) {
                            d.paymentDate = moment(d.paymentDate).format("MM/DD/YYYY")
                        }
                        if (d.amount !== null) {
                            d.amount = Number(d.amount).toFixed(2)
                        }
                    })
                    this.lstExpense = respExpense['data'];
                    this.LoadExpensesByStatus(this.DefaultStatusID);
                }
            } else {
                this.lstExpense = [];
            }
        })
    }

    LoadExpensesByStatus(statusID: number) {
        if (statusID !== 0) {
            this.lstExpensesList = this.lstExpense.filter(x => x.statusID === statusID);
        }
        else {
            this.lstExpensesList = this.lstExpense;
        }
    }

    

    add = () => {
        this.AddEditExpenseModal.Show(0);
    }


    editExpense = (selected: IExpenseForDisplay) => {
        this.AddEditExpenseModal.Show(selected.expenseID);
    }


    LoadList() {

    }

    searchText: string;
    searchableList: any[] = ['employeeName', 'expenseType', 'amount', 'submissionDate', 'amountPaid', 'paymentDate', 'status']

    onChangeStatus(event: any) {
        if (event !== undefined && event.value !== undefined) {
            this.OnStatusChange(event.value)
        }
    }


}
