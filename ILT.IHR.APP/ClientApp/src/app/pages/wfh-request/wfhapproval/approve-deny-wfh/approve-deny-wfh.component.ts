import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { async } from 'rxjs';
import {  forkJoin } from 'rxjs';
import { CommonUtils } from '../../../../common/common-utils';
import { Constants, ErrorMsg, Guid, ListTypeConstants, SessionConstants, Settings } from '../../../../constant';
import { EmailFields } from '../../../../core/interfaces/Common';
import { IEmailApproval, IEmailApprovalDisplay } from '../../../../core/interfaces/EmailApproval';
import { IEmployeeDisplay } from '../../../../core/interfaces/Employee';
import { IModuleDisplay } from '../../../../core/interfaces/Module';
import { IRolePermissionDisplay } from '../../../../core/interfaces/RolePermission';
import { IWFHDisplay } from '../../../../core/interfaces/WFH';
import { IModalPopupAlternateOptions } from '../../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { EmailApprovalService } from '../../../employee/emailApproval.service';
import { EmployeeService } from '../../../employee/employee.service';
import { LookUpService } from '../../../lookup/lookup.service';
import { RolePermissionService } from '../../../role-permission/role-permission.service';
import { WFHService } from '../../wfh.service';
import * as CryptoJS from 'crypto-js';
import * as moment from 'moment';


@Component({
  selector: 'app-approve-deny-wfh',
  templateUrl: './approve-deny-wfh.component.html',
  styleUrls: ['./approve-deny-wfh.component.scss']
})
export class ApproveDenyWfhComponent implements OnInit {
    commonUtils = new CommonUtils()
    settings = new Settings();
    @Output() UpdateWFHList = new EventEmitter<any>();
    @ViewChild('addEditWFHApproveModal') addEditWFHModalpopup: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;

