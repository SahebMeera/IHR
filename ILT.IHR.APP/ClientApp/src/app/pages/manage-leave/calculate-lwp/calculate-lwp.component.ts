import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import * as moment from 'moment';
import { MessageService } from 'primeng/api';
import { forkJoin } from 'rxjs';
import { CommonUtils } from '../../../common/common-utils';
import { Constants, LeaveStatus, LeaveType, ListTypeConstants, SessionConstants, Settings } from '../../../constant';
import { ICountryDisplay } from '../../../core/interfaces/Country';
import { IEmployeeDisplay } from '../../../core/interfaces/Employee';
import { IHolidayDisplay } from '../../../core/interfaces/Holiday';
import { ILeave } from '../../../core/interfaces/Leave';
import { ILeaveBalance, ILeaveBalanceDisplay } from '../../../core/interfaces/LeaveBalance';
import { IRolePermissionDisplay } from '../../../core/interfaces/RolePermission';
import { IModalPopupAlternateOptions } from '../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { EmployeeService } from '../../employee/employee.service';
import { HolidayService } from '../../holidays/holidays.service';
import { LeaveRequestService } from '../../leave-request/leave-request.service';
import { LookUpService } from '../../lookup/lookup.service';
import { LeaveBalanceService } from '../LeaveBalanceService';

@Component({
  selector: 'app-calculate-lwp',
  templateUrl: './calculate-lwp.component.html',
  styleUrls: ['./calculate-lwp.component.scss']
})
export class CalculateLwpComponent implements OnInit {
    settings = new Settings()
    commonUtils = new CommonUtils()
    @Output() UpdateLeaveList = new EventEmitter<any>();
    @ViewChild('addEditCalculateModal') addEditCalculateModal: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;
    user: any;
    submitted: boolean = false;
    moment: any = moment;
    ClientID: string;

    CalculateForm: FormGroup;
    RolePermissions: IRolePermissionDisplay[] = [];
    EmployeeInfoRolePermission: IRolePermissionDisplay;
    ExpenseId: number;
    ModalHeading: string = 'Pending LWP';
    isSaveButtonDisabled: boolean = false;
    isShow: boolean = true;
    dateTime = new Date();
    EmployeeID: number;

