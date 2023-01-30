import { ElementRef, EventEmitter, Output, ViewChild } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { forkJoin } from 'rxjs';
import { CommonUtils } from '../../../common/common-utils';
import { Constants, EmailApprovalUrl, ErrorMsg, SessionConstants } from '../../../constant';
import { ICompanyDisplay } from '../../../core/interfaces/company';
import { IEmployeeDisplay } from '../../../core/interfaces/Employee';
import { IRoleDisplay } from '../../../core/interfaces/Role';
import { IRolePermissionDisplay } from '../../../core/interfaces/RolePermission';
import { IUserRole, IUserRoleDisplay } from '../../../core/interfaces/UserRole';
import { Country } from '../../../demo/domain/customer';
import { IModalPopupAlternateOptions } from '../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { CompanyService } from '../../company/company.service';
import { EmployeeService } from '../../employee/employee.service';
import { RolePermissionService } from '../../role-permission/role-permission.service';
import { UserService } from '../user.service';
import { EmailFields } from '../../../core/interfaces/Common';
import { Dropdown } from 'primeng/dropdown/dropdown';
import { Renderer2 } from '@angular/core';


@Component({
    selector: 'app-add-edit-user',
    templateUrl: './add-edit-user.component.html',
    styleUrls: ['./add-edit-user.component.scss']
})
export class AddEditUserComponent implements OnInit {
    @ViewChild('addEditUserModal') addEditUserModal: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;
    ModalHeading: string = 'Add User';
    commonUtils = new CommonUtils()
    @Output() ListValueUpdated = new EventEmitter<any>();

    UserForm: FormGroup;
    user: any;
    loggedinuser: any;
    UserId: number;
    submitted: boolean
    @ViewChild('roleDropdown') roleDropdown: ElementRef;
    RolePermissions: IRolePermissionDisplay[] = [];
    constructor(private userService: UserService,
        private employeeService: EmployeeService,
        private roleService: RolePermissionService,
        private messageService: MessageService,
        private fb: FormBuilder,
        private companyService: CompanyService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.loggedinuser = JSON.parse(localStorage.getItem("User"));
        this.LoadDropDown();
        this.buildUserForm({}, 'New')


    }
    countries: any[];

    selectedCountries: any[] = [];

    ngOnInit(): void {

        this.countries = [
            { name: 'Australia', code: 'AU', IsDefault: true },
            { name: 'Brazil', code: 'BR', IsDefault: false },
            { name: 'China', code: 'CN', IsDefault: false },
            { name: 'Egypt', code: 'EG', IsDefault: false },
            { name: 'France', code: 'FR', IsDefault: false },
            { name: 'Germany', code: 'DE', IsDefault: false },
            { name: 'India', code: 'IN', IsDefault: false },
            { name: 'Japan', code: 'JP', IsDefault: false },
            { name: 'Spain', code: 'ES', IsDefault: false },
            { name: 'United States', code: 'US', IsDefault: false }
        ];
        this.modalOptions = {
            footerActions: [
                {
                    actionText: 'Save',
                    actionMethod: this.saveUser,
                    styleClass: 'p-button-raised p-mr-2 p-mb-2',
                    iconClass: 'mdi mdi-content-save',
                    disabled: this.isSaveButShowHide
                },
                {
                    actionText: 'Cancel',
                    actionMethod: this.cancel,
                    styleClass: 'p-button-raised p-button-danger  p-mb-2',
                    iconClass: 'p-button-raised p-button-danger'
                }
            ]
        }
    }

