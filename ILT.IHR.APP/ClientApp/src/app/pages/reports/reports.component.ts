import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonUtils } from '../../common/common-utils';
import { IReport } from '../../core/interfaces/Report';
import { ReportService } from './report.service';
//import { DatePipe } from '@angular/common'
import * as moment from 'moment';
import { IAuditLog } from '../../core/interfaces/AuditLog';
import { SortEvent } from 'primeng/api';
import { forkJoin } from 'rxjs';
import { LookUpService } from '../lookup/lookup.service';
import { ListTypeConstants } from '../../constant';
import { HolidayService } from '../holidays/holidays.service';
import { AssetReportComponent } from './asset-report/asset-report.component';
import { AuditReportComponent } from './audit-report/audit-report.component';
import { EmployeeDetailReportComponent } from './employee-detail-report/employee-detail-report.component';
import { I9expiryFormComponent } from './i9expiry-form/i9expiry-form.component';
import { PendingLeaveComponent } from './pending-leave/pending-leave.component';
import { LeaveReportComponent } from './leave-report/leave-report.component';

@Component({
  selector: 'app-reports',
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.scss']
})
export class ReportsComponent implements OnInit {
    commonUtils = new CommonUtils()

    constructor(private reportService: ReportService,
        private lookupService: LookUpService,
        private holidayService: HolidayService
    ) {
        this.loadTableColumns();
        this.LoadDropDown();
    }

    type: string;
    startDate: Date;
    endDate: Date;

    startDateForDisplay: string;
    endDateForDisplya: string;

    auditLogs: IAuditLog[] = [];


    reportType: string;
    StartDate: Date;
    EndDate: Date;

