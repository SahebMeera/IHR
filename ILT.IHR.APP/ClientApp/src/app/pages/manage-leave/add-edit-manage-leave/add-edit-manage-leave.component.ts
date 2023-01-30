import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import * as moment from 'moment';
import { MessageService } from 'primeng/api';
import { CommonUtils } from '../../../common/common-utils';
import { ErrorMsg, Settings } from '../../../constant';
import { ILeaveBalanceDisplay } from '../../../core/interfaces/LeaveBalance';
import { IModalPopupAlternateOptions } from '../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { LeaveBalanceService } from '../LeaveBalanceService';

@Component({
  selector: 'app-add-edit-manage-leave',
  templateUrl: './add-edit-manage-leave.component.html',
  styleUrls: ['./add-edit-manage-leave.component.scss']
})
export class AddEditManageLeaveComponent implements OnInit {
    settings = new Settings()
    commonUtils = new CommonUtils()
    @Output() UpdateLeaveList = new EventEmitter<any>();
    @ViewChild('AddEditLeaveBalanceModal') AddEditLeaveBalanceModal: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;
    user: any;
    submitted: boolean = false;
    moment: any = moment;
    ModalHeading: string = 'Manage Leave';

    constructor(private leaveBalanceService: LeaveBalanceService,
        private messageService: MessageService,
        private fb: FormBuilder) {
        this.user = JSON.parse(localStorage.getItem("User"));
        this.buildLeaveBalanceForm({}, 'New')
    }

    ngOnInit(): void {
        this.loadModalOptions();
  }
  LeaveBalanceID: number
    Show(Id: number) {
        this.isSaveButtonDisabled = false;
        this.loadModalOptions()
        this.LeaveBalanceID = Id;
        this.ResetDialog();
        if (this.LeaveBalanceID != 0) {
            this.GetDetails(this.LeaveBalanceID);
        }
    }

    LeaveBalance: ILeaveBalanceDisplay;
    GetDetails(Id: number) {
        this.leaveBalanceService.GetLeaveBalanceById(Id).subscribe(result => {
            if (result !== null && result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                this.LeaveBalance = result['data'];
                this.buildLeaveBalanceForm(this.LeaveBalance, 'Edit')
                this.AddEditLeaveBalanceModal.show();
            }
        })
    }

    LeaveBalanceForm: FormGroup;
    buildLeaveBalanceForm(data: any, keyName: string) {
        this.LeaveBalanceForm = this.fb.group({
            LeaveBalanceID: [keyName === 'New' ? 0 : data.leaveBalanceID],
            EmployeeID: [keyName === 'New' ? null : data.employeeID],
            EmployeeName: [keyName === 'New' ? null : data.employeeName],
            LeaveYear: [keyName === 'New' ? null : data.leaveYear],
            LeaveTypeID: [keyName === 'New' ? null : data.leaveTypeID, Validators.required],
            LeaveType: [keyName === 'New' ? '' : data.leaveType],
            VacationTotal: [keyName === 'New' ? '' : data.vacationTotal],
            VacationBalance: [keyName === 'New' ? '' : data.vacationBalance],
            VacationUsed: [keyName === 'New' ? '' : data.vacationUsed],
            UnpaidLeave: [keyName === 'New' ? '' : data.unpaidLeave],
            EncashedLeave: [keyName === 'New' ? null : data.encashedLeave],
            CreatedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : this.user.firstName + " " + this.user.lastName],
            CreatedDate: [keyName === 'New' ? new Date() : new Date(data.createdDate)],
            ModifiedDate: [keyName === 'New' ? new Date() : data.modifiedDate],
            ModifiedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : data.modifiedBy],
            TimeStamp: [''],
        });
    }

    onTotalChange(event: any) {
        this.LeaveBalanceForm.controls.VacationBalance.patchValue(this.LeaveBalanceForm.value.VacationTotal - this.LeaveBalanceForm.value.VacationUsed);
    }
    isSaveButtonDisabled: boolean = false;
    loadModalOptions() {
        this.modalOptions = {
            footerActions: [
                {
                    actionText: 'Save',
                    actionMethod: this.SaveLeaveBalance,
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
    ErrorMessage: string = ''
    SaveLeaveBalance = () => {
        this.ErrorMessage = '';
        this.submitted = true;
        if (this.LeaveBalanceForm.invalid) {
            this.LeaveBalanceForm.markAllAsTouched();
            return;
        } else {
            if (this.isSaveButtonDisabled) {
                return;
            } else {
                this.isSaveButtonDisabled = true;
                if (this.LeaveBalanceForm.value.LeaveBalanceID > 0) {
                    this.leaveBalanceService.UpdateLeaveBalance(this.LeaveBalanceForm.value.LeaveBalanceID, this.LeaveBalanceForm.value).subscribe(async (resp) => {
                        if (resp['data'] !== null && resp['messageType'] === 1) {
                            this.messageService.add({ severity: 'success', summary: 'Manage Leave saved successfully', detail: '' });
                            this.isSaveButtonDisabled = true;
                            this.loadModalOptions();
                            this.UpdateLeaveList.emit();
                            this.cancel();
                        } else {
                            this.isSaveButtonDisabled = false;
                            this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                        }
                    })
                    this.isSaveButtonDisabled = false;
                }
            }
        }
    }

    cancel = () => {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildLeaveBalanceForm({}, 'New')
        this.AddEditLeaveBalanceModal.hide()
    }

    ResetDialog() {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildLeaveBalanceForm({}, 'New')
        this.AddEditLeaveBalanceModal.hide()
    }

}
