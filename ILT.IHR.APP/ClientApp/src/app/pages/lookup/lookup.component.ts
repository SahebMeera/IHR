import { Component, OnInit, ViewChild } from '@angular/core';
import { Constants, SessionConstants } from '../../constant';
import { IDropDown } from '../../core/interfaces/IDropDown';
import { IListTypeDisplay } from '../../core/interfaces/ListType';
import { IListValueDisplay } from '../../core/interfaces/ListValue';
import { IRolePermissionDisplay } from '../../core/interfaces/RolePermission';
import { ITableHeaderAction, ITableRowAction } from '../../shared/ihr-table/table-options';
import { AddEditLookupComponent } from './add-edit-lookup/add-edit-lookup.component';
import { LookUpService } from './lookup.service';

@Component({
  selector: 'app-lookup',
  templateUrl: './lookup.component.html',
  styleUrls: ['./lookup.component.scss']
})
export class LookupComponent implements OnInit {
    @ViewChild('AddEditLookup') addEditLookupPopup: AddEditLookupComponent;
    cols: any[] = [];
    selectedColumns: any[];
    isPagination: boolean = true;
    loading: boolean = false;
    showCurrentPageReport: boolean = true;
    rowActions: ITableRowAction[] = [];
    headerActions: ITableHeaderAction[] = [];
    patientLanding: string = 'patientLanding';
    globalFilterFields = ['value', 'employeeName', 'valueDesc', 'isActive']


    LookupsList: IListValueDisplay[] = [];
    lstLookupType: any[] = [];
    Lookups: IListTypeDisplay[] = [];
    DefaultDwnDn1ID: number;

    RolePermissions: IRolePermissionDisplay[] = [];
    constructor(private LookupService: LookUpService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));}

    ngOnInit(): void {
        this.cols = [
            { field: 'value', header: 'Lookup Value' },
            { field: 'valueDesc', header: 'Lookup Description' },
            { field: 'isActive', header: 'Active' }
        ];

        this.selectedColumns = this.cols;

        //this.LookupService.getLookUpList().subscribe(result => {
        //    this.UsersList = result['data'];
        //})
        this.LoadDropDown();
        this.LoadTableConfig();
    }

    LoadDropDown() {
        this.LookupService.getLookUpList().subscribe(result => {
            if (result !== null && result['data'] != null) {
                this.Lookups = result['data'];
                this.setLookupList();
            }
        })
    }
    ListItem: IDropDown;
    setLookupList() {
        this.lstLookupType = [];
        if (this.Lookups != null) {
            this.Lookups.forEach(x => {
                this.lstLookupType.push({ ID: x.listTypeID, Value: x.typeDesc });
            })
            if (this.ListItem !== undefined) {
                this.ListItem.ID = 0;
                this.ListItem.Value = "Select";
                this.lstLookupType.push(0, this.ListItem);
            }
            this.DefaultDwnDn1ID = 0;
        }
    }
    OnLookupTypeChange(TypeID: any) {
        this.DefaultDwnDn1ID = TypeID;
        this.LoadLookupList(TypeID);
    }

    onLookupTypeChangeMobile(event: any) {
        if (event !== undefined && event.value !== undefined) {
            this.OnLookupTypeChange(event.value)
        }
    }

    LookupId: number = 0;
    LoadLookupList(TypeID: number) {
        this.LookupId = TypeID;
        this.LoadTableConfig();
        if (this.LookupId != 0) {
            this.LookupService.getLookupByIdAsync(this.LookupId).subscribe(result => {
                this.LookupsList = [];
                if (result !== null && result['data'] !== null) {
                    var listType = result['data'];
                    if (listType['listValues'] != null && listType['listValues'].length > 0) {
                        this.LookupsList = listType['listValues'];
                    } else {
                        this.LookupsList = [];
                    }
                }
            })
        }
    }
    Delete = () => {

    }
    rolePermission: IRolePermissionDisplay;
    LoadTableConfig() {
        this.rowActions = [];
        this.rolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.LOOKUP);
        if (this.rolePermission !== null && this.rolePermission !== undefined) {
            if (this.rolePermission.update === true) {
                this.rowActions.push(
                    {
                        actionMethod: this.editlookup,
                        iconClass: 'pi pi-pencil',
                        isHidden: this.rolePermission.update === true ? false : true
                    })
            }
            if (this.rolePermission.delete === true) {
                this.rowActions.push(
                    {
                        actionMethod: this.Delete,
                        styleClass: 'p-button-raised p-button-danger',
                        iconClass: 'pi pi-trash'
                    })
            }

          //this.rowActions = [
          //   {
          //        actionMethod: this.editlookup,
          //        iconClass: 'pi pi-pencil',
          //        isHidden: this.rolePermission.update === true ? false : true
          //  },
          //  ];
              this.headerActions = [
            {
                actionMethod: this.addLookUp,
                hasIcon: false,
                styleClass: 'btn btn-block btn-sm btn-info',
                actionText: 'Add',
                iconClass: 'fas fa-plus',
                isHidden: this.rolePermission.add === true ? false : true,
                isDisabled: this.LookupId !== 0 ? false : true
            }
        ];
        } else {
            this.rowActions = [];
            this.headerActions = [];
        }
    }

    addLookUp = () => {
        var lookType = this.Lookups.find(x => x.listTypeID == this.LookupId)['type'];
        this.addEditLookupPopup.Show(0, this.LookupId, lookType)
    }
    editlookup = (listValue: any) =>  {
        var lookType = this.Lookups.find(x => x.listTypeID == this.LookupId)['type'];
        this.addEditLookupPopup.Show(listValue.listValueID, this.LookupId, lookType)
    }

    searchText: string;
    searchableList: any[] = ['value', 'Description', 'isActive']

    loadListValues(event) {
        this.LookupId = event;
        this.LoadLookupList(this.LookupId);
    }

    onChangeLookupType(event: any) {
        if (event !== undefined && event.value !== undefined) {
            this.loadListValues(event.value)
        }
    }

}
