import { Component, OnInit, ViewChild } from '@angular/core';
import { Constants, SessionConstants } from '../../constant';
import { IDropDown } from '../../core/interfaces/IDropDown';
import { IRoleDisplay } from '../../core/interfaces/Role';
import { IRolePermissionDisplay } from '../../core/interfaces/RolePermission';
import { ITableHeaderAction, ITableRowAction } from '../../shared/ihr-table/table-options';
import { AddEditRolepermissionComponent } from './add-edit-rolepermission/add-edit-rolepermission.component';
import { RolePermissionService } from './role-permission.service';

@Component({
  selector: 'app-role-permission',
  templateUrl: './role-permission.component.html',
  styleUrls: ['./role-permission.component.scss']
})
export class RolePermissionComponent implements OnInit {
    @ViewChild('AddEditRolePermisssion') addEditRolepermissionPopup: AddEditRolepermissionComponent;
    cols: any[] = [];
    selectedColumns: any[];
    isPagination: boolean = true;
    loading: boolean = false;
    showCurrentPageReport: boolean = true;
    rowActions: ITableRowAction[] = [];
    headerActions: ITableHeaderAction[] = [];
    patientLanding: string = 'patientLanding';
    globalFilterFields = ['roleName', 'moduleName', 'view', 'add', 'update', 'delete']
    RolePermissions: IRolePermissionDisplay[] = [];

    Roles: IRoleDisplay[] = [];
    constructor(private roleService: RolePermissionService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
    }

    ngOnInit(): void {
        this.cols = [
            { field: 'roleName', header: 'Role' },
            { field: 'moduleName', header: 'Module' },
            { field: 'view', header: 'View' },
            { field: 'add', header: 'Add' },
            { field: 'update', header: 'Update' },
            { field: 'delete', header: 'Delete' },
        ];

        this.selectedColumns = this.cols;
        this.LoadDropDown();
        this.LoadTableConfig();

    }

    Delete = () => {

    }
    rolePermission: IRolePermissionDisplay;
    LoadTableConfig() {
        this.rowActions = [];
        this.rolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.PERMISSION);
        if (this.rolePermission !== null && this.rolePermission !== undefined) {
            if (this.rolePermission.update === true) {
                this.rowActions.push(
                    {
                        actionMethod: this.editRole,
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
                    actionMethod: this.addRole,
                    hasIcon: false,
                    styleClass: 'btn btn-block btn-sm btn-info',
                    actionText: 'Add',
                    iconClass: 'fas fa-plus',
                    isDisabled: this.RoleId !== 0 ? false : true

                }
            ];
        } else {
            this.rowActions = [];
            this.headerActions = [];
        }

        //this.rowActions = [
        //    {
        //        actionMethod: this.editRole,
        //        iconClass: 'pi pi-pencil'
        //    },
        //];
        //this.headerActions = [
        //    {
        //        actionMethod: this.addRole,
        //        hasIcon: false,
        //        styleClass: 'btn btn-block btn-sm btn-info',
        //        actionText: 'Add',
        //        iconClass: 'fas fa-plus',
        //        isDisabled: this.RoleId !== 0 ? false : true

        //    }
        //];
    }

    LoadDropDown() {
        this.roleService.getRoles().subscribe(result => {
            if (result !== null && result['data'] !== null) {
                this.Roles = result['data'];
                this.SetRolesList();
            }
        });
    }

    ListItem: IDropDown;
    lstRoles: any[] = [];
    DefaultDwnDn1ID: number;
    RoleId: number = 0;
    RolePermissionsList: IRolePermissionDisplay[] = [];
    SetRolesList() {
        this.lstRoles = [];
        if (this.Roles != null) {
            this.Roles.forEach(x => {
                this.lstRoles.push({ ID: x.roleID, Value: x.roleName });
            })
            if (this.ListItem !== undefined) {
                this.ListItem.ID = 0;
                this.ListItem.Value = "Select";
                this.lstRoles.push(0, this.ListItem);
            }
            this.DefaultDwnDn1ID = 0;
        }
    }
    onChangeRole(ID: number) {
        this.DefaultDwnDn1ID = ID;
        this.LoadRolePermissionList(ID);
    }

    onChangeRoleMobile(event: any) {
        if (event !== undefined && event.value !== undefined) {
            this.onChangeRole(event.value)
        }
    }

    LoadRolePermissionList(ID: number) {
        this.RoleId = ID;
        this.LoadTableConfig();
        if (this.RoleId != 0) {
            this.roleService.getRoleByIdAsync(this.RoleId).subscribe(respRole => {
                if (respRole !== null && respRole !== undefined && respRole['data'] !== null && respRole['data'] !== undefined) {
                    var role = respRole['data'];
                    this.RolePermissionsList = role['rolePermissions'];
                } else {
                    this.RolePermissionsList = [];
                }
            })
        } else {
            this.RolePermissionsList = [];
        }
            
    }

    searchText: string;
    searchableList: any[] = ['roleName', 'moduleName', 'view', 'add', 'update', 'delete']

    addRole = () => {
        var rolePermission = this.Roles.find(x => x.roleID == this.RoleId).roleName;
        this.addEditRolepermissionPopup.Show(0, this.RoleId, rolePermission);
    }
    editRole = (selected: any) => {
        this.addEditRolepermissionPopup.Show(selected.rolePermissionID, this.RoleId, selected.roleName);
    }

}
