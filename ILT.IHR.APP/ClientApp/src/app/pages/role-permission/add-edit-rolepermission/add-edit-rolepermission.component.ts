import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { CommonUtils } from '../../../common/common-utils';
import { ErrorMsg } from '../../../constant';
import { IModuleDisplay } from '../../../core/interfaces/Module';
import { IRolePermissionDisplay } from '../../../core/interfaces/RolePermission';
import { IModalPopupAlternateOptions } from '../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { RolePermissionService } from '../role-permission.service';

@Component({
  selector: 'app-add-edit-rolepermission',
  templateUrl: './add-edit-rolepermission.component.html',
  styleUrls: ['./add-edit-rolepermission.component.scss']
})
export class AddEditRolepermissionComponent implements OnInit {
    commonUtils = new CommonUtils()
    @Input() roleID: number = 0;
    @Output() loadRolePermissionyDetails = new EventEmitter<any>();
    @ViewChild('addEditRolePermissionModal') addEditRolePermissionModal: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;

    RolePermissionForm: FormGroup;
    user: any;

    ModalHeading: string = 'Add Role Permission';
    HolidayId: number;
    submitted: boolean
    isSaveButtonDisabled: boolean = false;

    constructor(private fb: FormBuilder,
        private rolePermissionService: RolePermissionService,
        private messageService: MessageService) {
        this.user = JSON.parse(localStorage.getItem("User"));
        this.buildRolePermissionForm({}, 'New');
    }

    ngOnInit(): void {
        this.LoadDropDown();
        this.loadModalOptions();
    }
    loadModalOptions() {
        this.modalOptions = {
            footerActions: [
                {
                    actionText: 'Save',
                    actionMethod: this.save,
                    styleClass: 'p-button-raised p-mr-2 p-mb-2',
                    iconClass: 'mdi mdi-content-save',
                    disabled: this.isSaveButtonDisabled
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
    moduleList: IModuleDisplay[] = [];
   
    LoadDropDown() {
        this.rolePermissionService.getModules().subscribe(result => {
            this.moduleList = result;
        });
    }
    RolePermissionId: number = -1;
    roleName: string = '';
    ErrorMessage: string = '';
    Show(Id: number,  roleId: number, roleName: string) {
        this.ErrorMessage = "";
        this.RolePermissionId = Id;
        this.roleName = roleName;
        this.ResetDialog();
        console.log(this.RolePermissionId)
        if (this.RolePermissionId != 0) {
            this.isSaveButtonDisabled = false;
            this.loadModalOptions();
            this.GetDetails(this.RolePermissionId);
        }
        else {
            console.log('Here')
            this.LoadRolePermissionList(roleId);
            this.RolePermissionForm.controls.RoleID.patchValue(roleId)
            this.addEditRolePermissionModal.show();
        }
    }
    GetDetails(Id: number) {
        this.rolePermissionService.getRolePermissionByIdAsync(Id).subscribe(respRole => {
            if (respRole !== null && respRole !== undefined && respRole['data'] !== null && respRole['data'] !== undefined) {
                this.buildRolePermissionForm(respRole['data'], 'Edit')
                this.isSaveButtonDisabled = false;
                this.submitted = false;
                this.ModalHeading = "Edit Role Permission";
                this.addEditRolePermissionModal.show();
            }
         });
    }

    buildRolePermissionForm(data: any, keyName: string) {
        this.RolePermissionForm = this.fb.group({
            RolePermissionID: [keyName === 'New' ? 0 : data.rolePermissionID],
            RoleID: [keyName === 'New' ? null : data.roleID],
            ModuleID: [keyName === 'New' ? null : data.moduleID, Validators.required],
            View: [keyName === 'New' ? false : data.view],
            Add: [keyName === 'New' ? false : data.add],
            Update: [keyName === 'New' ? false : data.update],
            Delete: [keyName === 'New' ? false : data.delete],
            CreatedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : this.user.firstName + " " + this.user.lastName],
            CreatedDate: [keyName === 'New' ? new Date() : data.createdDate],
            ModifiedDate: [keyName === 'New' ? new Date() : data.modifiedDate],
            ModifiedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : data.modifiedBy],
            TimeStamp: ['']
        });
    }
    get addEditRolePermissionFormControls() { return this.RolePermissionForm.controls; }

    PermissionsRolePermission: IRolePermissionDisplay;
    save = () => {
        this.submitted = true;
        if (this.RolePermissionForm.invalid) {
            this.RolePermissionForm.markAllAsTouched();
            return;
        } else {
            if (this.isSaveButtonDisabled) {
                return
            } else {
                this.isSaveButtonDisabled = true;
                if (this.RolePermissionId == 0) {
                    this.PermissionsRolePermission = this.RolePermissionsList.find(x => x.roleID == this.RolePermissionForm.value.RoleID && x.moduleID == this.RolePermissionForm.value.ModuleID);
                    this.ErrorMessage = "";
                    if (this.PermissionsRolePermission != null) {
                        this.ErrorMessage = "Role Permission already exists in the system";
                    } else {
                        this.rolePermissionService.SaveRolePermission(this.RolePermissionForm.value).subscribe(result => {
                            if (result['data'] !== null && result['messageType'] === 1) {
                                this.messageService.add({ severity: 'success', summary: 'Lookup saved successfully', detail: '' });
                                this.isSaveButtonDisabled = true;
                                this.loadModalOptions();
                                this.loadRolePermissionyDetails.emit(this.roleID);
                            } else {
                                this.isSaveButtonDisabled = false;
                                this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                            }
                        })
                    }
                }
                else if (this.RolePermissionId > 0) {
                    this.RolePermissionForm.value.ModifiedBy = this.user.firstName + " " + this.user.lastName;
                    this.RolePermissionForm.value.ModifiedDate = this.commonUtils.defaultDateTimeLocalSet(new Date());
                    this.rolePermissionService.UpdateRolePermission(this.RolePermissionForm.value).subscribe(result => {
                        if (result['data'] !== null && result['messageType'] === 1) {
                            this.messageService.add({ severity: 'success', summary: 'Role Permission saved successfully', detail: '' });
                            this.isSaveButtonDisabled = true;
                            this.loadModalOptions();
                            this.loadRolePermissionyDetails.emit(this.roleID);

                        } else {
                            this.isSaveButtonDisabled = false;
                            this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                        }
                    })
                    this.submitted = false;
                    this.cancel();
                }

            }
        }
        this.isSaveButtonDisabled = false;
    }
    RolePermissionsList: IRolePermissionDisplay[] = []
    LoadRolePermissionList(ID: number) {
        if (ID != 0) {
            this.rolePermissionService.getRoleByIdAsync(ID).subscribe(respRole => {
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
    cancel = () => {
        this.ErrorMessage = '';
        this.submitted = false;
        this.RolePermissionId = -1;
        this.isSaveButtonDisabled = false;
        this.addEditRolePermissionModal.hide();
        this.buildRolePermissionForm({}, 'New');
    }
    ResetDialog() {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildRolePermissionForm({}, 'New');
    }

}
