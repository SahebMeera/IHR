import { Component, Input, OnInit, ViewChild } from '@angular/core';
import * as moment from 'moment';
import { Constants, SessionConstants } from '../../../constant';
import { IRolePermissionDisplay } from '../../../core/interfaces/RolePermission';
import { ITableHeaderAction, ITableRowAction } from '../../../shared/ihr-table/table-options';
import { EmployeeService } from '../employee.service';
import { AddEditFormi9Component } from './add-edit-formi9/add-edit-formi9.component';
import { FormI9Service } from './formI9.service';
import { I9formChangesetComponent } from './i9form-changeset/i9form-changeset.component';

@Component({
  selector: 'app-form-i9',
  templateUrl: './form-i9.component.html',
  styleUrls: ['./form-i9.component.scss']
})
export class FormI9Component implements OnInit {
    @ViewChild('addEditEmployeeFormI9Modal') addEditEmployeeFormI9Modal: AddEditFormi9Component;
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
    globalFilterFields = ['employeeName', 'workAuthorization', 'i94ExpiryDate', 'listADocumentTitle', 'listAExpirationDate', 'listBDocumentTitle', 'listBExpirationDate', 'listCDocumentTitle','listCExpirationDate']


    RolePermissions: IRolePermissionDisplay[] = [];
    EmployeeInfoRolePermission: IRolePermissionDisplay;

    constructor(private employeeService: EmployeeService,
        private formI9Service: FormI9Service) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.EmployeeInfoRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.I9INFO);
    }

    ngOnInit(): void {
        this.LoadList();
        this.loadTableColumns();
    }
    loadTableColumns() {
        this.cols = [
            { field: 'employeeName', header: 'Employee Name' },
            { field: 'workAuthorization', header: 'Work Auth' },
            { header: 'I94 Expiry', field: 'i94ExpiryDate' },
            { header: 'List A Document', field: 'listADocumentTitle' },
            { header: 'List A Expiry', field: 'listAExpirationDate' },
            { header: 'List B Document', field: 'listBDocumentTitle' },
            { header: 'List B Expiry', field: 'listBExpirationDate' },
            { header: 'List C Document', field: 'listCDocumentTitle' },
            { header: 'List C Expiry', field: 'listCExpirationDate' },
        ];

        this.selectedColumns = this.cols;
    }
    formI9List: any[] = [];
    lstFormI9List: any[] = [];
    LoadList() {
        this.LoadTableConfig();
        if (this.EmployeeID !== undefined && this.EmployeeID !== 0) {
            this.formI9Service.getFormI9s(this.EmployeeID).subscribe(result => {
                if (result !== undefined && result['data'] !== null && result['data'] !== undefined && result['data'].length > 0) {
                    result['data'].forEach((d) => {
                        if (d.i94ExpiryDate !== null) {
                            d.i94ExpiryDate = moment(d.i94ExpiryDate).format("MM/DD/YYYY")
                        }
                        if (d.listAExpirationDate !== null) {
                            d.listAExpirationDate = moment(d.listAExpirationDate).format("MM/DD/YYYY")
                        }
                        if (d.listBExpirationDate !== null) {
                            d.listBExpirationDate = moment(d.listBExpirationDate).format("MM/DD/YYYY")
                        }
                         if (d.listCExpirationDate !== null) {
                            d.listCExpirationDate = moment(d.listCExpirationDate).format("MM/DD/YYYY")
                        }
                    })
                    console
                    this.formI9List = result['data'];
                    this.lstFormI9List = this.formI9List;
                }
            }, error => {
                //toastService.ShowError();
            })
        }
    }
    rolePermission: IRolePermissionDisplay;
    LoadTableConfig() {
        this.rolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.W4INFO);
        if (this.rolePermission !== null && this.rolePermission !== undefined) {
            if (this.rolePermission.update === true) {
                this.rowActions = [
                    {
                        actionMethod: this.editI9,
                        iconClass: 'pi pi-pencil'
                    },
                    {
                        actionMethod: this.i9ChangeSets,
                        iconClass: 'fa fa-retweet'
                    },
                ];
            } else {
                this.rowActions = [];
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

    editI9 = (selectedContact) => {
        this.addEditEmployeeFormI9Modal.Show(selectedContact.formI9ID, this.EmployeeID)
    }

    add = () => {
        this.addEditEmployeeFormI9Modal.Show(0, this.EmployeeID)
    }
    @ViewChild('notificationModal') notificationModal: I9formChangesetComponent;

    i9ChangeSets = (selectedContact) => {
        this.notificationModal.show(selectedContact.formI9ID);
    }

    //Mobiledropdown changes method
    searchText: string;
    searchableList: any[] = ['employeeName', 'workAuthorization', 'i94ExpiryDate', 'listADocumentTitle', 'listAExpirationDate', 'listBDocumentTitle', 'listBExpirationDate', 'listCDocumentTitle', 'listCExpirationDate']

    


}
