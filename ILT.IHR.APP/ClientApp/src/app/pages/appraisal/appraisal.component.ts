import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { CommonUtils } from '../../common/common-utils';
import { Constants, SessionConstants } from '../../constant';
import { IAppraisalDisplay } from '../../core/interfaces/Appraisal';
import { IDropDown } from '../../core/interfaces/IDropDown';
import { IRolePermissionDisplay } from '../../core/interfaces/RolePermission';
import { DataProvider } from '../../core/providers/data.provider';
import { ITableHeaderAction, ITableRowAction } from '../../shared/ihr-table/table-options';
import { HolidayService } from '../holidays/holidays.service';
import { AppraisalService } from './appraisal.service';

@Component({
  selector: 'app-appraisal',
  templateUrl: './appraisal.component.html',
  styleUrls: ['./appraisal.component.scss']
})
export class AppraisalComponent implements OnInit {
    //@ViewChild('AddEditHoliday') addEditHolidayPopup: AddEditHolidayComponent;
    commonUtils = new CommonUtils();
    cols: any[] = [];
    selectedColumns: any[];
    isPagination: boolean = true;
    loading: boolean = false;
    showCurrentPageReport: boolean = true;
    rowActions: ITableRowAction[] = [];
    headerActions: ITableHeaderAction[] = [];
    patientLanding: string = 'patientLanding';
    globalFilterFields = ['name', 'date', 'country']

    RolePermissions: IRolePermissionDisplay[] = [];

    Appraisals: IAppraisalDisplay[] = [];
    lstAppraisalsList: IAppraisalDisplay[] = [];

    yearId: number
    LeaveYearList: any[] = [];
    lstYear: any[] = [];
    DefaultDwnDn1ID: number;
    user: any;
    EmployeeID: number;

    constructor(private holidayService: HolidayService,
        private router: Router,
        private dataProvider: DataProvider,
      private appraisalService: AppraisalService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.user = JSON.parse(localStorage.getItem("User"));
        this.EmployeeID = Number(this.user.employeeID);
    }

    ngOnInit(): void {
        this.loadYearDropdown();
        this.yearId = Number(this.LeaveYearList.find(x => x.text.toLowerCase() === "Current Year".toLowerCase()).year);
        this.loadTableColumns();
        this.LoadTableConfig();
        if (this.user != null && this.user.employeeID > 0) {
            this.LoadAppraisals();
        }
    }

    loadTableColumns() {
        this.cols = [
            { field: 'employeeName', header: 'Employee Name' },
            { field: 'reviewYear', header: 'Year' },
            { field: 'assignedDate', header: 'Assigned Date' },
            { field: 'submitDate', header: 'Submit Date' },
            { field: 'manager', header: 'Manager' },
            { field: 'status', header: 'Status' }
        ];
        this.selectedColumns = this.cols;
    }

    searchText: string;
    searchableList: any[] = ['employeeName', 'reviewYear', 'assignedDate', 'submitDate', 'manager', 'status']
    rolePermission: IRolePermissionDisplay;
    LoadTableConfig() {
        this.rolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.APPRAISAL);
        this.rowActions = [];
        if (this.rolePermission !== null && this.rolePermission.update === true) {
            this.rowActions.push({
                actionMethod: this.edit,
                iconClass: 'pi pi-pencil'
            })
        }
        if (this.rolePermission !== null && this.rolePermission.delete === true) {
            this.rowActions.push({
                actionMethod: this.delete,
                styleClass: 'p-button-raised p-button-danger',
                iconClass: 'pi pi-trash'
            })
        }
        //this.rowActions = [
        //    {
        //        actionMethod: this.edit,
        //        iconClass: 'pi pi-pencil'
        //    }, {
        //        actionMethod: this.delete,
        //        styleClass: 'p-button-raised p-button-danger',
        //        iconClass: 'pi pi-trash'
        //    },
        //];
        this.headerActions = [
        ];
    }

    loadYearDropdown() {
        const currentYear = new Date().getFullYear(); // 2020
        const previousYear = currentYear - 1;
        this.LeaveYearList.push({ year: currentYear, text: "Current Year" }, { year: previousYear, text: "Previous Year" })
        this.SetYearList();
        this.DefaultDwnDn1ID = currentYear;
        this.lstAppraisalsList = this.Appraisals.filter(x => x.reviewYear == currentYear);

    }
    ListItem: IDropDown;
    SetYearList() {
        this.lstYear = [];
        if (this.LeaveYearList !== undefined && this.LeaveYearList.length > 0) {
            this.lstYear.push({ID: 0, Value: 'Select'})
            this.LeaveYearList.forEach(x => {
                this.lstYear.push({ ID: x.year, Value: x.text })
            });
        }
        if (this.ListItem !== undefined) {
            this.ListItem.ID = 0;
            this.ListItem.Value = "Select";
            this.lstYear.push(0, this.ListItem);
        }
    }

    onYearChange(Year: number) {
        this.yearId = Year;
        if (Year !== 0) {
            this.lstAppraisalsList = this.Appraisals.filter(x => x.reviewYear == this.yearId);
        } else {
            this.lstAppraisalsList = this.Appraisals;
        }
    }

    onChangeYear(event: any) {
        if (event !== undefined && event.value !== undefined) {
            this.onYearChange(event.value)
        }
    }

    LoadAppraisals() {
        if (this.user != null && this.user.employeeID != null) {
            const EmployeeID = Number(this.user.employeeID);
            this.appraisalService.GetAppraisalList(EmployeeID).subscribe(resp => {
                if (resp['data'] !== null && resp['messageType'] === 1) {
                    if (resp['data'] != null && resp['data'] != undefined) {
                        resp['data'].forEach(d => {
                            if (d.assignedDate !== null) {
                                d.assignedDate = moment(d.assignedDate).format("MM/DD/YYYY")
                            }
                            if (d.submitDate !== null) {
                                d.submitDate = moment(d.submitDate).format("MM/DD/YYYY")
                            }
                        })
                    }
                    this.Appraisals = resp['data']
                    this.lstAppraisalsList = this.Appraisals;
                    if (this.yearId != 0) {
                        this.onYearChange(this.yearId);
                    }
                }
            })
        }
    }

    edit = (selected: IAppraisalDisplay) => {
        if (selected !== undefined && selected !== null) {
            this.dataProvider.storage = {
                isEditMode: true,
                appraisalDetails: selected
            }
            this.router.navigate(['appraisal/appraisalDetails']);
        }
    }

    delete = () => {

    }

    
}
