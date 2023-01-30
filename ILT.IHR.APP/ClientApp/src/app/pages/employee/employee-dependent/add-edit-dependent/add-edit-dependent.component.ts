import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { CommonUtils } from '../../../../common/common-utils';
import { Constants, Countries, ErrorMsg, ListTypeConstants, SessionConstants } from '../../../../constant';
import { IRolePermissionDisplay } from '../../../../core/interfaces/RolePermission';
import { IModalPopupAlternateOptions } from '../../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { HolidayService } from '../../../holidays/holidays.service';
import { LookUpService } from '../../../lookup/lookup.service';
import { EmployeeService } from '../../employee.service';
import { DependentService } from '../dependent.service';

@Component({
  selector: 'app-add-edit-dependent',
  templateUrl: './add-edit-dependent.component.html',
  styleUrls: ['./add-edit-dependent.component.scss']
})
export class AddEditDependentComponent implements OnInit {
    commonUtils = new CommonUtils()
    @Input() EmployeeName: string;
    @Output() ListValueUpdated = new EventEmitter<any>();
    @ViewChild('addEditDependentModal') addEditDependentModal: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;
    isSaveButtonDisabled: boolean = false;
    DependentId: number;
    ModalHeading: string = 'Add Direct Deposit';
    isShow: boolean;

    VisaTypeList: any[] = [];
    RelationList: any[] = [];
    CountryList: any[] = [];
    user: any;
    submitted: boolean = false;
    StateList: any[] = [];

    DependentForm: FormGroup;
    RolePermissions: IRolePermissionDisplay[] = [];
    EmployeeInfoRolePermission: IRolePermissionDisplay;
    constructor(private fb: FormBuilder,
        private LookupService: LookUpService,
        private messageService: MessageService,
        private dependentService: DependentService,
        private employeeService: EmployeeService,
        private holidayService: HolidayService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.EmployeeInfoRolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.EMPLOYEEINFO);
        this.user = JSON.parse(localStorage.getItem("User"));
        this.LoadDropDown();
        this.buildDependentForm({}, 'New');
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
                    styleClass: this.isShow ? 'btn-width-height p-button-raised p-mr-2 p-mb-2' : 'p-button-raised p-mr-2 p-mb-2 display-none',
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
        this.DependentId = Id;
        this.ResetDialog();
        if (this.DependentId != 0) {
            this.isShow = this.EmployeeInfoRolePermission.update;
            this.loadModalOptions();
            this.GetDetails(this.DependentId);
        }
        else {
            this.isShow = this.EmployeeInfoRolePermission.add;
            this.loadModalOptions();
            this.ModalHeading = "Add Dependent";
            this.buildDependentForm({}, 'New');
            this.loadEmployeeData(employeeId);
            this.DependentForm.controls.EmployeeID.patchValue(employeeId)
            this.DependentForm.controls.EmployeeName.patchValue(this.EmployeeName);
            this.addEditDependentModal.show();
        }
    }
    LoadDropDown() {
        this.LookupService.getListValues().subscribe(resp => {
            if (resp['data'] !== undefined && resp['data'] !== null) {
                this.VisaTypeList = resp['data'].filter(x => x.type.toUpperCase() === ListTypeConstants.VISATYPE);
                this.RelationList = resp['data'].filter(x => x.type.toUpperCase() === ListTypeConstants.RELATION);
            }
        })
    }
    loadEmployeeData(employeeID: number) {
        this.employeeService.getEmployeeByIdAsync(employeeID).subscribe(result => {
            if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                if (result['data'].employeeID !== 0) {
                    if (this.RelationList.length != 0) {
                        if (result['data'].country.toUpperCase() == Countries.UNITEDSTATES) {
                            var Data = this.RelationList.filter(x => x.valueDesc.toUpperCase() != "Mother".toUpperCase() && x.valueDesc.toUpperCase() != "Father".toUpperCase());
                            this.RelationList = [];
                            this.RelationList = Data;
                        }
                    }
                }
            }
        }, error => {
            //toastService.ShowError();
        })
    }
    buildDependentForm(data: any, keyName: string) {
        this.DependentForm = this.fb.group({
            DependentID: [keyName === 'New' ? 0 : data.dependentID],
            RelationID: [keyName === 'New' ? null : data.relationID, Validators.required],
            Relation: [keyName === 'New' ? '' : data.relation],
            VisaTypeID: [keyName === 'New' ? null : data.visaTypeID, Validators.required],
            VisaType: [keyName === 'New' ? '' : data.visaType],
            EmployeeID: [keyName === 'New' ? null : data.employeeID],
            FirstName: [keyName === 'New' ? '' : data.firstName, Validators.required],
            EmployeeName: [keyName === 'New' ? '' : data.employeeName],
            LastName: [keyName === 'New' ? '' : data.lastName, Validators.required],
            MiddleName: [keyName === 'New' ? '' : data.middleName],
            BirthDate: [keyName === 'New' ? new Date() : new Date(data.birthDate), Validators.required],
            CreatedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : this.user.firstName + " " + this.user.lastName],
            CreatedDate: [keyName === 'New' ? new Date() : data.createdDate],
            ModifiedDate: [keyName === 'New' ? new Date() : data.modifiedDate],
            ModifiedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : data.modifiedBy],
            TimeStamp: ['']
        });
    }
    get addEditDependentControls() { return this.DependentForm.controls; }
    save = () => {
        this.submitted = true;
        if (this.DependentForm.invalid) {
            this.DependentForm.markAllAsTouched();
            return;
        } else {
            if (this.isSaveButtonDisabled) {
                return;
            } else {
                this.isSaveButtonDisabled = true;
                if (this.DependentId == 0) {
                    this.dependentService.SaveDependent(this.DependentForm.value).subscribe(result => {
                        if (result['data'] !== null && result['messageType'] === 1) {
                            this.messageService.add({ severity: 'success', summary: 'Dependent saved successfully', detail: '' });
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
                else if (this.DependentId > 0) {
                    this.dependentService.UpdateDependent(this.DependentForm.value).subscribe(result => {
                        if (result['data'] !== null && result['messageType'] === 1) {
                            this.messageService.add({ severity: 'success', summary: 'Contact saved successfully', detail: '' });
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
    GetDetails(Id: number) {
        console.log(Id)
        this.dependentService.getDependentByIdAsync(Id).subscribe(result => {
            if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                this.buildDependentForm(result['data'], 'Edit');
                this.ModalHeading = "Edit Dependent";
                this.DependentId = result['data'].dependentID;
                this.EmployeeName = result['data'].employeeName;
                this.addEditDependentModal.show();
            }
        })
    }
    cancel = () => {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildDependentForm({}, 'New');
        this.addEditDependentModal.hide();
    }
    ResetDialog() {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildDependentForm({}, 'New');
        this.addEditDependentModal.hide();
    }

}
