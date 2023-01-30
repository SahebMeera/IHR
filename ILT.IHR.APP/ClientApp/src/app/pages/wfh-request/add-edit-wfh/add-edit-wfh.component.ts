import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import * as moment from 'moment';
import { MessageService } from 'primeng/api';
import { forkJoin } from 'rxjs';
import { CommonUtils } from '../../../common/common-utils';
import { Constants, EmailApprovalUrl, ErrorMsg, Guid, ListTypeConstants, SessionConstants } from '../../../constant';
import { EmailFields } from '../../../core/interfaces/Common';
import { IEmailApproval, IEmailApprovalDisplay } from '../../../core/interfaces/EmailApproval';
import { IEmployeeDisplay } from '../../../core/interfaces/Employee';
import { IModuleDisplay } from '../../../core/interfaces/Module';
import { IRolePermissionDisplay } from '../../../core/interfaces/RolePermission';
import { IWFHDisplay } from '../../../core/interfaces/WFH';
import { IModalPopupAlternateOptions } from '../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { EmailApprovalService } from '../../employee/emailApproval.service';
import { EmployeeService } from '../../employee/employee.service';
import { AddEditLookupComponent } from '../../lookup/add-edit-lookup/add-edit-lookup.component';
import { LookUpService } from '../../lookup/lookup.service';
import { RolePermissionService } from '../../role-permission/role-permission.service';
import { WFHService } from '../wfh.service';

@Component({
  selector: 'app-add-edit-wfh',
  templateUrl: './add-edit-wfh.component.html',
  styleUrls: ['./add-edit-wfh.component.scss']
})
export class AddEditWfhComponent implements OnInit {
    commonUtils = new CommonUtils()
    @Input() lookupId: number = 0;
    @Output() UpdateWFHList = new EventEmitter<any>();
    @ViewChild('addEditWFHModal') addEditWFHModalpopup: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;

    WFHForm: FormGroup;
    user: any;
    ModalHeading: string = 'Add WFH';
    submitted: boolean
    RolePermissions: IRolePermissionDisplay[] = [];
    EmployeeInfoRolePermission: IRolePermissionDisplay;
    isSaveButtonDisabled: boolean = false;

    EmployeeID: number;

