import { Component, ElementRef, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import * as moment from 'moment';
import { MessageService } from 'primeng/api';
import { forkJoin } from 'rxjs';
import { CommonUtils } from '../../../../../common/common-utils';
import { Constants, ErrorMsg, Guid, ListTypeConstants, SessionConstants, Settings, TimeSheetStatusConstants } from '../../../../../constant';
import { IEmailApprovalDisplay } from '../../../../../core/interfaces/EmailApproval';
import { IEmployeeDisplay } from '../../../../../core/interfaces/Employee';
import { IEmployeeAssignmentDisplay } from '../../../../../core/interfaces/EmployeeAssignment';
import { IRolePermissionDisplay } from '../../../../../core/interfaces/RolePermission';
import { ITimeEntry, ITimeEntryDisplay } from '../../../../../core/interfaces/TimeEntry';
import { ITimesheetDisplay } from '../../../../../core/interfaces/Timesheet';
import { IModalPopupAlternateOptions } from '../../../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { EmailApprovalService } from '../../../../employee/emailApproval.service';
import { EmployeeService } from '../../../../employee/employee.service';
import { ExpenseService } from '../../../../expenses/expense.service';
import { LookUpService } from '../../../../lookup/lookup.service';
import { RolePermissionService } from '../../../../role-permission/role-permission.service';
import { UserService } from '../../../../user/user.service';
import { TimesheetService } from '../../../timesheet.service';

@Component({
  selector: 'app-timesheet-approval-deny',
  templateUrl: './timesheet-approval-deny.component.html',
  styleUrls: ['./timesheet-approval-deny.component.scss']
})
export class TimesheetApprovalDenyComponent implements OnInit {
    settings = new Settings()
    commonUtils = new CommonUtils()
    @Output() TimeSheetUpdated = new EventEmitter<any>();
    @ViewChild('AddEditTimesheetModal') AddEditTimesheetModal: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;
    user: any;
    submitted: boolean = false;
    moment: any = moment;

    TimesheetForm: FormGroup;
    RolePermissions: IRolePermissionDisplay[] = [];
    EmployeeInfoRolePermission: IRolePermissionDisplay;
    ExpenseId: number;
    ModalHeading: string = 'Add Timesheet';
    isSaveButtonDisabled: boolean = false;
    isShow: boolean = true;
    dateTime = new Date();

    formData: FormData = new FormData();
    progress: number = 30;
    @ViewChild('file', { static: false }) expenseFile: ElementRef;
    file: File = null;
    IsApprover: boolean = false;
    TimesheetEmail: string;
    ClientID: string;
    projectsList = [{ label: 'Non-Billable', value: 'Non-Billable' }, { label: 'Client Holiday', value: 'Client Holiday' }, { label: 'Personal Leave', value: 'Personal Leave' }]


    constructor(private fb: FormBuilder,
        private lookupService: LookUpService,
        private messageService: MessageService,
        private userService: UserService,
        private employeeService: EmployeeService,
        private timeSheetService: TimesheetService,
        private expenseService: ExpenseService,
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
        this.TimesheetEmail = this.settings.EmailNotifications[this.ClientID]['Expense']
        //if (RoleShort.toUpperCase() == UserRole.ADMIN || RoleShort.toUpperCase() == UserRole.FINADMIN) {
        //    this.IsApprover = true;
        //}
        this.LoadDropDown();
        this.buildTimesheetForm({}, 'New');
    }

    ngOnInit(): void {
        this.loadModalOptions();
    }
    Employees: IEmployeeDisplay[] = [];
    Modules: any[] = [];
    lstEmployees: IEmployeeDisplay[] = [];
    TicketEmailMapList: any[] = [];
    TimeSheetStatusList: any[] = [];
    WeekEndingDayList: any[] = [];
    UserList: any[] = []
    buttonsdisabledvalue: boolean = false;

    LoadDropDown() {
        forkJoin(
            this.employeeService.GetEmployees(),
            this.lookupService.getListValues(),
            this.rolePermissionService.getModules(),
            this.userService.getUserList()
        ).subscribe(async resultSet => {
            if (resultSet !== undefined && resultSet !== null) {
                //this.Employees = resultSet[0]['data']
                if (this.user.employeeID != null) {
                    this.Employees = resultSet[0]['data'].filter(x => x.managerID == this.user.employeeID || x.employeeID == this.user.employeeID);
                }
                else {
                    this.Employees = resultSet[0]['data'];
                }
                console.log('this.Employees', this.Employees)
                this.TimeSheetStatusList = resultSet[1]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.TIMESHEETSTATUS);
                this.WeekEndingDayList = resultSet[1]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.TIMESHEETTYPE);
                //if (this.ExpenseTypeList !== null && this.ExpenseTypeList !== undefined && this.ExpenseTypeList.length > 0) {
                //    var ExpenseType = this.ExpenseTypeList.find(x => x.value == "MISC");
                //    if (ExpenseType !== null && ExpenseType !== undefined) {
                //        this.defaultExpenseTypeID = ExpenseType.listValueID;
                //    }
                //}

                this.Modules = resultSet[2];
                this.UserList = resultSet[3]['data']
                if (this.user.employeeID != null) {
                    await this.GetEmployeeDetails(Number(this.user.employeeID));
                }
            }
        })
    }

    loadModalOptions() {
        this.modalOptions = {
            footerActions: [
                {
                    actionText: 'Approve',
                    actionMethod: this.ApproveTimeSheet,
                    styleClass: 'p-button-raised p-mr-2 p-mb-2',
                    iconClass: 'mdi mdi-content-save',
                    disabled: this.buttonsdisabledvalue
                },
                {
                    actionText: 'Reject',
                    actionMethod: this.RejectTimeSheet,
                    styleClass: 'p-button-raised p-button-danger p-mr-2 p-mb-2',
                    iconClass: 'p-button-raised p-button-danger',
                    disabled: this.buttonsdisabledvalue
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

    employeeDetails: IEmployeeDisplay;
    EmployeeAssignments: IEmployeeAssignmentDisplay[] = [];
    EmployeeAssignment: IEmployeeAssignmentDisplay;
    GetEmployeeDetails(employeeID: number) {
        this.employeeService.getEmployeeByIdAsync(employeeID).subscribe(result => {
            if (result !== undefined && result['data'] !== null && result['data'] !== undefined && result['data'].assignments.length > 0) {
                result['data'].assignments.forEach((d) => {
                    d.startDate = moment(d.startDate).format("MM/DD/YYYY")
                    if (d.endDate !== null) {
                        d.endDate = moment(d.endDate).format("MM/DD/YYYY")
                    }
                })
                this.employeeDetails = result['data'];
                this.EmployeeAssignments = result['data'].assignments;
                this.TimesheetForm.controls.EmployeeID.patchValue(this.employeeDetails.employeeID)
                this.TimesheetForm.controls.EmployeeName.patchValue(this.employeeDetails.employeeName)
                // this.EmployeeAssignment = this.EmployeeAssignments.find(ea => ea.clientID == 0);
            }
        }, error => {
            //toastService.ShowError();
        })
    }

    ErrorMessage: string = '';
    async GetClientDetails(clientid: number) {
        this.ErrorMessage = "";
            this.TimesheetForm.controls.ClientID.patchValue(clientid)
            this.TimesheetForm.controls.StatusID.patchValue(this.TimeSheetStatusList.find(tss => tss.value.toUpperCase() === TimeSheetStatusConstants.PENDING.toUpperCase()).listValueID)
            this.TimesheetForm.controls.Status.patchValue(this.TimeSheetStatusList.find(tss => tss.value.toUpperCase() === TimeSheetStatusConstants.PENDING.toUpperCase()).valueDesc)
            this.TimesheetForm.controls.SubmittedDate.patchValue(new Date())
            this.TimesheetForm.controls.ApprovedDate.patchValue(new Date())
            this.TimesheetForm.controls.ClosedDate.patchValue(null)

            if (this.EmployeeAssignment != null) {
                this.TimesheetForm.controls.EmployeeID.patchValue(this.EmployeeAssignment.employeeID)
                this.TimesheetForm.controls.TimeSheetTypeID.patchValue(this.EmployeeAssignment.timesheetTypeID !== null ? Number(this.EmployeeAssignment.timeSheetTypeID) : null)
                this.TimesheetForm.controls.TimeSheetType.patchValue(this.EmployeeAssignment.timeSheetType);
                this.TimesheetForm.controls.TSApproverEmail.patchValue(this.EmployeeAssignment.tsApproverEmail);
                this.TimesheetForm.controls.AssignmentID.patchValue(this.EmployeeAssignment.assignmentID);
            }
    }


    buildTimesheetForm(data: any, keyName: string) {
        this.TimesheetForm = this.fb.group({
            TimeSheetID: [keyName === 'New' ? 0 : data.timeSheetID],
            WeekEnding: [keyName === 'New' ? null : data.weekEnding !== null ? new Date(data.weekEnding) : null, Validators.required],
            EmployeeID: [keyName === 'New' ? null : data.employeeID],
            EmployeeName: [keyName === 'New' ? null : data.employeeName],
            ClientID: [keyName === 'New' ? null : data.clientID, Validators.required],
            ClientName: [keyName === 'New' ? '' : data.clientName],
            ClientManager: [keyName === 'New' ? '' : data.clientManager],
            AssignmentID: [keyName === 'New' ? null : data.assignmentID],
            TotalHours: [keyName === 'New' ? '0' : data.totalHours],
            FileName: [keyName === 'New' ? null : data.fileName],
            TSApproverEmail: [keyName === 'New' ? '' : data.tsApproverEmail],
            ApprovedEmailTo: [keyName === 'New' ? '' : data.approvedEmailTo],
            StatusID: [keyName === 'New' ? null : data.statusID, Validators.required],
            Status: [keyName === 'New' ? '' : data.status],
            StatusValue: [keyName === 'New' ? '' : data.statusValue],
            SubmittedByID: [keyName === 'New' ? null : data.submittedByID],
            SubmittedBy: [keyName === 'New' ? null : data.submittedBy],
            SubmittedDate: [keyName === 'New' ? null : data.submittedDate !== null ? new Date(data.submittedDate) : null],
            ApprovedDate: [keyName === 'New' ? null : data.approvedDate !== null ? new Date(data.approvedDate) : null],
            ApprovedByEmail: [keyName === 'New' ? '' : data.approvedByEmail],
            ClosedByID: [keyName === 'New' ? null : data.closedByID],
            ClosedBy: [keyName === 'New' ? '' : data.closedBy],
            ClosedDate: [keyName === 'New' ? null : data.closedDate !== null ? new Date(data.closedDate) : null],
            TimeSheetTypeID: [keyName === 'New' ? null : data.timeSheetTypeID],
            TimeSheetType: [keyName === 'New' ? '' : data.timeSheetType],
            Comment: [keyName === 'New' ? '' : data.comment],
            CreatedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : this.user.firstName + " " + this.user.lastName],
            CreatedDate: [keyName === 'New' ? new Date() : new Date(data.createdDate)],
            ModifiedDate: [keyName === 'New' ? new Date() : data.modifiedDate],
            ModifiedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : data.modifiedBy],
            TimeStamp: [''],
            LinkID: [keyName === 'New' ? Guid.Empty : data.linkID],
            TimeEntries: this.TimeEntryList
        });
    }
    TimeEntryList: ITimeEntry[] = [];
    TimeSheetId: number = 0;
    isCommentExist: boolean = false;
    totalhours: string = '0';
    disabledvalue: boolean = false;
    entrydisabledvalue: boolean = false;

    Show(Id: number) {
        this.ErrorMessage = "";
        this.TimeSheetId = Id;
        this.isCommentExist = false;
        //this.ResetDialog();
        if (this.TimeSheetId != 0) {
            this.totalhours = "0";
            this.disabledvalue = true;
            this.entrydisabledvalue = true;
            this.GetDetails(this.TimeSheetId);
        }
        else {
            this.TimeEntryList = [];
            this.totalhours = "0";
            this.disabledvalue = true;
            this.entrydisabledvalue = false;
        }
    }

    timesheet: ITimesheetDisplay;
    TimeEntryListDisplay: ITimeEntryDisplay[] = [];
    respEmailApproval: IEmailApprovalDisplay;
    async GetDetails(Id: number) {
        this.timeSheetService.GetTimesheetByIdAsync(Id).subscribe(async result => {
            if (result !== null && result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                this.timesheet = result['data'];

                this.TimeEntryListDisplay = this.timesheet.timeEntries;
                if (this.timesheet.timeEntries !== null && this.timesheet.timeEntries !== undefined && this.timesheet.timeEntries.length > 0) {
                    this.TimeEntryList = [];
                    this.timesheet.timeEntries.forEach(x => {
                        var TimesheetEntery = new ITimeEntry();
                        TimesheetEntery.TimeEntryID = x.timeEntryID
                        TimesheetEntery.TimeSheetID = x.timeSheetID
                        TimesheetEntery.WorkDate = x.workDate
                        TimesheetEntery.Project = x.project
                        TimesheetEntery.Activity = x.activity
                        TimesheetEntery.Hours = x.hours
                        TimesheetEntery.CreatedBy = x.createdBy
                        TimesheetEntery.CreatedDate = x.createdDate
                        TimesheetEntery.ModifiedBy = x.modifiedBy
                        TimesheetEntery.ModifiedDate = x.modifiedDate
                        this.TimeEntryList.push(TimesheetEntery)
                    })
                }
                //this.TimeEntryList = 
                await this.GetEmployeeDetails(Number(this.timesheet.employeeID));
                this.TimesheetForm.controls.ApprovedDate.patchValue(new Date())
                this.TimesheetForm.controls.ClosedDate.patchValue(null)
                //timesheet.SubmittedDate = DateTime.Now;
                //timesheet.ApprovedDate = null;
                //timesheet.ClosedDate = null;
                this.totalhours = this.timesheet.totalHours.toString();
                this.entrydisabledvalue = true;
                if (this.timesheet.statusValue.toUpperCase() !== TimeSheetStatusConstants.SUBMITTED.toUpperCase()) {
                    this.entrydisabledvalue = true;
                    this.buttonsdisabledvalue = true;
                }
                else {
                    this.entrydisabledvalue = false;
                    this.buttonsdisabledvalue = false;
                }
               
                if (this.timesheet.linkID !== null && this.timesheet.linkID !== Guid.Empty) {
                    this.emailApprovalService.GetEmailApprovalByIdAsync(this.timesheet.linkID).subscribe(async (resp) => {
                        if (resp['data'] !== null && resp['messageType'] === 1) {
                            this.respEmailApproval = resp['data'];
                        }
                    })
                }
                this.loadModalOptions();
                this.buildTimesheetForm(this.timesheet, 'Edit')
                this.ModalHeading = "Edit Timesheet";
                this.AddEditTimesheetModal.show()
            }
        })
    }

    ApproveTimeSheet = () => {
        this.ErrorMessage = '';
        this.submitted = true;
        if (this.TimesheetForm.invalid) {
            this.TimesheetForm.markAllAsTouched();
            return;
        } else {
            if (this.buttonsdisabledvalue) {
                return;
            } else {
                this.buttonsdisabledvalue = true;
                if (this.TimeSheetStatusList.find(tss => tss.value.toUpperCase() === TimeSheetStatusConstants.APPROVED.toUpperCase() !== undefined)) {
                    this.TimesheetForm.controls.StatusID.patchValue(this.TimeSheetStatusList.find(tss => tss.value.toUpperCase() === TimeSheetStatusConstants.APPROVED.toUpperCase()).listValueID)
                    this.TimesheetForm.controls.Status.patchValue(this.TimeSheetStatusList.find(tss => tss.value.toUpperCase() === TimeSheetStatusConstants.APPROVED.toUpperCase()).valueDesc)
                }
                this.eamilApprovalAction();
                this.buttonsdisabledvalue = false;
            }
        }
    }

    eamilApprovalAction() {
        this.TimesheetForm.controls.ApprovedByEmail.patchValue(this.user.email);
        this.TimesheetForm.controls.TimeEntries.patchValue(this.TimeEntryList);
        this.TimesheetForm.controls.ClientName.patchValue(this.EmployeeAssignments.find(cl => cl.clientID == this.TimesheetForm.value.ClientID).client);

        var LinkID = this.timesheet.linkID;

        var value = '';
        if (this.TimesheetForm.value.StatusID == this.TimeSheetStatusList.find(tss => tss.value.toUpperCase() == TimeSheetStatusConstants.APPROVED.toUpperCase()).listValueID || this.TimesheetForm.value.StatusID == this.TimeSheetStatusList.find(tss => tss.value.toUpperCase() == TimeSheetStatusConstants.REJECTED.toUpperCase()).listValueID) {
            if (this.TimesheetForm.value.Status.toUpperCase() == "APPROVED") {
                value = "APPROVE";
            } else {
                value = "REJECT";
            }
            if (LinkID !== Guid.Empty) {
                this.emailApprovalService.eamilApprovalAction(this.ClientID, LinkID, value).subscribe(result => {
                    if (result['data'] !== null && result['messageType'] === 1 && result['messageType'] === 1) {
                        this.messageService.add({ severity: 'success', summary: 'Timesheet saved successfully', detail: '' });
                        this.isSaveButtonDisabled = true;
                        this.loadModalOptions();
                        this.TimeSheetUpdated.emit();
                        this.cancel();
                    } else {
                        this.isSaveButtonDisabled = false;
                        this.messageService.add({ severity: 'error', summary: 'Timesheet Link Expired', detail: '' });
                        this.cancel();
                    }
                })
            } else {
                this.buttonsdisabledvalue = false;
                this.messageService.add({ severity: 'error', summary: 'Timesheet Link Expired', detail: '' });
              //  this.cancel();
            }
        }
    }

    RejectTimeSheet = () => {
        this.ErrorMessage = '';
        this.submitted = true;
        if (this.TimesheetForm.invalid) {
            this.TimesheetForm.markAllAsTouched();
            return;
        } else {
            if (this.buttonsdisabledvalue) {
                return;
            } else {
                if (this.TimesheetForm.value.Comment !== null && this.TimesheetForm.value.Comment != null && this.TimesheetForm.value.Comment != "") {
                    if (this.TimeSheetStatusList.find(tss => tss.value.toUpperCase() === TimeSheetStatusConstants.REJECTED.toUpperCase() !== undefined)) {
                        this.TimesheetForm.controls.StatusID.patchValue(this.TimeSheetStatusList.find(tss => tss.value.toUpperCase() === TimeSheetStatusConstants.REJECTED.toUpperCase()).listValueID)
                        this.TimesheetForm.controls.Status.patchValue(this.TimeSheetStatusList.find(tss => tss.value.toUpperCase() === TimeSheetStatusConstants.REJECTED.toUpperCase()).valueDesc)
                    }
                    this.buttonsdisabledvalue = true;
                    this.eamilApprovalAction();
                    this.buttonsdisabledvalue = false;
                } else {
                    this.isCommentExist = true;
                }
            }
        }
    }

    onCommentChange() {
        var paymentAmount = this.TimesheetForm.value.Comment;
        if (paymentAmount != null && paymentAmount !== undefined && paymentAmount != '') {
            this.isCommentExist = false;
        }
        else {
            this.isCommentExist = true;
        }
    }

    cancel = () => {
        this.submitted = false;
        this.isCommentExist = false;
        this.buildTimesheetForm({}, 'New');
        this.buttonsdisabledvalue = false;
        this.entrydisabledvalue = false;
        this.AddEditTimesheetModal.hide();
    }
    ResetDialog(){
        this.submitted = false;
        this.isCommentExist = false;
        this.buildTimesheetForm({}, 'New');
        this.buttonsdisabledvalue = false;
        this.entrydisabledvalue = false;
        //this.AddEditTimesheetModal.hide();
    }


}
