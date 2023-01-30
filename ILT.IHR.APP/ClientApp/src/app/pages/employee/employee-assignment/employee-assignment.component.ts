import { Component, Input, OnInit, ViewChild } from '@angular/core';
import * as moment from 'moment';
import { Constants, SessionConstants } from '../../../constant';
import { IAssignmentRateDisplay } from '../../../core/interfaces/AssignmentRate';
import { IEmployeeAssignmentDisplay } from '../../../core/interfaces/EmployeeAssignment';
import { IRolePermissionDisplay } from '../../../core/interfaces/RolePermission';
import { IHRTableComponent } from '../../../shared/ihr-table/ihr-table.component';
import { ITableHeaderAction, ITableRowAction } from '../../../shared/ihr-table/table-options';
import { EmployeeService } from '../employee.service';
import { AddEditAssignmentComponent } from './add-edit-assignment/add-edit-assignment.component';
import { AddEditAssignmentrateComponent } from './add-edit-assignmentrate/add-edit-assignmentrate.component';
import { EmployeeAssignmentService } from './assignment.service';

@Component({
  selector: 'app-employee-assignment',
  templateUrl: './employee-assignment.component.html',
  styleUrls: ['./employee-assignment.component.scss']
})
export class EmployeeAssignmentComponent implements OnInit {
    @ViewChild('addEditAssignmentModal') addEditAssignmentModal: AddEditAssignmentComponent;
    @ViewChild('addEditAssignmentRateModal') addEditAssignmentRateModal: AddEditAssignmentrateComponent;
    @Input() EmployeeID: number;
    @Input() Name: string;
    cols: any[] = [];
    selectedColumns: any[];
    isPagination: boolean = true;
    loading: boolean = false;
    showCurrentPageReport: boolean = true;
    rowActions: ITableRowAction[] = [];
    headerActions: ITableHeaderAction[] = [];
    patientLanding: string = 'patientLanding';
    globalFilterFields = ['startDate', 'endDate', 'client', 'vendor', 'title', 'clientManager', 'paymentType']


    childCols: any[] = [];
    selectedChildColumns: any[];
    isChildPagination: boolean = true;
   
    rowChildActions: ITableRowAction[] = [];
    headerChildActions: ITableHeaderAction[] = [];
   

    RolePermissions: IRolePermissionDisplay[] = [];
    EmployeeInfoRolePermission: IRolePermissionDisplay;

    //dependentsList: IW4EmployeeDisplay[] = [];
    lstEmployeeAssignmentList: IEmployeeAssignmentDisplay[] = [];
    employeeAssignmentList: IEmployeeAssignmentDisplay[] = [];

