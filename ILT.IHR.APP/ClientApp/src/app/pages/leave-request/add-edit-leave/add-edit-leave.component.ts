import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import * as moment from 'moment';
import { MessageService } from 'primeng/api';
import { forkJoin } from 'rxjs';
import { CommonUtils } from '../../../common/common-utils';
import { Constants, ErrorMsg, Guid, LeaveStatus, ListTypeConstants, SessionConstants } from '../../../constant';
import { EmailFields } from '../../../core/interfaces/Common';
import { IEmailApproval, IEmailApprovalDisplay } from '../../../core/interfaces/EmailApproval';
import { IEmployeeDisplay } from '../../../core/interfaces/Employee';
import { ILeaveDisplay } from '../../../core/interfaces/Leave';
import { IRolePermissionDisplay } from '../../../core/interfaces/RolePermission';
import { IModalPopupAlternateOptions } from '../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { EmailApprovalService } from '../../employee/emailApproval.service';
import { EmployeeService } from '../../employee/employee.service';
import { LookUpService } from '../../lookup/lookup.service';
import { RolePermissionService } from '../../role-permission/role-permission.service';
import { LeaveBalanceService } from '../leave-balance.service';
import { LeaveRequestService } from '../leave-request.service';

@Component({
  selector: 'app-add-edit-leave',
  templateUrl: './add-edit-leave.component.html',
  styleUrls: ['./add-edit-leave.component.scss']
})
export class AddEditLeaveComponent implements OnInit {
    commonUtils = new CommonUtils()
    @Input() lookupId: number = 0;
    @Output() UpdateLeaveList = new EventEmitter<any>();
    @ViewChild('addEditLeaveModal') addEditLeaveModal: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;

