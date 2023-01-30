import { Component, OnInit, ViewChild } from '@angular/core';
import { ITableRowAction, ITableHeaderAction } from 'src/app/shared/ihr-table/table-options';
import { Constants, SessionConstants } from '../../constant';
import { IRolePermissionDisplay } from '../../core/interfaces/RolePermission';
import { AddEditUserComponent } from './add-edit-user/add-edit-user.component';
import { UserService } from './user.service';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})
export class UserComponent implements OnInit {
    @ViewChild('AddEditUserModal') addEditUserModal: AddEditUserComponent;
  cols: any[] = [];
  selectedColumns: any[];
  isPagination: boolean = true;
  loading: boolean = false;
  showCurrentPageReport: boolean = true;
  rowActions: ITableRowAction[] = [];
  headerActions: ITableHeaderAction[] = [];
  patientLanding: string = 'patientLanding';
  globalFilterFields = ['employeeCode', 'firstName', 'lastName', 'email', 'roleName', 'companyName', 'isOAuth','isActive']
  RolePermissions: IRolePermissionDisplay[] = [];

  UsersList: any[] = [];

    constructor(private userService: UserService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
    }

  ngOnInit(): void {
    this.cols = [
      { field: 'employeeCode', header: 'Emp Code' }, 
      { field: 'firstName', header: 'First Name' },
      { field: 'lastName', header: 'Last Name' },
      { field: 'email', header: 'Email' },
      { field: 'roleName', header: 'Role' },
      { field: 'companyName', header: 'Company' },
      { field: 'isOAuth', header: 'OAuth' },
      { field: 'isActive', header: 'Active' },
     ];

    this.selectedColumns = this.cols;
      this.LoadTableConfig();
    
    //this.rowActions = [
    //   {
    //    actionMethod: this.editUser,
    //    iconClass: 'pi pi-pencil'
    //  },
    //];
    //this.headerActions = [
    //  {
    //        actionMethod: this.add,
    //        hasIcon: false,
    //        styleClass: 'btn btn-block btn-sm btn-info',
    //        actionText: 'Add',
    //        iconClass: 'fas fa-plus'
        
    //  }
    //];
      this.LoadList();
    }
    Delete = () => {

    }
    rolePermission: IRolePermissionDisplay;
    LoadTableConfig() {
        this.rowActions = [];
        this.rolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.USER);
        if (this.rolePermission !== null && this.rolePermission !== undefined) {
            if (this.rolePermission.update === true) {
                this.rowActions.push(
                    {
                        actionMethod: this.editUser,
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

            }];
        } else {
            this.rowActions = [];
            this.headerActions = [];
        }
    }

    LoadList() {
        this.userService.getUserList().subscribe(result => {
            this.UsersList = result['data'];
        })
    }


  editUser = (selected) => {
      this.addEditUserModal.Show(selected.userID, 'Edit');
  }
  add = () => {
      this.addEditUserModal.Show(0, 'Add');
    }


    searchText: string;
    searchableList: any[] = ['employeeCode', 'firstName', 'lastName', 'email', 'roleName', 'companyName', 'isOAuth', 'isActive']
}
