import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { EmployeeService } from './employee.service';
import { IRolePermissionDisplay } from '../../core/interfaces/RolePermission';
import { Constants, ListTypeConstants, SessionConstants, UserRole } from '../../constant';
import { DataProvider } from '../../core/providers/data.provider';
import { ITableHeaderAction, ITableRowAction } from '../../shared/ihr-table/table-options';
import { HolidayService } from '../holidays/holidays.service';
import { ICountryDisplay } from '../../core/interfaces/Country';
import { LookUpService } from '../lookup/lookup.service';
import { IListTypeDisplay } from '../../core/interfaces/ListType';
import { IListValueDisplay } from '../../core/interfaces/ListValue';
import { IDropDown } from '../../core/interfaces/IDropDown';
import { CommonUtils } from '../../common/common-utils';
import * as moment from 'moment';
import { Router } from '@angular/router';
import { IEmployeeDisplay } from '../../core/interfaces/Employee';
import { IHRTableComponent } from '../../shared/ihr-table/ihr-table.component';
import { EmployeeNotificationModalComponent } from './employee-notification-modal/employee-notification-modal.component';

@Component({
    selector: 'app-employee',
    templateUrl: './employee.component.html',
    styleUrls: ['./employee.component.scss']
})
export class EmployeeComponent implements OnInit {
    commonUtils = new CommonUtils()
    DefaultPageSize: number = 15;
    cols: any[] = [];
    selectedColumns: any[];
    isPagination: boolean = true;
    loading: boolean = false;
    showCurrentPageReport: boolean = true;
    Country: string = 'Country';
    rowActions: ITableRowAction[] = [];
    headerActions: ITableHeaderAction[] = [];
    patientLanding: string = 'patientLanding';
    globalFilterFields = ['employeeCode', 'employeeName', 'title', 'hireDate', 'department', 'workAuthorization', 'email', 'phone']
    holidaysList: any[] = [];

    EmployeesList: any[] = [];

    //table dropdown
    lstCountry: any[] = [];
    country: string;
    DefaultDwnDn1ID: number;
    // 
    lstEmployeeType: any[] = [];
    DropDown2DefaultID: number;
    selectedEmpTypeList: number[] = [];
    employeeType: string;
    employeeTypeList: number[] = [];

    //table dropdown
    lstStatus: any[] = [];
    status: string;
    DefaultDwnDn3ID: number;

    RolePermissions: IRolePermissionDisplay[] = []
    user: any;
    constructor(private fb: FormBuilder, private dataProvider: DataProvider,
        private holidayService: HolidayService,
        private LookupService: LookUpService,
        private router: Router,
        private employeeService: EmployeeService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.user = JSON.parse(localStorage.getItem("User"));
        this.LoadDropDown();
        this.loadTableColumns();
        this.LoadTableConfig();
        if (this.dataProvider !== undefined && this.dataProvider.EmployeeFilters !== undefined && this.dataProvider.EmployeeFilters !== null) {
            this.DefaultDwnDn1ID = this.dataProvider.EmployeeFilters["DefaultDwnDn1ID"]
            this.selectedEmpTypeList = this.dataProvider.EmployeeFilters["selectedEmpTypeList"]
            this.DefaultDwnDn3ID = this.dataProvider.EmployeeFilters["DefaultDwnDn3ID"]
            this.country = this.dataProvider.EmployeeFilters["country"]
            this.status = this.dataProvider.EmployeeFilters["status"]
            this.employeeType = this.dataProvider.EmployeeFilters["employeeType"]
            this.LookupService.getListValues().subscribe(result => {
                if (result['data'] !== undefined && result['data'] !== null) {
                    this.EmployMentList = result['data'].filter(x => x.type.toUpperCase() === ListTypeConstants.EMPLOYMENTTYPE);
                    this.setEmployeeTypeList();
                    this.LoadEmployees();
                }
            })
        }
     
    }

