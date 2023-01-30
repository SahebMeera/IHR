import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { CommonUtils } from '../../../../common/common-utils';
import { Constants, ErrorMsg, ListTypeConstants, SessionConstants } from '../../../../constant';
import { IRolePermissionDisplay } from '../../../../core/interfaces/RolePermission';
import { IModalPopupAlternateOptions } from '../../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { LookUpService } from '../../../lookup/lookup.service';
import { DependentService } from '../../employee-dependent/dependent.service';
import { EmployeeService } from '../../employee.service';
import { EmployeeW4Service } from '../w4.service';

@Component({
  selector: 'app-add-edit-employeew4',
  templateUrl: './add-edit-employeew4.component.html',
  styleUrls: ['./add-edit-employeew4.component.scss']
})
export class AddEditEmployeew4Component implements OnInit {
    commonUtils = new CommonUtils()
    @Input() EmployeeName: string;
    @Output() ListValueUpdated = new EventEmitter<any>();
    @ViewChild('addEditEmployeeW4Modal') addEditEmployeeW4Modal: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;
    isSaveButtonDisabled: boolean = false;
    ModalHeading: string = 'Add Employee W4';
    isShow: boolean;

    withHoldingStausList: any[] = [];
    W4TypeList: any[] = [];
    CountryList: any[] = [];
    user: any;
    submitted: boolean = false;
    StateList: any[] = [];
    EmployeeW4ID: number;

