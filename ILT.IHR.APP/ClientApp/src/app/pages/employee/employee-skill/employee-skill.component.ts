import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { Constants, SessionConstants } from '../../../constant';
import { IEmployeeSkillDisplay } from '../../../core/interfaces/EmployeeSkill';
import { IRolePermissionDisplay } from '../../../core/interfaces/RolePermission';
import { ITableHeaderAction, ITableRowAction } from '../../../shared/ihr-table/table-options';
import { EmployeeService } from '../employee.service';
import { AddEditSkillComponent } from './add-edit-skill/add-edit-skill.component';
import { SkillService } from './skill.service';

@Component({
  selector: 'app-employee-skill',
  templateUrl: './employee-skill.component.html',
  styleUrls: ['./employee-skill.component.scss']
})
export class EmployeeSkillComponent implements OnInit {
    @ViewChild('addEditEmployeeSkillModal') addEditEmployeeSkillModal: AddEditSkillComponent;
    @Input() EmployeeID: number;
    @Input() SSN: string;
    @Input() Name: string;
    cols: any[] = [];
    selectedColumns: any[];
    isPagination: boolean = true;
    loading: boolean = false;
    showCurrentPageReport: boolean = true;
    rowActions: ITableRowAction[] = [];
    headerActions: ITableHeaderAction[] = [];
    patientLanding: string = 'patientLanding';
    globalFilterFields = ['skill', 'skillType', 'experience']
    RolePermissions: IRolePermissionDisplay[] = [];
    EmployeeInfoRolePermission: IRolePermissionDisplay;

    constructor(private employeeService: EmployeeService,
        private skillService: SkillService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.EmployeeInfoRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.SKILL);
    }

    ngOnInit(): void {
        this.LoadList();
        this.loadTableColumns();
    }
    loadTableColumns() {
        this.cols = [
            { field: 'skill', header: 'Skill' },
            { field: 'skillType', header: 'Skill Type' },
            { field: 'experience', header: 'Experience' }
        ];

        this.selectedColumns = this.cols;
    }
    skillList: IEmployeeSkillDisplay[] = [];
    lstSkillList: IEmployeeSkillDisplay[] = [];
    LoadList() {
        this.LoadTableConfig();
        if (this.EmployeeID !== undefined && this.EmployeeID !== 0) {
            this.skillService.getEmployeeSkill(this.EmployeeID).subscribe(result => {
                if (result !== undefined && result['data'] !== null && result['data'] !== undefined && result['data'].length > 0) {
                    this.skillList = result['data'];
                    this.lstSkillList = this.skillList;
                }
            }, error => {
                //toastService.ShowError();
            })
        }
    }
    rolePermission: IRolePermissionDisplay;
    LoadTableConfig() {
        this.rolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.SKILL);
        this.rowActions = [];
        if (this.rolePermission !== null && this.rolePermission !== undefined) {
            if (this.rolePermission !== null && this.rolePermission.update === true) {
                this.rowActions.push({
                    actionMethod: this.editSkill,
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

            //    this.rowActions = [
            //        {
            //            actionMethod: this.editSkill,
            //            iconClass: 'pi pi-pencil'
            //        },
            //        {
            //            actionMethod: this.Delete,
            //            styleClass: 'p-button-raised p-button-danger',
            //            iconClass: 'pi pi-trash'
            //        },
            //    ];
            //} else {
            //    this.rowActions = [];
            //}
            if (this.rolePermission.add === true) {
                this.headerActions = [
                    {
                        actionMethod: this.add,
                        hasIcon: false,
                        styleClass: 'btn-width-height btn btn-block btn-sm btn-info',
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

    editSkill = (selected) => {
        console.log(selected)
        this.addEditEmployeeSkillModal.Show(selected.employeeSkillID, this.EmployeeID)
    }

    add = () => {
        this.addEditEmployeeSkillModal.Show(0, this.EmployeeID)
    }

    Delete = () => {

    }

    //Mobiledropdown changes method
    searchText: string;
    searchableList: any[] = ['skill', 'skillType', 'experience']


}
