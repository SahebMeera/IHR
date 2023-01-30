import { toBase64String } from '@angular/compiler/src/output/source_map';
import { Byte } from '@angular/compiler/src/util';
import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import * as moment from 'moment';
import { MessageService } from 'primeng/api';
import { forkJoin } from 'rxjs';
import { CommonUtils } from '../../../common/common-utils';
import { Constants, ErrorMsg, Guid, ListTypeConstants, SessionConstants, Settings, UserRole } from '../../../constant';
import { EmailFields } from '../../../core/interfaces/Common';
import { IEmailApproval, IEmailApprovalDisplay } from '../../../core/interfaces/EmailApproval';
import { IEmployeeDisplay } from '../../../core/interfaces/Employee';
import { IExpenseForDisplay } from '../../../core/interfaces/Expense';
import { IRolePermissionDisplay } from '../../../core/interfaces/RolePermission';
import { IModalPopupAlternateOptions } from '../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { EmailApprovalService } from '../../employee/emailApproval.service';
import { EmployeeService } from '../../employee/employee.service';
import { LookUpService } from '../../lookup/lookup.service';
import { RolePermissionService } from '../../role-permission/role-permission.service';
import { ExpenseService } from '../expense.service';

@Component({
  selector: 'app-add-edit-expenses',
  templateUrl: './add-edit-expenses.component.html',
  styleUrls: ['./add-edit-expenses.component.scss']
})

export class AddEditExpensesComponent implements OnInit {
    settings = new Settings()
    commonUtils = new CommonUtils()
    //@Input() EmployeeName: string;
    @Output() ExpenseUpdated = new EventEmitter<any>();
    @ViewChild('ExpenseModal') ExpenseModal: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;
    //ContactTypeList: any[] = [];
    //CountryList: any[] = [];
     user: any;
    //submitted: boolean = false;
    //StateList: any[] = [];

    ExpenseForm: FormGroup;
    RolePermissions: IRolePermissionDisplay[] = [];
    EmployeeInfoRolePermission: IRolePermissionDisplay;
    ExpenseId: number;
    ModalHeading: string = 'Edit Expense';
    isSaveButtonDisabled: boolean = false;
    isShow: boolean = true;
    dateTime = new Date();

    formData: FormData = new FormData();
    progress: number = 30;
    @ViewChild('file', { static: false }) expenseFile: ElementRef;
    file: File = null;
    IsApprover: boolean = false;
    ExpenseEmail: string;
    ClientID: string;

    

