import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import * as moment from 'moment';
import { forkJoin } from 'rxjs';
import { CommonUtils } from '../../common/common-utils';
import { Constants, ListTypeConstants, SessionConstants, UserRole } from '../../constant';
import { IAssetDisplay } from '../../core/interfaces/Asset';
import { IRolePermissionDisplay } from '../../core/interfaces/RolePermission';
import { DataProvider } from '../../core/providers/data.provider';
import { ITableHeaderAction, ITableRowAction } from '../../shared/ihr-table/table-options';
import { EmployeeService } from '../employee/employee.service';
import { LookUpService } from '../lookup/lookup.service';
import { AddEditAssetComponent } from './add-edit-asset/add-edit-asset.component';
import { AssetChangesetComponent } from './asset-changeset/asset-changeset.component';
import { AssetService } from './asset.service';

@Component({
  selector: 'app-asset',
  templateUrl: './asset.component.html',
  styleUrls: ['./asset.component.scss']
})
export class AssetComponent implements OnInit {
    @ViewChild('AddEditAssetModal') AddEditAssetModal: AddEditAssetComponent;
    commonUtils = new CommonUtils()
    cols: any[] = [];
    selectedColumns: any[];
    isPagination: boolean = true;
    loading: boolean = false;
    showCurrentPageReport: boolean = true;
    rowActions: ITableRowAction[] = [];
    headerActions: ITableHeaderAction[] = [];
    patientLanding: string = 'patientLanding';
    globalFilterFields = ['tag', 'assetType', 'make', 'model', 'purchaseDate', 'warantyExpDate', 'assignedTo', 'status']

    RolePermissions: IRolePermissionDisplay[] = []
    user: any;

    EmployeeID: number = 0;
    currentLoginUserRole: string = '';
    UserName: string = '';

    AssetStatusList: any[] = [];
    AssetStatus: string = '';
    lstAssetStatus: any[] = [];
    AssetTypeList: any[] = [];
    AssetType: string = ''
    lstAssetsList: IAssetDisplay[] = [];

