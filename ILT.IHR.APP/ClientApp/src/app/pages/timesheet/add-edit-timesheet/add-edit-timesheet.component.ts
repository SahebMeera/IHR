import { Component, ElementRef, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import * as moment from 'moment';
import { MessageService } from 'primeng/api';
import { forkJoin } from 'rxjs';
import { CommonUtils } from '../../../common/common-utils';
import { Constants, EmailApprovalUrl, ErrorMsg, Guid, ListTypeConstants, SessionConstants, Settings, TimeSheetStatusConstants, TimeSheetType, UserRole } from '../../../constant';
import { EmailFields } from '../../../core/interfaces/Common';
import { IEmailApproval, IEmailApprovalDisplay } from '../../../core/interfaces/EmailApproval';
import { IEmployeeDisplay } from '../../../core/interfaces/Employee';
import { IEmployeeAssignmentDisplay } from '../../../core/interfaces/EmployeeAssignment';
import { IRolePermissionDisplay } from '../../../core/interfaces/RolePermission';
import { ITimeEntry, ITimeEntryDisplay } from '../../../core/interfaces/TimeEntry';
import { ITimesheetDisplay } from '../../../core/interfaces/Timesheet';
import { DataProvider } from '../../../core/providers/data.provider';
import { IModalPopupAlternateOptions } from '../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { UserToken } from '../../../_models';
import { EmailApprovalService } from '../../employee/emailApproval.service';
import { EmployeeService } from '../../employee/employee.service';
import { ExpenseService } from '../../expenses/expense.service';
import { LookUpService } from '../../lookup/lookup.service';
import { RolePermissionService } from '../../role-permission/role-permission.service';
import { UserService } from '../../user/user.service';
import { TimesheetService } from '../timesheet.service';

@Component({
  selector: 'app-add-edit-timesheet',
  templateUrl: './add-edit-timesheet.component.html',
  styleUrls: ['./add-edit-timesheet.component.scss']
})
export class AddEditTimesheetComponent implements OnInit {
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
        private dataProvider: DataProvider,
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
                    actionText: 'Save',
                    actionMethod: this.timeSheetValidPending,
                    styleClass:  'p-button-raised p-mr-2 p-mb-2',
                    iconClass: 'mdi mdi-content-save',
                    disabled: this.buttonsdisabledvalue
                },
                {
                    actionText: 'Submit',
                    actionMethod: this.timeSheetValidSubmit,
                    styleClass: 'p-button-raised p-mr-2 p-mb-2',
                    iconClass: 'mdi mdi-content-save',
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
                this.TimesheetForm.controls.EmployeeName.patchValue(this.employeeDetails.employeeName)
               // this.EmployeeAssignment = this.EmployeeAssignments.find(ea => ea.clientID == 0);
            }
        }, error => {
            //toastService.ShowError();
        })
    }
    buttonsdisabledvalue: boolean = false;
    ErrorMessage: string = '';
    TimeSheetId: number = -1;
    totalhours: string;
    disabledvalue: boolean = false;
    entrydisabledvalue: boolean = false;
    isMonthly: boolean = false;
    TimeEntryList: ITimeEntry[] = [];
    TimeEntryListDisplay: ITimeEntryDisplay[] = [];

    async Show(Id: number) {
        this.ErrorMessage = "";
        this.ResetDialog();
        this.loadModalOptions();
        this.TimeSheetId = Id;
        this.buttonsdisabledvalue = false;
        if (this.user.employeeID !== null) {
            //   timesheet.EmployeeID = Numbuser.EmployeeID;
            this.buttonsdisabledvalue = false;
            this.file = null;
            if (this.TimeSheetId != 0) {
                this.totalhours = "0";
                this.disabledvalue = true;
                this.entrydisabledvalue = true;
                this.GetDetails(this.TimeSheetId);
                this.loadModalOptions();
            }
            else {
                this.TimeEntryList = [];
                this.totalhours = "0";
                this.disabledvalue = false;
                this.entrydisabledvalue = false;
                this.buildTimesheetForm({}, 'New');
                this.TimesheetForm.controls.EmployeeID.patchValue(this.user.employeeID)
                await this.GetEmployeeDetails(Number(this.user.employeeID));
                this.ModalHeading = "Add Timesheet";
                //isfirstElementFocus = true;
                this.isMonthly = false;
                this.loadModalOptions();
                this.AddEditTimesheetModal.show()

            }
        }
        else {
            this.messageService.add({ severity: 'error', summary: `Timesheet can't be created for non - Employees`, detail: '' })
        }
    }
    timesheet: ITimesheetDisplay;
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
                //timesheet.SubmittedDate = DateTime.Now;
                //timesheet.ApprovedDate = null;
                //timesheet.ClosedDate = null;
                this.totalhours = this.timesheet.totalHours.toString();
                //var tempTotalHours = 0;
                //this.TimeEntryList.forEach(tl =>
                //{
                //    tempTotalHours = tempTotalHours + tl.Hours;
                //});
              //  this.totalhours = tempTotalHours.toString();
                if (this.timesheet.statusValue.toUpperCase() !== TimeSheetStatusConstants.PENDING.toUpperCase()) {
                    this.entrydisabledvalue = true;
                    this.buttonsdisabledvalue = true;
                }
                else {
                    this.entrydisabledvalue = false;
                    this.buttonsdisabledvalue = false;
                }
                if (this.timesheet.timeSheetType.toUpperCase() == TimeSheetType.MONTHLY) {
                    this.isMonthly = true;
                }
                else {
                    this.isMonthly = false;
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
    get addEditTicketControls() { return this.TimesheetForm.controls; }
   // EmployeeAssignment: IEmployeeAssignmentDisplay;
     async changeemployee(e: any) {
        var employeeid = Number(e.value);
        await this.GetEmployeeDetails(employeeid);
        await this.GetClientDetails(0);
     }

    async changeclient(e: any) {
        var clientid = Number(e.value);
        await this.GetEmployeeDetails(this.TimesheetForm.value.EmployeeID);
        this.EmployeeAssignment = this.EmployeeAssignments.find(ea => ea.clientID === clientid);
        await this.GetClientDetails(clientid);
    }

    async GetClientDetails(clientid: number) {
        this.ErrorMessage = "";
         this.TimeEntryList = [];
         this.TimesheetForm.controls.WeekEnding.patchValue(null)
        this.isMonthly = false;
         if (clientid === 0) {
             this.TimesheetForm.controls.ClientID.patchValue(null)
             this.TimesheetForm.controls.StatusID.patchValue(null)
             this.TimesheetForm.controls.Status.patchValue('')
             this.TimesheetForm.controls.TimeSheetTypeID.patchValue(null)
             this.TimesheetForm.controls.TimeSheetType.patchValue('')
             this.TimesheetForm.controls.TSApproverEmail.patchValue('')
             this.TimesheetForm.controls.AssignmentID.patchValue(null)
             this.totalhours = '0';
        }
         else {
             this.TimesheetForm.controls.ClientID.patchValue(clientid)
             this.TimesheetForm.controls.StatusID.patchValue(this.TimeSheetStatusList.find(tss => tss.value.toUpperCase() === TimeSheetStatusConstants.PENDING.toUpperCase()).listValueID)
             this.TimesheetForm.controls.Status.patchValue(this.TimeSheetStatusList.find(tss => tss.value.toUpperCase() === TimeSheetStatusConstants.PENDING.toUpperCase()).valueDesc)
             this.TimesheetForm.controls.SubmittedDate.patchValue(new Date())
             this.TimesheetForm.controls.ApprovedDate.patchValue(null)
             this.TimesheetForm.controls.ClosedDate.patchValue(null)
   
             if (this.EmployeeAssignment != null) {
                 this.TimesheetForm.controls.EmployeeID.patchValue(this.EmployeeAssignment.employeeID)
                 this.TimesheetForm.controls.TimeSheetTypeID.patchValue(this.EmployeeAssignment.timesheetTypeID !== null ? Number(this.EmployeeAssignment.timeSheetTypeID) : null)
                 this.TimesheetForm.controls.TimeSheetType.patchValue(this.EmployeeAssignment.timeSheetType);
                     
                 if (this.TimesheetForm.value.TimeSheetType.toUpperCase() === TimeSheetType.MONTHLY) {
                    this.isMonthly = true;
                }
                else {
                    this.isMonthly = false;
                }
                //timesheet.TimesheetApproverID = EmployeeAssignment.TimesheetApproverID != null ? (int)EmployeeAssignment.TimesheetApproverID : 0;
                //timesheet.TSApproverName = EmployeeAssignment.TimesheetApprover;
                 this.TimesheetForm.controls.TSApproverEmail.patchValue(this.EmployeeAssignment.tsApproverEmail);
                 this.TimesheetForm.controls.AssignmentID.patchValue(this.EmployeeAssignment.assignmentID);
            }
        }

     }

     getDays(year, month){
        return new Date(year, month, 0).getDate();
    };

    async LoadWorkDates(e: any) {
        this.ErrorMessage = "";
        this.TimeEntryList = [];
        var dateexists = false;
        var weekendingdate = new Date(e);
        if (this.EmployeeAssignment != null && this.EmployeeAssignment != undefined && this.EmployeeAssignment.timeSheetType.toUpperCase() === TimeSheetType.MONTHLY) {
            //weekendingdate = new DateTime(weekendingdate.Year, weekendingdate.Month, DateTime.DaysInMonth(weekendingdate.Year, weekendingdate.Month));
            var lastDayOfMonth = new Date(weekendingdate.getFullYear(), weekendingdate.getMonth() + 1, 0);
            weekendingdate = new Date(weekendingdate.getFullYear(), weekendingdate.getMonth() + 1, 0);
        }
        await this.timeSheetService.GetTimeSheets(this.TimesheetForm.value.EmployeeID, 0).subscribe(async respRequest => {
            if (respRequest['data'] !== null && respRequest['messageType'] === 1) {
                var TimeSheetsList = respRequest['data'].filter(tsl => tsl.employeeID == this.TimesheetForm.value.EmployeeID && tsl.clientID == this.TimesheetForm.value.ClientID);
                dateexists = TimeSheetsList.findIndex(ts => moment(ts.weekEnding).format('MM/DD/YYYY') === moment(weekendingdate).format('MM/DD/YYYY') && ts.statusValue.toUpperCase() !== TimeSheetStatusConstants.REJECTED.toUpperCase() && ts.statusValue.toUpperCase() !== TimeSheetStatusConstants.VOID.toUpperCase()) > -1;
            }
            if (this.EmployeeAssignment != null && this.EmployeeAssignment != undefined) {
                var assignmentStartDate = new Date(moment(this.EmployeeAssignment.startDate).format('MM/DD/YYYY').toString())
                var assignmentEndDate = this.EmployeeAssignment.endDate !== null ? new Date(moment(this.EmployeeAssignment.endDate).format('MM/DD/YYYY').toString()) : null;

                if (this.EmployeeAssignment != null && assignmentEndDate !== null && (new Date(this.TimesheetForm.value.WeekEnding) < assignmentStartDate || new Date(this.TimesheetForm.value.WeekEnding) > assignmentEndDate)) {
                    this.ErrorMessage = "Please Check Assignment dates and enter valid week ending";
                }
                else if (this.EmployeeAssignment != null && assignmentEndDate == null && new Date(this.TimesheetForm.value.WeekEnding) < assignmentStartDate) {
                    this.ErrorMessage = "Please Check Assignment dates and enter valid week ending";
                } else if (dateexists) {
                    if (this.EmployeeAssignment != null && this.EmployeeAssignment.timeSheetType.toUpperCase() === TimeSheetType.MONTHLY) {
                        this.ErrorMessage = "Timesheet already exists for the selected month ending";
                    }
                    else {
                        this.ErrorMessage = "Timesheet already exists for the selected week ending";
                    }
                } else {
                    var currentStartDate = new Date(moment(weekendingdate).subtract(7, 'd').format('MM/DD/YYYY'));
                    if (this.EmployeeAssignment != null) {
                        var timesheettype = this.EmployeeAssignment.timeSheetType;
                        this.TimeEntryList = [];
                        if (timesheettype == null) {
                            this.ErrorMessage = "Timesheet Type not defined";
                        } else if (timesheettype.toUpperCase() == TimeSheetType.MONTHLY) {
                            this.ErrorMessage = "";
                            this.TimesheetForm.controls.WeekEnding.patchValue(weekendingdate);
                            var daysInMonth = [];
                            var monthDate = moment(weekendingdate).startOf('month');
                            for (let i = 0; i < monthDate.daysInMonth(); i++) {
                                var temptimeentry = new ITimeEntry();
                                let newDay = monthDate.clone().add(i, 'days');
                                temptimeentry.WorkDate = new Date(newDay.format('MM/DD/YYYY'));
                                temptimeentry.Hours = 0;
                                daysInMonth.push(temptimeentry);
                                this.TimeEntryList.push(temptimeentry);
                            }
                        } else if (timesheettype.toUpperCase() == TimeSheetType.FRIDAY) {
                            var daysInMonth = [];
                            if (moment(currentStartDate).format('dddd').toUpperCase() !== "FRIDAY") {
                                this.ErrorMessage = "Selected Date must be Friday";
                            } else {
                                this.ErrorMessage = "";
                                var date = moment(currentStartDate);
                                for (let i = 0; i < 7; i++) {
                                    var temptimeentry = new ITimeEntry();
                                    let newDay = date.clone().add(i+1, 'days');
                                    temptimeentry.WorkDate = new Date(newDay.format('MM/DD/YYYY'));
                                    temptimeentry.Hours = 0;
                                    daysInMonth.push(temptimeentry);
                                    this.TimeEntryList.push(temptimeentry);
                                }

                            }
                        } else if (timesheettype.toUpperCase() == TimeSheetType.SATURDAY) {
                            var daysInMonth = [];
                            if (moment(currentStartDate).format('dddd').toUpperCase() !== "SATURDAY") {
                                this.ErrorMessage = "Selected Date must be Saturday";
                            } else {
                                this.ErrorMessage = "";
                                var date = moment(currentStartDate);
                                for (let i = 0; i < 7; i++) {
                                    var temptimeentry = new ITimeEntry();
                                    let newDay = date.clone().add(i+1, 'days');
                                    temptimeentry.WorkDate = new Date(newDay.format('MM/DD/YYYY'));
                                    temptimeentry.Hours = 0;
                                    daysInMonth.push(temptimeentry);
                                    this.TimeEntryList.push(temptimeentry);
                                }
                            }
                        }
                        else if (timesheettype.toUpperCase() == TimeSheetType.SUNDAY) {
                            var daysInMonth = [];
                            if (moment(currentStartDate).format('dddd').toUpperCase() !== "SUNDAY") {
                                this.ErrorMessage = "Selected Date must be Sunday";
                            } else {
                                this.ErrorMessage = "";
                                var date = moment(currentStartDate);
                                for (let i = 0; i < 7; i++) {
                                    var temptimeentry = new ITimeEntry();
                                    let newDay = date.clone().add(i+1, 'days');
                                    temptimeentry.WorkDate = new Date(newDay.format('MM/DD/YYYY'));
                                    temptimeentry.Hours = 0;
                                    daysInMonth.push(temptimeentry);
                                    this.TimeEntryList.push(temptimeentry);
                                }
                            }
                        }

                    } else {
                        this.TimesheetForm.controls.TimeSheetTypeID.patchValue(null);
                        this.TimesheetForm.controls.TimeSheetType.patchValue('');
                    }
                }
            }
        })

    }

    async LoadProject(projectvalue: string, workdate: any) {
        
        this.TimeEntryList.forEach(te => {
            if (moment(workdate).format("ddd").toUpperCase() !== "SAT" && moment(workdate).format("ddd").toUpperCase() !== "SUN") {
                if (this.TimesheetForm.value.TimeSheetType.toUpperCase() === "MONTHLY") {
                    for (let i = 0; i < 30; i++) {
                        if (te.WorkDate == new Date(workdate)) {
                            if (projectvalue != null && (projectvalue.toUpperCase() == "CLIENT HOLIDAY" || projectvalue.toUpperCase() == "PERSONAL LEAVE" || projectvalue.toUpperCase() == "NON-BILLABLE")) {
                                if (te.Hours > 0) {
                                    const tempTotalHours = Number(this.totalhours) - te.Hours;
                                    this.totalhours = tempTotalHours.toString();
                                }
                                te.Hours = 0;
                                te.Activity = "";
                            }
                        }
                        let newDay = moment(workdate).clone().add(i, 'days');
                        if (moment(te.WorkDate).format('MM/DD/YYYY') === moment(newDay).format('MM/DD/YYYY')) {
                            if ((te.Project == null || te.Project == "" || te.Project == undefined) && (moment(te.WorkDate).format("ddd").toUpperCase() !== "SAT" && moment(te.WorkDate).format("ddd").toUpperCase() !== "SUN")) {
                                te.Project = projectvalue;
                            }
                        }

                    }
                } else {
                    for (let i = 0; i < 7; i++) {
                        if (new Date(te.WorkDate) == new Date(workdate)) {
                            if (projectvalue != null && (projectvalue.toUpperCase() == "CLIENT HOLIDAY" || projectvalue.toUpperCase() == "PERSONAL LEAVE" || projectvalue.toUpperCase() == "NON-BILLABLE")) {
                                if (te.Hours > 0) {
                                    const tempTotalHours = Number(this.totalhours) - te.Hours;
                                    this.totalhours = tempTotalHours.toString();
                                }
                                te.Hours = 0;
                                te.Activity = "";
                            }
                        }
                        let newDay = moment(workdate).clone().add(i, 'days');
                        if (moment(te.WorkDate).format('MM/DD/YYYY') === moment(newDay).format('MM/DD/YYYY')) {
                            if ((te.Project == null || te.Project == "") && (moment(te.WorkDate).format("ddd").toUpperCase() != "SAT" && moment(te.WorkDate).format("ddd").toUpperCase() != "SUN")) {
                                te.Project = projectvalue;
                            }
                        }
                    }

        
                }
            }
        })
    }

    async LoadActivity(activityvalue: string, workdate: any) {
        this.TimeEntryList.forEach(te => {
            if (moment(workdate).format("ddd").toUpperCase() !== "SAT" && moment(workdate).format("ddd").toUpperCase() !== "SUN") {
                if (this.TimesheetForm.value.TimeSheetType.toUpperCase() === "MONTHLY") {
                    for (let i = 0; i < 30; i++) {
                        let newDay = moment(workdate).clone().add(i, 'days');
                        if (moment(te.WorkDate).format('MM/DD/YYYY') === moment(newDay).format('MM/DD/YYYY')) {
                            if ((te.Activity == null || te.Activity == "" || te.Activity == undefined) && (moment(te.WorkDate).format("ddd").toUpperCase() !== "SAT" && moment(te.WorkDate).format("ddd").toUpperCase() !== "SUN")) {
                                te.Activity = activityvalue;
                            }
                        }
                    }
                } else {
                    for (let i = 0; i < 7; i++) {
                        let newDay = moment(workdate).clone().add(i, 'days');
                        if (moment(te.WorkDate).format('MM/DD/YYYY') === moment(newDay).format('MM/DD/YYYY')) {
                            if ((te.Activity == null || te.Activity == "" || te.Activity == undefined) && (moment(te.WorkDate).format("ddd").toUpperCase() != "SAT" && moment(te.WorkDate).format("ddd").toUpperCase() != "SUN")) {
                                te.Activity = activityvalue;
                            }
                        }
                    }
                }
            }
        })
    }

    async TotalHours(hoursvalue: string, workdate: any) {
        this.TimeEntryList.forEach(te => {
            if (moment(workdate).format("ddd").toUpperCase() !== "SAT" && moment(workdate).format("ddd").toUpperCase() !== "SUN") {
                if (this.TimesheetForm.value.TimeSheetType.toUpperCase() === "MONTHLY") {
                    for (let i = 0; i < 30; i++) {
                        let newDay = moment(workdate).clone().add(i, 'days');
                        if (moment(te.WorkDate).format('MM/DD/YYYY') === moment(newDay).format('MM/DD/YYYY')) {
                            if ((te.Hours == 0 || te.Hours == null || te.Hours == undefined) && (moment(te.WorkDate).format("ddd").toUpperCase() !== "SAT" && moment(te.WorkDate).format("ddd").toUpperCase() !== "SUN")) {
                                te.Hours = Number(hoursvalue);
                            }
                        }
                    }
                } else {
                    for (let i = 0; i < 7; i++) {
                        let newDay = moment(workdate).clone().add(i, 'days');
                        if (moment(te.WorkDate).format('MM/DD/YYYY') === moment(newDay).format('MM/DD/YYYY')) {
                            if ((te.Hours == null || te.Hours == 0 || te.Hours == undefined) && (moment(te.WorkDate).format("ddd").toUpperCase() != "SAT" && moment(te.WorkDate).format("ddd").toUpperCase() != "SUN")) {
                                te.Hours = Number(hoursvalue);
                            }
                        }
                    }
                }
            }
        })
        let temptotalhours = 0;
        this.TimeEntryList.forEach(te => {
            if (moment(te.WorkDate).format('MM/DD/YYYY') === moment(workdate).format('MM/DD/YYYY')) {
                te.Hours = Number(hoursvalue);
            }
            temptotalhours = temptotalhours + te.Hours;
        });
        this.totalhours = temptotalhours.toString();
        this.TimesheetForm.controls.TotalHours.patchValue(Number(this.totalhours))
    }

    isLoading
    fileNames: any[] = [];
    fileName: string = ''
    async onFileChanged(event: any) {
        this.file = null;
        this.fileName = '';
        if (event.target.files !== undefined && event.target.files !== null && event.target.files !== '') {
            this.file = event.target.files;
            this.fileName = event.target.files[0].name;
        }
    }
    removeSelectedFile(index) {
        this.file = null;
        this.fileName = '';
        this.TimesheetForm.controls.FileName.patchValue(null);
    }


    fileDownload(file: string) {
        if (file !== null && file !== undefined && file !== '') {
            this.timeSheetService.DownloadFile(this.ClientID, this.timesheet.docGuid).subscribe(result => {
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

    timeSheetValidPending = ()  =>{
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
                if (this.TimeSheetStatusList !== undefined && this.TimeSheetStatusList.length > 0) {
                    if (this.TimeSheetStatusList.find(tss => tss.value.toUpperCase() === TimeSheetStatusConstants.PENDING.toUpperCase() !== undefined)) {
                        this.TimesheetForm.controls.StatusID.patchValue(this.TimeSheetStatusList.find(tss => tss.value.toUpperCase() === TimeSheetStatusConstants.PENDING.toUpperCase()).listValueID)
                        this.TimesheetForm.controls.Status.patchValue(this.TimeSheetStatusList.find(tss => tss.value.toUpperCase() === TimeSheetStatusConstants.PENDING.toUpperCase()).valueDesc)
                    }
                }
                this.ValidTimeSheet();
                this.buttonsdisabledvalue = false;
            }
        }
    }

    timeSheetValidSubmit = () => {
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
                if (this.TimeSheetStatusList.find(tss => tss.value.toUpperCase() === TimeSheetStatusConstants.SUBMITTED.toUpperCase() !== undefined)) {
                    this.TimesheetForm.controls.StatusID.patchValue(this.TimeSheetStatusList.find(tss => tss.value.toUpperCase() === TimeSheetStatusConstants.SUBMITTED.toUpperCase()).listValueID)
                    this.TimesheetForm.controls.Status.patchValue(this.TimeSheetStatusList.find(tss => tss.value.toUpperCase() === TimeSheetStatusConstants.SUBMITTED.toUpperCase()).valueDesc)
                }
                this.ValidTimeSheet();
                this.buttonsdisabledvalue = false;
            }
        }
    }

  async ValidTimeSheet() {
        this.ErrorMessage = "";
        var isTableValues = true;
       if (this.TimeSheetId != -1) {
           if (this.TimeEntryList.length > 0) {
                this.TimeEntryList.forEach(tel => {
                    if (isTableValues) {
                        var workdate = new Date(tel.WorkDate);
                        let currentworkdate = moment(new Date(workdate))
                        if (moment(currentworkdate).format('dddd').toUpperCase() !== "SATURDAY" && moment(currentworkdate).format('dddd').toUpperCase() !== "SUNDAY") {
                            if (tel.Project == "" || tel.Project == null) {
                                this.ErrorMessage = "Please populate Project or Select from Projection options";
                                isTableValues = false;
                            }
                            else if ((tel.Activity == "" || tel.Activity == null) && tel.Project.toUpperCase() !== "CLIENT HOLIDAY" && tel.Project.toUpperCase() !== "PERSONAL LEAVE") {
                                this.ErrorMessage = "Please populate Activity";
                                isTableValues = false;
                            }
                            else if (tel.Project.toUpperCase() !== "CLIENT HOLIDAY" && tel.Project.toUpperCase() !== "PERSONAL LEAVE" && tel.Project.toUpperCase() !== "NON-BILLABLE" && tel.Hours == 0) {
                                this.ErrorMessage = "Please populate Hours";
                                isTableValues = false;
                            }
                        }
                    }
                });

               if (this.TimesheetForm.value.TSApproverEmail == null || this.TimesheetForm.value.TSApproverEmail == "" || this.TimesheetForm.value.TimeSheetTypeID == 0) {
                   this.ErrorMessage = "Timesheet Type or Approver not defined";
                    isTableValues = false;
                }
            }
            //else if(timesheet.TimesheetApproverID == 0 || timesheet.TimeSheetTypeID == 0)
           else {
                this.ErrorMessage = "Time Entries cannot be empty, please select valid Week Ending";
                isTableValues = false;
            }
        }

      if (isTableValues) {
          if (this.TimesheetForm.value.Status.toUpperCase() == TimeSheetStatusConstants.SUBMITTED.toUpperCase()) {
              this.SubmitTimeSheet();
          } else {
           await this.PendingTimeSheet();
          }
         
        }
    }

    PendingTimeSheet() {

       // this.totalhours = '0';
        this.TimesheetForm.controls.SubmittedByID.patchValue(this.user.userID);
        this.TimesheetForm.controls.TimeEntries.patchValue(this.TimeEntryList);
        //this.TimeEntryList.forEach(te => {
        //    this.totalhours = this.totalhours;
        //});
        //this.TimesheetForm.controls.TotalHours.patchValue(this.totalhours);
        this.TimesheetForm.controls.ClientName.patchValue(this.EmployeeAssignments.find(cl => cl.clientID == this.TimesheetForm.value.ClientID).client);
        this.TimesheetForm.value.WeekEnding = this.commonUtils.defaultDateTimeLocalSet(new Date(this.TimesheetForm.value.WeekEnding));
        if (this.TimesheetForm.value.SubmittedDate !== null) {
            this.TimesheetForm.value.SubmittedDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.TimesheetForm.value.SubmittedDate));
        }
        if (this.TimesheetForm.value.ApprovedDate !== null) {
            this.TimesheetForm.value.ApprovedDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.TimesheetForm.value.ApprovedDate));
        }
        // file upload
        let timesheetdate = "";
        if (this.TimesheetForm.value.WeekEnding === null) {
            timesheetdate = '00000000'
        } else {
            if (this.TimesheetForm.value.TimeSheetType.toUpperCase() === TimeSheetType.MONTHLY) {
                timesheetdate = (moment(this.TimesheetForm.value.WeekEnding).format("yyyyMMdd"));
            }
        }

        var filePath = "";
        if (this.file != null) {
            filePath = this.file.name;
        }
        if (this.TimeSheetId === 0) {
            if (filePath != "") {
                this.formData = new FormData();
                this.formData.append(this.file[0].name, this.file[0]);
                this.timeSheetService.uploadFile(this.TimesheetForm.value.EmployeeName.replace(/\s+/g, ""), this.formData).then(async (result) => {
                    if (result['data'] !== null && result['messageType'] === 1) {
                      //  var timeSheet = result['data'];
                        if (result['data'].fileName !== '' && result['data'].fileName !== undefined && result['data'].fileName !== null) {
                            this.TimesheetForm.controls.FileName.patchValue(result['data'].fileName.toString())
                            this.timeSheetService.SaveTimesheet(this.TimesheetForm.value).subscribe(resp => {
                                if (resp['data'] !== null && resp['messageType'] === 1) {
                                    this.messageService.add({ severity: 'success', summary: 'Timesheet saved successfully', detail: '' });
                                    this.isSaveButtonDisabled = true;
                                    this.loadModalOptions();
                                    this.TimeSheetUpdated.emit();
                                    this.cancel();
                                } else {
                                    this.isSaveButtonDisabled = false;
                                    this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                                }

                            })
                        }
                    }
                })
            } else {
                this.timeSheetService.SaveTimesheet(this.TimesheetForm.value).subscribe(resp => {
                    if (resp['data'] !== null && resp['messageType'] === 1) {
                        this.messageService.add({ severity: 'success', summary: 'Timesheet saved successfully', detail: '' });
                        this.isSaveButtonDisabled = true;
                        this.loadModalOptions();
                        this.TimeSheetUpdated.emit();
                        this.cancel();
                    } else {
                        this.isSaveButtonDisabled = false;
                        this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                    }
                })
            }
        }
        else if (this.TimeSheetId > 0) {
            if (filePath != "") {
                this.formData = new FormData();
                //this.formData.append(this.file.name, this.file);
                this.formData.append(this.file[0].name, this.file[0]);
                this.timeSheetService.uploadFile(this.TimesheetForm.value.EmployeeName.replace(/\s+/g, ""), this.formData).then(async result => {
                    if (result['data'] !== null && result['messageType'] === 1) {
                        var timeSheet = result['data'];
                        if (result['data'].fileName !== '' && result['data'].fileName !== undefined && result['data'].fileName !== null) {
                            this.TimesheetForm.controls.FileName.patchValue(result['data'].fileName)
                            this.timeSheetService.UpdateTimesheet(this.TimesheetForm.value.TimeSheetID, this.TimesheetForm.value).subscribe(resp => {
                                if (resp['data'] !== null && resp['messageType'] === 1) {
                                    this.messageService.add({ severity: 'success', summary: 'Timesheet saved successfully', detail: '' });
                                    this.buttonsdisabledvalue = true;
                                    this.loadModalOptions();
                                    this.TimeSheetUpdated.emit();
                                    this.cancel();
                                } else {
                                    this.buttonsdisabledvalue = false;
                                    this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                                }
                            })
                        }
                    }
                })
            } else {
                this.timeSheetService.UpdateTimesheet(this.TimesheetForm.value.TimeSheetID, this.TimesheetForm.value).subscribe(resp => {
                    if (resp['data'] !== null && resp['messageType'] === 1) {
                        this.messageService.add({ severity: 'success', summary: 'Timesheet saved successfully', detail: '' });
                        this.buttonsdisabledvalue = true;
                        this.loadModalOptions();
                        this.TimeSheetUpdated.emit();
                        this.cancel();
                    } else {
                        this.buttonsdisabledvalue = false;
                        this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                    }
                })
            }
        }
        this.buttonsdisabledvalue = true;
    }
    respEmailApproval: IEmailApprovalDisplay;
    SubmitTimeSheet(){
        this.totalhours = '0';
        this.TimesheetForm.controls.SubmittedByID.patchValue(this.user.userID);
        this.TimesheetForm.controls.TimeEntries.patchValue(this.TimeEntryList);
        //this.TimesheetForm.controls.TotalHours.patchValue(this.totalhours);
        this.TimesheetForm.controls.ClientName.patchValue(this.EmployeeAssignments.find(cl => cl.clientID == this.TimesheetForm.value.ClientID).client);
        this.TimesheetForm.value.WeekEnding = new Date(this.TimesheetForm.value.WeekEnding);
        if (this.TimesheetForm.value.SubmittedDate !== null) {
            this.TimesheetForm.value.SubmittedDate = new Date(this.TimesheetForm.value.SubmittedDate);
        }
        if (this.TimesheetForm.value.ApprovedDate !== null) {
            this.TimesheetForm.value.ApprovedDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.TimesheetForm.value.ApprovedDate));
        }
        // file upload
        let timesheetdate = "";
        if (this.TimesheetForm.value.WeekEnding === null) {
            timesheetdate = '00000000'
        } else {
            if (this.TimesheetForm.value.TimeSheetType.toUpperCase() === TimeSheetType.MONTHLY) {
                timesheetdate = (moment(this.TimesheetForm.value.WeekEnding).format("yyyyMMdd"));
            }
        }

        var filePath = "";
        if (this.file != null) {
            filePath = this.file.name;
        }
        if (this.TimeSheetId == 0) {
            this.EmployeeAssignment = this.EmployeeAssignments.find(x => x.clientID === this.TimesheetForm.value.ClientID);
            if (filePath != "") {
                this.formData = new FormData();
                //this.formData.append(this.file.name, this.file);
                this.formData.append(this.file[0].name, this.file[0]);
                this.timeSheetService.uploadFile(this.TimesheetForm.value.EmployeeName.replace(/\s+/g, ""), this.formData).then(async result => {
                    if (result['data'] !== null && result['messageType'] === 1) {
                        var timeSheet = result['data'];
                        if (result['data'].fileName !== '' && result['data'].fileName !== undefined && result['data'].fileName !== null) {
                            this.TimesheetForm.controls.FileName.patchValue(result['data'].fileName)
                        }

                        this.timeSheetService.SaveTimesheet(this.TimesheetForm.value).subscribe(resp => {
                            if (resp['data'] !== null && resp['messageType'] === 1) {
                                var emailapproval: IEmailApproval = new IEmailApproval();
                                this.timeSheetService.GetTimesheetByIdAsync(resp['data'].timeSheetID).subscribe(async (result) => {
                                    if (result['data'] !== null && result['messageType'] === 1) {
                                        this.timesheet = result['data'];
                                        emailapproval.EmailApprovalID = 0;
                                        emailapproval.ModuleID = this.Modules.find(m => m.moduleShort.toUpperCase() == "TIMESHEET").moduleID;
                                        emailapproval.ID = this.timesheet.timeSheetID;
                                        emailapproval.Value = this.timesheet.statusValue;
                                        emailapproval.ValidTime = new Date(moment().add(7, 'day').format('YYYY-MM-DD HH:mm:ss'));
                                        emailapproval.IsActive = true;
                                        emailapproval.LinkID = this.commonUtils.newGuid();
                                        emailapproval.ApproverEmail = this.EmployeeAssignment.tsApproverEmail;
                                        emailapproval.CreatedBy = this.user.firstName + " " + this.user.lastName;
                                        var emailFields = await this.PrepareEmail(emailapproval);
                                        emailapproval.EmailSubject = emailFields.EmailSubject;
                                        emailapproval.EmailBody = emailFields.EmailBody;
                                        emailapproval.EmailFrom = emailFields.EmailFrom;
                                        emailapproval.EmailTo = emailFields.EmailTo;
                                        emailapproval.EmailCC = emailFields.EmailCC;
                                        await this.emailApprovalService.SaveEmailApproval(emailapproval).subscribe()
                                    }
                                })
                                this.messageService.add({ severity: 'success', summary: 'Timesheet saved successfully', detail: '' });
                                this.isSaveButtonDisabled = true;
                                this.loadModalOptions();
                                this.TimeSheetUpdated.emit();
                                this.cancel();
                            } else {
                                this.isSaveButtonDisabled = false;
                                this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                            }
                        })
                    }
                })
            } else {
                this.timeSheetService.SaveTimesheet(this.TimesheetForm.value).subscribe(resp => {
                    if (resp['data'] !== null && resp['messageType'] === 1) {
                            var emailapproval: IEmailApproval = new IEmailApproval();
                            this.timeSheetService.GetTimesheetByIdAsync(resp['data'].timeSheetID).subscribe(async (result) => {
                                if (result['data'] !== null && result['messageType'] === 1) {
                                    this.timesheet = result['data'];
                                    emailapproval.EmailApprovalID = 0;
                                    emailapproval.ModuleID = this.Modules.find(m => m.moduleShort.toUpperCase() == "TIMESHEET").moduleID;
                                    emailapproval.ID = this.timesheet.timeSheetID;
                                    emailapproval.Value = this.timesheet.statusValue;
                                    emailapproval.ValidTime = new Date(moment().add(7, 'day').format('YYYY-MM-DD HH:mm:ss'));
                                    emailapproval.IsActive = true;
                                    emailapproval.LinkID = this.commonUtils.newGuid();
                                    emailapproval.ApproverEmail = this.EmployeeAssignment.tsApproverEmail;
                                    emailapproval.CreatedBy = this.user.firstName + " " + this.user.lastName;
                                    var emailFields = await this.PrepareEmail(emailapproval);
                                    emailapproval.EmailSubject = emailFields.EmailSubject;
                                    emailapproval.EmailBody = emailFields.EmailBody;
                                    emailapproval.EmailFrom = emailFields.EmailFrom;
                                    emailapproval.EmailTo = emailFields.EmailTo;
                                    emailapproval.EmailCC = emailFields.EmailCC;
                                    await this.emailApprovalService.SaveEmailApproval(emailapproval).subscribe()
                                }
                            })
                        this.messageService.add({ severity: 'success', summary: 'Timesheet saved successfully', detail: '' });
                        this.isSaveButtonDisabled = true;
                        this.loadModalOptions();
                        this.TimeSheetUpdated.emit();
                        this.cancel();
                    } else {
                        this.isSaveButtonDisabled = false;
                        this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                    }
                })
            }
        }
        else if (this.TimeSheetId > 0) {
            this.EmployeeAssignment = this.EmployeeAssignments.find(x => x.clientID === this.TimesheetForm.value.ClientID);
            if (filePath != "") {
                this.formData = new FormData();
              //  this.formData.append(this.file.name, this.file);
                this.formData.append(this.file[0].name, this.file[0]);
                this.timeSheetService.uploadFile(this.TimesheetForm.value.EmployeeName, this.formData).then(async result => {
                    if (result['data'] !== null && result['messageType'] === 1) {
                        var timeSheet = result['data'];
                        if (result['data'].fileName !== '' && result['data'].fileName !== undefined && result['data'].fileName !== null) {
                            this.TimesheetForm.controls.FileName.patchValue(result['data'].fileName)
                        }
                        this.timeSheetService.UpdateTimesheet(this.TimesheetForm.value.TimeSheetID, this.TimesheetForm.value).subscribe(async (resp) => {
                            if (resp['data'] !== null && resp['messageType'] === 1) {
                                    this.timesheet = resp['data'];
                                    var emailapproval: IEmailApproval = new IEmailApproval();
                                    if (this.timesheet.linkID !== null && this.timesheet.linkID !== Guid.Empty && this.respEmailApproval.linkID !== Guid.Empty) {
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
                                        emailapproval.ModuleID = this.Modules.find(m => m.moduleShort.toUpperCase() == "TIMESHEET").moduleID;
                                        emailapproval.ID = this.timesheet.timeSheetID;
                                        emailapproval.IsActive = true;
                                        emailapproval.LinkID = this.commonUtils.newGuid();
                                        emailapproval.ApproverEmail = this.EmployeeAssignment.tsApproverEmail;
                                        emailapproval.CreatedBy = this.user.firstName + " " + this.user.lastName;
                                        emailapproval.CreatedDate = new Date();
                                    }
                                    emailapproval.ValidTime = new Date(moment().add(7, 'day').format('YYYY-MM-DD HH:mm:ss'));
                                var emailFields: EmailFields = await this.PrepareEmail(emailapproval);
                                    emailapproval.EmailSubject = emailFields.EmailSubject;
                                    emailapproval.EmailBody = emailFields.EmailBody;
                                    emailapproval.EmailFrom = emailFields.EmailFrom;
                                    emailapproval.EmailTo = emailFields.EmailTo;
                                    emailapproval.EmailBCC = emailFields.EmailCC
                                    await this.emailApprovalService.SaveEmailApproval(emailapproval).subscribe(result => {
                                    })

                                this.messageService.add({ severity: 'success', summary: 'Timesheet saved successfully', detail: '' });
                                this.buttonsdisabledvalue = true;
                                this.loadModalOptions();
                                this.TimeSheetUpdated.emit();
                                this.cancel();
                            } else {
                                this.buttonsdisabledvalue = false;
                                this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                            }
                        })
                    }
                })
            } else {
                this.timeSheetService.UpdateTimesheet(this.TimesheetForm.value.TimeSheetID, this.TimesheetForm.value).subscribe(async (resp) => {
                    if (resp['data'] !== null && resp['messageType'] === 1) {
                        this.timesheet = resp['data'];
                        var emailapproval: IEmailApproval = new IEmailApproval();
                        if (this.timesheet.linkID !== null && this.timesheet.linkID !== Guid.Empty && this.respEmailApproval.linkID !== Guid.Empty) {
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
                            emailapproval.ModuleID = this.Modules.find(m => m.moduleShort.toUpperCase() == "TIMESHEET").moduleID;
                            emailapproval.ID = this.timesheet.timeSheetID;
                            emailapproval.IsActive = true;
                            emailapproval.LinkID = this.commonUtils.newGuid();
                            emailapproval.ApproverEmail = this.EmployeeAssignment.tsApproverEmail;
                            emailapproval.CreatedBy = this.user.firstName + " " + this.user.lastName;
                            emailapproval.CreatedDate = new Date();
                        }
                        emailapproval.ValidTime = new Date(moment().add(7, 'day').format('YYYY-MM-DD HH:mm:ss'));
                        var emailFields: EmailFields = await this.PrepareEmail(emailapproval);
                        emailapproval.EmailSubject = emailFields.EmailSubject;
                        emailapproval.EmailBody = emailFields.EmailBody;
                        emailapproval.EmailFrom = emailFields.EmailFrom;
                        emailapproval.EmailTo = emailFields.EmailTo;
                        emailapproval.EmailBCC = emailFields.EmailCC
                        await this.emailApprovalService.SaveEmailApproval(emailapproval).subscribe(result => {
                        })
                        this.messageService.add({ severity: 'success', summary: 'Timesheet saved successfully', detail: '' });
                        this.buttonsdisabledvalue = true;
                        this.loadModalOptions();
                        this.TimeSheetUpdated.emit();
                        this.cancel();
                    } else {
                        this.buttonsdisabledvalue = false;
                        this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                    }
                })
            }
        }
        this.buttonsdisabledvalue = true;
    }


    async PrepareEmail(emailapproval: any) {
        var tableInfo = "";
        tableInfo = tableInfo + "<tr><th style='border: 1px solid black;'>Work Date</th><th style='border: 1px solid black;'>Project</th>" +
            "<th style='border: 1px solid black;'>Activity</th><th style='border: 1px solid black;'>Hours</th>";
        this.timesheet.timeEntries.forEach(te => {
            if (te.project === null) {
                te.project = '';
            }
            if (te.activity === null) {
                te.activity = '';
            }
            tableInfo = tableInfo + "<tr><td style='border: 1px solid black;' width='15%'>&nbsp;" + moment(te.workDate).format("MM/DD/YYYY") + "</td>" +
                "<td style='border: 1px solid black;' width='35%'>&nbsp;" + te.project + "</td>" +
                "<td style='border: 1px solid black;' width='40%'>&nbsp;" + te.activity + "</td>" +
                "<td style='border: 1px solid black;' width='10%' align='right'>" + te.hours + "&nbsp;</td>";
        });

        var emailFields: EmailFields = new EmailFields();
        emailFields.isMultipleEmail = true;
        var managerName = "";
        var clientManager = "";
        if (this.EmployeeAssignment === undefined) {
            this.EmployeeAssignment = this.EmployeeAssignments.find(x => x.clientID === this.TimesheetForm.value.ClientID)
            if (this.EmployeeAssignment !== null && this.EmployeeAssignment.tsApproverEmail != null) {
                //var approverEmail = UserList.Find(ul => ul.UserID == EmployeeAssignment.TimesheetApproverID).Email;
                var approverEmail = this.EmployeeAssignment.tsApproverEmail;
                clientManager = this.EmployeeAssignment.clientManager;
                emailFields.EmailTo = approverEmail;
                managerName = this.employeeDetails.manager;
            }
        } else {
            if (this.EmployeeAssignment !== null && this.EmployeeAssignment.tsApproverEmail != null) {
                //var approverEmail = UserList.Find(ul => ul.UserID == EmployeeAssignment.TimesheetApproverID).Email;
                var approverEmail = this.EmployeeAssignment.tsApproverEmail;
                clientManager = this.EmployeeAssignment.clientManager;
                emailFields.EmailTo = approverEmail;
                managerName = this.employeeDetails.manager;
            }
        }
      
        // common.EmailCC= employeeDetails.Email;
        if (this.isMonthly == true) {
            emailFields.EmailSubject = this.employeeDetails.employeeName + " has submitted timesheet for month ending " + moment(this.timesheet.weekEnding).format('MM/DD/YYYY')
        }
        else {
            emailFields.EmailSubject = this.employeeDetails.employeeName + " has submitted timesheet for week ending " + moment(this.timesheet.weekEnding).format('MM/DD/YYYY');
        }
        var uri = EmailApprovalUrl[this.ClientID]
        //var uri = Configuration["EmailApprovalUrl:" + user.ClientID];
        var emailBody = "<table><tr><td><b>Employee</b></td>" + "<td>: " + this.employeeDetails.employeeName + "</td></tr>";
        if (this.timesheet.timeSheetType.toUpperCase() == TimeSheetType.MONTHLY) {
            emailBody = emailBody + "<tr><td><b>Month Ending</b></td>" + "<td>: " + moment(this.timesheet.weekEnding).format('MM/DD/YYYY') + "</td></tr>";
        }
        else {
            emailBody = emailBody + "<tr><td><b>Week Ending</b></td>" + "<td>: " + moment(this.timesheet.weekEnding).format('MM/DD/YYYY') + "</td></tr>";
        }
        emailBody = emailBody + "<tr><td><b>Client</b></td>" + "<td>: " + this.timesheet.clientName + "</td></tr>";
        if (clientManager != null && clientManager != "") {
            emailBody = emailBody + "<tr><td><b>Client Manager</b></td>" + "<td>: " + clientManager + "</td></tr>";
        }
        if (managerName != null && managerName != "") {
            emailBody = emailBody + "<tr><td><b>Manager</b></td>" + "<td>: " + managerName + "</td></tr>";
        }
        emailBody = emailBody + "<tr><td><b>Status</b></td>" + "<td>: Submitted</td></tr>" +
            "<tr><td><b>Total Hours</b></td>" + "<td>: " + this.timesheet.totalHours + "</td></tr></table><br/>" +
            "<table width='80%'><tr align='center'><td align='right'><a (click)='" + this.goToLinkApprove(uri + "emailapproval?ClientID=" + this.user.clientID + "&LinkID=" + emailapproval.LinkID + "&Value=APPROVE", emailapproval.LinkID) + "'><img src='" + this.settings.ImageURL[this.ClientID]["Approve"] + "' /></a></td><td align='left'><a (click)=" + this.gotoLinkReject(emailapproval.LinkID)+"'><img src='" + this.settings.ImageURL[this.ClientID]['Reject'] + "' /></a></td></tr></table>" +
            "<br/><table style='border: 1px solid black; border-collapse:collapse;' width='80%'>" + tableInfo + "</table>";
        emailFields.EmailBody = emailBody;
        return emailFields;
    }

    goToLinkApprove(link: any, LinkID: string) {
        var uri = EmailApprovalUrl[this.ClientID]
        window.open(uri+this.ClientID+"/"+ LinkID+"/"+'APPROVE');
        //window.open('https://localhost:5001/'+this.ClientID+"/"+ LinkID+"/"+'APPROVE');
    }

    gotoLinkReject(LinkID: string) {
        var uri = EmailApprovalUrl[this.ClientID]
        window.open(uri + this.ClientID + "/" + LinkID + "/" + 'REJECT');
        //window.open('https://localhost:5001/'+this.ClientID+"/"+ LinkID+"/"+'APPROVE');
    }

    AddRow(selected) {
        this.ErrorMessage = "";
        var date = selected.WorkDate;
        const index = this.TimeEntryList.indexOf(selected);
        if (selected != null && selected.Project.toUpperCase() != "CLIENT HOLIDAY" && selected.Project.toUpperCase() != "PERSONAL LEAVE" && selected.Project.toUpperCase() != "NON-BILLABLE") {
            var temptimeentry = new ITimeEntry();
            temptimeentry.WorkDate = date;
            this.TimeEntryList.splice(index + 1, 0, temptimeentry);
        }
    }

    DeleteRow(selected: any) {
        this.ErrorMessage = "";
        var date = selected.WorkDate;
        const index = this.TimeEntryList.indexOf(selected);
        if (this.TimeEntryList.filter(x => x.WorkDate == date).length > 1) {
            var temphours = Number(this.totalhours) - selected.Hours;
            this.totalhours = temphours.toString();
            this.TimeEntryList.splice(index, 1);
        }
        else {
            this.ErrorMessage = "Cannot remove default row";
        }

    }
    ResetDialog() {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buttonsdisabledvalue = false;
        this.buildTimesheetForm({}, 'New');
        this.AddEditTimesheetModal.hide();
    }


    cancel = () => {
        this.submitted = false;
        this.buttonsdisabledvalue = false;
        this.buildTimesheetForm({}, 'New');
        this.AddEditTimesheetModal.hide();
    }

}
