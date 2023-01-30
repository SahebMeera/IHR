import { Component, OnInit, ViewChild } from '@angular/core';
import { ITableRowAction, ITableHeaderAction } from 'src/app/shared/ihr-table/table-options';
import { HolidayService } from './holidays.service';
import { IRolePermissionDisplay } from '../../core/interfaces/RolePermission';
import { Constants, SessionConstants } from '../../constant';
import { IDropDown } from '../../core/interfaces/IDropDown';
import { IHolidayDisplay } from '../../core/interfaces/Holiday';
import { CommonUtils } from '../../common/common-utils';
import { AddEditHolidayComponent } from './add-edit-holiday/add-edit-holiday.component';

@Component({
    selector: 'app-holidays',
    templateUrl: './holidays.component.html',
    styleUrls: ['./holidays.component.scss']
})
export class HolidaysComponent implements OnInit {
    @ViewChild('AddEditHoliday') addEditHolidayPopup: AddEditHolidayComponent;
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

    HolidaysList: IHolidayDisplay[] = [];
    lstHolidaysList: IHolidayDisplay[] = [];
    yearId: number

    LeaveYearList: any[] = [];
    lstYear: any[] = [];
    DefaultDwnDn1ID: number;
    lstStatus: any[] = [];

    constructor(private holidayService: HolidayService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
    }

    ngOnInit(): void {
        this.loadYearDropdown();
        this.yearId = Number(this.LeaveYearList.find(x => x.text.toLowerCase() === "Current Year".toLowerCase()).year);
        this.LoadList();
        this.loadTableColumns();
    }

    loadTableColumns() {
        this.cols = [
            { field: 'name', header: 'Name' },
            { field: 'date', header: 'Date' },
            { field: 'country', header: 'Country' }
        ];

        this.selectedColumns = this.cols;
    }

    LoadList() {
        this.LoadTableConfig();
        this.holidayService.getHolidayList().subscribe(result => {
            if (result['data'] !== undefined && result['data'] !== null) {
                var lstValues = result['data'].filter(x => x['date'] = new Date(x.startDate).toLocaleDateString('en-US', {
                    weekday: "long",
                    year: "numeric",
                    month: "short",
                    day: "numeric"
                }))
                this.HolidaysList = lstValues;
                this.lstHolidaysList = this.HolidaysList
                if (this.yearId !== 0) {
                    this.onYearChange(this.yearId);
                }
            }
            
        }, error => {
            //toastService.ShowError();
        })
    }
    Delete = () => {

    }
    HolidayRolePermission: IRolePermissionDisplay;
    LoadTableConfig() {
        this.rowActions = [];
        this.HolidayRolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.HOLIDAY);
        if (this.HolidayRolePermission !== null && this.HolidayRolePermission !== undefined) {
            if (this.HolidayRolePermission.update === true) {
                this.rowActions.push(
                    {
                        actionMethod: this.editHoliday,
                        iconClass: 'pi pi-pencil'
                    },
                );
            }
            if (this.HolidayRolePermission.delete === true) {
                this.rowActions.push(
                    {
                        actionMethod: this.Delete,
                        styleClass: 'p-button-raised p-button-danger',
                        iconClass: 'pi pi-trash'
                    })
            }
            if (this.HolidayRolePermission.add === true) {
                this.headerActions = [
                    {
                        actionMethod: this.add,
                        hasIcon: false,
                        styleClass: 'btn btn-block btn-sm btn-info',
                        actionText: 'Add',
                        iconClass: 'fas fa-plus'
                    }
                ];
            } else {
                this.headerActions = [];
            }
        } else {
            this.rowActions = [];
            this.headerActions = [];
        }
    }

    onYearChange(Year: number) {
        this.yearId = Year;
        if (Year !== 0) {
            this.lstHolidaysList = this.HolidaysList.filter(x => new Date(x.startDate).getFullYear() === Year);
        } else {
            this.lstHolidaysList = this.HolidaysList;
        }
    }

    onChangeYear(event: any) {
        if (event !== undefined && event.value !== undefined) {
            this.onYearChange(event.value)
        }
    }

    loadYearDropdown() {
        const currentYear = new Date().getFullYear(); // 2020
        const previousYear = currentYear - 1;
        this.LeaveYearList.push({ year: currentYear, text: "Current Year" }, { year: previousYear, text: "Previous Year" })
        this.SetYearList();
        this.DefaultDwnDn1ID = currentYear;
        this.lstHolidaysList = this.HolidaysList.filter(x => new Date(x.startDate).getFullYear() === currentYear);
    }
    ListItem: IDropDown;
    SetYearList() {
        this.lstYear = [];
        if (this.LeaveYearList !== undefined && this.LeaveYearList.length > 0) {
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

    add = () => {
        console.log('New')
        this.addEditHolidayPopup.show(0);
    }
    editHoliday = (holiday: any) => {
        console.log(holiday)
        this.addEditHolidayPopup.show(holiday.holidayID);
    }

    searchText: string;
    searchableList: any[] = ['name', 'startDate', 'country']

    loadHolidays(event) {
        this.yearId = event;
        this.LoadList();
    }


}
