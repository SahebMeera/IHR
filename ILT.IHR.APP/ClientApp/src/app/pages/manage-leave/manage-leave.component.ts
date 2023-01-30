import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonUtils } from '../../common/common-utils';
import { Constants, SessionConstants } from '../../constant';
import { IDropDown } from '../../core/interfaces/IDropDown';
import { ILeaveBalanceDisplay } from '../../core/interfaces/LeaveBalance';
import { IRolePermissionDisplay } from '../../core/interfaces/RolePermission';
import { DataProvider } from '../../core/providers/data.provider';
import { ITableHeaderAction, ITableRowAction } from '../../shared/ihr-table/table-options';
import { HolidayService } from '../holidays/holidays.service';
import { LookUpService } from '../lookup/lookup.service';
import { AccrueLeaveComponent } from './accrue-leave/accrue-leave.component';
import { AddEditLwpComponent } from './add-edit-lwp/add-edit-lwp.component';
import { AddEditManageLeaveComponent } from './add-edit-manage-leave/add-edit-manage-leave.component';
import { CalculateLwpComponent } from './calculate-lwp/calculate-lwp.component';
import { LeaveBalanceService } from './LeaveBalanceService';

@Component({
  selector: 'app-manage-leave',
  templateUrl: './manage-leave.component.html',
  styleUrls: ['./manage-leave.component.scss']
})
export class ManageLeaveComponent implements OnInit {
    commonUtils = new CommonUtils()
    cols: any[] = [];
    selectedColumns: any[];
    LeaveRequestcols: any[] = [];
    selectedLeaveRequestColumns: any[];
    isPagination: boolean = true;
    loading: boolean = false;
    showCurrentPageReport: boolean = true;
    Country: string = 'Country';
    rowActions: ITableRowAction[] = [];
    headerActions: ITableHeaderAction[] = [];
    patientLanding: string = 'patientLanding';

    index: number;

    EmployeeID: number;
    //table dropdown
    lstStatus: any[] = [];
    status: string;
    DefaultDwnDn1ID: number;


    RolePermissions: IRolePermissionDisplay[] = []
    user: any;