    constructor(private fb: FormBuilder, private dataProvider: DataProvider,
        private assetService: AssetService,
        private lookupService: LookUpService,
        private employeeService: EmployeeService,
        private router: Router) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.user = JSON.parse(localStorage.getItem("User"));
        this.EmployeeID = Number(this.user.employeeID);
        this.UserName = this.user.firstName + " " + this.user.lastName;
        this.currentLoginUserRole = localStorage.getItem("RoleName");
        this.LoadDropDown();
        this.loadTableColumns();
        this.LoadTableConfig();
        this.LoadList();
    }

    ngOnInit(): void {
       this.LoadList();
       // this.loadTableColumns();
       // this.LoadTableConfig();
    }

    loadTableColumns() {
        this.cols = [
            { field: 'tag', header: 'Tag ' },
            { field: 'assetType', header: 'Asset Type' },
            { field: 'make', header: 'Make' },
            { field: 'model', header: 'Model' },
            { field: 'purchaseDate', header: 'Purchase Date' },
            { field: 'warantyExpDate', header: 'Warranty Exp Date' },
            { field: 'assignedTo', header: 'Assigned To' },
            { field: 'status', header: 'Status' },
        ];

        this.selectedColumns = this.cols;
    }

    LoadTableConfig() {
        var AssetRolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.ASSET);
        if (AssetRolePermission != null) {
            if (AssetRolePermission.update == true) {
                this.rowActions = [
                    {
                        actionMethod: this.edit,
                        iconClass: 'pi pi-pencil'
                    }, {
                        actionMethod: this.assetChangeSets,
                        iconClass: 'fa fa-retweet'
                    },
                ];
            } else {
                this.rowActions = []
            }
        }
        if (AssetRolePermission != null) {
            if (AssetRolePermission.add == true) {
                this.headerActions = [
                    {
                        actionMethod: this.add,
                        hasIcon: false,
                        styleClass: 'btn btn-block btn-sm btn-info',
                        actionText: 'Add',
                        iconClass: 'fas fa-plus'

                    }
                ];
            }
        } else {
            this.headerActions = []
        }
    }


  
    LoadDropDown() {
        forkJoin(
            this.lookupService.getListValues(),
        ).subscribe(async resultSet => {
            if (resultSet !== undefined && resultSet !== null) {
                this.AssetStatusList = resultSet[0]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.ASSETSTATUS);
                if (this.AssetStatus === null || this.AssetStatus === '') {
                    this.setAssetStatusList();
                }
                else {
                    //this.lstAssetStatus = this.selectedStatusList;
                }
                this.AssetTypeList = resultSet[0]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.ASSETTYPE);
                if (this.AssetType == null || this.AssetType === '') {
                    this.setAssetTypeList();
                }
                this.LoadList()
            }
        });
    }

    selectedStatusList: number[] = [];
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

    onStatusChange(event: any[]) {
        this.selectedStatusList = [];
        this.selectedStatusList = event;
        if (this.selectedStatusList.length > 0) {
            if (this.selectedStatusList.length === this.lstAssetStatus.length) {
                this.AssetStatus = 'All';
            } else {
                this.AssetStatus = 'NotAll';
            }
        }
        this.LoadList();
    }

    lstAssetType: any[] = [];
    selectedAssetTypeList: number[] = [];
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

    OnAssetTypeChange(event: any[]) {
        this.selectedAssetTypeList = [];
        this.selectedAssetTypeList = event;
        if (this.selectedAssetTypeList.length > 0) {
            if (this.selectedAssetTypeList.length === this.lstAssetType.length) {
                this.AssetType = 'All';
            } else {
                this.AssetType = 'NotAll';
            }
        }
        this.LoadList();
    }
    AssetsList: IAssetDisplay[] = [];
    reponses: any[] = [];
    async LoadList() {
        await this.LoadTableConfig();
        var RoleShort = await localStorage.getItem("RoleShort");
        this.assetService.GetAsset().subscribe(resultSet => {
            if (resultSet['messageType'] !== null && resultSet['messageType'] === 1) {
                this.reponses = resultSet['data'];
                if (this.reponses != null && (RoleShort.toUpperCase() == UserRole.EMP || RoleShort.toUpperCase() == UserRole.CONTRACTOR)) {
                    this.reponses.forEach((d) => {
                        if (d.purchaseDate !== null) {
                            d.purchaseDate = moment(d.purchaseDate).format("MM/DD/YYYY")
                        }
                        if (d.warantyExpDate !== null) {
                            d.warantyExpDate = moment(d.warantyExpDate).format("MM/DD/YYYY")
                        }
                    })
                    this.AssetsList = this.reponses.filter(x => x.assignedToID == this.user.employeeID)
                    this.loadAssetList();
                } else {
                    this.reponses.forEach((d) => {
                        if (d.purchaseDate !== null) {
                            d.purchaseDate = moment(d.purchaseDate).format("MM/DD/YYYY")
                        }
                        if (d.warantyExpDate !== null) {
                            d.warantyExpDate = moment(d.warantyExpDate).format("MM/DD/YYYY")
                        }
                    })
                    this.AssetsList = this.reponses;
                    this.loadAssetList();
                }
            }
        })
    }
 
    loadAssetList() {
        console.log(this.AssetType)
        console.log(this.selectedAssetTypeList)
        console.log(this.AssetStatus)
        console.log(this.selectedStatusList)
        if (this.AssetType !== "All" && this.selectedAssetTypeList !== null && this.AssetStatus !== "All" && this.selectedStatusList != null) {
            this.lstAssetsList = this.AssetsList.filter(x => this.selectedAssetTypeList.includes(x.assetTypeID) && this.selectedStatusList.includes(x.statusID))
        }
        else if (this.AssetType === "All" && this.AssetStatus !== "All") {
            this.lstAssetsList = this.AssetsList.filter(x => this.selectedStatusList.includes(x.statusID));

        } else if (this.AssetType !== "All" && this.AssetStatus === "All") {
            this.lstAssetsList = this.AssetsList.filter(x => this.selectedAssetTypeList.includes(x.assetTypeID));
        }
        else {
            this.lstAssetsList = this.AssetsList;
        }
    }

    add = () => {
        this.AddEditAssetModal.Show(0);
    }


    edit = (selected: IAssetDisplay) => {
        this.AddEditAssetModal.Show(selected.assetID);
    }
    @ViewChild('notificationModal') notificationModal: AssetChangesetComponent;

    assetChangeSets = (selected: IAssetDisplay) => {
        this.notificationModal.show(selected.assetID);
    }

    searchText: string;
    searchableList: any[] = ['tag', 'assetType', 'make', 'assignedTo','status']

    onChangeStatus(event: any) {
        if (event !== undefined && event.value !== undefined) {
            this.selectedStatusList = event.value;
            this.onStatusChange(event.value);
        }
    }

    OnChangeAssetType(event: any) {
        if (event !== undefined && event.value !== undefined) {
            this.selectedStatusList = event.value;
            this.onStatusChange(event.value);
        }
    }

}