    constructor(private fb: FormBuilder,
        private employeeService: EmployeeService,
        private messageService: MessageService,
        private rolePermissionService: RolePermissionService,
        private wfhService: WFHService,
        private emailApprovalService: EmailApprovalService,
        private lookupService: LookUpService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.EmployeeInfoRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.WFHREQUEST);
        this.user = JSON.parse(localStorage.getItem("User"));
        this.EmployeeID = Number(this.user.employeeID);
        this.GetEmployeeDetails(this.user.employeeID)
        this.LoadDropDown();
        this.buildWFHForm({}, 'New');
    }

    ngOnInit(): void {
        this.loadModalOptions();
    }

    loadModalOptions() {
        this.modalOptions = {
            footerActions: [
                {
                    actionText: this.isCancel === true ? 'Yes' : 'Save',
                    actionMethod:  this.save,
                    styleClass:  'btn-width-height p-button-raised p-mr-2 p-mb-2',
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
    isCancel: boolean = false;
    disabled: boolean = false;
    WFHID: number;
    isDisabled: boolean = false
    isShowComments: boolean = false;
    Show(Id: number, employeeId: number, cancel: boolean) {
        this.isCancel = cancel;
        this.disabled = cancel;
        this.WFHID = Id;
        this.EmployeeID = employeeId;
        this.isDisabled = false;
        this.isShowComments = false;
        this.isSaveButtonDisabled = false;
        this.lstEmployees = this.EmployeeList.filter(x => x.managerID === employeeId || x.employeeID === employeeId);
        this.ResetDialog();
        if (this.WFHID !== 0) {
            if (this.isCancel === true) {
                this.loadModalOptions();
            }
            this.GetDetails(this.WFHID);
            this.loadModalOptions();
        }
        else if (employeeId != 0) {
            this.loadModalOptions();
            var currentEmployee = this.EmployeeList.find(x => x.employeeID === employeeId);
            if (currentEmployee.managerID !== null && currentEmployee.managerID !== undefined  && currentEmployee.managerID !== 0) {
                this.buildWFHForm({}, 'New');
                this.WFHForm.controls.EmployeeID.patchValue(employeeId);
                this.WFHForm.controls.RequesterID.patchValue(employeeId);
                this.WFHForm.controls.StartDate.patchValue(new Date());
                this.WFHForm.controls.EndDate.patchValue(new Date());
                this.WFHForm.controls.StatusID.patchValue(this.StatusList.find(x => x.valueDesc.toUpperCase() == "PENDING").listValueID);
                this.addEditWFHModalpopup.show();
            }
            else {
                this.messageService.add({severity: 'error', summary: `WFH request can't be created for Employee with no manager`, detail: '' });
            }
        }
        else {
            this.messageService.add({ severity: 'error', summary: `WFH request can't be created for non-Employees`, detail: '' });
        }
    }
    WFH: IWFHDisplay;
    GetDetails(Id: number) {
        this.isShowComments = false;
        this.wfhService.GetWFHByIdAsync(Id, 0, 0).subscribe(result => {
            if (result !== null && result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                this.buildWFHForm(result['data'], 'Edit')
                this.WFH = result['data'];
                if (this.WFH.status.toLowerCase() !== "Pending".toLowerCase()) {
                    this.isDisabled = true;
                } else {
                    this.isDisabled = false;
                }
                if (this.isCancel === true) {
                    var StatusID: number = this.StatusList.find(x => x.valueDesc.toUpperCase() === "CANCELLED").listValueID;
                    this.WFHForm.controls.StatusID.patchValue(StatusID)
                   // this.WFHForm.controls.Title.patchValue({ Title: this.WFH.title, disabled: true })
                }
                if (this.WFH.status.toLowerCase() === "Cancelled".toLowerCase() || this.WFH.status.toLowerCase() == "Approved".toLowerCase() || this.WFH.status.toLowerCase() === "Denied".toLowerCase()) {
                   this.isShowComments = true;
                } else {
                    this.isShowComments = false;
                }
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



    buildWFHForm(data: any, keyName: string) {
        this.WFHForm = this.fb.group({
            WFHID: [keyName === 'New' ? 0 : data.wfhid],
            EmployeeID: [keyName === 'New' ? null : data.employeeID, Validators.required],
            RequesterID: [keyName === 'New' ? null : data.requesterID, Validators.required],
            ApproverID: [keyName === 'New' ? null : data.approverID],
            Approver: [keyName === 'New' ? '' : data.approver],
            StatusID: [keyName === 'New' ? null : data.statusID, Validators.required],
            Title: [keyName === 'New' ?  '': data.title, Validators.required],
            Comment: [keyName === 'New' ? '' : data.comment],
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
            this.WFHForm.controls.Approver.patchValue(this.employeeDetails.manager);
            this.WFHForm.controls.ApproverID.patchValue(this.employeeDetails.managerID);
        }
    }
    get addEditWFHFormControls() { return this.WFHForm.controls; }

    onEmployeeChange(e: any) {
        if (e.value !== null) {
            this.GetEmployeeDetails(Number(e.value));
        }
    }
    respEmailApproval: IEmailApprovalDisplay;
     save = () => {
        this.submitted = true;
        if (this.WFHForm.invalid) {
            this.WFHForm.markAllAsTouched();
            return;
        } else {
            if (this.isSaveButtonDisabled) {
                return;
            } else {
                this.isSaveButtonDisabled = true;
                
                if (this.WFHID === 0) {
                    if (this.WFHForm.value.StartDate <= this.WFHForm.value.EndDate) {
                        this.WFHForm.value.StartDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.WFHForm.value.StartDate));
                        if (this.WFHForm.value.EndDate !== null) {
                            this.WFHForm.value.EndDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.WFHForm.value.EndDate));
                        }
                        this.wfhService.SaveWFH(this.WFHForm.value).subscribe(result => {
                            if (result['data'] !== null && result['messageType'] === 1) {
                                var emailapproval: IEmailApproval = new IEmailApproval();

                                this.wfhService.GetWFHByIdAsync(result['data'].wfhid, 0, 0).subscribe(async (resp) => {
                                    if (resp['data'] !== null && resp['messageType'] === 1) {
                                        this.WFH = resp['data'];
                                        emailapproval.EmailApprovalID = 0;
                                        emailapproval.ModuleID = this.moduleList.find(m => m.moduleShort.toUpperCase() == "WFHREQUEST").moduleID;
                                        emailapproval.ID = this.WFH.wfhid;
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
                                        emailapproval.CreatedBy = this.user.firstName + " " + this.user.lastName;
                                        emailapproval.CreatedDate = new Date()
                                        await this.emailApprovalService.SaveEmailApproval(emailapproval).subscribe()
                                    }
                                })
                                this.messageService.add({ severity: 'success', summary: 'WFH saved successfully', detail: '' });
                                this.isSaveButtonDisabled = true;
                                this.loadModalOptions();
                                this.UpdateWFHList.emit();
                                this.cancel();
                            } else {
                                this.isSaveButtonDisabled = false;
                                this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                            }
                        })
                    } else {
                        this.messageService.add({ severity: 'error', summary: 'End date must be greater than start date', detail: '' });
                    }
                } else {
                    this.WFHForm.value.StartDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.WFHForm.value.StartDate));
                    if (this.WFHForm.value.EndDate !== null) {
                        this.WFHForm.value.EndDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.WFHForm.value.EndDate));
                    }
                    this.wfhService.UpdateWFH(this.WFHForm.value.WFHID, this.WFHForm.value).subscribe(async (result) => {
                        if (result['data'] !== null && result['messageType'] === 1) {
                            this.WFH = result['data'];
                            var emailapproval: IEmailApproval = new IEmailApproval();
                            if (this.WFH.linkID !== null && this.WFH.linkID !== Guid.Empty && this.respEmailApproval.linkID !== Guid.Empty) {
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
                                emailapproval.ModuleID = this.moduleList.find(m => m.moduleShort.toUpperCase() == "WFHREQUEST").moduleID;
                                emailapproval.ID = this.WFH.wfhid;
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
                            this.messageService.add({ severity: 'success', summary: 'WFH saved successfully', detail: '' });
                            this.isSaveButtonDisabled = true;
                            this.loadModalOptions();
                            this.UpdateWFHList.emit();
                            this.cancel();
                        } else {
                            this.isSaveButtonDisabled = false;
                            this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                        }
                    })
                }
            }
            this.isSaveButtonDisabled = false;
        }
    }


     prepareEmail() {
         var emailFields: EmailFields = new EmailFields();
        emailFields.EmailTo = this.employeeDetails.managerEmail;
        emailFields.EmailSubject = "WFH request submitted for " + this.employeeDetails.employeeName;
        emailFields.EmailBody = "There is a WFH request from " + this.employeeDetails.employeeName +
            " pending for your approval<br/>" +
            "<ul style='margin-bottom: 0px;'><li>Title: " + this.WFH.title +
            "</li><li>Start Date: " + moment(this.WFH.startDate).format('dddd, MMMM dd')+
            "</li><li>End Date: " + moment(this.WFH.endDate).format('dddd, MMMM dd') +
            "</li></ul>";

        return emailFields;
    }

    prepareCancelMail() {
        var emailFields: EmailFields = new EmailFields();
        emailFields.EmailTo = this.employeeDetails.managerEmail;
        emailFields.EmailSubject = "WFH request cancelled for " + this.employeeDetails.employeeName;
        emailFields.EmailBody = this.employeeDetails.employeeName + " cancelled his WFH" +
            "<ul style='margin-bottom: 0px;'><li>Title: " + this.WFH.title +
            "</li><li>Start Date: " + moment(this.WFH.startDate).format('dddd, MMMM dd') +
            "</li><li>End Date: " + moment(this.WFH.endDate).format('dddd, MMMM dd') +
            "</li></ul>";
        return emailFields;
    }


    startDateChange(event: any) {
        if (event !== null) {
            this.WFHForm.controls.EndDate.patchValue(new Date(event));
        }
    }
    cancel = () => {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.isCancel = false;
        this.isDisabled = false;
        this.isShowComments = false;
        this.buildWFHForm({}, 'New');
        this.addEditWFHModalpopup.hide();
    }

    ResetDialog() {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildWFHForm({}, 'New');
        this.addEditWFHModalpopup.hide();
    }

}
