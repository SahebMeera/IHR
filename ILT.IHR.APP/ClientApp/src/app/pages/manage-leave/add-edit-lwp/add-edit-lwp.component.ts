import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import * as moment from 'moment';
import { MessageService } from 'primeng/api';
import { forkJoin } from 'rxjs';
import { CommonUtils } from '../../../common/common-utils';
import { Constants, Guid, ListTypeConstants, SessionConstants, Settings } from '../../../constant';
import { IEmployeeDisplay } from '../../../core/interfaces/Employee';
import { IRolePermissionDisplay } from '../../../core/interfaces/RolePermission';
import { IModalPopupAlternateOptions } from '../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { EmployeeService } from '../../employee/employee.service';
import { LeaveRequestService } from '../../leave-request/leave-request.service';
import { LookUpService } from '../../lookup/lookup.service';
import { UserService } from '../../user/user.service';

@Component({
  selector: 'app-add-edit-lwp',
  templateUrl: './add-edit-lwp.component.html',
  styleUrls: ['./add-edit-lwp.component.scss']
})
export class AddEditLwpComponent implements OnInit {
    settings = new Settings()
    commonUtils = new CommonUtils()
    @Output() UpdateLeaveList = new EventEmitter<any>();
    @ViewChild('addEditLeaveModal') addEditLeaveModal: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;
    user: any;
    submitted: boolean = false;
    moment: any = moment;
    ClientID: string;

    LeaveForm: FormGroup;
    RolePermissions: IRolePermissionDisplay[] = [];
    EmployeeInfoRolePermission: IRolePermissionDisplay;
    ExpenseId: number;
    ModalHeading: string = 'Add Unpaid Leave';
    isSaveButtonDisabled: boolean = false;
    isShow: boolean = true;
    dateTime = new Date();
    EmployeeID: number;

