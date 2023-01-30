import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { Constants, SessionConstants } from '../../../constant';
import { IEmergnecyContactDisplay } from '../../../core/interfaces/EmergencyContact';
import { IRolePermissionDisplay } from '../../../core/interfaces/RolePermission';
import { ITableHeaderAction, ITableRowAction } from '../../../shared/ihr-table/table-options';
import { EmployeeService } from '../employee.service';
import { AddEditContactComponent } from './add-edit-contact/add-edit-contact.component';
import { EmerygencyService } from './emerygency.service';

@Component({
  selector: 'app-employee-emergency',
  templateUrl: './employee-emergency.component.html',
  styleUrls: ['./employee-emergency.component.scss']
})
export class EmployeeEmergencyComponent implements OnInit {
    @ViewChild('addEditContactModal') addEditContactModal: AddEditContactComponent;
    @Input() EmployeeID: number;
    @Input() Name: string;
    cols: any[] = [];
    selectedColumns: any[];
    isPagination: boolean = true;
    loading: boolean = false;
    showCurrentPageReport: boolean = true;
    rowActions: ITableRowAction[] = [];
    headerActions: ITableHeaderAction[] = [];
    globalFilterFields = ['contactType', 'firstName', 'lastName', 'phone','email']

    patientLanding: string = 'patientLanding';

    RolePermissions: IRolePermissionDisplay[] = [];

    ContactsList: IEmergnecyContactDisplay[] = [];
    lstContactsList: IEmergnecyContactDisplay[] = [];
    yearId: number

    constructor(private emerygencyService: EmerygencyService, private employeeService: EmployeeService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));    }

    ngOnInit(): void {
        this.LoadList();
        this.loadTableColumns();
    }
    loadTableColumns() {
        this.cols = [
            { field: 'contactType', header: 'Contact Type' },
            { field: 'firstName', header: 'First Name' },
            { field: 'lastName', header: 'Last Name' },
            { field: 'phone', header: 'Phone' },
            { field: 'email', header: 'Email' },
        ];

        this.selectedColumns = this.cols;
    }
    LoadList() {
        this.LoadTableConfig();
        if (this.EmployeeID !== undefined && this.EmployeeID !== 0) {
            this.employeeService.getEmployeeByIdAsync(this.EmployeeID).subscribe(result => {
                if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                    this.ContactsList = result['data'].contacts;
                    this.lstContactsList = this.ContactsList
                }
            }, error => {
                //toastService.ShowError();
            })
        }
    }
    Delete = () => {

    }
    ContactRolePermission: IRolePermissionDisplay;
    LoadTableConfig() {
        this.rowActions = [];
        this.ContactRolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.EMPLOYEEINFO);
        if (this.ContactRolePermission !== null && this.ContactRolePermission !== undefined) {
            if (this.ContactRolePermission.update === true) {
                this.rowActions.push(
                    {
                        actionMethod: this.editContact,
                        iconClass: 'pi pi-pencil'
                    })
            }
            if (this.ContactRolePermission.delete === true) {
                this.rowActions.push(
                    {
                        actionMethod: this.Delete,
                        styleClass: 'p-button-raised p-button-danger',
                        iconClass: 'pi pi-trash'
                    })
            }
            if (this.ContactRolePermission.add === true) {
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
        console.log('selectedContact')
        this.addEditContactModal.Show(selectedContact.contactID, this.EmployeeID)
    }

    add = () => {
        this.addEditContactModal.Show(0, this.EmployeeID)
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