    constructor(private fb: FormBuilder, private dataProvider: DataProvider,
        private LookupService: LookUpService,
        private holidayService: HolidayService,
        private router: Router,
        private leaveBalanceService: LeaveBalanceService
    ) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.user = JSON.parse(localStorage.getItem("User"));
        this.EmployeeID = Number(this.user.employeeID);
        this.LoadDropDown();
        this.loadYearDropdown()
        this.loadTableColumns();
        this.LoadTableConfig();

    }

    ngOnInit(): void {
        this.LoadLeaveBalance();
    }

    globalFilterFields = ['employeeName', 'leaveYear', 'leaveType', 'vacationTotal', 'vacationUsed', 'unpaidLeave', 'vacationBalance', 'encashedLeave']

    loadTableColumns() {
        this.cols = [
            { field: 'employeeName', header: 'Employee Name' },
            { field: 'leaveYear', header: 'Year' },
            { field: 'leaveType', header: 'Leave Type' },
            { field: 'vacationTotal', header: 'Total' },
            { field: 'vacationUsed', header: 'Used' },
            { field: 'unpaidLeave', header: 'Unpaid' },
            { field: 'vacationBalance', header: 'Balance' },
            { field: 'encashedLeave', header: 'Encashed' },
        ];
        this.selectedColumns = this.cols;
    }


    CompanyRolePermission: IRolePermissionDisplay;
    LoadTableConfig() {
        this.CompanyRolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.MANAGELEAVE);
        this.rowActions = [];
                var m1 = {
                    actionMethod: this.edit,
                    iconClass: 'pi pi-pencil',
                }
                this.rowActions.push(m1)
                var m2 = {
                    actionMethod: this.AddLWP,
                    iconClass: 'pi pi-calendar-plus',
                }
                this.rowActions.push(m2);
           
        this.headerActions = [
            {
                actionMethod: this.OnLeaveAccrue,
                hasIcon: false,
                styleClass: 'btn btn-block btn-sm btn-info',
                actionText: 'ACCRUE LEAVE',
                iconClass: 'fas fa-plus'

            },
                    {
                        actionMethod: this.OnCalculateLWP,
                        hasIcon: false,
                        styleClass: 'btn btn-block btn-sm btn-info',
                        actionText: 'PENDING LWP',
                        iconClass: 'fas fa-plus'

                    },
                   
                ];
    }


    CountryList: any[] = [];
    LoadDropDown() {
        this.holidayService.getCountry().subscribe(result => {
            if (result['data'] !== undefined && result['data'] !== null) {
                this.CountryList = result['data'];
                this.setCountryList();
             
            }
        })
    }
    lstCountry: any[] = [];
    country: string = '';
    DropDown2DefaultID: number;
    setCountryList() {
        this.lstCountry = [];
        if (this.CountryList !== undefined) {
            this.lstCountry.push({ ID: 0, Value: 'All' });
            this.CountryList.forEach(x => {
                this.lstCountry.push({ ID: x.countryID, Value: x.countryDesc })
            });
        }

        if (this.country == undefined || this.country == '') {
            this.country = "United States";
        }
        this.DropDown2DefaultID = this.lstCountry.find(x => x.Value.toLowerCase() == this.country.toLowerCase()).ID;
    }

    onCountryChange(event: any) {
        this.DefaultDwnDn1ID = event;
        this.country = this.lstCountry.find(x => x.ID == event).Value;
        this.loadLeaveBalanceList();
    }

    LeaveYearList: any[] = [];
    lstYear: any[] = [];
    yearId: number;
    loadYearDropdown() {
        const currentYear = new Date().getFullYear(); // 2020
        const previousYear = currentYear - 1;
        this.LeaveYearList.push({ year: currentYear, text: "Current Year" }, { year: previousYear, text: "Previous Year" })
        this.SetYearList();
        this.yearId = Number(this.LeaveYearList.find(x => x.text.toLowerCase() === "Current Year".toLowerCase()).year);
        this.DefaultDwnDn1ID = currentYear;
       // this.lstHolidaysList = this.HolidaysList.filter(x => new Date(x.startDate).getFullYear() === currentYear);
    }
    ListItem: IDropDown;
    SetYearList() {
        this.lstYear = [];
        if (this.LeaveYearList !== undefined && this.LeaveYearList.length > 0) {
            this.lstYear.push({ ID: 0, Value: 'Select' })
            this.LeaveYearList.forEach(x => {
                this.lstYear.push({ ID: x.year, Value: x.text })
            });
        }
        //if (this.ListItem !== undefined) {
        //    this.ListItem.ID = 0;
        //    this.ListItem.Value = "Select";
        //    this.lstYear.push(0, this.ListItem);
        //}
    }
    onYearChange(event: any) {
        this.yearId = event;
        this.loadLeaveBalanceList();
    }
    LeaveBalanceList: ILeaveBalanceDisplay[] = []
    lstManageLeave: ILeaveBalanceDisplay[] = []
    async LoadLeaveBalance() {
        await this.leaveBalanceService.GetLeaveBalance(0).subscribe(respRequest => {
            if (respRequest['data'] !== null && respRequest['messageType'] === 1) {
                this.LeaveBalanceList = respRequest['data'];
                this.loadLeaveBalanceList();
                //lstManageLeave = LeaveBalanceList;
                //if (yearId != 0 ) {
                //    onYearChange(yearId);
                //} 
            }
            else {
                this.LeaveBalanceList = [];
               this.lstManageLeave = this.LeaveBalanceList;
            }
        })
    }

    loadLeaveBalanceList() {
        if (this.yearId != 0 && this.country != "All") {
            this.lstManageLeave = this.LeaveBalanceList.filter(x => x.leaveYear == this.yearId && x.country == this.country);
        }
        else if (this.yearId == 0 && this.country != "All") {
            this.lstManageLeave = this.LeaveBalanceList.filter(x => x.country == this.country);
        }
        else if (this.yearId != 0 && this.country == "All") {
            this.lstManageLeave = this.LeaveBalanceList.filter(x => x.leaveYear == this.yearId);
        }
        else {
            this.lstManageLeave = this.LeaveBalanceList;
        }
    }
    @ViewChild('AddEditManageLeaveModal') AddEditManageLeaveModal: AddEditManageLeaveComponent;
    @ViewChild('AddEditLWPModal') AddEditLWPModal: AddEditLwpComponent;
    @ViewChild('AccrueLeaveModal') AccrueLeaveModal: AccrueLeaveComponent;
    @ViewChild('CalculateLWPModal') CalculateLWPModal: CalculateLwpComponent;

    OnCalculateLWP = () => {
        this.CalculateLWPModal.Show()
    }

    OnLeaveAccrue = () => {
        this.AccrueLeaveModal.Show()
    }

    edit = (selected: ILeaveBalanceDisplay) => {
        this.AddEditManageLeaveModal.Show(selected.leaveBalanceID)
    }
    AddLWP = (selected: any) => {
       this.AddEditLWPModal.Show(0, Number(selected.employeeID));

    }

    searchText: string;
    searchableList: any[] = ['employeeName', 'leaveYear', 'leaveType', 'vacationTotal', 'vacOnCalculateLWPationUsed', 'unpaidLeave', 'vacationBalance', 'encashedLeave']

    OnChangeCountry(event: any) {
        if (event !== undefined && event.value !== undefined) {
            this.onCountryChange(event.value)
        }
    }


    onChangeYear(event: any) {
        if (event !== undefined && event.value !== undefined) {
            this.onYearChange(event.value)
        }
    }


}