    WFHApproveDenyForm: FormGroup;
    user: any;
    ModalHeading: string = 'Add WFH';
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
        private wfhService: WFHService,
        private emailApprovalService: EmailApprovalService,
        private lookupService: LookUpService) {
        var ClientID: string = localStorage.getItem("ClientID");
        this.AccountsEmail = this.settings.EmailNotifications[ClientID]['WFHRequest']
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.EmployeeInfoRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.WFHREQUEST);
        this.user = JSON.parse(localStorage.getItem("User"));
        this.EmployeeID = Number(this.user.employeeID);
        this.GetEmployeeDetails(this.user.employeeID)
        this.LoadDropDown();
        this.buildWFHApproveForm({}, 'New');
    }



    ngOnInit(): void {
        this.loadModalOptions();
    }
    loadModalOptions() {
        this.modalOptions = {
            footerActions: [
                {
                    actionText: 'Approve',
                    actionMethod: this.Approve,
                    styleClass: 'btn-width-height p-button-raised p-mr-2 p-mb-2',
                    iconClass: 'mdi mdi-content-save',
                    disabled: this.WFHApproveDenyForm.value.Status.toLowerCase() === "Approved".toLowerCase() || this.WFHApproveDenyForm.value.Status.toLowerCase() !== "Pending".toLowerCase() || this.isApproveButtonDisabled
                },
                {
                    actionText: 'Deny',
                    actionMethod: this.WFHDeny,
                    styleClass: 'btn-width-height p-button-raised p-button-danger p-mr-2 p-mb-2',
                    iconClass: 'p-button-raised p-button-danger',
                    disabled: this.WFHApproveDenyForm.value.Status.toLowerCase() === "Denied".toLowerCase() || this.WFHApproveDenyForm.value.Status.toLowerCase() !== "Pending".toLowerCase() || this.isDenyButtonDisabled
                },
                {
                    actionText: 'Cancel WFH',
                    actionMethod: this.WFHCancel,
                    styleClass: 'btn-width-height p-button-raised p-button-danger p-mr-2 p-mb-2',
                    iconClass: 'p-button-raised p-button-danger',
                    disabled: this.WFHApproveDenyForm.value.Status.toLowerCase() === "Approved".toLowerCase() || !this.isCancelButtonDisabled ? false : true
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

    EmployeeList: IEmployeeDisplay[] = [];
    lstEmployees: IEmployeeDisplay[] = [];
    moduleList: any[] = [];
    StatusList: any[] = [];
    LoadDropDown() {
        forkJoin(
            this.rolePermissionService.getModules(),
            this.employeeService.GetEmployees(),
            this.lookupService.getListValues(),
        ).subscribe(resultSet => {
            if (resultSet !== undefined && resultSet !== null) {
                this.moduleList = resultSet[0];
                this.EmployeeList = resultSet[1]['data']
                this.StatusList = resultSet[2]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.WFHSTATUS);
            }
        })
    }


    buildWFHApproveForm(data: any, keyName: string) {
        this.WFHApproveDenyForm = this.fb.group({
            WFHID: [keyName === 'New' ? 0 : data.wfhid],
            EmployeeID: [keyName === 'New' ? null : data.employeeID, Validators.required],
            RequesterID: [keyName === 'New' ? null : data.requesterID, Validators.required],
            ApproverID: [keyName === 'New' ? null : data.approverID],
            Approver: [keyName === 'New' ? '' : data.approver],
            StatusID: [keyName === 'New' ? null : data.statusID, Validators.required],
            Status: [keyName === 'New' ? '' : data.status],
            Title: [keyName === 'New' ? '' : data.title, Validators.required],
            Comment: [keyName === 'New' ? '' : data.comment],
            EmployeeName: [keyName === 'New' ? '' : data.employeeName],
            LinkID: [keyName === 'New' ? Guid.Empty : data.linkID],
            StartDate: [keyName === 'New' ? null : data.startDate !== null ? new Date(data.startDate) : null],
            EndDate: [keyName === 'New' ? null : data.endDate !== null ? new Date(data.endDate) : null],
            CreatedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : this.user.firstName + " " + this.user.lastName],
            CreatedDate: [keyName === 'New' ? new Date() : data.createdDate],
            ModifiedDate: [keyName === 'New' ? new Date() : data.modifiedDate],
            ModifiedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : data.modifiedBy],
            TimeStamp: ['']
        });
    }

    get addEditWFHFormControls() { return this.WFHApproveDenyForm.controls; }
    WFHID: number;
    Show(Id: number) {
        this.WFHID = Id;
        this.ResetDialog();
        if (this.WFHID != 0) {
           this.GetDetails(this.WFHID);
        }
    }
    respEmailApproval: IEmailApprovalDisplay;
    WFH: IWFHDisplay;
    GetDetails(Id: number) {
        this.wfhService.GetWFHByIdAsync(Id, 0, 0).subscribe(result => {
            if (result !== null && result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                this.buildWFHApproveForm(result['data'], 'Edit')
                this.loadModalOptions();
                this.WFH = result['data'];
                this.isSaveButtonDisabled = false;
                this.submitted = false;
                this.ModalHeading = "Edit WFH";
                if (this.WFH.linkID !== null && this.WFH.linkID !== Guid.Empty) {
                    this.emailApprovalService.GetEmailApprovalByIdAsync(this.WFH.linkID).subscribe(async (resp) => {
                        if (resp['data'] !== null && resp['messageType'] === 1) {
                            this.respEmailApproval = resp['data'];
                        }
                    })
                }
                this.addEditWFHModalpopup.show();
            }
        })
    }
    cancel = () => {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildWFHApproveForm({}, 'New');
        this.addEditWFHModalpopup.hide();
    }

    ResetDialog() {
        this.submitted = false;
        this.WFH = new IWFHDisplay();
        this.isSaveButtonDisabled = false;
        this.buildWFHApproveForm({}, 'New');
        this.addEditWFHModalpopup.hide();
    }

   
     Approve = () => {
        if (this.isApproveButtonDisabled)
            return;
        this.isApproveButtonDisabled = true;
        var ClientID: string = localStorage.getItem("ClientID");
         this.WFHApproveDenyForm.controls.StatusID.patchValue(this.StatusList.find(x => x.valueDesc.toUpperCase() == "APPROVED").listValueID)
         this.WFHApproveDenyForm.value.StartDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.WFHApproveDenyForm.value.StartDate));
         if (this.WFHApproveDenyForm.value.EndDate !== null) {
             this.WFHApproveDenyForm.value.EndDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.WFHApproveDenyForm.value.EndDate));
         }
         this.wfhService.UpdateWFH(this.WFHApproveDenyForm.value.WFHID, this.WFHApproveDenyForm.value).subscribe(async (result) => {
             if (result['data'] !== null && result['messageType'] === 1) {
                 var emailApproval: IEmailApproval = new IEmailApproval();
                 if (this.WFH.linkID !== null && this.WFH.linkID !== Guid.Empty  && this.respEmailApproval.linkID !== Guid.Empty) {
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
                     await this.emailApprovalService.EamilApprovalAction(ClientID, this.WFH.linkID, "APPROVED", "WFH").subscribe()
                 } else {
                     emailApproval.ModuleID = this.moduleList.find(m => m.moduleShort.toUpperCase() == "WFHREQUEST").moduleID;
                     emailApproval.ID = this.WFH.wfhid;
                     emailApproval.ValidTime = new Date(moment().add(7, 'day').format('YYYY-MM-DD HH:mm:ss'));
                     emailApproval.IsActive = true;
                     emailApproval.LinkID = this.commonUtils.newGuid();
                     emailApproval.ApproverEmail = await this.GetEmail(this.WFH.approverID);
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
                 await this.emailApprovalService.SaveEmailApproval(emailApproval).subscribe(res => { });
                 this.messageService.add({ severity: 'success', summary: 'WFH saved successfully', detail: '' });
                 this.isSaveButtonDisabled = true;
                 this.loadModalOptions();
                 this.UpdateWFHList.emit();
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
        var RequesterEmail: string  = await this.GetEmail(this.WFH.employeeID);
        var ApproverEmail: string = await this.GetEmail(this.WFH.approverID);
        emailFields.EmailTo = RequesterEmail;
        emailFields.EmailCCList = [];
        emailFields.EmailCCList.push(this.AccountsEmail);
        emailFields.EmailCCList.push(ApproverEmail);
        //emailFields.EmailCC = emailFields.EmailCCList.Aggregate((x, y) => x + ";" + y); // String.Join(";", emailFields.EmailCCList.ToArray());
        //common.EmailCC=AccountsEmail;
        emailFields.EmailSubject = "WFH request approved for " + this.WFH.employeeName;
        emailFields.EmailBody = "WFH Request for " + this.WFH.employeeName +
            " has been approved.<br/>" +
            "<ul style='margin-bottom: 0px;'><li>Title: " + this.WFH.title +
            "</li><li>Start Date: " + moment(this.WFH.startDate).format('dddd, MMMM dd') +
            "</li><li>End Date: " + moment(this.WFH.endDate).format('dddd, MMMM dd') +
            "</li>";
        if (this.WFH.comment !== "" && this.WFH.comment !== null && this.WFH.comment !== undefined) {
            emailFields.EmailBody = emailFields.EmailBody + "</li><li>Comments: " + this.WFH.comment + "</ul>";
        }
        return emailFields;
    }

    WFHDeny = () => {
        if (this.isDenyButtonDisabled) {
            return;
        } else {
            this.isDenyButtonDisabled = true;
            var ClientID: string = localStorage.getItem("ClientID");
            this.WFHApproveDenyForm.controls.StatusID.patchValue(this.StatusList.find(x => x.valueDesc.toUpperCase() == "DENIED").listValueID)
            this.WFHApproveDenyForm.value.StartDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.WFHApproveDenyForm.value.StartDate));
            if (this.WFHApproveDenyForm.value.EndDate !== null) {
                this.WFHApproveDenyForm.value.EndDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.WFHApproveDenyForm.value.EndDate));
            }
            this.wfhService.UpdateWFH(this.WFHApproveDenyForm.value.WFHID, this.WFHApproveDenyForm.value).subscribe(async (result) => {
                if (result['data'] !== null && result['messageType'] === 1) {
                    var emailApproval: IEmailApproval = new IEmailApproval();
                    if (this.WFH.linkID !== null && this.WFH.linkID !== Guid.Empty && this.respEmailApproval.linkID !== Guid.Empty) {
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
                        await this.emailApprovalService.EamilApprovalAction(ClientID, this.WFH.linkID, "DENIED", "WFH").subscribe()
                    }
                    else {
                        emailApproval.ModuleID = this.moduleList.find(m => m.moduleShort.toUpperCase() == "WFHREQUEST").moduleID;
                        emailApproval.ID = this.WFH.wfhid;
                        emailApproval.ValidTime = new Date(moment().add(7, 'day').format('YYYY-MM-DD HH:mm:ss'));
                        emailApproval.IsActive = true;
                        emailApproval.LinkID = this.commonUtils.newGuid();
                        emailApproval.ApproverEmail = await this.GetEmail(this.WFH.approverID);
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
                    this.messageService.add({ severity: 'success', summary: 'WFH saved successfully', detail: '' });
                    this.isSaveButtonDisabled = true;
                    this.loadModalOptions();
                    this.UpdateWFHList.emit();
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
        var emailFields: EmailFields  = new EmailFields();
        emailFields.EmailTo = await this.GetEmail(this.WFH.employeeID);
        emailFields.EmailCC = await this.GetEmail(this.WFH.approverID);
        emailFields.EmailSubject = "WFH request denied for " + this.WFH.employeeName;
        emailFields.EmailBody = "WFH request for " + this.WFH.employeeName +
            " has been denied.<br/>" +
            "<ul style='margin-bottom: 0px;'><li>Title: " + this.WFH.title +
            "</li><li>Start Date: " + moment(this.WFH.startDate).format('dddd, MMMM dd') +
            "</li><li>End Date: " + this.WFH.endDate !== null ? moment(this.WFH.endDate).format('dddd, MMMM dd') : "" +
            "</li>";
        if (this.WFH.comment !== "" && this.WFH.comment !== null  && this.WFH.comment !== undefined) {
            emailFields.EmailBody = emailFields.EmailBody + "</li><li>Comments: " + this.WFH.comment + "</ul>";
        }
        return emailFields;
    }

    WFHCancel = () => {
        if (this.isCancelButtonDisabled) {
            return;
        } else {
            this.isCancelButtonDisabled = true;
            var ClientID: string = localStorage.getItem("ClientID");
            this.WFHApproveDenyForm.controls.StatusID.patchValue(this.StatusList.find(x => x.valueDesc.toUpperCase() == "CANCELLED").listValueID)
            this.WFHApproveDenyForm.value.StartDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.WFHApproveDenyForm.value.StartDate));
            if (this.WFHApproveDenyForm.value.EndDate !== null) {
                this.WFHApproveDenyForm.value.EndDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.WFHApproveDenyForm.value.EndDate));
            }
            this.wfhService.UpdateWFH(this.WFHApproveDenyForm.value.WFHID, this.WFHApproveDenyForm.value).subscribe(async (result) => {
                if (result['data'] !== null && result['messageType'] === 1) {
                    var emailApproval: IEmailApproval = new IEmailApproval();
                    if (this.WFH.linkID !== null && this.WFH.linkID !== Guid.Empty && this.respEmailApproval.linkID !== Guid.Empty) {
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
                        await this.emailApprovalService.EamilApprovalAction(ClientID, this.WFH.linkID, "CANCELLED", "WFH").subscribe()
                    }
                    else {
                        emailApproval.ModuleID = this.moduleList.find(m => m.moduleShort.toUpperCase() == "WFHREQUEST").moduleID;
                        emailApproval.ID = this.WFH.wfhid;
                        emailApproval.ValidTime = new Date(moment().add(7, 'day').format('YYYY-MM-DD HH:mm:ss'));
                        emailApproval.IsActive = true;
                        emailApproval.LinkID = this.commonUtils.newGuid();
                        emailApproval.ApproverEmail = await this.GetEmail(this.WFH.approverID);
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
                    this.messageService.add({ severity: 'success', summary: 'WFH saved successfully', detail: '' });
                    this.isSaveButtonDisabled = true;
                    this.loadModalOptions();
                    this.UpdateWFHList.emit();
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
         var RequesterEmail: string = await this.GetEmail(this.WFH.employeeID);
         var ApproverEmail: string = await this.GetEmail(this.WFH.approverID);
        emailFields.EmailTo = RequesterEmail;
        emailFields.EmailCCList = []
        emailFields.EmailCCList.push(this.AccountsEmail);
        emailFields.EmailCCList.push(ApproverEmail);
        // emailFields.EmailCC = emailFields.EmailCCList.Aggregate((x, y) => x + ";" + y);
        //common.EmailCC=AccountsEmail;
        emailFields.EmailSubject = "Approved WFH Request Cancelled";
        emailFields.EmailBody = "Below approved WFH Request for " + this.WFH.employeeName +
            " has been cancelled.<br/>" +
            "<ul style='margin-bottom: 0px;'><li>Title: " + this.WFH.title +
            "</li><li>Start Date: " + moment(this.WFH.startDate).format('dddd, MMMM dd') +
            "</li><li>End Date: " + moment(this.WFH.endDate).format('dddd, MMMM dd') +
            "</li>";
         if (this.WFH.comment !== "" && this.WFH.comment !== null && this.WFH.comment !== undefined) {
            emailFields.EmailBody = emailFields.EmailBody + "</li><li>Comments: " + this.WFH.comment + "</ul>";
        }
        return emailFields;
    }

    GetEmail(EmployeeID: number): string {
        return this.EmployeeList && this.EmployeeList.filter(x => x.employeeID === EmployeeID).length > 0
            ? this.EmployeeList.find(x => x.employeeID === EmployeeID).loginEmail : this.EmployeeList.find(x => x.employeeID === EmployeeID).email;
    }
}