    constructor(private fb: FormBuilder,
        private lookupService: LookUpService,
        private messageService: MessageService,
        private leaveRequestService: LeaveRequestService,
        private leaveBalanceService: LeaveBalanceService,
        private holidayService: HolidayService,
        private employeeService: EmployeeService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.EmployeeInfoRolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.EMPLOYEEINFO);
        this.user = JSON.parse(localStorage.getItem("User"));
        this.EmployeeID = this.user.employeeID
        this.ClientID = localStorage.getItem("ClientID");
        this.dateTime.setDate(this.dateTime.getDate());
       // this.GetEmployeeDetails();
        this.LoadDropDown();
        this.buildCalculateFormForm({}, 'New');
    }
    isDisableLWP: boolean = false;
    ngOnInit(): void {
        this.isDisableLWP = true;
        this.loadModalOptions();
    }
    EmployeeList: IEmployeeDisplay[] = []
    CountryList: ICountryDisplay[] = [];
    Holidays: IHolidayDisplay[] = [];
    VacationTypeList: any[] = [];
    StatusList: any[] = [];
    LoadDropDown() {
        forkJoin(
            this.employeeService.GetEmployees(),
            this.lookupService.getListValues(),
            this.holidayService.getCountry(),
            this.holidayService.getHolidayList(),
        ).subscribe(async resultSet => {
            if (resultSet !== undefined && resultSet !== null) {
                this.EmployeeList = resultSet[0]['data'];
                var vacList = resultSet[1]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.VACATIONTYPE);
                this.VacationTypeList = vacList.filter(x => x.value.toUpperCase() == ListTypeConstants.LWP);
                this.StatusList = resultSet[1]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.LEAVEREQUESTSTATUS);
                this.CountryList = resultSet[2]['data']
                this.Holidays = resultSet[3]['data']
            }
        })
    }

    Show() {
        this.showLWPGrid = false;
        this.isDisableProcessbtn = false;
        this.buildCalculateFormForm({}, 'New')
        this.addEditCalculateModal.show()
    }

    onChangeCountry(e: any) {
        var country = e.value.toString();
        if (country !== null && country !== "") {
            // this.GetStates(country);
            this.isDisableLWP = false;
        }
        else {
            //this.StateList = [];
        }
    }

    isStartDateRequired: boolean = false;
    isEndDateRequired: boolean = false;
    isDisableProcessbtn: boolean = false;
    showLWPGrid: boolean = false;
    startDate: Date;
    endDate: Date;
    selectedCountry: string;
    lstLeaveBalance: ILeaveBalanceDisplay[] = []
    lstUnpaidLeaveBalance: ILeaveBalanceDisplay[] = []
    lstFinalLeaveBalance: ILeaveBalance[] = []

    async calculateLWP() {
        this.showLWPGrid = false;
        this.isStartDateRequired = false;
        this.isEndDateRequired = false;
        this.isDisableProcessbtn = false;
        if (this.CalculateForm.value.StartDate === null) {
            this.isStartDateRequired = true;
        } else {
            this.isStartDateRequired = false;
        }
        if (this.CalculateForm.value.EndDate == null) {
            this.isEndDateRequired = true;
        } else {
            this.isEndDateRequired = false;
        }
        if (this.isStartDateRequired == false && this.isEndDateRequired == false) {
            this.showLWPGrid = true;
        }
         this.lstLeaveBalance = []
         this.lstUnpaidLeaveBalance = []
         this.lstFinalLeaveBalance = []
         var reportReq = {};
         reportReq['Country'] = this.CalculateForm.value.Country;
         reportReq['StartDate'] = this.commonUtils.defaultDateTimeLocalSet(new Date(this.CalculateForm.value.StartDate));
         reportReq['EndDate'] = this.commonUtils.defaultDateTimeLocalSet(new Date(this.CalculateForm.value.EndDate));
         this.leaveBalanceService.GetReportLeaveDetailInfo(reportReq, null).subscribe(result => {
             if (result['data'] !== undefined && result['data'] !== null && result['data'].length > 0) {
                 var dd = result['data'];
                 dd.forEach(row => {
                     if (row['vacationBalance'] < 0 && row['leaveType'].toString() !== 'Unpaid Leave') {
                         var lb = new ILeaveBalanceDisplay()
                         lb.employeeCode = row.employeeCode
                         lb.employeeName = row.employeeName
                         lb.startDate = row.startDate,
                         lb.endDate = row.endDate
                         lb.leaveType = row.leaveType
                         lb.leaveInRange = row.leaveInRange
                         lb.vacationBalance = row.vacationBalance
                         lb.vacationTotal = row.vacationTotal
                         lb.vacationUsed = row.vacationUsed
                         lb.lWPAccounted = row.lwpAccounted
                         this.lstLeaveBalance.push(lb)
                     }
                    
                 })
                 const results = [];
                 if (this.lstLeaveBalance.length > 0) {
                     this.lstLeaveBalance.reduce((res, value) => {
                         if (!res[value.employeeCode]) {
                             res[value.employeeCode] = { EmployeeCode: value.employeeCode, EmployeeName: '', LeaveType: '', balance: 0, TotalLeaves: 0 };
                             results.push(res[value.employeeCode]);
                         }
                         res[value.employeeCode].EmployeeName = value['employeeName'];
                         res[value.employeeCode].LeaveType = value['leaveType'];
                         res[value.employeeCode].balance = value['vacationBalance'];
                         res[value.employeeCode].TotalLeaves += value['leaveInRange'];
                         return res;
                     }, {});
                 }
                 if (results.length > 0) {
                     results.forEach(d => {
                         var counter = d.balance * (-1);
                         var leaves = this.lstLeaveBalance.filter(x => x.employeeCode === d.EmployeeCode);
                         leaves = leaves.sort((d1, d2) => new Date(d1.startDate).getTime() - new Date(d2.startDate).getTime());
                         console.log(leaves)
                         leaves.forEach(x => {
                             if (counter !== 0) {
                                 if (counter > Number(x.leaveInRange)) {
                                     x.leaveInRange = x.leaveInRange;
                                     counter = counter - parseFloat(x.leaveInRange);
                                     var lb = new ILeaveBalance()
                                     lb.EmployeeCode = x.employeeCode,
                                         lb.EmployeeName = x.employeeName,
                                         lb.StartDate = x.startDate,
                                         lb.EndDate = x.endDate,
                                         lb.LeaveType = x.leaveType,
                                         lb.LeaveInRange = x.leaveInRange,
                                         lb.VacationBalance = x.vacationBalance,
                                         this.lstFinalLeaveBalance.push(lb);
                                 } else {
                                     var dates = []
                                     dates = this.GetDateRange(x.startDate, x.endDate);
                                     console.log(dates)
                                     var lb = new ILeaveBalance()
                                         lb.EmployeeCode = x.employeeCode,
                                         lb.EmployeeName = x.employeeName,
                                         lb.LeaveType = x.leaveType,
                                         lb.LeaveInRange = counter.toString(),
                                         lb.VacationBalance = x.vacationBalance
                                        counter = Math.ceil(counter)
                                     if (dates.length > 0) {
                                         dates.sort((a, b) =>  moment(a,'MM/DD/yyyy').isBefore(moment(b.date, 'MM/DD/yyyy')) ? 1 : -1,)
                                         console.log(dates)
                                         lb.StartDate = dates.length === 1 ? dates[0] : dates[dates.length - 1];
                                         lb.EndDate = dates[0];
                                     } else {
                                         lb.StartDate = x.startDate;
                                         lb.EndDate = x.endDate;
                                     }
                                     this.lstFinalLeaveBalance.push(lb);
                                     //this.lstFinalLeaveBalance = [];
                                     counter = 0;

                                 }
                             }
                         })
                     })
                     if (this.lstFinalLeaveBalance.length == 0) {
                         this.isDisableProcessbtn = true;
                     }
                 } else {
                     if (this.lstFinalLeaveBalance.length == 0) {
                         this.isDisableProcessbtn = true;
                     }
                 }
             } else {
                 this.isDisableProcessbtn = true;
             }
         })
    }

    GetDateRange(startDate, stopDate) {
        var dateArray = [];
        var currentDate = moment(startDate).format('MM/DD/yyyy')
        var endDate = moment(stopDate).format('MM/DD/yyyy')
        while (currentDate <= endDate) {
            if (!this.Holidays.find(x => moment(x.startDate).format('MM/DD/yyyy') === moment(startDate).format('MM/DD/yyyy')) && moment(startDate).isoWeekday() !== 0
                && moment(startDate).isoWeekday() !== 6) {
                dateArray.push(currentDate)
            }
            currentDate = moment(currentDate).add(1, 'days').format('MM/DD/yyyy');
        }
        return dateArray;
    }



    buildCalculateFormForm(data: any, keyName: string) {
        this.CalculateForm = this.fb.group({
            Country: [keyName === 'New' ? null : data.country, Validators.required],
            StartDate: [keyName === 'New' ? null : data.startDate !== null ? new Date(data.startDate) : null, Validators.required],
            EndDate: [keyName === 'New' ? null : data.endDate !== null ? new Date(data.endDate) : null, Validators.required],
        });

    }
    get addEditWFHFormControls() { return this.CalculateForm.controls; }

    loadModalOptions() {
        this.modalOptions = {
            footerActions: [
            ]
        }
    }

    Search = () => {
        this.submitted = true;
        if (this.CalculateForm.invalid) {
            this.CalculateForm.markAllAsTouched();
            return;
        } else {
            if (this.isSaveButtonDisabled)
                return;
            this.isSaveButtonDisabled = true;
            this.calculateLWP()
            this.isSaveButtonDisabled = false;
        }
    }

    cancel = () => {
        this.isDisableProcessbtn = false;
        this.isDisableLWP = false;
        this.isStartDateRequired = false;
        this.isEndDateRequired = false;
        this.isDisableProcessbtn = false;
        this.showLWPGrid = false;
        this.addEditCalculateModal.hide()
    }

    UpdatePendingLWP() {
        //isDisableProcessbtn = true;
        for (var lwp of this.lstFinalLeaveBalance)
        {
            var startDate = new Date(lwp.StartDate)
            var sdate = startDate.setDate(startDate.getDate() + 1)
            var enDate = new Date(lwp.EndDate)
            var edate = enDate.setDate(enDate.getDate() + 1)
               var leave = new ILeave()
                leave.LeaveID = 0,
                leave.CreatedBy = this.user.firstName + " " + this.user.lastName,
                leave.CreatedDate = new Date(),
                leave.ModifiedBy = this.user.firstName + " " + this.user.lastName,
                leave.ModifiedDate = new Date(),
                leave.ApproverID = this.user.employeeID,
                //leave.Approver = this.user.firstName + " " + this.user.lastName,
                //leave.LeaveType = this.VacationTypeList.find(x => x.value.ToUpperCase() == LeaveType.LWP).valueDesc,
                leave.LeaveTypeID = this.VacationTypeList.find(x => x.value.toUpperCase() == LeaveType.LWP).listValueID,
                leave.StatusID = this.StatusList.find(x => x.valueDesc.toUpperCase() == LeaveStatus.APPROVED).listValueID,
                leave.EmployeeID = this.EmployeeList.find(x => x.employeeCode.toString() === lwp.EmployeeCode.toString()) !== undefined ? this.EmployeeList.find(x => x.employeeCode.toString() === lwp.EmployeeCode.toString()).employeeID : null
                leave.StartDate = new Date(sdate),
                leave.EndDate = new Date(edate),
                leave.Duration = lwp.LeaveInRange,
                leave.RequesterID = this.EmployeeList.find(x => x.employeeCode.toString() === lwp.EmployeeCode.toString()) !== undefined ? this.EmployeeList.find(x => x.employeeCode.toString() === lwp.EmployeeCode.toString()).employeeID : null
                leave.Title = "Unpaid Leave",
                leave.Detail = "Unpaid Leave",
                leave.IncludesHalfDay = (Number(lwp.LeaveInRange) % 1) > 0 ? true : false,
                 Comment = this.StatusList.find(x => x.valueDesc.toUpperCase() == LeaveStatus.APPROVED).valueDesc,
                  this.leaveRequestService.SaveLeave(leave).subscribe(result => {
               })
        }
        this.messageService.add({ severity: 'success', summary: 'Unpaid Leave saved successfully', detail: '' });
        this.isDisableProcessbtn = true;
       // this.loadModalOptions();
        this.UpdateLeaveList.emit();
        this.cancel();
    }

}
