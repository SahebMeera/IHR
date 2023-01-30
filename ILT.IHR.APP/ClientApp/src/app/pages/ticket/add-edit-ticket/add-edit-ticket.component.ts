import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import * as moment from 'moment';
import { MessageService } from 'primeng/api';
import { forkJoin } from 'rxjs';
import { CommonUtils } from '../../../common/common-utils';
import { Constants, ErrorMsg, Guid, ListTypeConstants, SessionConstants, Settings, TicketStatusConstants, UserRole } from '../../../constant';
import { EmailFields } from '../../../core/interfaces/Common';
import { IEmailApproval, IEmailApprovalDisplay } from '../../../core/interfaces/EmailApproval';
import { IEmployeeDisplay } from '../../../core/interfaces/Employee';
import { IRolePermissionDisplay } from '../../../core/interfaces/RolePermission';
import { ITicketForDisplay } from '../../../core/interfaces/Ticket';
import { IModalPopupAlternateOptions } from '../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { EmailApprovalService } from '../../employee/emailApproval.service';
import { EmployeeService } from '../../employee/employee.service';
import { LookUpService } from '../../lookup/lookup.service';
import { RolePermissionService } from '../../role-permission/role-permission.service';
import { UserService } from '../../user/user.service';
import { TicketService } from '../ticket.service';

@Component({
  selector: 'app-add-edit-ticket',
  templateUrl: './add-edit-ticket.component.html',
  styleUrls: ['./add-edit-ticket.component.scss']
})
export class AddEditTicketComponent implements OnInit {
  settings = new Settings();
  commonUtils = new CommonUtils()
  @Output() TicketUpdated = new EventEmitter<any>();
    @Output() WizadTicketUpdatedList = new EventEmitter<any>();
  @ViewChild('addEditTicketModal') addEditTicketModal: IHRModalPopupComponent;
  modalOptions: IModalPopupAlternateOptions;

  TicketForm: FormGroup;
  user: any;
  ModalHeading: string = 'Add Ticket';
  submitted: boolean
  RolePermissions: IRolePermissionDisplay[] = [];
  TicketInfoRolePermission: IRolePermissionDisplay;
  isSaveButtonDisabled: boolean = false;
  ClientID: string;
  EmployeeID: number;
  TicketEmail: string;
  isCommentExist: boolean = false;

