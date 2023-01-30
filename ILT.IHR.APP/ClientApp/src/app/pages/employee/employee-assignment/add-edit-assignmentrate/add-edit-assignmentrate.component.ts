import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import * as moment from 'moment';
import { MessageService } from 'primeng/api';
import { CommonUtils } from '../../../../common/common-utils';
import { Constants, ErrorMsg, SessionConstants } from '../../../../constant';
import { IAssignmentRateDisplay } from '../../../../core/interfaces/AssignmentRate';
import { IEmployeeAssignmentDisplay } from '../../../../core/interfaces/EmployeeAssignment';
import { IRolePermissionDisplay } from '../../../../core/interfaces/RolePermission';
import { IModalPopupAlternateOptions } from '../../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { EmployeeService } from '../../employee.service';
import { EmployeeAssignmentService } from '../assignment.service';

@Component({
  selector: 'app-add-edit-assignmentrate',
  templateUrl: './add-edit-assignmentrate.component.html',
  styleUrls: ['./add-edit-assignmentrate.component.scss']
})
export class AddEditAssignmentrateComponent implements OnInit {
    commonUtils = new CommonUtils()
    @Input() EmployeeName: string;
    @Input() EmployeeId: number;
    @Output() ListValueUpdated = new EventEmitter<any>();
    @ViewChild('addEditAssignmentRateModal') addEditAssignmentRateModal: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;
    ContactTypeList: any[] = [];
    CountryList: any[] = [];
    user: any;
    submitted: boolean = false;
    StateList: any[] = [];

    AssignmentRateForm: FormGroup;
    RolePermissions: IRolePermissionDisplay[] = [];
    AssigmentsRolePermission: IRolePermissionDisplay;
    isSaveButtonDisabled: boolean = false;
    isShow: boolean = false;
    ErrorMessage: string = '';