    ngOnInit(): void {
        //this.startDateForDisplay = this.datePipe.transform(this.startDate, 'D Month, Yr');
        //this.endDateForDisplya = this.datePipe.transform(this.startDate, 'D Month, Yr');
        //console.log(this.startDateForDisplay + '   ' + this.endDateForDisplya);
    }
    reportTypes: any[] = [];
    CountryList: any[] = [];
    lstCountry: any[] = [];
    EmployMentList: any[] = [];
    lstEmployeeType: any[] = [];
    lstStatus: any[] = [];
    AssetTypeList: any[] = [];
    AssetStatusList: any[] = [];
    country: string;
    employeeType: string;
    DefaultCountryID: number;
    DefaultStatusID: number;
    status: string;
    LoadDropDown() {
        forkJoin(
            //this.rolePermissionService.getModules(),
            //this.employeeService.GetEmployees(),
            this.lookupService.getListValues(),
            this.holidayService.getCountry(),
        ).subscribe(resultSet => {
            if (resultSet !== undefined && resultSet !== null) {
                this.reportTypes = resultSet[0]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.REPORTTYPE);
                this.EmployMentList = resultSet[0]['data'].filter(x => x.type.toUpperCase() === ListTypeConstants.EMPLOYMENTTYPE);
                this.CountryList = resultSet[1]['data'];
                this.AssetTypeList = resultSet[0]['data'].filter(x => x.type.toUpperCase()  == ListTypeConstants.ASSETTYPE);
                this.AssetStatusList = resultSet[0]['data'].filter(x => x.type.toUpperCase()  == ListTypeConstants.ASSETSTATUS);
                this.setCountryList();
                this.setEmployeeTypeList();
                this.setStatusList();
                this.setLeaveStatusList();
                this.setAssetStatusList();
                this.setAssetTypeList();
                this.loadDaysDropdown();
            }
        })
    }

    isReportTypeRequired: boolean = false;
    isShowAssetReport: boolean = false;
    isShowAssetChangeSets: boolean = false;
    isShowI9ExpiryForm: boolean = false;
    isStartDateRequired: boolean = false;
    isEndDateRequired: boolean = false;
    isCountryHidden: boolean = false;
    isShowEmployeeDetail: boolean = false;
    isDateHidden: boolean = false;
    TicketStatus: string;
    reportHeader: string;
    onReportChange(e) {
        this.isReportTypeRequired = false;
        this.isShowAssetReport = false;
        this.isShowAssetChangeSets = false;
        this.isShowI9ExpiryForm = false;
        if (e.value != null && e.value != "") {
            this.isStartDateRequired = false;
            this.isEndDateRequired = false;
            this.isReportTypeRequired = false;
            this.isShowAssetReport = false;
            this.isShowAssetChangeSets = false;
            this.isShowI9ExpiryForm = false;
            var value = e.value.toString();
            if (this.reportTypes !== null && this.reportTypes !== undefined && this.reportTypes.length > 0) {
                if (this.reportTypes.find(x => x.value === e.value).valueDesc !== undefined) {
                    this.reportHeader = this.reportTypes.find(x => x.value === e.value).valueDesc;
                    //this.reportHeader = 'Shaik Papi';
                }
            }
           // this.reportHeader = value;
            this.reportType = value.toUpperCase();
            if (value.toUpperCase() != '') {
                this.setLeaveStatusList();
                this.isShowAssetReportComponent = false;
                if (value.toUpperCase() == "AUDITLOG") {
                    this.isCountryHidden = true;
                    this.isShowEmployeeDetail = false;
                    this.isDateHidden = false;
                    this.isShowAssetReport = false;
                    this.isShowAssetChangeSets = false;
                    this.isShowI9ExpiryForm = false;
                }
                else if (value.toUpperCase() == "EMPLOYEEDETAIL") {
                    this.setCountryList();
                    this.setStatusList();
                    this.setEmployeeTypeList();
                    this.isShowEmployeeDetail = true;
                    this.isDateHidden = true;
                    this.isShowAssetReport = false;
                    this.isShowAssetChangeSets = false;
                    this.isShowI9ExpiryForm = false;
                }
                else if (value.toUpperCase() == "PENDINGLEAVE") {
                    if (this.CountryList != null) {
                        this.country = this.CountryList[0].countryDesc;
                    }
                    this.isDateHidden = true;
                    this.isCountryHidden = false;
                    this.isShowEmployeeDetail = false;
                    this.isShowAssetReport = false;
                    this.isShowAssetChangeSets = false;
                    this.isShowI9ExpiryForm = false;
                }
                else if (value.toUpperCase() == "ASSET") {
                    this.isDateHidden = true;
                    this.TicketStatus = null;
                    this.AssetType = null;
                    this.setAssetTypeList();
                    this.setAssetStatusList();
                    this.isCountryHidden = true;
                    this.isShowAssetReport = true;
                    this.isShowEmployeeDetail = false;
                    this.isShowAssetChangeSets = false;
                    this.isShowI9ExpiryForm = false;
                }
                else if (value.toUpperCase() == "ASSETHISTORY") {
                    this.isDateHidden = true;
                    this.isCountryHidden = true;
                    this.isShowAssetReport = false;
                    this.isShowEmployeeDetail = false;
                    this.AssetChangeSetsType = null;
                    this.TicketChangeSetsStatus = null;
                    this.setAssetChangeSetsTypeList();
                    this.setAssetChangeSetsStatusList();
                    this.isShowAssetChangeSets = true;
                    this.isShowI9ExpiryForm = false;
                }
                else if (value == "I9EXPIRYFORM") {
                    if (this.I9FormDaysList != null) {
                        this.dayID = Number(this.I9FormDaysList.find(x => x.Day == 180).Day);
                    }
                    this.isDateHidden = true;
                    this.isCountryHidden = true;
                    this.isShowAssetReport = false;
                    this.isShowEmployeeDetail = false;
                    this.AssetChangeSetsType = null;
                    this.TicketChangeSetsStatus = null;
                    this.setAssetChangeSetsTypeList();
                    this.setAssetChangeSetsStatusList();
                    this.isShowAssetChangeSets = false;
                    this.isShowI9ExpiryForm = true;
                }
                else {
                    if (value === "LEAVEDETAIL" || value === "LEAVESUMMARY") {
                        if (this.CountryList != null) {
                            this.country = this.CountryList[1].countryDesc;
                        }
                    }
                    this.isDateHidden = false;
                    this.isCountryHidden = false;
                    this.isShowEmployeeDetail = false;
                    this.isShowAssetReport = false;
                    this.isShowAssetChangeSets = false;
                    this.isShowI9ExpiryForm = false;
                }
            } else {
                this.isReportTypeRequired = true;
            }

        } else {
            this.isReportTypeRequired = true;
        }
    }

    setCountryList() {
        this.lstCountry = [];
        if (this.CountryList !== undefined) {
            this.lstCountry.push({ ID: 0, Value: 'All' });
            this.CountryList.forEach(x => {
                this.lstCountry.push({ ID: x.countryID, Value: x.countryDesc })
            });
        }
        if (this.country == undefined) {
            this.country = "United States";
        }
        this.DefaultCountryID = this.lstCountry.find(x => x.Value.toLowerCase() == this.country.toLowerCase()).ID;
    }

    OnCountryChange(event) {
        this.DefaultCountryID = Number(event.value);
        this.country = this.lstCountry.find(x => x.ID == this.DefaultCountryID).Value;
    }
    selectedEmpTypeList: number[] = [];
    setEmployeeTypeList() {
        this.lstEmployeeType = [];
        if (this.EmployMentList !== undefined) {
            // this.lstEmployeeType.push({ ID: 0, Value: 'All' })
            this.EmployMentList.forEach(x => {
                this.lstEmployeeType.push({ ID: x.listValueID, Value: x.valueDesc })
            });
        }
        if (this.employeeType == undefined) {
            this.employeeType = "Fulltime Salaried";
        }
        if (this.lstEmployeeType !== undefined && this.lstEmployeeType.length > 0) {
            this.lstEmployeeType.forEach(x => {
                if (x.Value.toLowerCase() === this.employeeType.toLowerCase()) {
                this.selectedEmpTypeList.push(x.ID);
                }
            })
        }
    }
    onChangeEmpTypes(event: any) {
        this.selectedEmpTypeList = [];
        this.selectedEmpTypeList = event.value;
        if (this.selectedEmpTypeList.length > 0) {
            if (this.selectedEmpTypeList.length === this.lstEmployeeType.length) {
                this.employeeType = 'All';
            } else {
                this.employeeType = 'NotAll';
            }
        }
       // this.LoadEmployees();
    }
    EmployeeStatus: string;
    setStatusList() {
        this.lstStatus = [{ 'ID': 0, Value: 'All' }, { 'ID': 1, Value: 'Active' }, { 'ID': 2, Value: 'Termed' }]
        this.EmployeeStatus = 'Active';
        this.DefaultStatusID = this.lstStatus.find(x => x.Value.toLowerCase() == this.EmployeeStatus.toLowerCase()).ID
    }
    OnStatusChange(e) {
        this.DefaultStatusID = Number(e.value);
        this.EmployeeStatus = this.lstStatus.find(x => x.ID == this.DefaultStatusID).Value;
        // LoadEmployees();
    }
    lstLeaveStatus: any[] = [];
    LeaveDetailStatus: string;
    DefaultLeaveStatusID: number;
    setLeaveStatusList() {
        this.lstLeaveStatus = [{ 'ID': 0, Value: 'All' }, { 'ID': 1, Value: 'Active' }, { 'ID': 2, Value: 'Termed' }]
        this.LeaveDetailStatus = 'Active';
        this.DefaultLeaveStatusID = this.lstLeaveStatus.find(x => x.Value.toLowerCase() == this.LeaveDetailStatus.toLowerCase()).ID
    }

    OnLeaveStatusChange(e) {
        this.DefaultLeaveStatusID = Number(e.value);
        this.LeaveDetailStatus = this.lstLeaveStatus.find(x => x.ID == this.DefaultLeaveStatusID).Value;
        // LoadEmployees();
    }

    selectedStatusList: number[] = [];
    lstAssetStatus: any[] = [];
    AssetStatus: string;
    setAssetStatusList() {
        this.selectedStatusList = [];
        this.lstAssetStatus = [];
        if (this.AssetStatusList !== undefined) {
            // this.lstEmployeeType.push({ ID: 0, Value: 'All' })
            this.AssetStatusList.forEach(x => {
                this.lstAssetStatus.push({ ID: x.listValueID, Value: x.valueDesc })
            });
        }
        //if (this.AssetStatus === '') {
        this.AssetStatus = "Unassigned";
        //}
        if (this.lstAssetStatus !== undefined && this.lstAssetStatus.length > 0) {
            this.lstAssetStatus.forEach(x => {
                if (x.Value.toLowerCase() === 'Assigned'.toLowerCase()) {
                    this.selectedStatusList.push(x.ID);
                }
                if (x.Value.toLowerCase() === 'Unassigned'.toLowerCase()) {
                    this.selectedStatusList.push(x.ID);
                }
                if (x.Value.toLowerCase() == "Assigned Temp".toLowerCase()) {
                    this.selectedStatusList.push(x.ID);
                }
            })
        }
    }

    OnMultiDropDownAssetStatus(event: any) {
        this.selectedStatusList = [];
        this.selectedStatusList = event.value;
        if (this.selectedStatusList.length > 0) {
            if (this.selectedStatusList.length === this.lstAssetStatus.length) {
                this.AssetStatus = 'All';
            } else {
                this.AssetStatus = 'NotAll';
            }
        }
       // this.assetReport.LoadList();
        //this.LoadList();
        // this.onMultiSelectDropDownChange.emit(this.onMultiSelectedDropDown);

    }


    selectedAssetTypeList: number[] = [];
    lstAssetType: any[] = [];
    AssetType: string;
    setAssetTypeList() {
        this.lstAssetType = [];
        this.selectedAssetTypeList = [];
        if (this.AssetTypeList !== undefined) {
            //this.lstTicketAssignedTo.push({ ID: null, Value: 'All' })
            this.AssetTypeList.forEach(x => {
                this.lstAssetType.push({ ID: x.listValueID, Value: x.valueDesc })
            });
        }
        //if (this.AssetType === '') {
        this.AssetType = "All";
        //  //}
        if (this.lstAssetType !== undefined && this.lstAssetType.length > 0) {
            this.lstAssetType.forEach(x => {
                this.selectedAssetTypeList.push(x.ID);
            })
        }
    }

    OnAssetTypeChange(event: any) {
        this.selectedAssetTypeList = [];
        this.selectedAssetTypeList = event.value;
        if (this.selectedAssetTypeList.length > 0) {
            if (this.selectedAssetTypeList.length === this.lstAssetType.length) {
                this.AssetType = 'All';
            } else {
                this.AssetType = 'NotAll';
            }
        }
        //this.LoadList();
    }

    lstTicketChangeSetsStatus: any[] = [];
    TicketChangeSetsStatus: string;
    setAssetChangeSetsStatusList() {
        this.selectedAssetChangeSetsStatusList = [];
        this.lstTicketChangeSetsStatus = [];
        if (this.AssetStatusList !== undefined) {
            // this.lstEmployeeType.push({ ID: 0, Value: 'All' })
            this.AssetStatusList.forEach(x => {
                this.lstTicketChangeSetsStatus.push({ ID: x.listValueID, Value: x.valueDesc })
            });
        }
        //if (this.AssetStatus === '') {
        this.TicketChangeSetsStatus = "Unassigned";
        //}
        if (this.lstAssetStatus !== undefined && this.lstAssetStatus.length > 0) {
            this.lstAssetStatus.forEach(x => {
                if (x.Value.toLowerCase() === 'Assigned'.toLowerCase()) {
                    this.selectedAssetChangeSetsStatusList.push(x.ID);
                }
                if (x.Value.toLowerCase() === 'Unassigned'.toLowerCase()) {
                    this.selectedAssetChangeSetsStatusList.push(x.ID);
                }
                if (x.Value.toLowerCase() == "Assigned Temp".toLowerCase()) {
                    this.selectedAssetChangeSetsStatusList.push(x.ID);
                }
            })
        }
    }
    selectedAssetChangeSetsStatusList: any[] = []
    OnMultiDropDownForAssetChangeSetsStatusChange(event: any) {
        this.selectedAssetChangeSetsStatusList = [];
        this.selectedAssetChangeSetsStatusList = event.value;
        if (this.selectedStatusList.length > 0) {
            if (this.selectedStatusList.length === this.lstAssetStatus.length) {
                this.TicketChangeSetsStatus = 'All';
            } else {
                this.TicketChangeSetsStatus = 'NotAll';
            }
        }
        //this.LoadList();
        // this.onMultiSelectDropDownChange.emit(this.onMultiSelectedDropDown);

    }
    lstAssetChangeSetsType: any[] = [];
    AssetChangeSetsType: string;
    selectedAssetChangeSetsTypeList: number[] = [];
    setAssetChangeSetsTypeList() {
        this.lstAssetChangeSetsType = [];
        this.selectedAssetTypeList = [];
        if (this.AssetTypeList !== undefined) {
            //this.lstTicketAssignedTo.push({ ID: null, Value: 'All' })
            this.AssetTypeList.forEach(x => {
                this.lstAssetChangeSetsType.push({ ID: x.listValueID, Value: x.valueDesc })
            });
        }
        //if (this.AssetType === '') {
        this.AssetChangeSetsType = "All";
        //  //}
        if (this.lstAssetChangeSetsType !== undefined && this.lstAssetChangeSetsType.length > 0) {
            this.lstAssetChangeSetsType.forEach(x => {
                this.selectedAssetChangeSetsTypeList.push(x.ID);
            })
        }
    }

    OnMultiDropDownAssetChangeSetsTypeChange(event: any) {
        this.selectedAssetChangeSetsTypeList = [];
        this.selectedAssetChangeSetsTypeList = event.value;
        if (this.selectedAssetChangeSetsTypeList.length > 0) {
            if (this.selectedAssetChangeSetsTypeList.length === this.lstAssetType.length) {
                this.AssetChangeSetsType = 'All';
            } else {
                this.AssetChangeSetsType = 'NotAll';
            }
        }
        //this.LoadList();
    }
    I9FormDaysList: any[] = []
    dayID: number;
    loadDaysDropdown() {
        this.I9FormDaysList = [
            { 'Day': 15, text: '15 days' },
            { 'Day': 30, text: '30 days' },
            {
                Day: 60,
                text: "60 days"
            }, {
                Day: 90,
                text: "90 days"
            }, {
                Day: 180,
                text:"180 days"
            }
        ]
        // this.LeaveDetailStatus = 'Active';
        if (this.I9FormDaysList.length > 0) {
            this.dayID = this.I9FormDaysList.find(x => x.Day == 180).Day;
        }
    }


    cols: any[] = [];
    selectedColumns: any[];
    isPagination: boolean = true;
    loading: boolean = false;
    showCurrentPageReport: boolean = true;
    //rowActions: ITableRowAction[] = [];
    //headerActions: ITableHeaderAction[] = [];
    loadTableColumns() {
        this.cols = [
            { field: 'auditLogID', header: 'Log ID' },
            { field: 'action', header: 'Action Data' },
            { field: 'tableName', header: 'Module' },
            { field: 'recordId', header: 'ID Date' },
            { field: 'values', header: 'Log Description' },
            { field: 'createdDateForDisplay', header: 'Date' },
            { field: 'createdBy', header: 'Created By' },
        ];

        this.selectedColumns = this.cols;
    }
    public dateFieldFormat: string = 'MM/DD/YYYY';
    isDateColumn(event, columnTitle: string) {
        for (const row of event.data) {
            const value = row[columnTitle];
            if (!moment(value, this.dateFieldFormat).isValid() && value !== null) {
                return false;
            }
        }
        return true;
    }
    sortColumn(event: SortEvent) {
        event.data.sort((item1, item2) => {
            const value1: string = item1[event.field];
            const value2: string = item2[event.field];

            if (value1 === null) {
                return 1;
            }

            if (this.isDateColumn(event, event.field)) {
                const date1 = moment(value1, this.dateFieldFormat);
                const date2 = moment(value2, this.dateFieldFormat);
                let result: number = -1;
                if (moment(date2).isBefore(date1, 'day')) { result = 1; }
                return result * event.order;
            }

            let result = null;


            if (value1 == null && value2 != null) {
                result = -1;
            } else if (value1 != null && value2 == null) {
                result = 1;
            } else if (value1 == null && value2 == null) {
                result = 0;
            } else if (typeof value1 === 'string' && typeof value2 === 'string') {
                result = value1.localeCompare(value2);
            } else {
                result = (value1 < value2) ? -1 : (value1 > value2) ? 1 : 0;
            }

            return (event.order * result);
        });
    }


    exportReport() {
        this.isShowReport = true
        this.isShowReport = true
        this.isShowAssetReportComponent = false;
        this.isShowAuditReportComponent = false;
        this.isShowI9ExpiryReport = false;
        this.isShowPendingLeaveReport = false;
        this.isShowLeaveDetailReport = false;
        if (this.reportType !== null && this.reportType !== undefined && this.reportType !== '') {
            if (this.reportType === 'EMPLOYEEDETAIL') {
                setTimeout(() =>
                    this.employeeDetailReport.downloadExcel()
                )
                //const employeeList = this.employeeDetailReport.lstEmployees;
                //if (employeeList.length > 0) {
                //    this.loadExcelReport()
                //}
                //console.log(this.employeeDetailReport.lstEmployees);
            } else if (this.reportType === 'ASSET') {
                this.isShowAssetReportComponent = true;
                this.isShowAuditReportComponent = false;
                setTimeout(() => this.assetReport.showExcel())
            } else if (this.reportType === 'ASSETHISTORY') {
                this.isShowAssetReportComponent = true;
                this.isShowAuditReportComponent = false;
                setTimeout(() => this.assetReport.showExcelAssetHistory());
            } else if (this.reportType === 'AUDITLOG') {
                if (this.StartDate === null || this.EndDate === null || this.StartDate === undefined || this.EndDate === undefined) {
                    this.isStartDateRequired = true;
                    this.isEndDateRequired = true;
                    this.isShowAuditReportComponent = false;
                } else {
                    let report: IReport = {
                        ReportName: 'AuditLog',
                        StartDate: this.commonUtils.defaultDateTimeLocalSet(new Date(this.StartDate)),
                        EndDate: this.commonUtils.defaultDateTimeLocalSet(new Date(this.EndDate)),
                        Country: 'India',
                    }
                    this.auditStartDate = moment(this.StartDate).format("D MMM YYYY")
                    this.auditEndDate = moment(this.EndDate).format("D MMM YYYY")
                    setTimeout(() => this.auditReport.showExcel(report));
                    this.isShowAuditReportComponent = true;
                    this.isStartDateRequired = false;
                    this.isEndDateRequired = false;
                }
                this.isShowAssetReportComponent = false;
            } else if (this.reportType === 'I9EXPIRYFORM') {
                this.isShowI9ExpiryReport = true
                setTimeout(() => this.i9expirysform.showExcel(this.dayID))
                this.isShowAuditReportComponent = false;
                this.isShowAssetReportComponent = false;
            } else if (this.reportType === 'PENDINGLEAVE') {
                this.isShowPendingLeaveReport = true
                setTimeout(() => this.pendingLeave.showExcel(this.country))
                this.isShowAuditReportComponent = false;
                this.isShowAssetReportComponent = false;
            } else if (this.reportType === 'LEAVEDETAIL') {
                if (this.StartDate === null || this.EndDate === null || this.StartDate === undefined || this.EndDate === undefined) {
                    this.isStartDateRequired = true;
                    this.isEndDateRequired = true;
                    this.isShowAuditReportComponent = false;
                } else {
                    let report: IReport = {
                        ReportName: 'LeaveDetail',
                        StartDate: this.commonUtils.defaultDateTimeLocalSet(new Date(this.StartDate)),
                        EndDate: this.commonUtils.defaultDateTimeLocalSet(new Date(this.EndDate)),
                        Country: this.country,
                    }
                    this.auditStartDate = moment(this.StartDate).format("D MMM YYYY")
                    this.auditEndDate = moment(this.EndDate).format("D MMM YYYY")
                    this.isShowLeaveDetailReport = true
                    setTimeout(() => this.LeaveDetailSummary.showLeaveDetailExcel(report, this.LeaveDetailStatus))
                    this.isShowAuditReportComponent = false;
                    this.isShowAssetReportComponent = false;
                }
            } else if (this.reportType === 'LEAVESUMMARY') {
                if (this.StartDate === null || this.EndDate === null || this.StartDate === undefined || this.EndDate === undefined) {
                    this.isStartDateRequired = true;
                    this.isEndDateRequired = true;
                    this.isShowAuditReportComponent = false;
                } else {
                    let report: IReport = {
                        ReportName: 'LeavesSummary',
                        StartDate: this.commonUtils.defaultDateTimeLocalSet(new Date(this.StartDate)),
                        EndDate: this.commonUtils.defaultDateTimeLocalSet(new Date(this.EndDate)),
                        Country: this.country,
                    }
                    this.auditStartDate = moment(this.StartDate).format("D MMM YYYY")
                    this.auditEndDate = moment(this.EndDate).format("D MMM YYYY")
                    this.isShowLeaveDetailReport = true

                    setTimeout(() => this.LeaveDetailSummary.showExcelLeaveSummary(report, this.LeaveDetailStatus))
                    this.isShowAuditReportComponent = false;
                    this.isShowAssetReportComponent = false;
                }
            }
        } else {
            this.isShowAuditReportComponent = false;
            this.isShowReport = false;
            this.isReportTypeRequired = true;
            this.isStartDateRequired = true;
            this.isEndDateRequired = true;
        
        }
       // console.log(this.employeeDetailReport.lstEmployees);
    }
    loadExcelReport() {


    }
    @ViewChild('assetReports') assetReport: AssetReportComponent;
    @ViewChild('auditReports') auditReport: AuditReportComponent;
    @ViewChild('employeeDetailReports ') employeeDetailReport: EmployeeDetailReportComponent;
    @ViewChild('i9expirysform ') i9expirysform: I9expiryFormComponent;
    @ViewChild('pendingLeave ') pendingLeave: PendingLeaveComponent;
    @ViewChild('LeaveDetailSummary ') LeaveDetailSummary: LeaveReportComponent;
    isShowAssetReportComponent: boolean = false;
    isShowAuditReportComponent: boolean = false;
    isShowI9ExpiryReport: boolean = false;
    isShowReport: boolean = false;
    isShowPendingLeaveReport: boolean = false;
    isShowLeaveDetailReport: boolean = false;
    auditStartDate:any  = new Date();
    auditEndDate:any  = new Date();
    generateReport() {
        this.isShowReport = true
        this.isShowAssetReportComponent = false;
        this.isShowAuditReportComponent = false;
        this.isShowI9ExpiryReport = false;
        this.isShowPendingLeaveReport = false;
        this.isShowLeaveDetailReport = false;
        if (this.reportType !== null && this.reportType !== undefined && this.reportType !== '') {
            if (this.reportType === 'ASSET') {
                this.isShowAssetReportComponent = true;
                this.isShowAuditReportComponent = false;
                setTimeout(() =>this.assetReport.show())
            }
            else if (this.reportType === 'ASSETHISTORY') {
                this.isShowAssetReportComponent = true;
                this.isShowAuditReportComponent = false;
                setTimeout(() =>this.assetReport.showAssetHistory());
            }
            else if (this.reportType === 'AUDITLOG') {
                if (this.StartDate === null || this.EndDate === null || this.StartDate === undefined || this.EndDate === undefined) {
                    this.isStartDateRequired = true;
                    this.isEndDateRequired = true;
                    this.isShowAuditReportComponent = false;
                } else {
                    let report: IReport = {
                        ReportName: 'AuditLog',
                        StartDate: this.commonUtils.defaultDateTimeLocalSet(new Date(this.StartDate)),
                        EndDate: this.commonUtils.defaultDateTimeLocalSet(new Date(this.EndDate)),
                        Country: 'India',
                    }
                    this.auditStartDate = moment(this.StartDate).format("D MMM YYYY")
                    this.auditEndDate = moment(this.EndDate).format("D MMM YYYY")
                    setTimeout(() => this.auditReport.show(report));
                    this.isShowAuditReportComponent = true;
                    this.isStartDateRequired = false;
                    this.isEndDateRequired = false;
                }
                this.isShowAssetReportComponent = false;
            } else if (this.reportType === 'EMPLOYEEDETAIL') {
                setTimeout(() =>  this.employeeDetailReport.show(false))
                this.isShowAuditReportComponent = false;
                this.isShowAssetReportComponent = false;
            } else if (this.reportType === 'I9EXPIRYFORM') {
                this.isShowI9ExpiryReport = true
                setTimeout(() => this.i9expirysform.show(this.dayID))
                this.isShowAuditReportComponent = false;
                this.isShowAssetReportComponent = false;
            } else if (this.reportType === 'PENDINGLEAVE') {
                this.isShowPendingLeaveReport = true
                setTimeout(() => this.pendingLeave.show(this.country))
                this.isShowAuditReportComponent = false;
                this.isShowAssetReportComponent = false;
            } else if (this.reportType === 'LEAVEDETAIL') {
                if (this.StartDate === null || this.EndDate === null || this.StartDate === undefined || this.EndDate === undefined) {
                    this.isStartDateRequired = true;
                    this.isEndDateRequired = true;
                    this.isShowAuditReportComponent = false;
                } else {
                    var Sdate: Date = new Date(this.StartDate)
                    
                    //if (this.StartDate <= this.EndDate || this.StartDate.Date === this.EndDate.Date) {
                    //    this.leaveRequestService.GetLeaveDays(this.ClientID, Number(this.EmployeeID), this.LeaveForm.value.StartDate, this.LeaveForm.value.EndDate, this.LeaveForm.value.IncludesHalfDay).subscribe(result => {
                    //        if (result['data'] !== null && result['messageType'] === 1) {
                    //            this.LeaveForm.controls.Duration.patchValue(result['data'].duration);
                    //        }
                    //    })
                    //}
                    //else {
                    //    this.messageService.add({ severity: 'error', summary: `End date must be greater than start date`, detail: '' });
                    //}


                    let report: IReport = {
                        ReportName: 'LeaveDetail',
                        StartDate: this.commonUtils.defaultDateTimeLocalSet(new Date(this.StartDate)),
                        EndDate: this.commonUtils.defaultDateTimeLocalSet(new Date(this.EndDate)),
                        Country: this.country,
                    }
                    this.auditStartDate = moment(this.StartDate).format("D MMM YYYY")
                    this.auditEndDate = moment(this.EndDate).format("D MMM YYYY")
                    this.isShowLeaveDetailReport = true
                    setTimeout(() => this.LeaveDetailSummary.show(report, this.LeaveDetailStatus))
                    this.isShowAuditReportComponent = false;
                    this.isShowAssetReportComponent = false;
                }
            } else if (this.reportType === 'LEAVESUMMARY') {
                if (this.StartDate === null || this.EndDate === null || this.StartDate === undefined || this.EndDate === undefined) {
                    this.isStartDateRequired = true;
                    this.isEndDateRequired = true;
                    this.isShowAuditReportComponent = false;
                } else {
                    let report: IReport = {
                        ReportName: 'LeavesSummary',
                        StartDate: this.commonUtils.defaultDateTimeLocalSet(new Date(this.StartDate)),
                        EndDate: this.commonUtils.defaultDateTimeLocalSet(new Date(this.EndDate)),
                        Country: this.country,
                    }
                    this.auditStartDate = moment(this.StartDate).format("D MMM YYYY")
                    this.auditEndDate = moment(this.EndDate).format("D MMM YYYY")
                    this.isShowLeaveDetailReport = true

                    setTimeout(() => this.LeaveDetailSummary.showLeaveSummary(report, this.LeaveDetailStatus))
                    this.isShowAuditReportComponent = false;
                    this.isShowAssetReportComponent = false;
                }
            }
        } else {
            this.isShowAuditReportComponent = false;
            this.isShowReport = false;
            this.isReportTypeRequired = true;
            this.isStartDateRequired = true;
            this.isEndDateRequired = true;
        }
    }

}