    EmployeeList: IEmployeeDisplay[] = [];
    moduleList: any[] = [];
    StatusList: any[] = [];
    VacationTypeList: any[] = [];
    Roles: IRoleDisplay[] = [];
    CompanyList: any[] = []
    UsersList: any[] = []
    LoadDropDown() {
        forkJoin(
            this.employeeService.GetEmployees(),
            this.roleService.getRoles(),
            this.companyService.getCompanyList(),
            this.userService.getUserList()
        ).subscribe(resultSet => {
            if (resultSet !== undefined && resultSet !== null) {
                // this.EmployeeList = resultSet[0]['data']
                this.Roles = resultSet[1]['data']
                this.CompanyList = resultSet[2]['data'];
                this.UsersList = resultSet[3]['data'];
                this.EmployeeList = resultSet[0]['data'] !== null && resultSet[0]['data'].length > 0 ? resultSet[0]['data'].filter(x => this.commonUtils.defaultDateTimeLocalSet(new Date()) <= this.commonUtils.defaultDateTimeLocalSet(x.termDate) || x.termDate === null) : []
                this.SetNewUserRoleList();
            }
        })
    }
    LoginMessage: string = '';
    isSaveButShowHide: boolean = false;
    isRoleUpdated: boolean = false;
    disabledvalue: boolean = false;
    Show(Id: number, profiletype: string) {
        this.isRoleUpdated = false;
        this.isSaveButShowHide = false;
        this.LoginMessage = "";
        this.UserId = Id;
        //this.ResetDialog();
        if (this.UserId !== 0) {
            this.GetDetails(this.UserId, profiletype);
        }
        else {
            this.disabledvalue = false;
            this.ModalHeading = "Add User";

            this.buildUserForm({}, 'New')
            this.UserForm.controls.Email.patchValue(this.loggedinuser.email)
            this.UserForm.controls.NewPassword.patchValue('')
            this.addEditUserModal.show();
            this.UserForm.controls.NewPassword.patchValue('')
            // 
            // StateHasChanged();
        }

    }
    empnonempdisabledvalue: boolean = false;
    sameUserIsActive: boolean = false;
    GetDetails(Id: number, profiletype: string) {
        this.sameUserIsActive = false;
        this.isSaveButShowHide = false;
        this.LoginMessage = '';
        this.userService.GetUserByIdAsync(Id).subscribe(async result => {
            if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                this.user = result['data']
                this.UserId = this.user.userID;
                this.buildUserForm(this.user, 'Edit')
                if (this.user.userRoles !== null) {
                    await this.UpdateSelectedList();
                }
                if (profiletype == "Edit") {
                    this.empnonempdisabledvalue = false;
                    if (this.user.employeeID !== null) {
                        this.disabledvalue = true;
                    }
                    else {
                        this.disabledvalue = false;
                    }
                } else if (profiletype == "Myprofile") {
                    var UserRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.USER);
                    if (UserRolePermission != null) {
                        if (UserRolePermission.update == false) {
                            this.isSaveButShowHide = true;
                        } else {
                            this.isSaveButShowHide = false;
                        }
                    }
                    this.disabledvalue = true;
                    this.empnonempdisabledvalue = true;
                }
                if (this.loggedinuser !== null && this.loggedinuser !== undefined && this.loggedinuser.userID === this.user.userID) {
                    this.sameUserIsActive = true;
                } else {
                    this.sameUserIsActive = false;
                }

                this.ModalHeading = "Edit User";
                this.addEditUserModal.show();
            }
        })

    }



    buildUserForm(data: any, keyName: string) {
        this.UserForm = this.fb.group({
            UserID: [keyName === 'New' ? 0 : data.userID],
            EmployeeID: [keyName === 'New' ? null : data.employeeID],
            EmployeeCode: [keyName === 'New' ? '' : data.employeeCode],
            FirstName: [keyName === 'New' ? '' : data.firstName, Validators.required],
            LastName: [keyName === 'New' ? null : data.lastName, Validators.required],
            Email: [keyName === 'New' ? null : data.email, [Validators.required, Validators.email]],
            RoleID: [keyName === 'New' ? [] : data.userRoles !== null ? data.userRoles : null],
            CompanyID: [keyName === 'New' ? null : data.companyID],
            IsOAuth: [keyName === 'New' ? false : data.isOAuth],
            IsActive: [keyName === 'New' ? false : data.isActive],
            NewPassword: [keyName === 'New' ? '' : data.password],
            Password: [keyName === 'New' ? '' : data.password],
            ConfirmPassword: [keyName === 'New' ? '' : data.password],
            CreatedBy: [keyName === 'New' ? this.loggedinuser.firstName + " " + this.loggedinuser.lastName : this.loggedinuser.firstName + " " + this.loggedinuser.lastName],
            CreatedDate: [keyName === 'New' ? new Date() : data.createdDate],
            ModifiedDate: [keyName === 'New' ? new Date() : data.modifiedDate],
            ModifiedBy: [keyName === 'New' ? this.loggedinuser.firstName + " " + this.loggedinuser.lastName : data.modifiedBy],
            TimeStamp: [''],
            UserRoles: [keyName === 'New' ? [] : []],
        });
        if (keyName === 'New') {
            this.UserForm.controls.NewPassword.patchValue('')
        }
    }
    get addEditUserControls() { return this.UserForm.controls; }
    EmployeeId: number;
    employee: IEmployeeDisplay;
    GetEmployeeDetails(e: any) {
        if (e.value !== "" && e.value !== null) {
            this.EmployeeId = Number(e.value);
            this.employeeService.getEmployeeByIdAsync(this.EmployeeId).subscribe(result => {
                if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                    if (result['data'].termDate != null) {
                        this.messageService.add({ severity: 'error', summary: 'Please select active employee.', detail: '' });
                    } else {
                        this.employee = result['data'];
                        this.UserForm.controls.FirstName.patchValue(this.employee.firstName);
                        this.UserForm.controls.LastName.patchValue(this.employee.lastName);
                        if (this.employee.workEmail !== null && this.employee.workEmail !== '') {
                            this.UserForm.controls.Email.patchValue(this.employee.workEmail);
                        } else {
                            this.UserForm.controls.Email.patchValue(this.employee.email);
                        }
                        this.UserForm.controls.CompanyID.patchValue(this.CompanyList.find(c => c.companyType.toUpperCase() === "SELF").companyID);
                        this.disabledvalue = true;
                    }
                }
            })

        }
        else {
            this.UserForm.controls.EmployeeID.patchValue(null);
            this.UserForm.controls.FirstName.patchValue('');
            this.UserForm.controls.LastName.patchValue('');
            this.UserForm.controls.Email.patchValue('');
            this.disabledvalue = false;
        }

    }

    Items: any[] = []
    SetNewUserRoleList() {
        this.Items = [];
        if (this.Roles !== null) {
            this.Roles.forEach(item => {
                var object = { UserRoleID: 0, RoleID: item.roleID, RoleName: item.roleName, RoleShort: item.roleShort, IsSelected: false, IsDefault: false }
                this.Items.push(object);
            })
        }
    }

    UpdateSelectedList() {
        if (this.user.userRoles !== null) {
            var rolesIDs = this.user.userRoles.map(x => x.roleID)
            if (this.Roles !== null) {
                this.Items = [];
                this.Roles.forEach(item => {
                    var roleselected = this.user.userRoles.find(x => x.roleID === item.roleID);
                    var tempRole = this.user.userRoles.find(x => x.roleShort === item.roleShort);
                    var userRole: any = {};
                    userRole.RoleID = item.roleID;
                    userRole.RoleName = item.roleName;
                    userRole.RoleShort = item.roleShort;
                    if (tempRole !== null && tempRole !== undefined) {
                        userRole.UserRoleID = tempRole.userRoleID;
                    }
                    if (roleselected !== null && roleselected !== undefined) {
                        userRole.CreatedBy = roleselected.createdBy;
                        userRole.CreatedDate = roleselected.createdDate;
                        userRole.IsSelected = true;
                        userRole.IsDefault = roleselected.isDefault;
                    } else {
                        userRole.IsSelected = false;
                    }
                    this.Items.push(userRole);
                })
                this.UserForm.get('RoleID').setValue(rolesIDs);
            }
        }
    }

    //CheckboxChanged(event): void {
    //    console.log(event.originalEvent.target.className)
    //    if (!event.originalEvent.target.className.includes("ui-chkbox-box")) {
    //        console.log('Herer')
    //        if (event.value.includes(event.itemValue)) {
    //            console.log('Herer---')
    //            //event.value.pop();
    //            /* removing the added value on select of row */
    //        } else if (!event.originalEvent.target.className.includes("ui-chkbox-icon")) {
    //             console.log('HererHi')
    //            event.value.push(event.itemValue);
    //            /* need to add this as by default on deselect 
    //             row value is getting removed so had to add in value array 
    //             so that it is not removed */
    //        }
    //    }
    //}
    checkboxChanged(e, key: number) {
        this.isRoleUpdated = true;
        if (this.Items != null) {
            var index = this.Items.findIndex(x => x.RoleID == key);
            var userRole = this.Items.find(x => x.RoleID == key);
            userRole.IsSelected = !userRole.IsSelected;
            if (userRole.IsSelected) {
                userRole.CreatedBy = this.loggedinuser.FirstName + " " + this.loggedinuser.LastName;
                userRole.CreatedDate = new Date();
                userRole.UserID = this.UserId;
                userRole.UserRoleID = 0;
            }
            else {
                userRole.IsDefault = false;
            }
            this.Items[index] = userRole;
        }
    }
    CheckboxChanged(event: any) {
        var key = event.value;
        this.isRoleUpdated = true;
        if (this.Items != null && key !== null) {
            var index = this.Items.findIndex(x => x.RoleID === key[0]);
            var userRole = this.Items.find(x => x.RoleID === key[0]);

            if (userRole !== undefined) {
                userRole.IsSelected = !userRole.IsSelected;
                if (userRole.IsSelected) {
                    userRole.CreatedBy = this.loggedinuser.FirstName + " " + this.loggedinuser.LastName;
                    userRole.CreatedDate = new Date();
                    userRole.UserID = this.UserId;
                    userRole.UserRoleID = 0;
                }
            }
            else {
                userRole.IsDefault = false;
            }
            this.Items[index] = userRole;
        }
    }

    setDefaultRole(action: string, role: any) {
        this.isRoleUpdated = true;
        if (role != null) {
            this.Items.forEach(x => {
                if (action == "Add" && x.RoleID == role.RoleID) {
                    x.IsDefault = true;
                }
                else {
                    x.IsDefault = false;
                }
            });
        }
    }
    saveUser = () => {
        this.submitted = true;
        if (this.LoginMessage != '') {
            this.isSaveButShowHide = false;
        }
        if (this.UserForm.invalid) {
            this.UserForm.markAllAsTouched();
            return;
        } else {
            if (this.isSaveButShowHide) {
                return;
            } else {
                this.isSaveButShowHide = true;
                this.UserSave()
            }
        }
    }
    UserSave() {
        this.LoginMessage = '';
        var selectedItems = this.Items.filter(x => x.IsSelected == true);
        var isDefaultSelected = this.Items.findIndex(x => x.IsDefault == true);
        if (this.UserForm.value.IsOAuth === false && (this.UserForm.value.NewPassword === null || this.UserForm.value.NewPassword == "")) {
            this.LoginMessage = "Please enter Password";
            this.messageService.add({ severity: 'error', summary: this.LoginMessage, detail: '' });
        }
        else if (this.UserForm.value.IsOAuth === false && (this.UserForm.value.ConfirmPassword === null || this.UserForm.value.ConfirmPassword == "")) {
            this.LoginMessage = "Please enter Confirm Password";
            this.messageService.add({ severity: 'error', summary: this.LoginMessage, detail: '' });
        }
        else if (this.UserForm.value.NewPassword !== this.UserForm.value.ConfirmPassword) {
            this.LoginMessage = "Password and Confirm Password should be same";
            this.messageService.add({ severity: 'error', summary: this.LoginMessage, detail: '' });
        }
        else if (selectedItems.length == 0) {
            this.LoginMessage = "Please select Role";
            this.messageService.add({ severity: 'error', summary: this.LoginMessage, detail: '' });
        }
        else if (selectedItems.length > 1 && isDefaultSelected == -1) {
            this.LoginMessage = "Please select Default Role";
            this.messageService.add({ severity: 'error', summary: this.LoginMessage, detail: '' });
        }
        else if (this.UserForm.value.CompanyID == null) {
            this.LoginMessage = "Please select Company";
            this.messageService.add({ severity: 'error', summary: this.LoginMessage, detail: '' });
        }
        else if (this.UserForm.value.UserID == 0 && this.UsersList.filter(x => x.email.toUpperCase() == this.UserForm.value.Email.toUpperCase()).length > 0) {
            this.LoginMessage = "User Email already exists in the system";
            this.messageService.add({ severity: 'error', summary: this.LoginMessage, detail: '' });
        }
        else {
            var password = this.UserForm.value.Password !== this.UserForm.value.ConfirmPassword ? this.UserForm.value.ConfirmPassword : null;
            this.UserForm.controls.Password.patchValue(password)
            //var selectedItems = this.Items.filter(x => this.UserForm.value.RoleID.includes(x.RoleID));
            //selectedItems[0]['IsDefault'] = true;
            selectedItems.forEach(x => {
                x.ModifiedBy = this.loggedinuser.firstName + " " + this.loggedinuser.lastName;
                x.ModifiedDate = new Date();
                x.CreatedBy = this.loggedinuser.firstName + " " + this.loggedinuser.lastName;
                x.CreatedDate = new Date();
                x.IsSelected = true;
                this.UserForm.value.UserRoles.push(x);
            });

            //this.UserForm.value.Password = this.UserForm.value.NewPassword;
            if (this.UserId == 0) {
                //if (user.EmployeeID == 0) {
                //    user.EmployeeID = null;
                //}
                this.userService.SaveUser(this.UserForm.value).subscribe(async result => {
                    if (result['data'] !== null && result['messageType'] === 1) {
                        this.messageService.add({ severity: 'success', summary: 'User saved successfully', detail: '' });
                        this.isSaveButShowHide = true;
                        //this.loadModalOptions();
                        await this.sendMail();
                        await this.cancel();
                        await this.ListValueUpdated.emit();
                    }
                    else {
                        this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                    }
                })
            }
            else if (this.UserId > 0) {
                this.userService.UpdateUser(this.UserForm.value).subscribe(result => {
                    if (result['data'] !== null && result['messageType'] === 1) {
                        this.messageService.add({ severity: 'success', summary: 'User saved successfully', detail: '' });
                        this.isSaveButShowHide = true;
                        //this.loadModalOptions();
                        this.cancel();
                        this.ListValueUpdated.emit();

                    } else {
                        this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });

                    }
                })
            }
            this.isSaveButShowHide = false;

        }
    }
    uri: string;
    //common: EmailFields
    loadClientName() {
        if ((Object.keys(EmailApprovalUrl) as string[]).includes(this.loggedinuser.clientID.toString())) {
            this.uri = EmailApprovalUrl[this.loggedinuser.clientID.toString()];
        }
    }
    async sendMail() {
        var strPassword: string = "";
        if (this.UserForm.value.IsOAuth === true) {
            strPassword = "Office 365 Password";
        }
        else {
            strPassword = "Contact your Manager/HR";
        }
        var common: EmailFields = new EmailFields();
        common.EmailTo = this.UserForm.value.Email;
        common.EmailSubject = "IHR User Created";
        common.EmailBody = "IHR User Details:" + "<br/>" + "Client ID: " + this.loggedinuser.clientID.toUpperCase() + "<br/>" +
            "Email Address: " + this.UserForm.value.Email + "<br/>" +
            "Password: " + strPassword + "<br/>" +
            "<br/>Login to <a href='" + this.uri + "'> InfoHR</a><br/><div>NOTE: This is an outgoing message only. Please do not reply to this message</div>";
        await this.userService.SendEmail(common).subscribe()
    }
    cancel = () => {
        this.submitted = false;
        this.isSaveButShowHide = false;
        this.buildUserForm({}, 'New');
        this.addEditUserModal.hide();
    }

    onAuthCheck(e: any) {
        if (e.checked === true) {
            this.UserForm.controls.NewPassword.patchValue('');
            this.UserForm.controls.ConfirmPassword.patchValue('');
        }
    }

    getSelectedRoles(): string {
        let roles: string = "";
        if (this.Items != null) {
            if (this.Items.findIndex(x => x.IsSelected == true) == -1) {
                return "Select Role";
            }
            else {
                this.Items.forEach(item => {
                    if ((roles == "" || roles == null) && item.IsSelected) {
                        roles = item.RoleName;
                    }
                    else if (item.IsSelected) {
                        roles = roles + ", " + item.RoleName;
                    }
                })
            }
        }
        return roles;
    }
    isMenuOpen: boolean = false;
    onShowDropdown() {
        this.isMenuOpen = !this.isMenuOpen;
    }






}
