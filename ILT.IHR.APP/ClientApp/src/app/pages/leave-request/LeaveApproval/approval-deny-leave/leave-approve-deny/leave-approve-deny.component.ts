import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import * as moment from 'moment';
import { MessageService } from 'primeng/api';
import { forkJoin } from 'rxjs';
import { CommonUtils } from '../../../../../common/common-utils';
import { Constants, ErrorMsg, Guid, ListTypeConstants, SessionConstants, Settings } from '../../../../../constant';
import { EmailFields } from '../../../../../core/interfaces/Common';
import { IEmailApproval, IEmailApprovalDisplay } from '../../../../../core/interfaces/EmailApproval';
import { IEmployeeDisplay } from '../../../../../core/interfaces/Employee';
import { ILeaveDisplay } from '../../../../../core/interfaces/Leave';
import { IRolePermissionDisplay } from '../../../../../core/interfaces/RolePermission';
import { IModalPopupAlternateOptions } from '../../../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { EmailApprovalService } from '../../../../employee/emailApproval.service';
import { EmployeeService } from '../../../../employee/employee.service';
import { LookUpService } from '../../../../lookup/lookup.service';
import { RolePermissionService } from '../../../../role-permission/role-permission.service';
import { LeaveBalanceService } from '../../../leave-balance.service';
import { LeaveRequestService } from '../../../leave-request.service';

@Component({
  selector: 'app-leave-approve-deny',
  templateUrl: './leave-approve-deny.component.html',
  styleUrls: ['./leave-approve-deny.component.scss']
})
export class LeaveApproveDenyComponent implements OnInit {
    commonUtils = new CommonUtils()
    settings = new Settings();
    @Output() UpdateLeaveList = new EventEmitter<any>();
    @ViewChild('addEditLeaveApproveModal') addEditLeaveApproveModalpopup: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;

    LeaveForm: FormGroup;
    user: any;
    ModalHeading: string = 'Add Leave';
    submitted: boolean
    RolePermissions: IRolePermissionDisplay[] = [];
    EmployeeInfoRolePermission: IRolePermissionDisplay;
    isSaveButtonDisabled: boolean = false;

    isApproveButtonDisabled: boolean = false;
    isDenyButtonDisabled: boolean = false;
    isCancelButtonDisabled: boolean = false;
    AccountsEmail: string;
    EmployeeID: number;