    constructor(private fb: FormBuilder,
        private employeeService: EmployeeService,
        private messageService: MessageService,
        private rolePermissionService: RolePermissionService,
        private emailApprovalService: EmailApprovalService,
        private userService: UserService,
        private ticketService: TicketService,
        private lookupService: LookUpService) {
        this.ClientID = localStorage.getItem("ClientID");
        this.TicketEmail = this.settings.EmailNotifications[this.ClientID]['LeaveRequest']
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.TicketInfoRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.TICKET);
        this.user = JSON.parse(localStorage.getItem("User"));
        this.LoadDropDown();
        this.buildTicketForm({}, 'New');
    }

    ngOnInit(): void {
        this.loadModalOptions();
    }
    isResolveButtonDisabled: boolean = false;
    loadModalOptions() {
        this.modalOptions = {
            footerActions: [
                {
                    actionText: 'Resolve',
                    actionMethod: this.resolve,
                    styleClass: !(this.TicketForm.value.AssignedToID != null && this.TicketForm.value.AssignedToID == this.EmployeeID && this.TicketForm.value.Status != null && this.TicketForm.value.Status != '' && this.TicketForm.value.Status.toUpperCase() === TicketStatusConstants.ASSIGNED) === true ? 'btn-width-height p-button-raised p-mr-2 p-mb-2 display-none' : 'btn-width-height p-button-raised p-mr-2 p-mb-2',
                    iconClass: 'mdi mdi-content-save',
                    disabled: this.isResolveButtonDisabled || !(this.TicketForm.value.AssignedToID != null && this.TicketForm.value.AssignedToID == this.EmployeeID && this.TicketForm.value.Status != null && this.TicketForm.value.Status != '' && this.TicketForm.value.Status.toUpperCase() === TicketStatusConstants.ASSIGNED),
                },
                {
                    actionText: 'Save',
                    actionMethod: this.Save,
                    styleClass: this.isSaveVisable === true ? 'btn-width-height p-button-raised p-mr-2 p-mb-2 display-none' : 'btn-width-height p-button-raised p-mr-2 p-mb-2' ,
                    iconClass: 'mdi mdi-content-save',
                    disabled: this.isSaveButtonDisabled || this.isSaveVisable,
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

    Employees: IEmployeeDisplay[] = [];
    Modules: any[] = [];
    lstEmployees: IEmployeeDisplay[] = [];
    TicketEmailMapList: any[] = [];
    TicketTypeList: any[] = [];
    TicketStatusList: any[] = [];
    UsersList: any[] = [];
    LoadDropDown() {
        forkJoin(
            this.employeeService.GetEmployees(),
            this.lookupService.getListValues(),
            this.rolePermissionService.getModules(),
            this.userService.getUserList()
        ).subscribe(resultSet => {
            if (resultSet !== undefined && resultSet !== null) {
                this.Employees = resultSet[0]['data']
                this.TicketEmailMapList = resultSet[1]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.TICKETEMAILMAP);
                this.TicketTypeList = resultSet[1]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.TICKETTYPE);
                this.TicketStatusList = resultSet[1]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.TICKETSTATUS);
                this.Modules = resultSet[2];
                this.UsersList = resultSet[3]['data'];
            }
        })
    }
    TicketId: number;
    WizardDataID: number;
    disabledvalue: boolean = false;
    isCommentDisable: boolean = false;
    Show(Id: number, employeeId: number, WizardDataId: number) {
        console.log(Id)
        this.TicketId = Id;
        this.EmployeeID = employeeId;
        this.WizardDataID = WizardDataId;
       // this.ResetDialog();
        if (this.TicketId != 0) {
            this.disabledvalue = true;
            this.isAssignedDisable = false;
            this.LoadDropDown()
            this.GetDetails(this.TicketId);
            this.lstEmployees = this.Employees;

        }
        else {
            this.isTicketTypeDescription = false;
            this.lstEmployees = this.Employees.filter(x => x.managerID === employeeId || x.employeeID === employeeId);
            this.ModalHeading = "Add Ticket";
            this.disabledvalue = false;
            this.buildTicketForm({}, 'New');
            this.TicketForm.controls.RequestedByID.patchValue(employeeId)
            //this.Ticket.RequestedByID = employeeId;
            //// Ticket.ResolvedDate = DateTime.Now;
            //Ticket.CreatedDate = DateTime.Now;
            this.isAssignedDisable = true;
            this.isCommentDisable = true;
            this.isSaveVisable = false;
            this.loadModalOptions();
            this.addEditTicketModal.show();
        }
    }

    Ticket: ITicketForDisplay;
    respEmailApproval: IEmailApprovalDisplay;
    lstAssignedList: IEmployeeDisplay[] = [];
    isTicketTypeDescription: boolean = false;
    isAssignedDisable: boolean = false;
    isSaveVisable: boolean = false;

    GetDetails(Id: number) {
        this.ticketService.getTicketByIdAsync(Id).subscribe(result => {
            if (result !== null && result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                this.Ticket = result['data'];
                this.TicketId = this.Ticket.ticketID;
                var Roleshort = localStorage.getItem("RoleShort");
                if (Roleshort.toUpperCase() === UserRole.ADMIN) {
                    this.isTicketTypeDescription = false;
                    this.lstAssignedList = this.Employees;
                } else if (this.Ticket.assignedToID != null) {
                    this.isTicketTypeDescription = true;
                    this.lstAssignedList = this.Employees.filter(x => x.managerID === this.EmployeeID || x.employeeID === this.EmployeeID || x.employeeID === this.Ticket.assignedToID);
                }
                else {
                    this.isTicketTypeDescription = false;
                    this.lstAssignedList = this.Employees.filter(x => x.managerID === this.EmployeeID || x.employeeID === this.EmployeeID);
                }
                if (this.Ticket.status !== null && this.Ticket.status.toUpperCase() === TicketStatusConstants.RESOLVED) {
                    this.isSaveVisable = true;
                    this.isAssignedDisable = true;
                } else {
                    this.isSaveVisable = false;
                    this.isAssignedDisable = false;
                }
                this.buildTicketForm(result['data'], 'Edit')
                this.isSaveButtonDisabled = false;
                this.submitted = false;
                this.loadModalOptions();
                this.ModalHeading = "Edit Ticket";
                if (this.Ticket.linkID !== null && this.Ticket.linkID !== Guid.Empty) {
                    this.emailApprovalService.GetEmailApprovalByIdAsync(this.Ticket.linkID).subscribe(async (resp) => {
                        if (resp['data'] !== null && resp['messageType'] === 1) {
                            this.respEmailApproval = resp['data'];
                        }
                    })
                }
                this.loadModalOptions();
               // this.isCommentExist = false;
                this.isCommentDisable = false;
                this.addEditTicketModal.show();
            }
        })
    }



    buildTicketForm(data: any, keyName: string) {
        this.TicketForm = this.fb.group({
            TicketID: [keyName === 'New' ? 0 : data.ticketID],
            TicketTypeID: [keyName === 'New' ? null : data.ticketTypeID, Validators.required],
            TicketType: [keyName === 'New' ? '' : data.ticketType],
            TicketShort: [keyName === 'New' ? '' : data.ticketShort],
            RequestedByID: [keyName === 'New' ? null : data.requestedByID, Validators.required],
            RequestedBy: [keyName === 'New' ? '' : data.requestedBy],
            ModuleID: [keyName === 'New' ? null : data.moduleID],
            ModuleName: [keyName === 'New' ? '' : data.moduleName],
            Description: [keyName === 'New' ? '' : data.description, Validators.required],
            AssignedToID: [keyName === 'New' ? null : data.assignedToID],
            AssignedTo: [keyName === 'New' ? '' : data.assignedTo],
            StatusID: [keyName === 'New' ? null : data.statusID, Validators.required],
            Status: [keyName === 'New' ? null : data.status],
            Title: [keyName === 'New' ? '' : data.title, Validators.required],
            Comment: [keyName === 'New' ? '' : data.comment],
            LinkID: [keyName === 'New' ? Guid.Empty : data.linkID],
            ResolvedDate: [keyName === 'New' ? null : data.resolvedDate !== null ? new Date(data.resolvedDate) : null],
            CreatedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : this.user.firstName + " " + this.user.lastName],
            CreatedDate: [keyName === 'New' ? new Date() : new Date(data.createdDate)],
            ModifiedDate: [keyName === 'New' ? new Date() : data.modifiedDate],
            ModifiedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : data.modifiedBy],
            TimeStamp: ['']
        });
        if (keyName === 'New' && this.TicketStatusList !== null && this.TicketStatusList.length > 0) {
            var StatusID = this.TicketStatusList.find(x => x.valueDesc.toUpperCase() === TicketStatusConstants.NEW);
            this.TicketForm.controls.StatusID.patchValue(StatusID.listValueID)
        }
    }
    get addEditTicketControls() { return this.TicketForm.controls; }


    onAssignedChange(e: any) {
        if (Number(e.value) !== 0 && e.value !== null) {
            this.Ticket.statusID = this.TicketStatusList.find(x => x.valueDesc.toUpperCase() === TicketStatusConstants.ASSIGNED).listValueID;
            this.TicketForm.controls.StatusID.patchValue(this.TicketStatusList.find(x => x.valueDesc.toUpperCase() === TicketStatusConstants.ASSIGNED).listValueID)
            this.isTicketTypeDescription = true;
        }
        else {
            this.isTicketTypeDescription = false;
            this.Ticket.statusID = this.TicketStatusList.find(x => x.valueDesc.toUpperCase() === TicketStatusConstants.NEW).listValueID;
            this.TicketForm.controls.StatusID.patchValue(this.TicketStatusList.find(x => x.valueDesc.toUpperCase() === TicketStatusConstants.NEW).listValueID)
        }
    }
    Save = () => {
        this.submitted = true;
        if (this.TicketForm.invalid) {
            this.TicketForm.markAllAsTouched();
            return;
        } else {
            if (this.isSaveButtonDisabled) {
                return;
            } else {
                this.isSaveButtonDisabled = true;
                this.submitTicket()
            }
        }
    }

    submitTicket() {
        if (this.TicketId === 0) {
                if (this.TicketForm.value.ResolvedDate !== null) {
                    this.TicketForm.value.ResolvedDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.TicketForm.value.ResolvedDate));
                }
                this.ticketService.SaveTicket(this.TicketForm.value).subscribe(result => {
                        if (result['data'] !== null && result['messageType'] === 1) {
                            var emailapproval: IEmailApproval = new IEmailApproval();
                            this.ticketService.getTicketByIdAsync(result['data'].ticketID).subscribe(async (resp) => {
                                if (resp['data'] !== null && resp['messageType'] === 1) {
                                    this.Ticket = resp['data'];
                                    emailapproval.EmailApprovalID = 0;
                                    emailapproval.ModuleID = this.Modules.find(m => m.moduleShort.toUpperCase() === "TICKET").moduleID;
                                    emailapproval.ID = this.Ticket.ticketID;
                                    emailapproval.ValidTime = new Date(moment().add(7, 'day').format('YYYY-MM-DD HH:mm:ss'));
                                    emailapproval.IsActive = true;
                                    emailapproval.LinkID = this.commonUtils.newGuid();
                                   // emailapproval.ApproverEmail = this.employeeDetails.managerEmail;
                                    emailapproval.CreatedBy = this.user.firstName + " " + this.user.lastName;
                                    var emailFields = await this.prepareTicketMail();
                                    emailapproval.EmailSubject = emailFields.EmailSubject;
                                    emailapproval.EmailBody = emailFields.EmailBody;
                                    emailapproval.EmailFrom = emailFields.EmailFrom;
                                    emailapproval.EmailTo = emailFields.EmailTo;
                                    emailapproval.EmailCC = emailFields.EmailCC;
                                    emailapproval.Value = this.Ticket.status;
                                    emailapproval.IsActive = true;
                                    await this.emailApprovalService.SaveEmailApproval(emailapproval).subscribe()
                                }
                            })
                            this.messageService.add({ severity: 'success', summary: 'Ticket saved successfully', detail: '' });
                            this.isSaveButtonDisabled = false;
                            this.loadModalOptions();
                            this.TicketUpdated.emit();
                            if (this.WizardDataID != null && this.WizardDataID != 0) {
                                 this.WizadTicketUpdatedList.emit(this.WizardDataID);
                            }
                            this.cancel();
                        } else {
                            this.isSaveButtonDisabled = false;
                            this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                        }
                    })
                
           
        } else {
            if (this.TicketForm.value.ResolvedDate !== null) {
                this.TicketForm.value.ResolvedDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.TicketForm.value.ResolvedDate));
            }
            this.ticketService.UpdateTicket(this.TicketForm.value.TicketID, this.TicketForm.value).subscribe(async (result) => {
                if (result['data'] !== null && result['messageType'] === 1) {
                    this.Ticket = result['data'];
                    var emailapproval: IEmailApproval = new IEmailApproval();
                    if (this.Ticket.linkID !== null && this.Ticket.linkID !== Guid.Empty && this.respEmailApproval.linkID !== Guid.Empty) {
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
                        emailapproval.ModuleID = this.Modules.find(m => m.moduleShort.toUpperCase() == "TICKET").moduleID;
                        emailapproval.ID = this.Ticket.ticketID;
                        emailapproval.IsActive = true;
                        emailapproval.LinkID = this.commonUtils.newGuid();
                      //  emailapproval.ApproverEmail = this.employeeDetails.managerEmail;
                        emailapproval.CreatedBy = this.user.firstName + " " + this.user.lastName;
                        emailapproval.CreatedDate = new Date();
                    }
                    emailapproval.ValidTime = new Date(moment().add(7, 'day').format('YYYY-MM-DD HH:mm:ss'));
                    var emailFields: EmailFields = new EmailFields()
                    emailFields = await this.prepareMail();
                    emailapproval.EmailSubject = emailFields.EmailSubject;
                    emailapproval.EmailBody = emailFields.EmailBody;
                    emailapproval.EmailFrom = emailFields.EmailFrom;
                    emailapproval.EmailTo = emailFields.EmailTo;
                    emailapproval.EmailCC = emailFields.EmailCC;
                    await this.emailApprovalService.SaveEmailApproval(emailapproval).subscribe(result => {
                    })
                    this.messageService.add({ severity: 'success', summary: 'Ticket saved successfully', detail: '' });
                    this.isSaveButtonDisabled = false;
                    this.loadModalOptions();
                    this.TicketUpdated.emit();
                    if (this.WizardDataID != null && this.WizardDataID != 0) {
                        this.WizadTicketUpdatedList.emit(this.WizardDataID);
                    }
                    this.cancel();
                } else {
                    this.isSaveButtonDisabled = false;
                    this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                }
            })
        }
    }

     async  prepareTicketMail() {
         var emailFields: EmailFields = new EmailFields();
        emailFields.EmailCCList = [];
        var RequesterEmail = await this.GetEmail(this.Ticket.requestedByID);
        await this.GetAdminEmail(this.Ticket, emailFields);
        emailFields.EmailTo = this.TicketEmail;
         emailFields.EmailCCList.push(RequesterEmail);
         let uniqueChars = [...new Set(emailFields.EmailCCList)];
         emailFields.EmailCC = uniqueChars.join(';');
       // emailFields.EmailCC = emailFields.EmailCCList.Distinct().Aggregate((x, y) => x + ";" + y);
        emailFields.EmailSubject = "IHR Ticket created for " + this.Ticket.requestedBy;
         emailFields.EmailBody = "Ticket #" + this.Ticket.ticketID + " has been Created " +
            "<br/>" +
             "<ul style='margin-bottom: 0px;'><li>Type: " + this.Ticket.ticketType + "<br/>" +
             "</li><li>Requester: " + this.Ticket.requestedBy +
             "</li><li>Title: " + this.Ticket.title +
             "</li><li>Description: " + this.Ticket.description +
             "</li><li>Submitted Date: " + moment(this.Ticket.createdDate).format('DD MMM yyy HH:mm:ss') + " GMT" +
            "</li>" +
             "<li>Comments: " + this.Ticket.comment +
            "</li></ul>";
        return emailFields;
    }


   async prepareMail() {
       var emailFields: EmailFields = new EmailFields();
        emailFields.isMultipleEmail = true;
        emailFields.EmailCCList = [];
        var RequesterEmail = await this.GetEmail(this.Ticket.requestedByID);
        await this.GetAdminEmail(this.Ticket, emailFields);
        if (this.Ticket.assignedToID != null) {
            var ApproverEmail = await this.GetEmail(this.Ticket.assignedToID);
            emailFields.EmailTo = ApproverEmail;
        }
        if (this.Ticket.assignedToID != null && this.lstAssignedList != null) {
            this.Ticket.assignedTo = this.lstAssignedList.find(x => x.employeeID == this.Ticket.assignedToID).employeeName;
        }
       emailFields.EmailCCList.push(RequesterEmail);
       let uniqueChars = [...new Set(emailFields.EmailCCList)];
       emailFields.EmailCC = uniqueChars.join(';');
       //emailFields.EmailCC = emailFields.EmailCCList.join(';');
        //emailFields.EmailCC = emailFields.EmailCCList.Distinct().Aggregate((x, y) => x + ";" + y);
        emailFields.EmailSubject = "IHR Ticket for " + this.Ticket.requestedBy + " assigned to " + this.Ticket.assignedTo;
        emailFields.EmailBody = "Ticket #" + this.Ticket.ticketID + " has been " + this.Ticket.status + " To " + this.Ticket.assignedTo +
            "<br/>" +
            "<ul style='margin-bottom: 0px;'><li>Type: " + this.Ticket.ticketType + "<br/>" +
            "</li><li>Requester: " + this.Ticket.requestedBy +
            "</li><li>Assigned To: " + this.Ticket.assignedTo +
            "</li><li>Title: " + this.Ticket.title +
            "</li><li>Description: " + this.Ticket.description +
            "</li><li>Submitted Date: " + moment(this.Ticket.createdDate).format('DD MMM yyy HH:mm:ss') + " GMT" +
            "</li>" +
            "<li>Comments: " + this.Ticket.comment +
            "</li></ul>";
        return emailFields;
   }

    GetEmail(EmployeeID: number): string {
        return this.Employees && this.Employees.filter(x => x.employeeID === EmployeeID).length > 0
            ? this.Employees.find(x => x.employeeID === EmployeeID).loginEmail : this.Employees.find(x => x.employeeID === EmployeeID).email;
    }

    async GetAdminEmail(Ticket: ITicketForDisplay, emailFields: EmailFields) {
        emailFields.EmailCCList = [];
        var AdminType = this.TicketEmailMapList.find(x => x.value === Ticket.ticketShort).valueDesc;
        if (AdminType !== null && AdminType !== undefined && this.UsersList != null) {
            this.UsersList.forEach(user => 
            {
                var exists = user.roleShort.toUpperCase().includes(AdminType.toUpperCase());
                if (exists) {
                    emailFields.EmailCCList.push(user.email);
                }
            })
        }
    }

    onCommentChange(e: any) {
        if (e !== undefined && e !== null && e !== '') {
            this.isCommentExist = false;
        }
        else {
            this.isCommentExist = true;
        }
    }

    resolve = () => {
        this.submitted = true;
        if (this.TicketForm.invalid) {
            this.TicketForm.markAllAsTouched();
            return;
        } else {
            //if (this.isSaveButtonDisabled) {
            //    return;
            //} else {
                this.isSaveButtonDisabled = false;
                if (this.TicketId > 0 && this.TicketForm.value.Comment !== null && this.TicketForm.value.Comment !== '') {
                    this.resolveTicket()
                } else {
                    //this.isResolvedDate = true;
                    this.isCommentExist = true;
                }
            //}
        }
    }

    resolveTicket() {
        this.isCommentExist = false;
        //this.isResolvedDate = false;
        this.TicketForm.value.StatusID = this.TicketStatusList.find(x => x.valueDesc.toUpperCase() === TicketStatusConstants.RESOLVED).listValueID;
        this.TicketForm.value.ResolvedDate =new Date();
        this.ticketService.UpdateTicket(this.TicketForm.value.TicketID, this.TicketForm.value).subscribe(async result => {
            if (result['data'] !== null && result['messageType'] === 1) {
                var emailapproval: IEmailApproval = new IEmailApproval();
                //this.ticketService.getTicketByIdAsync(result['data'].ticketID).subscribe(async (resp) => {
                    //if (resp['data'] !== null && resp['messageType'] === 1) {
                        this.Ticket = result['data'];
                        this.TicketId = this.Ticket.ticketID;
                        var emailapproval: IEmailApproval = new IEmailApproval();
                        var ClientID: string = localStorage.getItem("ClientID");
                        if (this.Ticket.linkID !== null && this.Ticket.linkID !== Guid.Empty && this.respEmailApproval.linkID !== Guid.Empty) {
                            //await EmailApprovalService.EamilApprovalAction(ClientID, Ticket.LinkID, "RESOLVED", "TICKET");
                            await this.emailApprovalService.EamilApprovalAction(ClientID, this.Ticket.linkID, "RESOLVED", "TICKET").subscribe(res => {
                            })
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
                      

                    } else {
                        emailapproval.EmailApprovalID = 0;
                        emailapproval.ModuleID = this.Modules.find(m => m.moduleShort.toUpperCase() == "TICKET").moduleID;
                        emailapproval.ID = this.Ticket.ticketID;
                        emailapproval.IsActive = true;
                        emailapproval.LinkID = this.commonUtils.newGuid();
                      //  emailapproval.ApproverEmail = this.employeeDetails.managerEmail;
                        emailapproval.CreatedBy = this.user.firstName + " " + this.user.lastName;
                        emailapproval.CreatedDate = new Date();
                   }
                    emailapproval.ValidTime = new Date(moment().add(7, 'day').format('YYYY-MM-DD HH:mm:ss'));
                    var emailFields: EmailFields = new EmailFields()
                        emailFields = await this.prepareResolveMail();
                    emailapproval.EmailSubject = emailFields.EmailSubject;
                    emailapproval.EmailBody = emailFields.EmailBody;
                    emailapproval.EmailFrom = emailFields.EmailFrom;
                    emailapproval.EmailTo = emailFields.EmailTo;
                    emailapproval.Value = null
                    emailapproval.IsActive = true
                    //emailapproval.EmailCC = emailFields.EmailCC;
                    await this.emailApprovalService.SaveEmailApproval(emailapproval).subscribe(result => {
                    })
                    this.messageService.add({ severity: 'success', summary: 'Ticket saved successfully', detail: '' });
                    this.isSaveButtonDisabled = true;
                   this.loadModalOptions();
                this.TicketUpdated.emit();
                if (this.WizardDataID != null && this.WizardDataID != 0) {
                    this.WizadTicketUpdatedList.emit(this.WizardDataID);
                }
                   this.cancel();
                } else {
                    this.isSaveButtonDisabled = false;
                    this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                }
            //    })
            //} else {
            //    this.isSaveButtonDisabled = false;
            //    this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
            //}
        })
    }


    async prepareResolveMail() {
        var emailFields: EmailFields = new EmailFields();
        // string uri = Configuration["EmailApprovalUrl:" + user.ClientID];
        emailFields.EmailCCList = [];
        var RequesterEmail = await this.GetEmail(this.Ticket.requestedByID);
        await this.GetAdminEmail(this.Ticket, emailFields);
        emailFields.EmailTo = RequesterEmail;
       // emailFields.EmailCCList.push(RequesterEmail);
        let uniqueChars = [...new Set(emailFields.EmailCCList)];
        emailFields.EmailCC = uniqueChars.join(';');
        //emailFields.EmailCC = emailFields.EmailCCList.Distinct().Aggregate((x, y) => x + ";" + y);
        emailFields.EmailSubject = "IHR Ticket resolved for " + this.Ticket.requestedBy;
        emailFields.EmailBody = "Ticket #" + this.Ticket.ticketID + " has been " + this.Ticket.status +
            "<br/>" +
            "<ul style='margin-bottom: 0px;'><li>Type: " + this.Ticket.ticketType + "<br/>" +
            "</li><li>Requester: " + this.Ticket.requestedBy +
            "</li><li>Assigned To: " + this.Ticket.assignedTo +
            "</li><li>Title: " + this.Ticket.title +
            "</li><li>Description: " + this.Ticket.description +
            "</li><li>Submitted Date: " + moment(this.Ticket.createdDate).format('DD MMM yyy HH:mm:ss') + " GMT" +
            "</li><li>Resolved Date: " + moment(this.Ticket.resolvedDate).format('DD MMM yyy HH:mm:ss') + " GMT"+
            "</li><li>Comments: " + this.Ticket.comment +
            "</li>" +
            "</ul>";
        return emailFields;
    }
    cancel = () => {
        this.TicketId = -1
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.isSaveVisable = false;
        this.isAssignedDisable = false;
        this.isTicketTypeDescription = false;
        this.buildTicketForm({}, 'New');
        this.addEditTicketModal.hide();
    }

    ResetDialog() {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.isSaveVisable = false;
        this.isAssignedDisable = false;
        this.isTicketTypeDescription = false;
        this.buildTicketForm({}, 'New');
        this.addEditTicketModal.hide();
    }

}