    LeaveForm: FormGroup;
    user: any;
    ModalHeading: string = 'Add Leave';
    submitted: boolean
    RolePermissions: IRolePermissionDisplay[] = [];
    EmployeeInfoRolePermission: IRolePermissionDisplay;
    isSaveButtonDisabled: boolean = false;
    ClientID: string;
    EmployeeID: number;
    constructor(private fb: FormBuilder,
        private employeeService: EmployeeService,
        private messageService: MessageService,
        private rolePermissionService: RolePermissionService,
        private leaveRequestService: LeaveRequestService,
        private leaveBalanceService: LeaveBalanceService,
        private emailApprovalService: EmailApprovalService,
        private lookupService: LookUpService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.EmployeeInfoRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.LEAVEREQUEST);
        this.user = JSON.parse(localStorage.getItem("User"));
        this.ClientID = localStorage.getItem("ClientID");
        this.EmployeeID = Number(this.user.employeeID);
        this.GetEmployeeDetails(this.user.employeeID)
        this.LoadDropDown();
        this.buildLeaveForm({}, 'New');
    }

    ngOnInit(): void {
        this.loadModalOptions();
    }
    loadModalOptions() {
        this.modalOptions = {
            footerActions: [
                {
                    actionText: this.isCancel === true ? 'Yes' : 'Save',
                    actionMethod: this.SaveLeave,
                    styleClass: 'btn-width-height p-button-raised p-mr-2 p-mb-2',
                    iconClass: 'mdi mdi-content-save',
                    disabled: this.isSaveButtonDisabled,
                    isConfirm: this.isCancel === true ? true : false,
                },
                {
                    actionText: this.isCancel === true ? 'No' : 'Cancel',
                    actionMethod: this.cancel,
                    styleClass: 'btn-width-height p-button-raised p-button-danger  p-mb-2',
                    iconClass: 'p-button-raised p-button-danger'
                }
            ]
        }
    }

    employeeDetails: IEmployeeDisplay;
    employeeManagerDetails: IEmployeeDisplay;
    GetEmployeeDetails(employeeId: number) {
        if (this.EmployeeID !== undefined && this.EmployeeID !== 0) {
            this.employeeService.getEmployeeByIdAsync(this.EmployeeID).subscribe(result => {
                if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                    this.employeeDetails = result['data'];
                    this.employeeService.getEmployeeByIdAsync(Number(this.employeeDetails.managerID)).subscribe(Managerresponse => {
                        if (Managerresponse['data'] !== null && Managerresponse['messageType'] === 1) {
                            this.employeeManagerDetails = Managerresponse['data'];
                            }
                        })
                }
            }, error => {
                //toastService.ShowError();
            })
        }
    }

    EmployeeList: IEmployeeDisplay[] = [];
    lstEmployees: IEmployeeDisplay[] = [];
    moduleList: any[] = [];
    StatusList: any[] = [];
    VacationTypeList: any[] = [];
    LoadDropDown() {
        forkJoin(
            this.rolePermissionService.getModules(),
            this.employeeService.GetEmployees(),
            this.lookupService.getListValues(),
        ).subscribe(resultSet => {
            if (resultSet !== undefined && resultSet !== null) {
                this.moduleList = resultSet[0];
                this.EmployeeList = resultSet[1]['data']
                this.StatusList = resultSet[2]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.LEAVEREQUESTSTATUS);
                var vacList = resultSet[2]['data'].filter(x => x.type.toUpperCase() === ListTypeConstants.VACATIONTYPE)
                this.VacationTypeList = vacList.filter(x => x.value.toUpperCase() !== ListTypeConstants.LWP.toUpperCase())
            }
        })
    }

    isCancel: boolean = false;
    disabled: boolean = false;
    LeaveID: number;
    isDisabled: boolean = false
    isShowComments: boolean = false;

    Show(Id: number, employeeId: number, cancel: boolean, leaves: ILeaveDisplay[]) {
        this.isCancel = cancel;
        this.disabled = cancel;
        this.LeaveID = Id;
        this.EmployeeID = employeeId;
        this.isDisabled = false;
        this.isShowComments = false;
        this.isSaveButtonDisabled = false;
        this.lstEmployees = this.EmployeeList.filter(x => x.managerID === employeeId || x.employeeID === employeeId);
        this.ResetDialog();
        if (this.LeaveID !== 0) {
            if (this.isCancel === true) {
                this.loadModalOptions();
            }
            this.GetDetails(this.LeaveID);
            this.loadModalOptions();
        }
        else if (employeeId != 0) {
            this.loadModalOptions();
            var currentEmployee = this.EmployeeList.find(x => x.employeeID === employeeId);
            if (currentEmployee.managerID !== null && currentEmployee.managerID !== undefined && currentEmployee.managerID !== 0) {
                this.buildLeaveForm({}, 'New');
                if (this.VacationTypeList !== null && this.VacationTypeList.length == 1) {
                    this.LeaveForm.controls.LeaveTypeID.patchValue(this.VacationTypeList[0].listValueID);
                }
                this.LeaveForm.controls.EmployeeID.patchValue(employeeId);
                this.LeaveForm.controls.RequesterID.patchValue(employeeId);
                this.LeaveForm.controls.StartDate.patchValue(new Date());
                this.LeaveForm.controls.EndDate.patchValue(new Date());
                this.LeaveForm.controls.StatusID.patchValue(this.StatusList.find(x => x.valueDesc.toUpperCase() == "PENDING").listValueID);
                this.getBalanceLeaves(employeeId);
                this.getEmployeeLeaves(employeeId);
                this.duration();
                this.ModalHeading = 'Add Leave'
                this.addEditLeaveModal.show();
            }
            else {
                this.messageService.add({ severity: 'error', summary: `Leave request can't be created for Employee with no manager`, detail: '' });
            }
        }
        else {
            this.messageService.add({ severity: 'error', summary: `Leave request can't be created for non-Employees`, detail: '' });
        }
    }

    Leave: ILeaveDisplay;
    respEmailApproval: IEmailApprovalDisplay;
    GetDetails(Id: number) {
        this.isShowComments = false;
        this.leaveRequestService.getGetLeaveByIdAsync(Id, 0, 0).subscribe(result => {
            if (result !== null && result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                this.buildLeaveForm(result['data'], 'Edit')
                this.Leave = result['data'];
                this.getBalanceLeaves(this.Leave.employeeID);
                this.getEmployeeLeaves(this.Leave.employeeID);
                if (this.Leave.status.toLowerCase() !== "Pending".toLowerCase()) {
                    this.isDisabled = true;
                } else {
                    this.isDisabled = false;
                }
                if (this.isCancel === true) {
                    var StatusID: number = this.StatusList.find(x => x.valueDesc.toUpperCase() === "CANCELLED").listValueID;
                    this.LeaveForm.controls.StatusID.patchValue(StatusID)
                    // this.WFHForm.controls.Title.patchValue({ Title: this.WFH.title, disabled: true })
                }
                if (this.Leave.status.toLowerCase() === "Cancelled".toLowerCase() || this.Leave.status.toLowerCase() == "Approved".toLowerCase() || this.Leave.status.toLowerCase() === "Denied".toLowerCase()) {
                    this.isShowComments = true;
                } else {
                    this.isShowComments = false;
                }
                this.isSaveButtonDisabled = false;
                this.submitted = false;
                this.ModalHeading = "Edit Leave";
                if (this.Leave.linkID !== null && this.Leave.linkID !== Guid.Empty) {
                    this.emailApprovalService.GetEmailApprovalByIdAsync(this.Leave.linkID).subscribe(async (resp) => {
                        if (resp['data'] !== null && resp['messageType'] === 1) {
                            this.respEmailApproval = resp['data'];
                        }
                    })
                }
                this.addEditLeaveModal.show();
            }
        })
    }


    buildLeaveForm(data: any, keyName: string) {
        this.LeaveForm = this.fb.group({
            LeaveID: [keyName === 'New' ? 0 : data.leaveID],
            EmployeeID: [keyName === 'New' ? null : data.employeeID, Validators.required],
            RequesterID: [keyName === 'New' ? null : data.requesterID, Validators.required],
            LeaveTypeID: [keyName === 'New' ? null : data.leaveTypeID, Validators.required],
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
        if (this.employeeDetails !== undefined && this.employeeDetails !== null && keyName === 'New') {
            this.LeaveForm.controls.Approver.patchValue(this.employeeDetails.manager);
            this.LeaveForm.controls.ApproverID.patchValue(this.employeeDetails.managerID);
        }
    }

    get addEditWFHFormControls() { return this.LeaveForm.controls; }

    onEmployeeChange(e: any) {
        if (e.value !== null) {
            this.GetEmployeeDetails(Number(e.value));
            this.getBalanceLeaves(Number(e.value));
            this.getEmployeeLeaves(Number(e.value));
        }
    }
    lstEmployeeLeave: ILeaveDisplay[] =[]
    getEmployeeLeaves(employeeId: any) {
        this.leaveRequestService.GetLeave("EmployeeID", employeeId).subscribe(respLeaveRequest => {
            if (respLeaveRequest['data'] !== null && respLeaveRequest['messageType'] === 1) {
                respLeaveRequest['data'].forEach((d) => {
                    d.startDate = moment(d.startDate).format("MM/DD/YYYY")
                    if (d.endDate !== null) {
                        d.endDate = moment(d.endDate).format("MM/DD/YYYY")
                    }
                })
                this.lstEmployeeLeave = respLeaveRequest['data'];
            } else {
                this.lstEmployeeLeave = [];
            }
        })
    }
    BalanceLeaves: string;
    getBalanceLeaves(EmployeeID: any) {
        if (this.EmployeeID != 0) {
            this.leaveBalanceService.GetLeaveBalance(EmployeeID).subscribe(respLeaveBalance => {
                if (respLeaveBalance['data'] !== null && respLeaveBalance['messageType'] === 1) {
                    var VacationBalance = respLeaveBalance['data'].find(x => x.leaveYear === this.commonUtils.getTodaysYear());
                    console.log(VacationBalance)
                    this.BalanceLeaves = VacationBalance.vacationBalance;
                    this.LeaveForm.controls.BalanceLeaves.patchValue(parseFloat(this.BalanceLeaves))
                } else {
                    this.BalanceLeaves = null;
                }
            })
            //    if (respLeaveBalance.MessageType == MessageType.Success)
            //        LeaveBalanceList = respLeaveBalance.Data.ToList();
            //    else
            //        LeaveBalanceList = new List < LeaveBalance > {};
        }
    }

    startDateChange(event: any) {
        if (event !== null) {
            this.LeaveForm.controls.EndDate.patchValue(new Date(event));
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

    ErrorMessage: string = ''
    SaveLeave = () => {
        this.submitted = true;
        if (this.LeaveForm.invalid) {
            this.LeaveForm.markAllAsTouched();
            return;
        } else {
            if (this.isSaveButtonDisabled)
                return;
            this.isSaveButtonDisabled = true;
            if (this.employeeManagerDetails != null && (this.employeeManagerDetails.termDate === null || new Date() <= this.employeeManagerDetails.termDate)) {
                this.SubmitLeave();
            } else {
                this.messageService.add({ severity: 'error', summary: `Manager (Leave Approver) is inactive`, detail: '' });
                this.isSaveButtonDisabled = false;
            }
            this.isSaveButtonDisabled = false;
        }
    }

    SubmitLeave() {
        if (this.LeaveID === 0) {
            if (this.LeaveForm.value.StartDate <= this.LeaveForm.value.EndDate) {
                this.LeaveForm.value.StartDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.LeaveForm.value.StartDate));
                if (this.LeaveForm.value.EndDate !== null) {
                    this.LeaveForm.value.EndDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.LeaveForm.value.EndDate));
                }
                var existingLeaveIndex = this.lstEmployeeLeave.findIndex(x => ((this.commonUtils.formatDateDefault(x.startDate) <= this.commonUtils.formatDateDefault(this.LeaveForm.value.EndDate) && this.commonUtils.formatDateDefault(this.LeaveForm.value.StartDate) <= this.commonUtils.formatDateDefault(x.endDate)) ||
                    (this.commonUtils.formatDateDefault(x.startDate) >= this.commonUtils.formatDateDefault(this.LeaveForm.value.StartDate) && this.commonUtils.formatDateDefault(x.endDate) <= this.commonUtils.formatDateDefault(this.LeaveForm.value.EndDate))) && (x.statusValue != LeaveStatus.CANCELLED) && (x.statusValue != LeaveStatus.DENIED));
                if (existingLeaveIndex === -1) {
                    console.log(this.LeaveForm.value)
                    this.leaveRequestService.SaveLeave(this.LeaveForm.value).subscribe(result => {
                        if (result['data'] !== null && result['messageType'] === 1) {
                            var emailapproval: IEmailApproval = new IEmailApproval();

                            this.leaveRequestService.getGetLeaveByIdAsync(result['data'].leaveID, 0, 0).subscribe(async (resp) => {
                                if (resp['data'] !== null && resp['messageType'] === 1) {
                                    this.Leave = resp['data'];
                                    emailapproval.EmailApprovalID = 0;
                                    emailapproval.ModuleID = this.moduleList.find(m => m.moduleShort.toUpperCase() == "LEAVEREQUEST").moduleID;
                                    emailapproval.ID = this.Leave.leaveID;
                                    emailapproval.ValidTime = new Date(moment().add(7, 'day').format('YYYY-MM-DD HH:mm:ss'));
                                    emailapproval.IsActive = true;
                                    emailapproval.LinkID = this.commonUtils.newGuid();
                                    emailapproval.ApproverEmail = this.employeeDetails.managerEmail;
                                    emailapproval.CreatedBy = this.user.firstName + " " + this.user.lastName;
                                    var emailFields = await this.prepareEmail();
                                    emailapproval.EmailSubject = emailFields.EmailSubject;
                                    emailapproval.EmailBody = emailFields.EmailBody;
                                    emailapproval.EmailFrom = emailFields.EmailFrom;
                                    emailapproval.EmailTo = emailFields.EmailTo;
                                    await this.emailApprovalService.SaveEmailApproval(emailapproval).subscribe()
                                }
                            })
                            this.messageService.add({ severity: 'success', summary: 'Leave saved successfully', detail: '' });
                            this.isSaveButtonDisabled = true;
                            this.loadModalOptions();
                            this.UpdateLeaveList.emit();
                            this.cancel();
                        } else {
                            this.isSaveButtonDisabled = false;
                            this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                        }
                    })
                } else {
                    this.messageService.add({ severity: 'error', summary: 'Leave exists for current range', detail: '' });
                }
            } else {
                this.messageService.add({ severity: 'error', summary: 'End date must be greater than start date', detail: '' });
            }
        } else {
            this.LeaveForm.value.StartDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.LeaveForm.value.StartDate));
            if (this.LeaveForm.value.EndDate !== null) {
                this.LeaveForm.value.EndDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.LeaveForm.value.EndDate));
            }
            this.leaveRequestService.UpdateLeave(this.LeaveForm.value.LeaveID, this.LeaveForm.value).subscribe(async (result) => {
                if (result['data'] !== null && result['messageType'] === 1) {
                    this.Leave = result['data'];
                    var emailapproval: IEmailApproval = new IEmailApproval();
                    if (this.Leave.linkID !== null && this.Leave.linkID !== Guid.Empty && this.respEmailApproval.linkID !== Guid.Empty) {
                        emailapproval.EmailApprovalID = this.respEmailApproval.emailApprovalID
                        emailapproval.ModuleID = this.respEmailApproval.moduleID
                        emailapproval.ID = this.respEmailApproval.id
                        emailapproval.ValidTime = this.respEmailApproval.validTime
                        emailapproval.IsActive = this.respEmailApproval.isActive
                        emailapproval.Value = this.respEmailApproval.value
                        emailapproval.LinkID = this.respEmailApproval.linkID
                        emailapproval.ApproverEmail = this.respEmailApproval.approverEmail
                        emailapproval.EmailSubject = this.respEmailApproval.emailSubject
                        emailapproval.EmailFrom = this.respEmailApproval.emailFrom
                        emailapproval.EmailTo = this.respEmailApproval.emailTo
                        emailapproval.EmailCC = this.respEmailApproval.emailCC
                        emailapproval.EmailBCC = this.respEmailApproval.emailBCC
                        emailapproval.EmailBody = this.respEmailApproval.emailBody
                        emailapproval.SendDate = this.respEmailApproval.sendDate
                        emailapproval.SentCount = this.respEmailApproval.sentCount
                        emailapproval.ReminderDuration = this.respEmailApproval.reminderDuration
                        emailapproval.CreatedBy = this.respEmailApproval.createdBy
                        emailapproval.CreatedDate = this.respEmailApproval.createdDate
                        emailapproval.ModifiedDate = new Date()
                        emailapproval.ModifiedBy = this.user.firstName + " " + this.user.lastName;
                        emailapproval.SentCount = this.respEmailApproval.sentCount > 0 ? this.respEmailApproval.sentCount - 1 : 0;

                    } else {
                        emailapproval.EmailApprovalID = 0;
                        emailapproval.ModuleID = this.moduleList.find(m => m.moduleShort.toUpperCase() == "LEAVEREQUEST").moduleID;
                        emailapproval.ID = this.Leave.leaveID;
                        emailapproval.IsActive = true;
                        emailapproval.LinkID = this.commonUtils.newGuid();
                        emailapproval.ApproverEmail = this.employeeDetails.managerEmail;
                        emailapproval.CreatedBy = this.user.firstName + " " + this.user.lastName;
                        emailapproval.CreatedDate = new Date();
                    }
                    emailapproval.ValidTime = new Date(moment().add(7, 'day').format('YYYY-MM-DD HH:mm:ss'));
                    var emailFields: EmailFields = new EmailFields()
                    if (this.isCancel) {
                        emailapproval.LinkID = null;
                        emailapproval.LinkID = Guid.Empty;
                        emailFields = await this.prepareCancelMail();
                    }
                    else {
                        emailFields = await this.prepareEmail();
                    }
                    emailapproval.EmailSubject = emailFields.EmailSubject;
                    emailapproval.EmailBody = emailFields.EmailBody;
                    emailapproval.EmailFrom = emailFields.EmailFrom;
                    emailapproval.EmailTo = emailFields.EmailTo;
                    await this.emailApprovalService.SaveEmailApproval(emailapproval).subscribe(result => {
                    })
                    this.messageService.add({ severity: 'success', summary: 'Leave saved successfully', detail: '' });
                    this.isSaveButtonDisabled = true;
                    this.loadModalOptions();
                    this.UpdateLeaveList.emit();
                    this.cancel();
                } else {
                    this.isSaveButtonDisabled = false;
                    this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                }
            })
        }
    }

    prepareEmail() {
        var emailFields: EmailFields = new EmailFields();
        emailFields.EmailTo = this.employeeDetails.managerEmail;
        emailFields.EmailSubject = "Leave request submitted for " + this.employeeDetails.employeeName;
        emailFields.EmailBody = "There is a leave request from " + this.employeeDetails.employeeName +
            " pending your approval<br/>" +
            "<ul style='margin-bottom: 0px;'><li>Type: " + this.Leave.leaveType + "<br/>" +
            "</li><li>Title: " + this.Leave.title +
            "</li><li>Description: " + this.Leave.detail +
            "</li><li>Start Date: " + moment(this.Leave.startDate).format('dddd, MMMM dd') +
            "</li><li>End Date: " + moment(this.Leave.endDate).format('dddd, MMMM dd') +
            "</li><li> Duration: " + this.Leave.duration + " Days" +
            "</ul>";

        return emailFields;
    }

    prepareCancelMail() {
        var emailFields: EmailFields = new EmailFields();
        emailFields.EmailTo = this.employeeDetails.managerEmail;
        emailFields.EmailSubject = "Leave Request Cancelled ";
        emailFields.EmailBody = this.employeeDetails.employeeName + " cancelled his leave" +
            "<ul style='margin-bottom: 0px;'><li>Type: " + this.Leave.leaveType + "<br/>" +
            "</li><li>Title: " + this.Leave.title +
            "</li><li>Description: " + this.Leave.detail +
            "</li><li>Start Date: " + moment(this.Leave.startDate).format('dddd, MMMM dd') +
            "</li><li>End Date: " + moment(this.Leave.endDate).format('dddd, MMMM dd') +
            "</li><li> Duration: " + this.Leave.duration + " Days" +
            "</ul>";
        return emailFields;
    }

 
   cancel = () => {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.isCancel = false;
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