    constructor(private fb: FormBuilder,
        private employeeService: EmployeeService,
        private messageService: MessageService,
        private rolePermissionService: RolePermissionService,
        private leaveBalanceService: LeaveBalanceService,
        private leaveRequestService: LeaveRequestService,
        private emailApprovalService: EmailApprovalService,
        private lookupService: LookUpService) {
        var ClientID: string = localStorage.getItem("ClientID");
        this.AccountsEmail = this.settings.EmailNotifications[ClientID]['LeaveRequest']
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.EmployeeInfoRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.LEAVEREQUEST);
        this.user = JSON.parse(localStorage.getItem("User"));
        this.EmployeeID = Number(this.user.employeeID);
        this.GetEmployeeDetails(this.user.employeeID)
        this.LoadDropDown();
        this.buildLeaveForm({}, 'New');
    }

    ngOnInit(): void {
        this.loadModalOptions();
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
                this.VacationTypeList = resultSet[2]['data'].filter(x => x.type.toUpperCase() === ListTypeConstants.VACATIONTYPE)
               // this.VacationTypeList = vacList.filter(x => x.value.toUpperCase() !== ListTypeConstants.LWP.toUpperCase())
            }
        })
    }

    LeaveID: number = 0;
    isCancel: boolean = true;
    Show(Id: number) {
        //this.LeaveID = 0;
        this.LeaveID = Id;
        this.ResetDialog();
        if (this.LeaveID !== 0) {
            this.GetDetails(this.LeaveID);
        }
    }

    respEmailApproval: IEmailApprovalDisplay;
    Leave: ILeaveDisplay;
    GetDetails(Id: number) {
        this.leaveRequestService.getGetLeaveByIdAsync(Id, 0, 0).subscribe(result => {
            if (result !== null && result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                this.buildLeaveForm(result['data'], 'Edit')
                this.loadModalOptions();
                this.Leave = result['data'];
                this.getBalanceLeaves(this.Leave.employeeID)
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
                this.addEditLeaveApproveModalpopup.show();
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
            Status: [keyName === 'New' ? '' : data.status],
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
    }

    loadModalOptions() {
        this.modalOptions = {
            //footerActions: []
            footerActions: [
                {
                    actionText: 'Approve',
                    actionMethod: this.Approve,
                    styleClass: 'btn-width-height p-button-raised p-mr-2 p-mb-2',
                    iconClass: 'mdi mdi-content-save',
                    disabled: this.LeaveForm.value.Status.toLowerCase() === "Approved".toLowerCase() || this.LeaveForm.value.Status.toLowerCase() !== "Pending".toLowerCase() || this.isApproveButtonDisabled
                },
                {
                    actionText: 'Deny',
                    actionMethod: this.LeaveDeny,
                    styleClass: 'btn-width-height p-button-raised p-button-danger p-mr-2 p-mb-2',
                    iconClass: 'p-button-raised p-button-danger',
                    disabled: this.LeaveForm.value.Status.toLowerCase() === "Denied".toLowerCase() || this.LeaveForm.value.Status.toLowerCase() !== "Pending".toLowerCase() || this.isDenyButtonDisabled
                },
                {
                    actionText: 'Cancel Leave',
                    actionMethod: this.LeaveCancel,
                    styleClass: 'btn-width-height p-button-raised p-button-danger p-mr-2 p-mb-2',
                    iconClass: 'p-button-raised p-button-danger',
                    disabled: this.LeaveForm.value.Status.toLowerCase() === "Approved".toLowerCase() ? false : true  || !this.isCancelButtonDisabled
                },
                {
                    actionText: 'Close',
                    actionMethod: this.cancel,
                    styleClass: 'btn-width-height p-button-raised p-button-danger  p-mb-2',
                    iconClass: 'p-button-raised p-button-danger'
                }
            ]
        }
    }

    BalanceLeaves: string;
    getBalanceLeaves(EmployeeID: any) {
        if (this.EmployeeID != 0) {
            this.leaveBalanceService.GetLeaveBalance(EmployeeID).subscribe(respLeaveBalance => {
                if (respLeaveBalance['data'] !== null && respLeaveBalance['messageType'] === 1) {
                    var VacationBalance = respLeaveBalance['data'].find(x => x.leaveYear === this.commonUtils.getTodaysYear());
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

    employeeDetails: IEmployeeDisplay;
    GetEmployeeDetails(employeeId: number) {
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



    Approve = () => {
        if (this.isApproveButtonDisabled)
            return;
        this.isApproveButtonDisabled = true;
        var ClientID: string = localStorage.getItem("ClientID");
        this.LeaveForm.controls.StatusID.patchValue(this.StatusList.find(x => x.valueDesc.toUpperCase() == "APPROVED").listValueID)
        this.LeaveForm.value.StartDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.LeaveForm.value.StartDate));
        if (this.LeaveForm.value.EndDate !== null) {
            this.LeaveForm.value.EndDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.LeaveForm.value.EndDate));
        }
        this.leaveRequestService.UpdateLeave(this.LeaveForm.value.LeaveID, this.LeaveForm.value).subscribe(async (result) => {
            if (result['data'] !== null && result['messageType'] === 1) {
                this.Leave.comment = result['data'].comment;
                var emailApproval: IEmailApproval = new IEmailApproval();
                if (this.Leave.linkID !== null && this.Leave.linkID !== Guid.Empty && this.respEmailApproval.linkID !== Guid.Empty) {
                    emailApproval.EmailApprovalID = this.respEmailApproval.emailApprovalID
                    emailApproval.ModuleID = this.respEmailApproval.moduleID
                    emailApproval.ID = this.respEmailApproval.id
                    emailApproval.ValidTime = this.respEmailApproval.validTime
                    emailApproval.IsActive = this.respEmailApproval.isActive
                    emailApproval.Value = this.respEmailApproval.value
                    emailApproval.LinkID = this.respEmailApproval.linkID
                    emailApproval.ApproverEmail = this.respEmailApproval.approverEmail
                    emailApproval.EmailSubject = this.respEmailApproval.emailSubject
                    emailApproval.EmailFrom = this.respEmailApproval.emailFrom
                    emailApproval.EmailTo = this.respEmailApproval.emailTo
                    emailApproval.EmailCC = this.respEmailApproval.emailCC
                    emailApproval.EmailBCC = this.respEmailApproval.emailBCC
                    emailApproval.EmailBody = this.respEmailApproval.emailBody
                    emailApproval.SendDate = this.respEmailApproval.sendDate
                    emailApproval.SentCount = this.respEmailApproval.sentCount
                    emailApproval.ReminderDuration = this.respEmailApproval.reminderDuration
                    emailApproval.CreatedBy = this.respEmailApproval.createdBy
                    emailApproval.CreatedDate = this.respEmailApproval.createdDate
                    emailApproval.ModifiedDate = new Date()
                    emailApproval.ModifiedBy = this.user.firstName + " " + this.user.lastName;
                    await this.emailApprovalService.EamilApprovalAction(ClientID, this.Leave.linkID, "APPROVED", "LEAVE").subscribe(res => {
                    })
                } else {
                    emailApproval.ModuleID = this.moduleList.find(m => m.moduleShort.toUpperCase() == "LEAVEREQUEST").moduleID;
                    emailApproval.ID = this.Leave.leaveID;
                    emailApproval.ValidTime = new Date(moment().add(7, 'day').format('YYYY-MM-DD HH:mm:ss'));
                    emailApproval.IsActive = true;
                    emailApproval.LinkID = this.commonUtils.newGuid();
                    emailApproval.ApproverEmail = await this.GetEmail(this.Leave.approverID);
                    emailApproval.CreatedBy = this.user.firstName + " " + this.user.lastName;
                }
                var emailFields: EmailFields = await this.prepareApprovedMail();
                emailApproval.EmailApprovalID = 0;
                emailApproval.LinkID = Guid.Empty;
                emailApproval.EmailSubject = emailFields.EmailSubject;
                emailApproval.EmailBody = emailFields.EmailBody;
                emailApproval.EmailCC = emailFields.EmailCC;
                emailApproval.EmailTo = emailFields.EmailTo;
                emailApproval.Value = null;
                emailApproval.IsActive = true;
                await this.emailApprovalService.SaveEmailApproval(emailApproval).subscribe(res => {
                })
                this.messageService.add({ severity: 'success', summary: 'Leave saved successfully', detail: '' })
                this.isSaveButtonDisabled = true;
                this.loadModalOptions();
                this.UpdateLeaveList.emit();
                this.cancel();
            } else {
                this.isSaveButtonDisabled = false;
                this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
            }

            this.isApproveButtonDisabled = false;
        })
    }

    async prepareApprovedMail() {
        var emailFields: EmailFields = new EmailFields();
        emailFields.isMultipleEmail = true;
        var RequesterEmail: string = await this.GetEmail(this.Leave.employeeID);
        var ApproverEmail: string = await this.GetEmail(this.Leave.approverID);
        emailFields.EmailTo = RequesterEmail;
        emailFields.EmailCCList = [];
        emailFields.EmailCCList.push(this.AccountsEmail);
        emailFields.EmailCCList.push(ApproverEmail);
        emailFields.EmailCC = emailFields.EmailCCList.join(';');
            //emailFields.EmailCCList.Aggregate((x, y) => x + ";" + y);
        // String.Join(";", emailFields.EmailCCList.ToArray());
        //common.EmailCC=AccountsEmail;
        emailFields.EmailSubject = "Leave request approved for " + this.Leave.employeeName;
        emailFields.EmailBody = "Leave Request for " + this.Leave.employeeName +
            " has been approved.<br/>" +
            "<ul style='margin-bottom: 0px;'><li>Type: " + this.Leave.leaveType + "<br/>" +
            "</li><li>Title: " + this.Leave.title +
            "</li><li>Description: " + this.Leave.detail +
            "</li><li>Start Date: " + moment(this.Leave.startDate).format('dddd, MMMM dd') +
            "</li><li>End Date: " + moment(this.Leave.endDate).format('dddd, MMMM dd') +
            "</li><li>Duration: " + this.Leave.duration + " Days";
            "</li>";
        if (this.Leave.comment !== "" && this.Leave.comment !== null && this.Leave.comment !== undefined) {
            emailFields.EmailBody = emailFields.EmailBody + "</li><li>Comments: " + this.Leave.comment + "</ul>";
        } else {
            emailFields.EmailBody += "</ul>";
        }
        return emailFields;
    }

    LeaveDeny = () => {
        if (this.isDenyButtonDisabled) {
            return;
        } else {
            this.isDenyButtonDisabled = true;
            var ClientID: string = localStorage.getItem("ClientID");
            this.LeaveForm.controls.StatusID.patchValue(this.StatusList.find(x => x.valueDesc.toUpperCase() == "DENIED").listValueID)
            this.LeaveForm.value.StartDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.LeaveForm.value.StartDate));
            if (this.LeaveForm.value.EndDate !== null) {
                this.LeaveForm.value.EndDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.LeaveForm.value.EndDate));
            }
            this.leaveRequestService.UpdateLeave(this.LeaveForm.value.LeaveID, this.LeaveForm.value).subscribe(async (result) => {
                if (result['data'] !== null && result['messageType'] === 1) {
                    this.Leave.comment = result['data'].comment;
                    var emailApproval: IEmailApproval = new IEmailApproval();
                    if (this.Leave.linkID !== null && this.Leave.linkID !== Guid.Empty && this.respEmailApproval.linkID !== Guid.Empty) {
                        emailApproval.EmailApprovalID = this.respEmailApproval.emailApprovalID
                        emailApproval.ModuleID = this.respEmailApproval.moduleID
                        emailApproval.ID = this.respEmailApproval.id
                        emailApproval.ValidTime = this.respEmailApproval.validTime
                        emailApproval.IsActive = this.respEmailApproval.isActive
                        emailApproval.Value = this.respEmailApproval.value
                        emailApproval.LinkID = this.respEmailApproval.linkID
                        emailApproval.ApproverEmail = this.respEmailApproval.approverEmail
                        emailApproval.EmailSubject = this.respEmailApproval.emailSubject
                        emailApproval.EmailFrom = this.respEmailApproval.emailFrom
                        emailApproval.EmailTo = this.respEmailApproval.emailTo
                        emailApproval.EmailCC = this.respEmailApproval.emailCC
                        emailApproval.EmailBCC = this.respEmailApproval.emailBCC
                        emailApproval.EmailBody = this.respEmailApproval.emailBody
                        emailApproval.SendDate = this.respEmailApproval.sendDate
                        emailApproval.SentCount = this.respEmailApproval.sentCount
                        emailApproval.ReminderDuration = this.respEmailApproval.reminderDuration
                        emailApproval.CreatedBy = this.respEmailApproval.createdBy
                        emailApproval.CreatedDate = this.respEmailApproval.createdDate
                        emailApproval.ModifiedDate = new Date()
                        emailApproval.ModifiedBy = this.user.firstName + " " + this.user.lastName;
                        await this.emailApprovalService.EamilApprovalAction(ClientID, this.Leave.linkID, "DENIED", "LEAVE").subscribe()
                    }
                    else {
                        emailApproval.ModuleID = this.moduleList.find(m => m.moduleShort.toUpperCase() == "LEAVEREQUEST").moduleID;
                        emailApproval.ID = this.Leave.leaveID;
                        emailApproval.ValidTime = new Date(moment().add(7, 'day').format('YYYY-MM-DD HH:mm:ss'));
                        emailApproval.IsActive = true;
                        emailApproval.LinkID = this.commonUtils.newGuid();
                        emailApproval.ApproverEmail = await this.GetEmail(this.Leave.approverID);
                        emailApproval.CreatedBy = this.user.firstName + " " + this.user.lastName;
                        emailApproval.CreatedDate = new Date()
                    }

                    var emailFields = await this.prepareDenyMail();
                    emailApproval.EmailApprovalID = 0;
                    emailApproval.LinkID = Guid.Empty;
                    emailApproval.EmailSubject = emailFields.EmailSubject;
                    emailApproval.EmailBody = emailFields.EmailBody;
                    emailApproval.EmailCC = emailFields.EmailCC;
                    emailApproval.EmailTo = emailFields.EmailTo;
                    emailApproval.Value = null;
                    emailApproval.IsActive = true;
                    await this.emailApprovalService.SaveEmailApproval(emailApproval).subscribe()
                    this.messageService.add({ severity: 'success', summary: 'Leave saved successfully', detail: '' });
                    this.isSaveButtonDisabled = true;
                    this.loadModalOptions();
                    this.UpdateLeaveList.emit();
                    this.cancel();
                } else {
                    this.isSaveButtonDisabled = false;
                    this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                }
                this.isDenyButtonDisabled = false;
            })
        }


    }

    async prepareDenyMail() {
        var emailFields: EmailFields = new EmailFields();
        emailFields.EmailTo = await this.GetEmail(this.Leave.employeeID);
        emailFields.EmailCC = await this.GetEmail(this.Leave.approverID);
        emailFields.EmailSubject = "Leave request denied for " + this.Leave.employeeName;
        emailFields.EmailBody = "Leave Request for " + this.Leave.employeeName +
            " has been denied.<br/>" +
            "<ul style='margin-bottom: 0px;'><li>Type: " + this.Leave.leaveType + "<br/>" +
            "</li><li>Title: " + this.Leave.title +
            "</li><li>Description: " + this.Leave.detail +
            "</li><li>Start Date: " + moment(this.Leave.startDate).format('dddd, MMMM dd') +
            "</li><li>End Date: " + moment(this.Leave.endDate).format('dddd, MMMM dd') +
            "</li><li>Duration: " + this.Leave.duration + " Days";
        "</li>";
        if (this.Leave.comment !== "" && this.Leave.comment !== null && this.Leave.comment !== undefined) {
            emailFields.EmailBody = emailFields.EmailBody + "</li><li>Comments: " + this.Leave.comment + "</ul>";
        } else {
            emailFields.EmailBody += "</ul>";
        }
        return emailFields;
    }



    LeaveCancel = () => {
        if (this.isCancelButtonDisabled) {
            return;
        } else {
            this.isCancelButtonDisabled = true;
            var ClientID: string = localStorage.getItem("ClientID");
            this.LeaveForm.controls.StatusID.patchValue(this.StatusList.find(x => x.valueDesc.toUpperCase() == "CANCELLED").listValueID)
            this.LeaveForm.value.StartDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.LeaveForm.value.StartDate));
            if (this.LeaveForm.value.EndDate !== null) {
                this.LeaveForm.value.EndDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.LeaveForm.value.EndDate));
            }
            this.leaveRequestService.UpdateLeave(this.LeaveForm.value.LeaveID, this.LeaveForm.value).subscribe(async (result) => {
                if (result['data'] !== null && result['messageType'] === 1) {
                    this.Leave.comment = result['data'].comment;
                    var emailApproval: IEmailApproval = new IEmailApproval();
                    if (this.Leave.linkID !== null && this.Leave.linkID !== Guid.Empty && this.respEmailApproval.linkID !== Guid.Empty) {
                        emailApproval.EmailApprovalID = this.respEmailApproval.emailApprovalID
                        emailApproval.ModuleID = this.respEmailApproval.moduleID
                        emailApproval.ID = this.respEmailApproval.id
                        emailApproval.ValidTime = this.respEmailApproval.validTime
                        emailApproval.IsActive = this.respEmailApproval.isActive
                        emailApproval.Value = this.respEmailApproval.value
                        emailApproval.LinkID = this.respEmailApproval.linkID
                        emailApproval.ApproverEmail = this.respEmailApproval.approverEmail
                        emailApproval.EmailSubject = this.respEmailApproval.emailSubject
                        emailApproval.EmailFrom = this.respEmailApproval.emailFrom
                        emailApproval.EmailTo = this.respEmailApproval.emailTo
                        emailApproval.EmailCC = this.respEmailApproval.emailCC
                        emailApproval.EmailBCC = this.respEmailApproval.emailBCC
                        emailApproval.EmailBody = this.respEmailApproval.emailBody
                        emailApproval.SendDate = this.respEmailApproval.sendDate
                        emailApproval.SentCount = this.respEmailApproval.sentCount
                        emailApproval.ReminderDuration = this.respEmailApproval.reminderDuration
                        emailApproval.CreatedBy = this.respEmailApproval.createdBy
                        emailApproval.CreatedDate = this.respEmailApproval.createdDate
                        emailApproval.ModifiedDate = new Date()
                        emailApproval.ModifiedBy = this.user.firstName + " " + this.user.lastName;
                        await this.emailApprovalService.EamilApprovalAction(ClientID, this.Leave.linkID, "CANCELLED", "Leave").subscribe()
                    }
                    else {
                        emailApproval.ModuleID = this.moduleList.find(m => m.moduleShort.toUpperCase() == "LEAVEREQUEST").moduleID;
                        emailApproval.ID = this.Leave.leaveID;
                        emailApproval.ValidTime = new Date(moment().add(7, 'day').format('YYYY-MM-DD HH:mm:ss'));
                        emailApproval.IsActive = true;
                        emailApproval.LinkID = this.commonUtils.newGuid();
                        emailApproval.ApproverEmail = await this.GetEmail(this.Leave.approverID);
                        emailApproval.CreatedBy = this.user.firstName + " " + this.user.lastName;
                        emailApproval.CreatedDate = new Date();
                    }

                    var emailFields = await this.prepareCancelMail();
                    emailApproval.EmailApprovalID = 0;
                    emailApproval.LinkID = Guid.Empty;
                    emailApproval.EmailSubject = emailFields.EmailSubject;
                    emailApproval.EmailBody = emailFields.EmailBody;
                    emailApproval.EmailCC = emailFields.EmailCC;
                    emailApproval.EmailTo = emailFields.EmailTo;
                    emailApproval.Value = null;
                    emailApproval.IsActive = true;
                    await this.emailApprovalService.SaveEmailApproval(emailApproval).subscribe()
                    this.messageService.add({ severity: 'success', summary: 'Leave saved successfully', detail: '' });
                    this.isSaveButtonDisabled = true;
                    this.loadModalOptions();
                    this.UpdateLeaveList.emit();
                    this.cancel();
                } else {
                    this.isSaveButtonDisabled = false;
                    this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                }
                this.isCancelButtonDisabled = false;
            })
        }
    }


    async prepareCancelMail() {
        var emailFields: EmailFields = new EmailFields();
        emailFields.isMultipleEmail = true;
        emailFields.EmailCCList = [];
        var RequesterEmail: string = await this.GetEmail(this.Leave.employeeID);
        var ApproverEmail: string = await this.GetEmail(this.Leave.approverID);
        emailFields.EmailTo = RequesterEmail;
        emailFields.EmailCCList = []
        emailFields.EmailCCList.push(this.AccountsEmail);
        emailFields.EmailCCList.push(ApproverEmail);
        emailFields.EmailCC = emailFields.EmailCCList.join( "; ");
        //common.EmailCC=AccountsEmail;
        emailFields.EmailSubject = "Leave request cancelled for" + this.Leave.employeeName;
        emailFields.EmailBody = "Below approved Leave Request for " + this.Leave.employeeName +
            " has been cancelled.<br/>" +
            "<ul style='margin-bottom: 0px;'><li>Type: " + this.Leave.leaveType + "<br/>" +
            "</li><li>Title: " + this.Leave.title +
            "</li><li>Description: " + this.Leave.detail +
            "</li><li>Start Date: " + moment(this.Leave.startDate).format('dddd, MMMM dd') +
            "</li><li>End Date: " + moment(this.Leave.endDate).format('dddd, MMMM dd') +
            "</li><li>Duration: " + this.Leave.duration + " Days";
        "</li>";
        if (this.Leave.comment !== "" && this.Leave.comment !== null && this.Leave.comment !== undefined) {
            emailFields.EmailBody = emailFields.EmailBody + "</li><li>Comments: " + this.Leave.comment + "</ul>";
        } else {
            emailFields.EmailBody += "</ul>";
        }
        return emailFields;
    }

    cancel = () => {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildLeaveForm({}, 'New');
        this.addEditLeaveApproveModalpopup.hide();
    }

    ResetDialog() {
        this.submitted = false;
        this.Leave = new ILeaveDisplay();
        this.isSaveButtonDisabled = false;
        this.buildLeaveForm({}, 'New');
        this.addEditLeaveApproveModalpopup.hide();
    }

    GetEmail(EmployeeID: number): string {
        return this.EmployeeList && this.EmployeeList.filter(x => x.employeeID === EmployeeID).length > 0
            ? this.EmployeeList.find(x => x.employeeID === EmployeeID).loginEmail : this.EmployeeList.find(x => x.employeeID === EmployeeID).email;
    }

}
