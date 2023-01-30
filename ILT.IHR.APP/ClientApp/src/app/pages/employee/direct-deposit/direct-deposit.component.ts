import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { Constants, SessionConstants } from '../../../constant';
import { IDirectDepositDisplay } from '../../../core/interfaces/EmployeeDirectDeposit';
import { IRolePermissionDisplay } from '../../../core/interfaces/RolePermission';
import { ITableHeaderAction, ITableRowAction } from '../../../shared/ihr-table/table-options';
import { EmployeeService } from '../employee.service';
import { AddEditDirectDepositComponent } from './add-edit-direct-deposit/add-edit-direct-deposit.component';

@Component({
  selector: 'app-direct-deposit',
  templateUrl: './direct-deposit.component.html',
  styleUrls: ['./direct-deposit.component.scss']
})
export class DirectDepositComponent implements OnInit {
    @ViewChild('addEditDirectDepositModal') addEditDirectDepositModal: AddEditDirectDepositComponent;
    @Input() EmployeeID: number;
    @Input() Name: string;
    cols: any[] = [];
    selectedColumns: any[];
    isPagination: boolean = true;
    loading: boolean = false;
    showCurrentPageReport: boolean = true;
    rowActions: ITableRowAction[] = [];
    headerActions: ITableHeaderAction[] = [];
    patientLanding: string = 'patientLanding';
    globalFilterFields = ['bankName', 'accountType', 'routingNumber', 'accountNumber', 'country', 'state', 'amount']


    RolePermissions: IRolePermissionDisplay[] = [];
    EmployeeInfoRolePermission: IRolePermissionDisplay;

    directDepositsList: IDirectDepositDisplay[] = [];
    lstDirectDepositsList: IDirectDepositDisplay[] = [];
 
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
            { field: 'bankName', header: 'Bank Name' },
            { field: 'accountType', header: 'Account Type' },
            { field: 'routingNumber', header: 'Routing Number' },
            { field: 'accountNumber', header: 'Account Number' },
            { field: 'country', header: 'Country' },
            { field: 'state', header: 'State' },
            { field: 'amount', header: 'Amount' },
        ];

        this.selectedColumns = this.cols;
    }
    LoadList() {
        this.LoadTableConfig();
        if (this.EmployeeID !== undefined && this.EmployeeID !== 0) {
            this.employeeService.getEmployeeByIdAsync(this.EmployeeID).subscribe(result => {
                if (result !== undefined && result['data'] !== null && result['data'] !== undefined && result['data'].directDeposits.length > 0) {
                    this.directDepositsList = result['data'].directDeposits;
                    this.lstDirectDepositsList = this.directDepositsList;
                }
            }, error => {
                //toastService.ShowError();
            })
        }
    }
    Delete = () => {

    }
    DirectDepositRolePermission: IRolePermissionDisplay;
    LoadTableConfig() {
        this.rowActions = [];
        this.DirectDepositRolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.EMPLOYEEINFO);
        if (this.DirectDepositRolePermission !== null && this.DirectDepositRolePermission !== undefined) {
            if (this.DirectDepositRolePermission.update === true) {
                this.rowActions.push(
                    {
                        actionMethod: this.editContact,
                        iconClass: 'pi pi-pencil'
                    })
            }
            if (this.DirectDepositRolePermission.delete === true) {
                this.rowActions.push(
                    {
                        actionMethod: this.Delete,
                        styleClass: 'p-button-raised p-button-danger',
                        iconClass: 'pi pi-trash'
                    })
            }
            if (this.DirectDepositRolePermission.add === true) {
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
        this.addEditDirectDepositModal.Show(selectedContact.directDepositID, this.EmployeeID)
    }

    add = () => {
        this.addEditDirectDepositModal.Show(0, this.EmployeeID)
    }

    //Mobiledropdown changes method
    searchText: string;
    searchableList: any[] = ['bankName', 'accountType', 'routingNumber', 'accountNumber']

}