    constructor(private fb: FormBuilder,
        private messageService: MessageService,
        private employeeService: EmployeeService,
        private assignmentService: EmployeeAssignmentService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.AssigmentsRolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.ASSIGNMENT);
        this.user = JSON.parse(localStorage.getItem("User"));
        //this.LoadDropDown();
        this.buildAssignmentRateForm({}, 'New');
    }


    ngOnInit(): void {
        this.loadModalOptions();
    }
    loadModalOptions() {
        this.modalOptions = {
            footerActions: [
                {
                    actionText: 'Save',
                    actionMethod: this.AssignmentRateValidation,
                    styleClass: this.isShow ? 'btn-width-height p-button-raised p-mr-2 p-mb-2' : 'btn-width-height p-button-raised p-mr-2 p-mb-2 display-none',
                    iconClass: 'mdi mdi-content-save',
                    disabled: this.isSaveButtonDisabled || !this.isShow
                },
                {
                    actionText: 'Cancel',
                    actionMethod: this.cancelChild,
                    styleClass: 'btn-width-height p-button-raised p-button-danger  p-mb-2',
                    iconClass: 'p-button-raised p-button-danger'
                }
            ]
        }
    }

    AssignmentId: number;
    AssignmentRateId: number = 0;
    ModalHeading: string = '';
   ShowChild(assignmentId: number, assignmentRateId: number) {
        this.AssignmentId = assignmentId;
        this.AssignmentRateId = assignmentRateId;
        this.ResetDialog();
        this.GetAssignmentDetails();
        //this.GetAssignmentRates(this.AssignmentId);
        if (this.AssignmentRateId != 0) {
            this.isShow = this.AssigmentsRolePermission.update;
            this.loadModalOptions();
            this.assignmentRates = [];
            this.GetAssignmentDetails();
            this.GetAssignmentRateDetails(this.AssignmentRateId);
        }
        else {
            this.isShow = this.AssigmentsRolePermission.add;
            this.ModalHeading = "Add Assigment Rate";
            this.buildAssignmentRateForm({}, 'New');
            this.AssignmentRateForm.controls.AssignmentID.patchValue(this.AssignmentId);
            this.loadModalOptions();
            this.addEditAssignmentRateModal.show();
        }
   }
    assignment: IEmployeeAssignmentDisplay;
    assignmentRates: IAssignmentRateDisplay[] = [];
    GetAssignmentDetails() {
        if (this.AssignmentId !== 0) {
            this.assignmentService.getEmployeeAssignmentById(this.AssignmentId).subscribe(result => {
                if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                    this.assignment = result['data'];
                    this.assignmentRates = [];
                    this.assignmentRates = this.assignment.assignmentRates;
                    if (this.assignmentRates.length > 0) {
                        this.assignmentRates.forEach((d) => {
                                d.startDate = moment(d.startDate).format("MM/DD/YYYY")
                                if (d.endDate !== null) {
                                    d.endDate = moment(d.endDate).format("MM/DD/YYYY")
                                }
                              //  this.assignmentRates.push(d);
                            });
                    }
                }
            });
        }
    }
    GetAssignmentRates(id: number) {
        if (this.AssignmentId !== 0) {
            this.assignmentService.getEmployeeAssignmentById(this.AssignmentId).subscribe(result => {
                if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                    var assignment = result['data'];
                    if (assignment.assignmentRates.length > 0) {
                        assignment.assignmentRates.forEach((d) => {
                            d.startDate = moment(d.startDate).format("MM/DD/YYYY")
                            if (d.endDate !== null) {
                                d.endDate = moment(d.endDate).format("MM/DD/YYYY")
                            }
                            this.assignmentRates.push(d);
                        });
                    }
                }
            });
        }
    }
    buildAssignmentRateForm(data: any, keyName: string) {
       this.AssignmentRateForm = this.fb.group({
            AssignmentRateID: [keyName === 'New' ? 0 : data.assignmentRateID],
            AssignmentID: [keyName === 'New' ? 0 : data.assignmentID],
            StartDate: [keyName === 'New' ? new Date() : new Date(data.startDate), Validators.required],
            EndDate: [keyName === 'New' ? null : data.endDate !== null ? new Date(data.endDate) : null],
            BillingRate: [keyName === 'New' ? null : data.billingRate !== null ? parseFloat(data.billingRate) : null, Validators.required],
            PaymentRate: [keyName === 'New' ? null : data.paymentRate !== null ? parseFloat(data.paymentRate) : null, Validators.required],
            IsFLSAExempt: false,
            CreatedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : this.user.firstName + " " + this.user.lastName],
            CreatedDate: [keyName === 'New' ? new Date() : data.createdDate],
            ModifiedDate: [keyName === 'New' ? new Date() : data.modifiedDate],
            ModifiedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : data.modifiedBy],
            TimeStamp: ['']
        });
    }
    get addEditAssignmentRateControls() { return this.AssignmentRateForm.controls; }

    GetAssignmentRateDetails(Id: number) {
        this.assignmentService.GetAssignmentRateById(Id).subscribe(result => {
            if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                this.buildAssignmentRateForm(result['data'], 'Edit');
                this.ModalHeading = 'Edit Assignment Rate';
                this.addEditAssignmentRateModal.show();
            }
        });
    }

    AssignmentRateValidation = () => {
        this.ErrorMessage = '';
        this.submitted = true;
        if (this.AssignmentRateForm.invalid) {
            this.AssignmentRateForm.markAllAsTouched();
            return;
        } else {
            if (this.isSaveButtonDisabled) {
                return;
            } else {
                this.isSaveButtonDisabled = true;
                var assignmentStartDate = new Date(moment(this.assignment.startDate).format('MM/DD/YYYY').toString())
                var assignmentRateStartDate = new Date(moment(this.AssignmentRateForm.value.StartDate).format('MM/DD/YYYY').toString());
                var assignmentEndDate = this.assignment.endDate !== null ? new Date(moment(this.assignment.endDate).format('MM/DD/YYYY').toString()) : null;
                var assignmentRateEndtDate = this.AssignmentRateForm.value.EndDate !== null ? new Date(moment(this.AssignmentRateForm.value.EndDate).format('MM/DD/YYYY').toString()) : null;
                if (this.assignment.endDate != null && assignmentStartDate <= assignmentRateStartDate) {
                    if (assignmentRateEndtDate === null && assignmentEndDate >= assignmentRateStartDate) {
                        this.isCheckAssignmentRateValid();
                    } else {
                        if (assignmentRateEndtDate != null && assignmentRateEndtDate <= assignmentEndDate) {
                           this.isCheckAssignmentRateValid();
                        }
                        else {
                            if (assignmentRateEndtDate !== null) {
                                this.ErrorMessage = "End date must be less than or equal to assignment end date";
                            }
                            else {
                                this.ErrorMessage = "Start date must be less than or equal to assignment end date";
                            }
                            // toastService.ShowError("AssignmentRate end date must be less than or equal to assignment end date", "");
                        }

                    }

                }
                else {
                    if (assignmentEndDate == null && assignmentStartDate <= assignmentRateStartDate) {
                        if (assignmentRateEndtDate !== null) {
                            this.assignmentRates.forEach(x => {
                                if (x.assignmentRateID !== this.AssignmentRateId && x.endDate === null) {
                                    x.endDate = '12/31/9999'
                                }
                            });
                            var assignmentRateAlreadyexist: boolean = this.assignmentRates.findIndex(x => x.assignmentRateID !== this.AssignmentRateId && x.endDate !== null &&
                                ((new Date(x.startDate) <= assignmentRateStartDate && x.endDate === null) ||
                                    (new Date(x.startDate) <= assignmentRateEndtDate && assignmentRateStartDate <= new Date(x.endDate)) ||
                                    (new Date(x.startDate) >= assignmentRateStartDate && new Date(x.endDate) <= assignmentRateEndtDate) ||
                                    (new Date(x.startDate) >= assignmentRateStartDate && new Date(x.endDate) === null) ||
                                    (new Date(x.startDate) <= assignmentRateStartDate && new Date(x.endDate) >= assignmentRateStartDate))) > -1;

                            console.log(assignmentRateAlreadyexist)
                            if (assignmentRateAlreadyexist) {
                                this.ErrorMessage = "End date must be less than or equal to assignment rate start date";
                            } else {
                                this.isCheckAssignmentRateValid();
                            }
                        } else {
                            this.isCheckAssignmentRateValid();
                        }
                    } else {
                        this.ErrorMessage = "Start date must be greater than or equal to assignment start date";
                        //toastService.ShowError("AssignmentRate start date must be greater than or equal to assignment start date", "");
                    }
                }
                this.isSaveButtonDisabled = false;
            }
        }
    }

    isCheckAssignmentRateValid() {
        console.log(this.assignmentRates)
        if (this.assignmentRates === null || this.assignmentRates.length === 0) {
            this.SaveAssignmentRate();
        } else {
            var assignmentRateStartDate = new Date(moment(this.AssignmentRateForm.value.StartDate).format('MM/DD/YYYY').toString())
            var assignmentRateEndtDate = this.AssignmentRateForm.value.EndDate !== null ? new Date(moment(this.AssignmentRateForm.value.EndDate).format('MM/DD/YYYY').toString()) : null;
            var endDate = new Date(moment(new Date()).format('MM/DD/YYYY').toString())

            if (assignmentRateEndtDate === null || assignmentRateStartDate <= assignmentRateEndtDate) {
                var currentRec = this.AssignmentRateId !== 0 ? this.assignmentRates.find(x => x.assignmentRateID !== this.AssignmentRateId) : null;
                if (currentRec === null || currentRec === undefined || currentRec.endDate !== null) {
                    var assignmentRateAlreadyexist: boolean = this.assignmentRates.findIndex(x => x.assignmentRateID !== this.AssignmentRateId && x.endDate !== null &&
                        ((new Date(x.startDate) <= assignmentRateStartDate && x.endDate === null) ||
                        (new Date(x.startDate) <= assignmentRateEndtDate && assignmentRateStartDate <= new Date(x.endDate)) ||
                        (new Date(x.startDate) >= assignmentRateStartDate && new Date(x.endDate) <= assignmentRateEndtDate) ||
                        (new Date(x.startDate) >= assignmentRateStartDate && new Date(x.endDate) === null) ||
                        (new Date(x.startDate) <= assignmentRateStartDate && new Date(x.endDate) >= assignmentRateStartDate))) > -1;
                    if (assignmentRateAlreadyexist) {
                        this.ErrorMessage = "Assignment Rate already exists for this period";
                    }
                    else {
                       this.SaveAssignmentRate();
                    }
                } else {
                    this.SaveAssignmentRate();
                }
            } else {
                this.ErrorMessage = "End date must be greater than start date";
            }
        }
    }

    SaveAssignmentRate() {
        if (this.AssignmentRateForm.value.BillingRate !== null && this.AssignmentRateForm.value.BillingRate !== undefined) {
            this.AssignmentRateForm.value.BillingRate = this.AssignmentRateForm.value.BillingRate.toString();
        }
        if (this.AssignmentRateForm.value.PaymentRate !== null && this.AssignmentRateForm.value.PaymentRate !== undefined) {
            this.AssignmentRateForm.value.PaymentRate = this.AssignmentRateForm.value.PaymentRate.toString();
        }
        this.AssignmentRateForm.value.StartDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.AssignmentRateForm.value.StartDate));
        if (this.AssignmentRateForm.value.EndDate !== null) {
            this.AssignmentRateForm.value.EndDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.AssignmentRateForm.value.EndDate));
        }
        if (this.AssignmentRateId === 0) {
            this.assignmentService.SaveAssignmentRate(this.AssignmentRateForm.value).subscribe(result => {
                if (result['data'] !== null && result['messageType'] === 1) {
                    this.messageService.add({ severity: 'success', summary: 'AssignmentRate saved successfully', detail: '' });
                    this.isSaveButtonDisabled = true;
                    this.loadModalOptions();
                    this.ListValueUpdated.emit(this.assignment);
                    this.cancelChild();
                } else {
                    this.isSaveButtonDisabled = false;
                    this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                }
            })

        }
        else if (this.AssignmentRateId !== 0) {
            this.assignmentService.UpdateAssignmentRate(this.AssignmentRateForm.value).subscribe(result => {
                if (result['data'] !== null && result['messageType'] === 1) {
                    this.messageService.add({ severity: 'success', summary: 'AssignmentRate saved successfully', detail: '' });
                    this.isSaveButtonDisabled = true;
                    this.loadModalOptions();
                    this.ListValueUpdated.emit(this.assignment);

                } else {
                    this.isSaveButtonDisabled = false;
                    this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                }
            })
            this.submitted = false;
            this.cancelChild();
        }
        this.isSaveButtonDisabled = false;
    }

    cancelChild = () => {
        this.submitted = false;
        this.ErrorMessage = '';
        this.assignmentRates = [];
        this.isSaveButtonDisabled = false;
        this.buildAssignmentRateForm({}, 'New');
        this.addEditAssignmentRateModal.hide();
    }
    ResetDialog() {
        this.submitted = false;
        this.ErrorMessage = '';
        this.assignmentRates = [];
        this.isSaveButtonDisabled = false;
        this.buildAssignmentRateForm({}, 'New');
        this.addEditAssignmentRateModal.hide();
    }
}