    constructor(private employeeService: EmployeeService,
                private assignmentService: EmployeeAssignmentService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.EmployeeInfoRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.EMPLOYEEINFO);
    }

    ngOnInit(): void {
        this.LoadList();
        this.loadTableColumns();
  }

    loadTableColumns() {
        this.cols = [
            { field: 'startDate', header: 'Start Date' },
            { field: 'endDate', header: 'End Date' },
            { field: 'client', header: 'Client' },
            { field: 'vendor', header: 'Vendor' },
            { field: 'title', header: 'Title' },
            { field: 'clientManager', header: 'Client Manager' },
            { field: 'paymentType', header: 'Payment Type' },
        ];
        this.selectedColumns = this.cols;
        this.childCols = [
            { field: 'startDate', header: 'Start Date' },
            { field: 'endDate', header: 'End Date' },
            { field: 'billingRate', header: 'Billing Rate' },
            { field: 'paymentRate', header: 'Payment Rate' },
           
        ];
        this.selectedChildColumns = this.childCols;
    }

    LoadList() {
        this.LoadTableConfig();
        if (this.EmployeeID !== undefined && this.EmployeeID !== 0) {
            this.assignmentService.getEmployeeAssignments(this.EmployeeID).subscribe(resp => {
            })
            this.employeeService.getEmployeeByIdAsync(this.EmployeeID).subscribe(result => {
                if (result !== undefined && result['data'] !== null && result['data'] !== undefined && result['data'].assignments.length > 0) {
                    result['data'].assignments.forEach((d) => {
                        d.startDate = moment(d.startDate).format("MM/DD/YYYY")
                        if (d.endDate !== null) {
                            d.endDate = moment(d.endDate).format("MM/DD/YYYY")
                        }
                    })
                    this.employeeAssignmentList = result['data'].assignments;
                    this.lstEmployeeAssignmentList = this.employeeAssignmentList;
                }
            }, error => {
                //toastService.ShowError();
            })
        }
    }
    rolePermission: IRolePermissionDisplay;
    LoadTableConfig() {
        this.rolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.ASSIGNMENT);
        this.rowActions = [];
        if (this.rolePermission !== null && this.rolePermission.update === true) {
            this.rowActions.push({
                actionMethod: this.Edit,
                iconClass: 'pi pi-pencil'
            })
        }
        if (this.rolePermission !== null && this.rolePermission.delete === true) {
            this.rowActions.push({
                actionMethod: this.Delete,
                styleClass: 'p-button-raised p-button-danger',
                iconClass: 'pi pi-trash'
            })
        }
        
        this.headerActions = [
            {
                actionMethod: this.add,
                hasIcon: false,
                styleClass: 'btn-width-height btn btn-block btn-sm btn-info',
                actionText: 'Add',
                iconClass: 'fas fa-plus'
            }
        ];
        this.headerChildActions = [
            {
                actionMethod: this.addChild,
                hasIcon: false,
                styleClass: 'btn-width-height btn btn-block btn-sm btn-info',
                actionText: 'Add',
                iconClass: 'fas fa-plus'
            }
        ];
        this.rowChildActions = [
            {
                actionMethod: this.EditChild,
                iconClass: 'pi pi-pencil'
            },
            {
                actionMethod: this.DeleteChild,
                styleClass: 'p-button-raised p-button-danger',
                iconClass: 'pi pi-trash'
            },
        ];
    }

    add = () => {
        this.addEditAssignmentModal.Show(0)
    }
    Edit = (selected: any) => {
        this.addEditAssignmentModal.Show(selected.assignmentID)
    }
    Delete = () => {

    }
    selected: IEmployeeAssignmentDisplay;
    selectedChild: IAssignmentRateDisplay;
    EmployeeAssignmentRate: IAssignmentRateDisplay[] = [];
    GetAssignmentRates() {
        this.assignmentService.getEmployeeAssignmentById(this.selected.assignmentID).subscribe(resp => {
            this.EmployeeAssignmentRate = resp['Data'].assignmentRates;
        })
    }

    loadChildTableConfig() {

    }
    addChild = (assignment: any) => {
        this.addEditAssignmentRateModal.ShowChild(assignment.assignmentID, 0)
    }
    EditChild = (selectedAssignmentRate: any) => {
        this.addEditAssignmentRateModal.ShowChild(selectedAssignmentRate.assignmentID, selectedAssignmentRate.assignmentRateID)
    }
    @ViewChild('dt') dt: IHRTableComponent;
    LoadRateList(assignment: any) {
        //this.dt.isExpandRowRate = true;
        //this.dt.isRowExpand = false;
        //this.dt.isRowExpandContact = false;
        this.isOpen(assignment);
    }

    DeleteChild = () => {

    }


    isSearchRequired: boolean = false;
    isExpandRowRate: boolean = false;
    assignmenRates: IAssignmentRateDisplay[] = [];
    defaultPageSize: number = 15;
    data2: any[] = [];
    isOpen(rowData: any) {
        setTimeout(() => this.isExpandRowRate = true);
        this.assignmentService.getEmployeeAssignmentById(rowData.assignmentID).subscribe(result => {
            if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                var assignment = result['data'];
                if (assignment.assignmentRates.length > 0) {
                    assignment.assignmentRates.forEach((d) => {
                        d.startDate = moment(d.startDate).format("MM/DD/YYYY")
                        if (d.endDate !== null) {
                            d.endDate = moment(d.endDate).format("MM/DD/YYYY")
                        }
                    })
                }
                this.data2 = assignment.assignmentRates !== null || assignment.assignmentRates.length > 0 ? assignment.assignmentRates : [];
                this.assignmenRates = assignment.assignmentRates;
            }
        })
    }
    //}

    //Mobiledropdown changes method
    searchText: string;
    searchableList: any[] = ['startDate', 'endDate', 'client', 'vendor', 'title', 'clientManager', 'paymentType']
}