    constructor(private fb: FormBuilder,
        private lookupService: LookUpService,
        private messageService: MessageService,
        private leaveRequestService: LeaveRequestService,
        private employeeService: EmployeeService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.EmployeeInfoRolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.EMPLOYEEINFO);
        this.user = JSON.parse(localStorage.getItem("User"));
        this.EmployeeID = this.user.employeeID
        this.ClientID = localStorage.getItem("ClientID");
        this.dateTime.setDate(this.dateTime.getDate());
        this.GetEmployeeDetails();
        this.LoadDropDown();
        this.buildLeaveForm({}, 'New');
    }

    ngOnInit(): void {
        this.loadModalOptions()
  }
    employeeDetails: IEmployeeDisplay;
    GetEmployeeDetails() {
      //  this.LoadTableConfig();
        if (this.EmployeeID !== undefined && this.EmployeeID !== 0) {
            this.employeeService.getEmployeeByIdAsync(this.EmployeeID).subscribe(result => {
                if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                    this.employeeDetails = result['data'];
                }
            }, error => {
                //toastService.ShowError();
            })
        }
    }
    EmployeeList: IEmployeeDisplay[] = []
    VacationTypeList: any[] = [];
    StatusList: any[] = [];
    LoadDropDown() {
        forkJoin(
            this.employeeService.GetEmployees(),
            this.lookupService.getListValues(),
        ).subscribe(async resultSet => {
            if (resultSet !== undefined && resultSet !== null) {
                this.EmployeeList = resultSet[0]['data'];
                var vacList = resultSet[1]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.VACATIONTYPE);
                this.VacationTypeList = vacList.filter(x => x.value.toUpperCase() == ListTypeConstants.LWP);
                this.StatusList = resultSet[1]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.LEAVEREQUESTSTATUS);
            }
        })
    }

    loadModalOptions() {
        this.modalOptions = {
            footerActions: [
                {
                    actionText: 'Save',
                    actionMethod: this.SaveLeave,
                    styleClass: 'btn-width-height p-button-raised p-mr-2 p-mb-2',
                    iconClass: 'mdi mdi-content-save',
                    disabled: this.isSaveButtonDisabled,
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
    LeaveID: number;
    isDisabled: boolean = false
    isShowComments: boolean = false;
    lstEmployees: IEmployeeDisplay[] = [];
    Show(Id: number,  employeeId: number) {

        this.LeaveID = Id;
        this.EmployeeID = employeeId;
        this.isDisabled = false;
        this.isShowComments = false;
        this.lstEmployees = this.EmployeeList.filter(x => x.employeeID == employeeId);
        this.ResetDialog();

        if (employeeId != 0) {
            this.loadModalOptions();
                this.buildLeaveForm({}, 'New');
                if (this.VacationTypeList !== null && this.VacationTypeList.length == 1) {
                    this.LeaveForm.controls.LeaveTypeID.patchValue(this.VacationTypeList[0].listValueID);
                }
                this.LeaveForm.controls.EmployeeID.patchValue(employeeId);
                this.LeaveForm.controls.RequesterID.patchValue(employeeId);
                this.LeaveForm.controls.StartDate.patchValue(new Date());
                this.LeaveForm.controls.EndDate.patchValue(new Date());
                this.LeaveForm.controls.StatusID.patchValue(this.StatusList.find(x => x.valueDesc.toUpperCase() == "APPROVED").listValueID);
                this.LeaveForm.controls.Detail.patchValue('Unpaid Leave');
                this.LeaveForm.controls.Title.patchValue('Unpaid Leave');
                this.duration();
                this.ModalHeading = 'Add Unpaid Leave'
                this.addEditLeaveModal.show();
        }
        else {
            this.messageService.add({ severity: 'error', summary: `Leave request can't be created for non-Employees`, detail: '' });
        }
    }

    buildLeaveForm(data: any, keyName: string) {
        this.LeaveForm = this.fb.group({
            LeaveID: [keyName === 'New' ? 0 : data.leaveID],
            EmployeeID: [keyName === 'New' ? null : data.employeeID, Validators.required],
            RequesterID: [keyName === 'New' ? null : this.user.employeeID, Validators.required],
            LeaveTypeID: [keyName === 'New' ? null : data.leaveTypeID, Validators.required],
            LeaveType: [keyName === 'New' ? '' : data.leaveType],
            ApproverID: [keyName === 'New' ? null : data.approverID],
            Approver: [keyName === 'New' ? '' : data.approver],
            StatusID: [keyName === 'New' ? null : data.statusID, Validators.required],
            Title: [keyName === 'New' ? '' : data.title, Validators.required],
            Detail: [keyName === 'New' ? '' : data.detail],
            IncludesHalfDay: [keyName === 'New' ? false : data.includesHalfDay],
            Duration: [keyName === 'New' ? 0 : data.duration !== null ? parseFloat(data.duration) : 0],
            Comment: [keyName === 'New' ? '' : data.comment],
            BalanceLeaves: [keyName === 'New' ? null : data.duration !== null ? parseFloat(data.duration) : 0],
            EmployeeName: [keyName === 'New' ? '' : data.employeeName],
            LinkID: [keyName === 'New' ? Guid.Empty : data.linkID],
            StartDate: [keyName === 'New' ? null : data.startDate !== null ? new Date(data.startDate) : null, Validators.required],
            EndDate: [keyName === 'New' ? null : data.endDate !== null ? new Date(data.endDate) : null, Validators.required],
            CreatedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : this.user.firstName + " " + this.user.lastName],
            CreatedDate: [keyName === 'New' ? new Date() : data.createdDate],
            ModifiedDate: [keyName === 'New' ? new Date() : data.modifiedDate],
            ModifiedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : data.modifiedBy],
            TimeStamp: ['']
        });
        if (this.user !== undefined && this.user !== null && keyName === 'New') {
            this.LeaveForm.controls.Approver.patchValue(this.user.firstName + " " + this.user.lastName);
            this.LeaveForm.controls.ApproverID.patchValue(this.user.employeeID);
        }
    }
    get addEditWFHFormControls() { return this.LeaveForm.controls; }

    startDateChange(event: any) {
        if (event !== null) {
            this.LeaveForm.controls.EndDate.patchValue(new Date(event));
            this.duration();
        }
    }

    endDateChange(event: any) {
        if (event !== null) {
            //this.LeaveForm.controls.EndDate.patchValue(new Date(event));
            this.duration();
        }
    }


    duration() {
        if (this.LeaveForm.value.StartDate <= this.LeaveForm.value.EndDate || this.LeaveForm.value.StartDate.Date === this.LeaveForm.value.EndDate.Date) {
            this.leaveRequestService.GetLeaveDays(this.ClientID, Number(this.EmployeeID), this.LeaveForm.value.StartDate, this.LeaveForm.value.EndDate, this.LeaveForm.value.IncludesHalfDay).subscribe(result => {
                if (result['data'] !== null && result['messageType'] === 1) {
                    this.LeaveForm.controls.Duration.patchValue(result['data'].duration);
                }
            })
        }
        else {
            this.messageService.add({ severity: 'error', summary: `End date must be greater than start date`, detail: '' });
        }

    }

    SaveLeave = () => {
        this.submitted = true;
        if (this.LeaveForm.invalid) {
            this.LeaveForm.markAllAsTouched();
            return;
        } else {
            if (this.isSaveButtonDisabled)
                return;
            this.isSaveButtonDisabled = true;
            //if (this.employeeManagerDetails != null && (this.employeeManagerDetails.termDate === null || new Date() <= this.employeeManagerDetails.termDate)) {
                this.SubmitLeave();
            //} else {
            //    this.messageService.add({ severity: 'error', summary: `Manager (Leave Approver) is inactive`, detail: '' });
            //    this.isSaveButtonDisabled = false;
            //}
            this.isSaveButtonDisabled = false;
        }
    }

    onCheckChange(event: any) {
       // this.LeaveForm.controls.IncludesHalfDay.patchValue()
       this.duration();
       
    }

    SubmitLeave() {
        if (this.LeaveID === 0) {
            if (this.LeaveForm.value.StartDate <= this.LeaveForm.value.EndDate ) {
                this.LeaveForm.controls.CreatedBy.patchValue(this.user.firstName + " " + this.user.lastName);
                this.LeaveForm.controls.ApproverID.patchValue(this.user.employeeID);
                this.LeaveForm.controls.Approver.patchValue(this.user.firstName + " " + this.user.lastName);
                this.LeaveForm.controls.LeaveType.patchValue(this.VacationTypeList.find(x => x.listValueID == this.LeaveForm.value.LeaveTypeID).valueDesc)
                this.leaveRequestService.SaveLeave(this.LeaveForm.value).subscribe(async result => {
                    if (result['data'] !== null && result['messageType'] === 1) {
                        this.messageService.add({ severity: 'success', summary: 'Unpaid Leave saved successfully', detail: '' });
                        await this.UpdateLeaveList.emit(true);
                        this.cancel();
                    }
                    else {
                        this.messageService.add({ severity: 'error', summary: `Error occured`, detail: '' });
                    }
                })
            }
            else {

                this.messageService.add({ severity: 'error', summary: `End date must be greater than start date`, detail: '' });
            }
        } else {
            this.messageService.add({ severity: 'error', summary: `Error occured`, detail: '' });
        }
    }

    cancel = () => {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.isDisabled = false;
        this.isShowComments = false;
        this.buildLeaveForm({}, 'New');
        this.addEditLeaveModal.hide();
    }

    ResetDialog() {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildLeaveForm({}, 'New');
        this.addEditLeaveModal.hide();
    }

}