    ngOnInit(): void {
        this.DefaultPageSize = 15;
        if (this.dataProvider.country != null) {
            this.country = this.dataProvider.country;
        }
        if (this.dataProvider.status != null) {
            this.status = this.dataProvider.status;
        }
        if (this.dataProvider.employeeType != null) {
            this.employeeTypeList = this.dataProvider.employeeType;
            this.selectEmployeeTypeList();
        }
        if (this.dataProvider.storage != null) {
            var Empdata = this.dataProvider.storage;
            if (Empdata.employeeID != 0) {
                setTimeout(() => this.employeeChangeSets(Empdata))
            }
            this.dataProvider.storage = null;
        }
        this.LoadEmployees();
    }

    loadTableColumns() {
        this.cols = [
            { field: 'employeeCode', header: 'Emp Code' },
            { field: 'employeeName', header: 'Employee Name' },
            { field: 'title', header: 'Title' },
            { field: 'hireDate', header: 'Hire Date' },
            { field: 'department', header: 'Department' },
            { field: 'workAuthorization', header: 'Work Auth' },
            { field: 'email', header: 'Email' },
            { field: 'phone', header: 'Phone' }
        ];

        this.selectedColumns = this.cols;
    }
    Delete = () => {

    }
    EmployeeRolePermission: IRolePermissionDisplay
    LoadTableConfig() {
        this.EmployeeRolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.EMPLOYEE);
        if (this.EmployeeRolePermission !== null && this.EmployeeRolePermission !== undefined) {
            //if (this.EmployeeRolePermission.update === true) {
            this.rowActions = [];
            if (this.EmployeeRolePermission !== null && this.EmployeeRolePermission.update === true) {
                this.rowActions.push({
                    actionMethod: this.editEmployee,
                    iconClass: 'pi pi-pencil'
                })

            }
            if (this.EmployeeRolePermission !== null && this.EmployeeRolePermission.delete === true) {
                this.rowActions.push({
                    actionMethod: this.Delete,
                    styleClass: 'p-button-raised p-button-danger',
                    iconClass: 'pi pi-trash'
                })
            }
            this.rowActions.push({
                actionMethod: this.employeeChangeSets,
                iconClass: 'fa fa-retweet'
            })


                //this.rowActions = [
                //    {
                //        actionMethod: this.editEmployee,
                //        iconClass: 'pi pi-pencil'
                //    },
                //    {
                //        actionMethod: this.employeeChangeSets,
                //        iconClass: 'fa fa-retweet'
                //    },
                //];
            //} else {
            //    this.rowActions = [];
            //}
            if (this.EmployeeRolePermission.add === true) {
                this.headerActions = [
                    {
                        actionMethod: this.add,
                        hasIcon: false,
                        styleClass: 'btn-width-height btn btn-block btn-sm btn-info',
                        actionText: 'Add',
                        iconClass: 'fas fa-plus'

                    }
                ];
            }
        }
    }
    CountryList: ICountryDisplay[] = [];
    EmployMentList: IListValueDisplay[] = [];
    LoadDropDown() {
        this.holidayService.getCountry().subscribe(result => {
            if (result['data'] !== undefined && result['data'] !== null) {
                this.CountryList = result['data'];
                this.setCountryList();
                this.setStatusList();
                this.LoadEmployees();
            }
        })
        this.LookupService.getListValues().subscribe(result => {
            if (result['data'] !== undefined && result['data'] !== null) {
                this.EmployMentList = result['data'].filter(x => x.type.toUpperCase() === ListTypeConstants.EMPLOYMENTTYPE);
                if (this.employeeType == undefined) {
                    this.setEmployeeTypeList();
                }
                else {
                    this.lstEmployeeType = this.employeeTypeList;
                }
                this.LoadEmployees();
            }
        })
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
        this.DefaultDwnDn1ID = this.lstCountry.find(x => x.Value.toLowerCase() == this.country.toLowerCase()).ID;
    }

    selectEmployeeTypeList() {

    }

    OncountryChange(event: any) {
        this.DefaultDwnDn1ID = event;
        this.country = this.lstCountry.find(x => x.ID == event).Value;
        this.LoadEmployees();
    }
    ListItemEmpType: IDropDown;
    setEmployeeTypeList() {
        this.lstEmployeeType = [];
        if (this.EmployMentList !== undefined) {
            // this.lstEmployeeType.push({ ID: 0, Value: 'All' })
            this.EmployMentList.forEach(x => {
                this.lstEmployeeType.push({ ID: x.listValueID, Value: x.valueDesc })
            });
        }
        if (this.employeeType == undefined) {
            this.employeeType = "All";
        }
        if (this.lstEmployeeType !== undefined && this.lstEmployeeType.length > 0 && this.selectedEmpTypeList.length === 0) {
            this.lstEmployeeType.forEach(x => {
                //if (x.Value.toLowerCase() === this.employeeType.toLowerCase()) {
                this.selectedEmpTypeList.push(x.ID);
                //}
            })
        }
    }


    onChangeEmpTypes(event: any[]) {
        this.selectedEmpTypeList = [];
        this.selectedEmpTypeList = event;
        if (this.selectedEmpTypeList.length > 0) {
            if (this.selectedEmpTypeList.length === this.lstEmployeeType.length) {
                this.employeeType = 'All';
            } else {
                this.employeeType = 'NotAll';
            }
        }
        this.LoadEmployees();
    }
    setStatusList() {
        this.lstStatus = [{ 'ID': 0, Value: 'All' }, { 'ID': 1, Value: 'Active' }, { 'ID': 2, Value: 'Termed' }]
        if (this.DefaultDwnDn3ID === undefined || this.DefaultDwnDn3ID === null) {
            this.status = 'Active';
            this.DefaultDwnDn3ID = this.lstStatus.find(x => x.Value.toLowerCase() == this.status.toLowerCase()).ID
        }
    }
    OnStatusChange(event: any) {
        this.DefaultDwnDn3ID = event;
        this.status = this.lstStatus.find(x => x.ID == event).Value;
        this.LoadEmployees();
    }
    lstEmployees: any[] = [];
    LoadEmployees() {
        var RoleShort = localStorage.getItem("RoleShort");
        this.employeeService.GetEmployees().subscribe(respEmployee => {
            if (respEmployee['messageType'] !== null && respEmployee['messageType'] === 1) {
                if (respEmployee['data'] != null && (RoleShort.toUpperCase() === UserRole.EMP || RoleShort.toUpperCase() == UserRole.CONTRACTOR)) {
                    this.lstCountry = [];
                    this.lstEmployeeType = [];
                    this.lstStatus = [];
                    this.country = 'All';
                    this.status = 'All';
                    this.employeeType = 'All'
                    respEmployee['data'].forEach((d) => {
                        d.hireDate = moment(d.hireDate).format("MM/DD/YYYY")
                    })
                    this.EmployeesList = [];
                    this.EmployeesList = respEmployee['data'].filter(x => x.employeeID === this.user.employeeID);
                    this.lstEmployees = [];
                    this.lstEmployees = this.EmployeesList;
                    this.loadEmployeeList();
                }
                else {
                    respEmployee['data'].forEach((d) => {
                        d.hireDate = moment(d.hireDate).format("MM/DD/YYYY")
                    })
                    this.EmployeesList = respEmployee['data'];
                    if (this.EmployeesList.length > 0) {
                        this.loadEmployeeList();
                    }
                }
            } else {
                this.EmployeesList = []
                this.lstEmployees = this.EmployeesList;
            }
        })
    }
    loadEmployeeList() {
        if (this.country !== "All" && this.employeeType !== 'All' && this.status !== "All") {
            if (this.status == "Active") {
                this.lstEmployees = this.EmployeesList.filter(x => x.country == this.country && (x.termDate == null || new Date(x.termDate) > new Date()) && this.selectedEmpTypeList.includes(x.employmentTypeID));
            }
            else {
                this.lstEmployees = this.EmployeesList.filter(x => x.country == this.country && x.termDate != null && new Date(x.termDate) <= new Date() && this.selectedEmpTypeList.includes(x.employmentTypeID));
            }
        }
        else if (this.country == "All" && this.employeeType !== 'All' && this.status !== "All") {
            //lstEmployees = EmployeesList.Where(x => selectedempTypeList.Any(s => s.ID == x.EmploymentTypeID)).ToList();
            if (this.status == "Active") {
                this.lstEmployees = this.EmployeesList.filter(x => (x.termDate == null || new Date(x.termDate) > new Date()) && this.selectedEmpTypeList.includes(x.employmentTypeID));
            }
            else {
                this.lstEmployees = this.EmployeesList.filter(x => x.termDate != null && new Date(x.termDate) <= new Date() && this.selectedEmpTypeList.includes(x.employmentTypeID));
            }
        }
        else if (this.country !== "All" && this.employeeType == "All" && this.status !== "All") {
            if (this.status == "Active") {
                this.lstEmployees = this.EmployeesList.filter(x => x.country === this.country && (x.termDate == null || new Date(x.termDate) > new Date()));
            }
            else {
                this.lstEmployees = this.EmployeesList.filter(x => x.country == this.country && x.termDate != null && new Date(x.termDate) <= new Date());
            }
        }
        else if (this.country !== "All" && this.employeeType !== "All" && this.status === "All") {
            this.lstEmployees = this.EmployeesList.filter(x => x.country == this.country && this.selectedEmpTypeList.includes(x.employmentTypeID));
        }
        else if (this.country === "All" && this.employeeType === "All" && this.status !== "All") {
            if (this.status === "Active") {
                this.lstEmployees = this.EmployeesList.filter(x => x.termDate == null || new Date(x.termDate) > new Date());
            }
            else {
                this.lstEmployees = this.EmployeesList.filter(x => x.termDate != null && new Date(x.termDate) <= new Date());
            }
        }
        else if (this.country !== "All" && this.employeeType === "All" && this.status === "All") {
            this.lstEmployees = this.EmployeesList.filter(x => x.country == this.country);
        }
        else if (this.country === "All" && this.employeeType !== "All" && this.status === "All") {
            this.lstEmployees = this.EmployeesList.filter(x => this.selectedEmpTypeList.includes(x.employmentTypeID));
        }
        else {
            this.lstEmployees = this.EmployeesList;
        }
      
    }

    add = () => {
        this.dataProvider.storage = {
            isEditMode: false,
            country: this.country,
            status: this.status,
            employeeType: this.selectedEmpTypeList,
            employeeDetails: null
        };
        this.dataProvider.EmployeeFilters = {
            DefaultDwnDn1ID: this.DefaultDwnDn1ID,
            selectedEmpTypeList: this.selectedEmpTypeList,
            DefaultDwnDn3ID: this.DefaultDwnDn3ID,
            country: this.country,
            status: this.status,
            employeeType: this.selectedEmpTypeList,
        };
        this.router.navigate(['employees/AddEmployee']);
        //this.addEditHolidayModal.show();
    }
    editEmployee = (selected: IEmployeeDisplay) => {
        if (selected !== undefined && selected !== null) {
            this.dataProvider.storage = {
                isEditMode: true,
                country: this.country,
                status: this.status,
                employeeType: this.selectedEmpTypeList,
                employeeDetails: selected
            }
            this.dataProvider.EmployeeFilters = {
                DefaultDwnDn1ID: this.DefaultDwnDn1ID,
                selectedEmpTypeList: this.selectedEmpTypeList,
                DefaultDwnDn3ID: this.DefaultDwnDn3ID,
                country: this.country,
                status: this.status,
                employeeType: this.selectedEmpTypeList,
            };
            this.router.navigate(['employees/AddEmployee']);
        }
    }
    @ViewChild('employeeNotificationModal') employeeNotificationModal: EmployeeNotificationModalComponent;

    employeeChangeSets = (selected: IEmployeeDisplay) => {
      this.employeeNotificationModal.show(selected.employeeID);
    }
    saveHoliday() {

    }
    cancelHoliday() {

    }

    //Mobiledropdown changes method
    searchText: string;
    searchableList: any[] = ['employeeName', 'employeeCode', 'title', 'email', 'phone']
    onChangeCountry(event: any) {
        if (event !== undefined && event.value !== undefined) {
            this.OncountryChange(event.value)
        }
    }
    onChangeStatus(event: any) {
        if (event !== undefined && event.value !== undefined) {
            this.OnStatusChange(event.value)
        }
    }

    OnEmpChange(event: any) {
        if (event !== undefined && event.value !== undefined) {
            this.selectedEmpTypeList = event.value;
            this.onChangeEmpTypes(event.value);
        }
    }
    AddEmployee() {
        this.add();
    }
    EditMobile(employee: IEmployeeDisplay) {
        this.editEmployee(employee);
    }
}
