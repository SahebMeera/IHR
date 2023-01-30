import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { CommonUtils } from '../../../../common/common-utils';
import { Constants, ErrorMsg, ListTypeConstants, SessionConstants } from '../../../../constant';
import { IRolePermissionDisplay } from '../../../../core/interfaces/RolePermission';
import { IModalPopupAlternateOptions } from '../../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { LookUpService } from '../../../lookup/lookup.service';
import { EmployeeService } from '../../employee.service';
import { SkillService } from '../skill.service';

@Component({
  selector: 'app-add-edit-skill',
  templateUrl: './add-edit-skill.component.html',
  styleUrls: ['./add-edit-skill.component.scss']
})
export class AddEditSkillComponent implements OnInit {
    commonUtils = new CommonUtils()
    @Input() EmployeeName: string;
    @Output() ListValueUpdated = new EventEmitter<any>();
    @ViewChild('addEditEmployeeSkillModal') addEditEmployeeSkillModal: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;
    isSaveButtonDisabled: boolean = false;
    ModalHeading: string = 'Add Employee Skill';
    isShow: boolean;

    SkillTypeList: any[] = [];
    user: any;
    submitted: boolean = false;
    
    EmployeeSkillID: number = 0;
    EmployeeSkillForm: FormGroup;
    RolePermissions: IRolePermissionDisplay[] = [];
    EmployeeInfoRolePermission: IRolePermissionDisplay;
    constructor(private fb: FormBuilder,
        private LookupService: LookUpService,
        private messageService: MessageService,
        private skillService: SkillService,
        private employeeService: EmployeeService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.EmployeeInfoRolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.SKILL);
        this.user = JSON.parse(localStorage.getItem("User"));
        this.LoadDropDown();
        this.buildEmployeeSkillForm({}, 'New');
    }

    ngOnInit(): void {
        this.loadModalOptions();
    }
    loadModalOptions() {
        this.modalOptions = {
            footerActions: [
                {
                    actionText: 'Save',
                    actionMethod: this.save,
                    styleClass: this.isShow ? 'btn-width-height p-button-raised p-mr-2 p-mb-2' : 'btn-width-height p-button-raised p-mr-2 p-mb-2 display-none',
                    iconClass: 'mdi mdi-content-save',
                    disabled: this.isSaveButtonDisabled
                },
                {
                    actionText: 'Cancel',
                    actionMethod: this.cancel,
                    styleClass: 'btn-width-height p-button-raised p-button-danger  p-mb-2',
                    iconClass: 'p-button-raised p-button-danger'
                }
            ]
        }
    }

    Show(Id: number, employeeId: number) {
        this.EmployeeSkillID = Id;
        this.ResetDialog();
        if (this.EmployeeSkillID !== 0) {
            this.isShow = this.EmployeeInfoRolePermission.update;
            this.loadModalOptions();
            console.log(this.EmployeeSkillID)
            this.GetDetails(this.EmployeeSkillID);
        }
        else {
            this.isShow = this.EmployeeInfoRolePermission.add;
            this.loadModalOptions();
            this.ModalHeading = "Add Employee Skill";
            this.buildEmployeeSkillForm({}, 'New');
            this.EmployeeSkillForm.controls.EmployeeID.patchValue(employeeId)
            this.EmployeeSkillForm.controls.EmployeeName.patchValue(this.EmployeeName);
            this.addEditEmployeeSkillModal.show();
        }
    }

    LoadDropDown() {
        this.LookupService.getListValues().subscribe(resp => {
            if (resp['data'] !== undefined && resp['data'] !== null) {
                this.SkillTypeList = resp['data'].filter(x => x.type.toUpperCase() === ListTypeConstants.SKILLTYPE);
            }
        })
    }

    buildEmployeeSkillForm(data: any, keyName: string) {
        this.EmployeeSkillForm = this.fb.group({
            EmployeeSkillID: [keyName === 'New' ? 0 : data.employeeSkillID],
            SkillTypeID: [keyName === 'New' ? null : data.skillTypeID, Validators.required],
            SkillType: [keyName === 'New' ? '' : data.skillType],
            EmployeeID: [keyName === 'New' ? null : data.employeeID],
            EmployeeName: [keyName === 'New' ? '' : data.employeeName],
            Skill: [keyName === 'New' ? '' : data.skill, Validators.required],
            Experience: [keyName === 'New' ? null : data.experience, Validators.required],
            CreatedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : this.user.firstName + " " + this.user.lastName],
            CreatedDate: [keyName === 'New' ? new Date() : data.createdDate],
            ModifiedDate: [keyName === 'New' ? new Date() : data.modifiedDate],
            ModifiedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : data.modifiedBy],
            TimeStamp: ['']
        });
    }
    get addEditEmployeeSkillControls() { return this.EmployeeSkillForm.controls; }

    GetDetails(Id: number) {
        this.skillService.getEmployeeSkillByIdAsync(Id).subscribe(result => {
            if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                this.ModalHeading = "Edit EmployeeSkill";
                this.buildEmployeeSkillForm(result['data'], 'Edit');
                this.EmployeeSkillID = result['data'].employeeSkillID;
                this.EmployeeName = result['data'].employeeName;
                this.addEditEmployeeSkillModal.show();
            }
        })
    }

    save = () => {
        this.submitted = true;
        if (this.EmployeeSkillForm.invalid) {
            this.EmployeeSkillForm.markAllAsTouched();
            return;
        } else {
            if (this.isSaveButtonDisabled) {
                return;
            } else {
                this.isSaveButtonDisabled = true;
                this.EmployeeSkillForm.value.Experience = this.EmployeeSkillForm.value.Experience.toString();
                if (this.EmployeeSkillID == 0) {
                    this.skillService.SaveEmployeeSkill(this.EmployeeSkillForm.value).subscribe(result => {
                        if (result['data'] !== null && result['messageType'] === 1) {
                            this.messageService.add({ severity: 'success', summary: 'Employee SKill saved successfully', detail: '' });
                            this.isSaveButtonDisabled = true;
                            this.loadModalOptions();
                            this.ListValueUpdated.emit();
                        } else {
                            this.isSaveButtonDisabled = false;
                            this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                        }
                    })
                    this.submitted = false;
                    this.cancel();
                }
                else if (this.EmployeeSkillID > 0) {
                    this.skillService.UpdateEmployeeSkill(this.EmployeeSkillForm.value).subscribe(result => {
                        if (result['data'] !== null && result['messageType'] === 1) {
                            this.messageService.add({ severity: 'success', summary: 'Employee Skill saved successfully', detail: '' });
                            this.isSaveButtonDisabled = true;
                            this.loadModalOptions();
                            this.ListValueUpdated.emit();

                        } else {
                            this.isSaveButtonDisabled = false;
                            this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                        }
                    })
                    this.submitted = false;
                    this.cancel();
                }
            }
            this.isSaveButtonDisabled = false;
        }
    }

    cancel = () => {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildEmployeeSkillForm({}, 'New');
        this.addEditEmployeeSkillModal.hide();
    }
    ResetDialog() {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildEmployeeSkillForm({}, 'New');
        this.addEditEmployeeSkillModal.hide();
    }

}