    constructor(private fb: FormBuilder,
        private lookupService: LookUpService,
        private messageService: MessageService,
        private expenseService: ExpenseService,
        private employeeService: EmployeeService,
        private rolePermissionService: RolePermissionService,
        private emailApprovalService: EmailApprovalService
        ) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.EmployeeInfoRolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.EMPLOYEEINFO);
        this.user = JSON.parse(localStorage.getItem("User"));
        var RoleShort = localStorage.getItem("RoleShort");
        this.ClientID = localStorage.getItem("ClientID");
        this.dateTime.setDate(this.dateTime.getDate());
        this.ExpenseEmail = this.settings.EmailNotifications[this.ClientID]['Expense']
        if (RoleShort.toUpperCase() == UserRole.ADMIN || RoleShort.toUpperCase() == UserRole.FINADMIN) {
            this.IsApprover = true;
        }
        this.LoadDropDown();
        this.buildExpenseForm({}, 'New');
    }

    ngOnInit(): void {
        this.loadModalOptions();
    }

    Employees: IEmployeeDisplay[] = [];
    Modules: any[] = [];
    lstEmployees: IEmployeeDisplay[] = [];
    TicketEmailMapList: any[] = [];
    ExpenseTypeList: any[] = [];
    StatusList: any[] = [];

    defaultExpenseTypeID: number;
    submittedStatusID: number;
    approveStatusID: number;
    denyStatusID: number;

    LoadDropDown() {
        forkJoin(
            this.employeeService.GetEmployees(),
            this.lookupService.getListValues(),
            this.rolePermissionService.getModules(),
        ).subscribe(resultSet => {
            if (resultSet !== undefined && resultSet !== null) {
                this.Employees = resultSet[0]['data']
                this.ExpenseTypeList = resultSet[1]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.ExpenseType);
                this.StatusList = resultSet[1]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.ExpenseStatus);
                if (this.ExpenseTypeList !== null && this.ExpenseTypeList !== undefined && this.ExpenseTypeList.length > 0) {
                    var ExpenseType = this.ExpenseTypeList.find(x => x.value == "MISC");
                    if (ExpenseType !== null && ExpenseType !== undefined) {
                        this.defaultExpenseTypeID = ExpenseType.listValueID;
                    }
                
                }
                this.submittedStatusID = this.StatusList.find(x => x.value == "SUBMITTED") !== undefined ? this.StatusList.find(x => x.value == "SUBMITTED").listValueID : null;
                this.approveStatusID = this.StatusList.find(x => x.value == "APPROVED") !== undefined ? this.StatusList.find(x => x.value == "APPROVED").listValueID : null;
                this.denyStatusID = this.StatusList.find(x => x.value == "REJECTED") !== undefined ? this.StatusList.find(x => x.value == "REJECTED").listValueID : null;
                this.Modules = resultSet[2];
            }
        })
    }
    fileList: File[] = [];
    listOfFiles: any[] = [];
    isLoading = false;
    Show(ExpenseID: number) {
        this.ExpenseId = ExpenseID;
        this.file = null;
        this.fileList = [];
        this.listOfFiles = [];
        this.fileNames = [];
        this.ResetDialog();
        if (ExpenseID != 0) {
            this.LoadDropDown();
            this.GetExpense(ExpenseID);
        }
        else {
            this.ModalHeading = "Add Expense";
            this.file = null;
            this.loadModalOptions();
            this.buildExpenseForm({}, 'New');
            this.ExpenseForm.controls.StatusID.patchValue(Number(this.submittedStatusID))
            this.ExpenseForm.controls.EmployeeID.patchValue(this.user.employeeID)
            this.ExpenseForm.controls.ExpenseTypeID.patchValue(this.defaultExpenseTypeID)
            this.ExpenseModal.show();
            
        }
    }

    buildExpenseForm(data: any, keyName: string) {
        this.ExpenseForm = this.fb.group({
            ExpenseID: [keyName === 'New' ? 0 : data.expenseID],
            EmployeeID: [keyName === 'New' ? null : data.employeeID],
            EmployeeName: [keyName === 'New' ? null : data.employeeName],
            ExpenseTypeID: [keyName === 'New' ? null : data.expenseTypeID, Validators.required],
            ExpenseType: [keyName === 'New' ? '' : data.expenseType],
            FileName: [keyName === 'New' ? '' : data.fileName],
            Amount: [keyName === 'New' ? null : data.amount !== null ? parseFloat(data.amount) : null, Validators.required],
            SubmissionDate: [keyName === 'New' ? new Date() : data.submissionDate !== null ? new Date(data.submissionDate) : null],
            SubmissionComment: [keyName === 'New' ? '' : data.submissionComment],
            StatusID: [keyName === 'New' ? null : data.statusID, Validators.required],
            Status: [keyName === 'New' ? null : data.status],
            AmountPaid: [keyName === 'New' ? null : data.amountPaid],
            PaymentDate: [keyName === 'New' ? null : data.paymentDate !== null ? new Date(data.paymentDate) : null],
            PaymentComment: [keyName === 'New' ? '' : data.paymentComment],
            CreatedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : this.user.firstName + " " + this.user.lastName],
            CreatedDate: [keyName === 'New' ? new Date() : new Date(data.createdDate)],
            ModifiedDate: [keyName === 'New' ? new Date() : data.modifiedDate],
            ModifiedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : data.modifiedBy],
            TimeStamp: [''],
            LinkID: [keyName === 'New' ? Guid.Empty : data.linkID],
        });
      
    }
    get addEditTicketControls() { return this.ExpenseForm.controls; }
    respEmailApproval: IEmailApprovalDisplay;
    expense: IExpenseForDisplay;
    isEditFileList: any[] = [];
    isApproveButtonDisabled: boolean = false;
    GetExpense(ExpenseID: number) {
        this.isSaveButtonDisabled = false;
        this.isApproveButtonDisabled = false;
        this.expenseService.GetExpenseByIdAsync(ExpenseID).subscribe(result => {
            if (result !== null && result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                this.expense = result['data'];
                this.ModalHeading = "Edit Expense";
                this.file = null;
                this.isEditFileList = [];
                if (this.expense.fileName !== null && this.expense.fileName !== undefined) {
                    var file = this.expense.fileName.split(',')
                    this.isEditFileList = file;
                }

                if (this.expense.statusID === this.approveStatusID || this.expense.statusID === this.denyStatusID) {
                    this.isSaveButtonDisabled = true;
                    this.isApproveButtonDisabled = true;
                } else {
                    this.isSaveButtonDisabled = false;
                    this.isApproveButtonDisabled = false;
                }
                this.isPaymentAmountInValid = false;
                this.isPaymentDateInValid = false;
                this.loadModalOptions();
                if (this.expense.linkID !== null && this.expense.linkID !== Guid.Empty) {
                    this.emailApprovalService.GetEmailApprovalByIdAsync(this.expense.linkID).subscribe(async (resp) => {
                        if (resp['data'] !== null && resp['messageType'] === 1) {
                            this.respEmailApproval = resp['data'];
                        }
                    })
                }
                this.buildExpenseForm(this.expense, 'Edit');
                this.ExpenseModal.show();
            }
        })
    }

    ResetDialog() {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildExpenseForm({}, 'New');
        this.ExpenseModal.hide();
    }


    loadModalOptions() {
        if (!this.IsApprover) {
            this.modalOptions = {
                footerActions: [
                    {
                        actionText: 'Submit',
                        actionMethod: this.save,
                        styleClass: !this.IsApprover ? 'p-button-raised p-mr-2 p-mb-2' : 'p-button-raised p-mr-2 p-mb-2 display-none',
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
        } else {
            this.modalOptions = {
                footerActions: [
                    
                    {
                        actionText: 'Approve',
                        actionMethod: this.approve,
                        styleClass: this.IsApprover ? 'p-button-raised p-mr-2 p-mb-2' : 'p-button-raised p-mr-2 p-mb-2 display-none',
                        iconClass: 'mdi mdi-content-save',
                        disabled: this.isApproveButtonDisabled
                    },
                    {
                        actionText: 'Deny',
                        actionMethod: this.deny,
                        styleClass: this.IsApprover ? 'btn-width-height p-button-raised p-button-danger p-mr-2 p-mb-2' : 'btn-width-height p-button-raised p-button-danger p-mr-2 p-mb-2 display-none',
                        iconClass: 'p-button-raised p-button-danger',
                        disabled: this.isApproveButtonDisabled
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
    }


    onPaymentDateChange() {
        var paymentDate = this.ExpenseForm.value.PaymentDate;
        if (paymentDate != null && paymentDate !== undefined) {
            this.isPaymentDateInValid = false;
        }
        else {
            this.isPaymentDateInValid = true;
        }
    }
    onPaymentDateselect() {
        var paymentDate = this.ExpenseForm.value.PaymentDate;
        if (paymentDate != null && paymentDate !== undefined) {
            this.isPaymentDateInValid = false;
        }
        else {
            this.isPaymentDateInValid = true;
        }
    }

    onPaymentAmountChange() {
        var paymentAmount = this.ExpenseForm.value.AmountPaid;
        if (paymentAmount != null && paymentAmount !== undefined && paymentAmount != '') {
            this.isPaymentAmountInValid = false;
        }
        else {
            this.isPaymentAmountInValid = true;
        }
    }
    onPaymentCommentChange() {
        var paymentAmount = this.ExpenseForm.value.PaymentComment;
        if (paymentAmount != null && paymentAmount !== undefined && paymentAmount != '') {
            this.isPaymentCommnetInValid = false;
        }
        else {
            this.isPaymentCommnetInValid = true;
        }
    }
    submitted: boolean = false;
    save = async () => {
        this.submitted = true;
        if (this.ExpenseForm.invalid) {
            this.ExpenseForm.markAllAsTouched();
            return;
        } else {
            if (this.isSaveButtonDisabled) {
                return;
            } else {
                this.isSaveButtonDisabled = true;
                if (this.Employees != null) {
                    if (this.fileList.length > 0) {
                        var employeeName: string = await this.Employees != null && this.Employees.find(x => x.employeeID == this.user.employeeID) !== undefined ? this.Employees.find(x => x.employeeID == this.user.employeeID).employeeName : null;
                        for (var file of this.fileList) {
                            this.formData = new FormData();
                            this.formData.append(file.name, file);
                            await this.expenseService.uploadFile(employeeName.replace(/\s+/g, ""), this.formData)
                                .then(async result => {
                                    if (result['data'] !== null && result['messageType'] === 1) {
                                        var expense = await new IExpenseForDisplay();
                                        expense = await result['data'];
                                        var fileNames: any = await expense.fileName.toString();
                                        await this.fileNames.push(fileNames)
                                    }
                                });
                        }
                        if (this.fileNames.length > 0 && this.fileNames.length === this.fileList.length) {
                            let uniqueChars = [...new Set(this.fileNames)];
                            this.ExpenseForm.value.FileName = uniqueChars.join(',');
                            await this.SubmitExpense()
                            //  this.cancel()
                        } else {
                            this.ExpenseForm.value.FileName = null
                            // this.cancel()
                            await this.SubmitExpense()
                        }
                    } else {
                        this.ExpenseForm.value.FileName = null
                        // this.cancel()
                        await this.SubmitExpense()
                    }
                }
                this.isSaveButtonDisabled = false;
            }
        }
    }
    SubmitExpense() {
        if (this.ExpenseId === 0) {
            this.ExpenseForm.value.SubmissionDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.ExpenseForm.value.SubmissionDate));
            if (this.ExpenseForm.value.PaymentDate !== null) {
                    this.ExpenseForm.value.PaymentDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.ExpenseForm.value.PaymentDate));
                }
                    this.expenseService.SaveExpense(this.ExpenseForm.value).subscribe(result => {
                        if (result['data'] !== null && result['messageType'] === 1) {
                            var emailapproval: IEmailApproval = new IEmailApproval();

                            this.expenseService.GetExpenseByIdAsync(result['data'].expenseID).subscribe(async (resp) => {
                                if (resp['data'] !== null && resp['messageType'] === 1) {
                                    this.expense = resp['data'];
                                    emailapproval.EmailApprovalID = 0;
                                    emailapproval.ModuleID = this.Modules.find(m => m.moduleShort.toUpperCase() == "EXPENSES").moduleID;
                                    emailapproval.ID = this.expense.expenseID;
                                    emailapproval.Value = this.expense.status;
                                    emailapproval.ValidTime = new Date(moment().add(7, 'day').format('YYYY-MM-DD HH:mm:ss'));
                                    emailapproval.IsActive = true;
                                    emailapproval.LinkID = this.commonUtils.newGuid();
                                    emailapproval.CreatedBy = this.user.firstName + " " + this.user.lastName;
                                    var emailFields = await this.prepareExpensetMail();
                                    emailapproval.EmailSubject = emailFields.EmailSubject;
                                    emailapproval.EmailBody = emailFields.EmailBody;
                                    emailapproval.EmailFrom = emailFields.EmailFrom;
                                    emailapproval.EmailTo = emailFields.EmailTo;
                                    emailapproval.EmailCC = emailFields.EmailCC;
                                    await this.emailApprovalService.SaveEmailApproval(emailapproval).subscribe()
                                }
                            })
                            this.messageService.add({ severity: 'success', summary: 'Expense saved successfully', detail: '' });
                            this.isSaveButtonDisabled = true;
                            this.loadModalOptions();
                            this.ExpenseUpdated.emit();
                            this.cancel();
                        } else {
                            this.isSaveButtonDisabled = false;
                            this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                        }
                    })
        } else {
            this.ExpenseForm.value.SubmissionDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.ExpenseForm.value.SubmissionDate));
            if (this.ExpenseForm.value.PaymentDate !== null) {
                this.ExpenseForm.value.PaymentDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.ExpenseForm.value.PaymentDate));
            }
            this.expenseService.UpdateExpense(this.ExpenseForm.value.ExpenseID, this.ExpenseForm.value).subscribe(async (result) => {
                if (result['data'] !== null && result['messageType'] === 1) {
                    this.expense = result['data'];
                    var emailapproval: IEmailApproval = new IEmailApproval();
                    if (this.expense.linkID !== null && this.expense.linkID !== Guid.Empty && this.respEmailApproval.linkID !== Guid.Empty) {
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
                        emailapproval.ModuleID = this.Modules.find(m => m.moduleShort.toUpperCase() == "LEAVEREQUEST").moduleID;
                        emailapproval.ID = this.expense.expenseID;
                        emailapproval.IsActive = true;
                        emailapproval.LinkID = this.commonUtils.newGuid();
                       // emailapproval.ApproverEmail = this.employeeDetails.managerEmail;
                        emailapproval.CreatedBy = this.user.firstName + " " + this.user.lastName;
                        emailapproval.CreatedDate = new Date();
                    }
                    emailapproval.ValidTime = new Date(moment().add(7, 'day').format('YYYY-MM-DD HH:mm:ss'));
                    var emailFields: EmailFields = await this.prepareUpdatedExpenseMail();
                    emailapproval.EmailSubject = emailFields.EmailSubject;
                    emailapproval.EmailBody = emailFields.EmailBody;
                    emailapproval.EmailFrom = emailFields.EmailFrom;
                    emailapproval.EmailTo = emailFields.EmailTo;
                    emailapproval.EmailBCC = emailFields.EmailCC
                    await this.emailApprovalService.SaveEmailApproval(emailapproval).subscribe(result => {
                    })
                    this.messageService.add({ severity: 'success', summary: 'Expense saved successfully', detail: '' });
                    this.isSaveButtonDisabled = true;
                    this.loadModalOptions();
                    this.ExpenseUpdated.emit();
                    this.cancel();
                } else {
                    this.isSaveButtonDisabled = false;
                    this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                }
            })
        }
    }

    async prepareExpensetMail() {
        var emailFields: EmailFields = new EmailFields();
        emailFields.EmailCCList = [];
        var RequesterEmail = await this.GetEmail(this.expense.employeeID);
        emailFields.EmailTo = this.ExpenseEmail;
        emailFields.EmailCC = RequesterEmail;
        emailFields.EmailSubject = "Expense submitted for " + this.expense.employeeName;
        emailFields.EmailBody = "Expense #" + this.expense.expenseID + " has been submitted " +
            "<br/>" +
            "<ul style='margin-bottom: 0px;'><li>Type: " + this.expense.expenseType + "<br/>" +
            "</li><li>Requester: " + this.expense.employeeName +
            "</li><li>Amount: " + this.expense.amount.toFixed(2) +
            "</li><li>Description: " + this.expense.submissionComment +
            "</li><li>Submitted Date: " + moment(this.expense.createdDate).format('DD MMM yyy HH:mm:ss' ) + " GMT"+
            "</li></li></ul>";
        return emailFields;
    }

    async prepareUpdatedExpenseMail() {
        var emailFields: EmailFields = new EmailFields();
        emailFields.EmailCCList = [];
        var RequesterEmail = await this.GetEmail(this.expense.employeeID);
        emailFields.EmailTo = this.ExpenseEmail;
        emailFields.EmailCC = RequesterEmail;
        emailFields.EmailSubject = "Expense submitted for " + this.expense.employeeName;
        emailFields.EmailBody = "Expense #" + this.expense.expenseID + " has been submitted " +
            "<br/>" +
            "<ul style='margin-bottom: 0px;'><li>Type: " + this.expense.expenseType + "<br/>" +
            "</li><li>Requester: " + this.expense.employeeName +
            "</li><li>Amount: " + this.expense.amount.toFixed(2) +
            "</li><li>Description: " + this.expense.submissionComment +
            "</li><li>Submitted Date: " + moment(this.expense.createdDate).format('DD MMM yyy HH:mm:ss') +' GMT' +
            "</li><li>Updated Date: " + moment(this.expense.modifiedDate).format('DD MMM yyy HH:mm:ss') + ' GMT' +
            "</li></li></ul>";
        return emailFields;
    }

    GetEmail(EmployeeID: number): string {
        return this.Employees && this.Employees.filter(x => x.employeeID === EmployeeID).length > 0
            ? this.Employees.find(x => x.employeeID === EmployeeID).loginEmail : this.Employees.find(x => x.employeeID === EmployeeID).email;
    }

    isPaymentDateInValid: boolean = false;
    isPaymentAmountInValid: boolean = false;
    isPaymentCommnetInValid: boolean = false;
    //isPaymentDateInValid: boolean = false;
    //isPaymentDateInValid: boolean = false;
    isValidForApprove() {
        this.isPaymentDateInValid = false;
        this.isPaymentAmountInValid = false;
        this.isPaymentCommnetInValid = false;
        if (this.ExpenseForm.value.PaymentDate == null && this.ExpenseForm.value.AmountPaid == null) {
            this.isPaymentAmountInValid = true;
            this.isPaymentDateInValid = true;
            return false;
        }
        else if (this.ExpenseForm.value.PaymentDate == null && this.ExpenseForm.value.AmountPaid != null) {
            this.isPaymentDateInValid = true;
            this.isPaymentAmountInValid = false;
            return false;
        }
        else if (this.ExpenseForm.value.PaymentDate != null && this.ExpenseForm.value.AmountPaid == null) {
            this.isPaymentAmountInValid = true;
            this.isPaymentDateInValid = false;
            return false;
        }
        else if (this.ExpenseForm.value.PaymentComment == null || this.ExpenseForm.value.PaymentComment == "") {
            this.isPaymentCommnetInValid = true;
            return false;
        }
        return true;
    }
    isValidForDeny() {
        this.isPaymentCommnetInValid = false;
        if (this.ExpenseForm.value.PaymentComment == null || this.ExpenseForm.value.PaymentComment == "") {
            this.isPaymentCommnetInValid = true;
            return false;
        }

        return true;
    }

    cancel = () => {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildExpenseForm({}, "New");
        this.ExpenseModal.hide();
    }

    fileNames: any[] = [];
    async onFileChanged(event: any) {
        this.isLoading = true;
        for (var i = 0; i <= event.target.files.length - 1; i++) {
            var selectedFile = event.target.files[i];
            if (this.listOfFiles.indexOf(selectedFile.name) === -1) {
                this.fileList.push(selectedFile);
               // this.formData = new FormData();
               // this.formData.append(selectedFile.name, selectedFile);
                this.listOfFiles.push(selectedFile.name);
               // this.uploadFile(this.formData, selectedFile)
            }
        }
        this.isLoading = false;
    }

    //async uploadFile(formData: FormData, file: any) {
    //    var employeeName: string = await this.Employees != null && this.Employees.find(x => x.employeeID == this.user.employeeID) !== undefined ?  this.Employees.find(x => x.employeeID == this.user.employeeID).employeeName : null;
    //     if (employeeName !== null) {
    //        //await this.expenseService.uploadFile(employeeName.replace(/\s+/g, ""), formData).subscribe(async result => {
    //        //    console.log('HHHHH', result)
    //        // if (result['data'] !== null && result['messageType'] === 1) {
    //        //     var expense = await new IExpenseForDisplay();
    //        //      expense =  await result['data'];
    //        //     var fileNames: any = await expense.fileName.toString();
    //        //     await this.fileNames.push(fileNames)
    //        //     await console.log(this.fileNames)
    //        //    }
    //        //});
    //    } else {
    //        this.fileNames = []
    //    }
    //    await console.log(this.fileNames)
    //}

    removeSelectedFile(index) {
        // Delete the item from fileNames list
        this.listOfFiles.splice(index, 1);
        // delete file from FileList
        this.fileList.splice(index, 1);
    }
    approve = () => {
        this.submitted = true;
        if (this.isValidForApprove()) {
                this.isSaveButtonDisabled = true;
                this.isSaveButtonDisabled = false;
                var ClientID: string = localStorage.getItem("ClientID");
                this.ExpenseForm.controls.StatusID.patchValue(this.approveStatusID)
                this.ExpenseForm.value.SubmissionDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.ExpenseForm.value.SubmissionDate));
                if (this.ExpenseForm.value.PaymentDate !== null) {
                    this.ExpenseForm.value.PaymentDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.ExpenseForm.value.PaymentDate));
                }
            this.expenseService.UpdateExpense(this.ExpenseForm.value.ExpenseID, this.ExpenseForm.value).subscribe(async (result) => {
                if (result['data'] !== null && result['messageType'] === 1) {
                        this.expense = result['data'];
                        this.expense.submissionComment = result['data'].submissionComment;
                        this.expense.paymentComment = result['data'].paymentComment;
                        var emailApproval: IEmailApproval = new IEmailApproval();
                        if (this.expense.linkID !== null && this.expense.linkID !== Guid.Empty && this.respEmailApproval.linkID !== Guid.Empty) {
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
                            await this.emailApprovalService.EamilApprovalAction(ClientID, this.expense.linkID, "APPROVED", "EXPENSES").subscribe(res => {
                            })
                        } else {
                            emailApproval.ModuleID = this.Modules.find(m => m.moduleShort.toUpperCase() == "EXPENSES").moduleID;
                            emailApproval.ID = this.expense.expenseID;
                            emailApproval.ValidTime = new Date(moment().add(7, 'day').format('YYYY-MM-DD HH:mm:ss'));
                            emailApproval.IsActive = true;
                            emailApproval.LinkID = this.commonUtils.newGuid();
                            //emailApproval.ApproverEmail = await this.GetEmail(this.Leave.approverID);
                            emailApproval.CreatedBy = this.user.firstName + " " + this.user.lastName;
                        }
                        var emailFields: EmailFields = await this.prepareApprovedMail();
                        emailApproval.EmailApprovalID = 0;
                        emailApproval.LinkID = Guid.Empty;
                        emailApproval.EmailSubject = emailFields.EmailSubject;
                        emailApproval.EmailBody = emailFields.EmailBody;
                        emailApproval.EmailFrom = emailFields.EmailFrom;
                        emailApproval.EmailTo = emailFields.EmailTo;
                        emailApproval.Value = null;
                        emailApproval.IsActive = true;
                        await this.emailApprovalService.SaveEmailApproval(emailApproval).subscribe(res => {
                        })
                        this.messageService.add({ severity: 'success', summary: 'Expense saved successfully', detail: '' })
                        this.isSaveButtonDisabled = true;
                        this.loadModalOptions();
                        this.ExpenseUpdated.emit();
                        this.cancel();
                    } else {
                        this.isSaveButtonDisabled = false;
                        this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                    }

                    this.isSaveButtonDisabled = false;
                })
            }
    }

    async prepareApprovedMail() {
        var status = this.StatusList.find(x => x.listValueID === this.approveStatusID) !== undefined ? this.StatusList.find(x => x.listValueID === this.approveStatusID).valueDesc : '';
        var emailFields: EmailFields = new EmailFields();
        emailFields.EmailCCList = [];
        var RequesterEmail = await this.GetEmail(this.expense.employeeID);
        emailFields.EmailTo = RequesterEmail;
        emailFields.EmailSubject = "Expense " + status.toLowerCase() + " for " + this.expense.employeeName;
        emailFields.EmailBody = "Expense #" + this.expense.expenseID + " has been " + status +
            "<br/>" +
            "<ul style='margin-bottom: 0px;'><li>Type: " + this.expense.expenseType + "<br/>" +
            "</li><li>Requester: " + this.expense.employeeName +
            "</li><li>Amount: " + this.expense.amount.toFixed(2) +
            "</li><li>Description: " + this.expense.submissionComment +
            "</li><li>Payment Date: " + moment(this.expense.paymentDate).format('DD MMM yyy HH:mm:ss') + ' GMT' +
            "</li><li>Payment Comment: " + this.expense.paymentComment +
            "</li><li>Amount Paid: " + this.expense.amountPaid +
            "</li><li>Submitted Date: " + moment(this.expense.createdDate).format('DD MMM yyy HH:mm:ss') + ' GMT' +
            "</li>" +
            "</ul>";
        return emailFields;
    }

    deny = () => {
        if (this.isValidForDeny()) {
            var ClientID: string = localStorage.getItem("ClientID");
            this.ExpenseForm.controls.StatusID.patchValue(this.denyStatusID)
            this.ExpenseForm.value.SubmissionDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.ExpenseForm.value.SubmissionDate));
            if (this.ExpenseForm.value.PaymentDate !== null) {
                this.ExpenseForm.value.PaymentDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.ExpenseForm.value.PaymentDate));
            }
            this.expenseService.UpdateExpense(this.ExpenseForm.value.ExpenseID, this.ExpenseForm.value).subscribe(async (result) => {
                if (result['data'] !== null && result['messageType'] === 1) {
                    this.expense = result['data'];
                    this.expense.submissionComment = result['data'].submissionComment;
                    this.expense.paymentComment = result['data'].paymentComment;
                    var emailApproval: IEmailApproval = new IEmailApproval();
                    if (this.expense.linkID !== null && this.expense.linkID !== Guid.Empty && this.respEmailApproval.linkID !== Guid.Empty) {
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
                        await this.emailApprovalService.EamilApprovalAction(ClientID, this.expense.linkID, "REJECTED", "EXPENSES").subscribe()
                    }
                    else {
                        emailApproval.ModuleID = this.Modules.find(m => m.moduleShort.toUpperCase() == "EXPENSES").moduleID;
                        emailApproval.ID = this.expense.expenseID;
                        emailApproval.ValidTime = new Date(moment().add(7, 'day').format('YYYY-MM-DD HH:mm:ss'));
                        emailApproval.IsActive = true;
                        emailApproval.LinkID = this.commonUtils.newGuid();
                       // emailApproval.ApproverEmail = await this.GetEmail(this.Leave.approverID);
                        emailApproval.CreatedBy = this.user.firstName + " " + this.user.lastName;
                        emailApproval.CreatedDate = new Date()
                    }

                    var emailFields = await this.prepareDenyMail();
                    emailApproval.EmailApprovalID = 0;
                    emailApproval.LinkID = Guid.Empty;
                    emailApproval.EmailSubject = emailFields.EmailSubject;
                    emailApproval.EmailBody = emailFields.EmailBody;
                    emailApproval.EmailFrom = emailFields.EmailFrom;
                  //  emailApproval.EmailCC = emailFields.EmailCC;
                    emailApproval.EmailTo = emailFields.EmailTo;
                    emailApproval.Value = null;
                    emailApproval.IsActive = true;
                    await this.emailApprovalService.SaveEmailApproval(emailApproval).subscribe()
                    this.messageService.add({ severity: 'success', summary: 'Expense saved successfully', detail: '' });
                    this.isSaveButtonDisabled = true;
                    this.loadModalOptions();
                    this.ExpenseUpdated.emit();
                    this.cancel();
                } else {
                    this.isSaveButtonDisabled = false;
                    this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                }
                this.isSaveButtonDisabled = false;
            })
        }
    }

    async prepareDenyMail() {
        var status = this.StatusList.find(x => x.listValueID === this.denyStatusID) !== undefined ? this.StatusList.find(x => x.listValueID === this.denyStatusID).valueDesc : '';
        var emailFields: EmailFields = new EmailFields();
        var RequesterEmail = await this.GetEmail(this.expense.employeeID);
        emailFields.EmailTo = RequesterEmail;
        emailFields.EmailSubject = "Expense " + status.toLowerCase() + " for " + this.expense.employeeName;
        emailFields.EmailBody = "Expense #" + this.expense.expenseID + " has been " + status +
            "<br/>" +
            "<ul style='margin-bottom: 0px;'><li>Type: " + this.expense.expenseType + "<br/>" +
            "</li><li>Requester: " + this.expense.employeeName +
            "</li><li>Amount: " + this.expense.amount.toFixed(2) +
            "</li><li>Description: " + this.expense.submissionComment +
            "</li><li>Payment Comment: " + this.expense.paymentComment +
            "</li><li>Submitted Date: " + moment(this.expense.createdDate).format('DD MMM yyy HH:mm:ss') + ' GMT' +
            "</li>" +
            "</ul>";
        return emailFields;
    }

    fileDownload(file: string) {
       // console.log(file)
        if (file !== null && file !== undefined && file !== '') {
            this.expenseService.DownloadFile(this.ClientID, file).subscribe(result => {
                this.downloadFile(result['data'].memoryStream, file);
            });
        }
    }


    downloadFile(base64, fileName: string) {
        const arrayBuffer = this.base64ToArrayBuffer(base64);
        this.createAndDownloadBlobFile(arrayBuffer, fileName);
    }

    base64ToArrayBuffer(base64) {
        const binaryString = window.atob(base64);
        const bytes = new Uint8Array(binaryString.length);
        return bytes.map((byte, i) => binaryString.charCodeAt(i));
    }

    createAndDownloadBlobFile(body, fileName) {
        const blob = new Blob(([body]));
        const link = document.createElement('a');
        if (link.download !== undefined) {
            const url = URL.createObjectURL(blob);
            link.setAttribute('href', url);
            link.setAttribute('download', fileName);
            link.style.visibility = 'hidden';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }
    }

}