    EmployeeW4Form: FormGroup;
    RolePermissions: IRolePermissionDisplay[] = [];
    EmployeeInfoRolePermission: IRolePermissionDisplay;
    NPIPermission: IRolePermissionDisplay;
    constructor(private fb: FormBuilder,
        private LookupService: LookUpService,
        private messageService: MessageService,
        private employeeW4Service: EmployeeW4Service,
        private employeeService: EmployeeService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.EmployeeInfoRolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.W4INFO);
        this.NPIPermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.NPI);

        this.user = JSON.parse(localStorage.getItem("User"));
        this.LoadDropDown();
        this.buildEmployeeW4Form({}, 'New');}

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

    Show(Id: number, employeeId: number, SSN: string) {
        this.EmployeeW4ID = Id;
        this.ResetDialog();
        if (this.EmployeeW4ID != 0) {
            this.isShow = this.EmployeeInfoRolePermission.update;
            this.loadModalOptions();
            this.GetDetails(this.EmployeeW4ID);
        }
        else {
            this.isShow = this.EmployeeInfoRolePermission.add;
            this.loadModalOptions();
            this.ModalHeading = "Add W4";
            this.buildEmployeeW4Form({}, 'New');
            this.isMultipleJob = false;
            this.setW4Type(0);
            this.EmployeeW4Form.controls.EmployeeID.patchValue(employeeId)
            this.EmployeeW4Form.controls.EmployeeName.patchValue(this.EmployeeName);
            this.EmployeeW4Form.controls.SSN.patchValue(SSN);
            this.EmployeeW4Form.controls.StartDate.patchValue(new Date());
            this.addEditEmployeeW4Modal.show();
        }
    }
    LoadDropDown() {
        this.LookupService.getListValues().subscribe(resp => {
            if (resp['data'] !== undefined && resp['data'] !== null) {
                this.withHoldingStausList = resp['data'].filter(x => x.type.toUpperCase() === ListTypeConstants.WITHHOLDINGSTATUS);
                this.W4TypeList = resp['data'].filter(x => x.type.toUpperCase() === ListTypeConstants.W4TYPE);
            }
        })
    }
    w4TypePre: boolean = false;
    w4TypeCurrent: boolean = false;
    isMultipleJob: boolean = false;
   onChangeW4Type(e: any) {
       var w4Type = e.value;
       this.setW4Type(w4Type);

   }
    setW4Type(type: any) {
        if (type !== null && type !== undefined && type !== 0) {
            if (this.W4TypeList.find(x => x.listValueID === Number(type)).value === "CURR") {
                this.w4TypePre = false;
                this.EmployeeW4Form.controls.W4Type.patchValue(this.W4TypeList.find(x => x.listValueID === Number(type)).valueDesc)
                this.w4TypeCurrent = true;
                this.isMultipleJob = true;
            }
            else {
                this.w4TypePre = true;
                this.EmployeeW4Form.controls.W4Type.patchValue(this.W4TypeList.find(x => x.listValueID === Number(type)).valueDesc)
                //EmployeeW4.W4Type = W4TypeList.Find(x => x.ListValueID == Convert.ToInt32(type)).ValueDesc;
                this.w4TypeCurrent = false;
                this.isMultipleJob = false;
            }
        }
        else {
            this.w4TypePre = true;
            this.w4TypeCurrent = true;
            this.isMultipleJob = false;
        }
    }
    buildEmployeeW4Form(data: any, keyName: string) {
        this.EmployeeW4Form = this.fb.group({
            EmployeeW4ID: [keyName === 'New' ? 0 : data.employeeW4ID],
            W4TypeID: [keyName === 'New' ? null : data.w4TypeID, Validators.required],
            W4Type: [keyName === 'New' ? '' : data.w4Type],
            WithHoldingStatusID: [keyName === 'New' ? null : data.withHoldingStatusID, Validators.required],
            WithHoldingStatus: [keyName === 'New' ? '' : data.withHoldingStatus],
            EmployeeID: [keyName === 'New' ? null : data.employeeID],
            SSN: [keyName === 'New' ? '' : data.ssn,],
            EmployeeName: [keyName === 'New' ? '' : data.employeeName],
            Allowances: [keyName === 'New' ? null : data.allowances],
            QualifyingChildren: [keyName === 'New' ? null : data.qualifyingChildren],
            OtherDependents: [keyName === 'New' ? null : data.otherDependents],
            OtherIncome: [keyName === 'New' ? null : data.otherIncome !== null ? parseFloat(data.otherIncome) : null],
            Deductions: [keyName === 'New' ? null : data.deductions !== null ? parseFloat(data.deductions) : null],
            IsMultipleJobsOrSpouseWorks: [keyName === 'New' ? false : data.isMultipleJobsOrSpouseWorks],
            StartDate: [keyName === 'New' ? null : new Date(data.startDate), Validators.required],
            EndDate: [keyName === 'New' ? null : data.endDate !== null ? new Date(data.endDate) : null],
            CreatedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : this.user.firstName + " " + this.user.lastName],
            CreatedDate: [keyName === 'New' ? new Date() : data.createdDate],
            ModifiedDate: [keyName === 'New' ? new Date() : data.modifiedDate],
            ModifiedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : data.modifiedBy],
            TimeStamp: ['']
        });
    }
    get addEditEmployeeW4Controls() { return this.EmployeeW4Form.controls; }


    save = () => {
        this.submitted = true;
        if (this.EmployeeW4Form.invalid) {
            this.EmployeeW4Form.markAllAsTouched();
            return;
        } else {
            if (this.isSaveButtonDisabled) {
                return;
            } else {
                this.isSaveButtonDisabled = true;
                if (this.EmployeeW4ID == 0) {
                    if (this.EmployeeW4Form.value.OtherIncome !== null && this.EmployeeW4Form.value.OtherIncome !== undefined) {
                        this.EmployeeW4Form.value.OtherIncome = this.EmployeeW4Form.value.OtherIncome.toString();
                    }
                    if (this.EmployeeW4Form.value.Deductions !== null && this.EmployeeW4Form.value.Deductions !== undefined) {
                        this.EmployeeW4Form.value.Deductions = this.EmployeeW4Form.value.Deductions.toString();
                    }
                    this.employeeW4Service.SaveEmployeeW4(this.EmployeeW4Form.value).subscribe(result => {
                        if (result['data'] !== null && result['messageType'] === 1) {
                            this.messageService.add({ severity: 'success', summary: 'EmployeeW4 saved successfully', detail: '' });
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
                else if (this.EmployeeW4ID > 0) {
                    if (this.EmployeeW4Form.value.OtherIncome !== null && this.EmployeeW4Form.value.OtherIncome !== undefined) {
                        this.EmployeeW4Form.value.OtherIncome = this.EmployeeW4Form.value.OtherIncome.toString();
                    }
                    if (this.EmployeeW4Form.value.Deductions !== null && this.EmployeeW4Form.value.Deductions !== undefined) {
                        this.EmployeeW4Form.value.Deductions = this.EmployeeW4Form.value.Deductions.toString();
                    }
                    this.employeeW4Service.UpdateEmployeeW4(this.EmployeeW4Form.value).subscribe(result => {
                        if (result['data'] !== null && result['messageType'] === 1) {
                            this.messageService.add({ severity: 'success', summary: 'EmployeeW4 saved successfully', detail: '' });
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
    isViewPermissionForNPIRole: boolean = false;
    GetDetails(Id: number) {
        this.isViewPermissionForNPIRole = false;
        this.employeeW4Service.getEmployeeW4ByIdAsync(Id).subscribe(result => {
            if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                this.ModalHeading = "Edit EmployeeW4";
                this.setW4Type(result['data'].w4TypeID);
                this.buildEmployeeW4Form(result['data'], 'Edit');
                this.EmployeeW4ID = result['data'].employeeW4ID;
                this.EmployeeName = result['data'].employeeName;
                this.isViewPermissionForNPIRole = this.NPIPermission.view;
                this.addEditEmployeeW4Modal.show();
            }
        })
    }
    cancel = () => {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildEmployeeW4Form({}, 'New');
        this.addEditEmployeeW4Modal.hide();
    }
    ResetDialog() {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildEmployeeW4Form({}, 'New');
        this.addEditEmployeeW4Modal.hide();
    }



}
