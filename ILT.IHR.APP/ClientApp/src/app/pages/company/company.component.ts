import { Component, OnInit, ViewChild } from '@angular/core';
import { ITableRowAction, ITableHeaderAction } from 'src/app/shared/ihr-table/table-options';
import { Constants, SessionConstants } from '../../constant';
import { IRolePermissionDisplay } from '../../core/interfaces/RolePermission';
import { AddEditCompanyComponent } from './add-edit-company/add-edit-company.component';
import { CompanyService } from './company.service';

@Component({
  selector: 'app-company',
  templateUrl: './company.component.html',
  styleUrls: ['./company.component.scss']
})
export class CompanyComponent implements OnInit {
  cols: any[] = [];
  selectedColumns: any[];
  isPagination: boolean = true;
  loading: boolean = false;
  showCurrentPageReport: boolean = true;
  rowActions: ITableRowAction[] = [];
  headerActions: ITableHeaderAction[] = [];
  patientLanding: string = 'patientLanding';
    globalFilterFields = ['Name', 'CompanyType', 'Title', 'InvoicingPeriod', 'PaymentTerm', 'City', 'State', 'ContactName']

    CompanyList: any[] = [];

    colsEndClient: any[] = [];
    selectedEndClientColumns: any[];

  lstCompanys: any[] = [];
  company: string = 'Company';
  DefaultDwnDn1ID: number;
    RolePermissions: IRolePermissionDisplay[] = []
    user: any;
    RoleShort: string = '';
    TypeId: number = 0;

    constructor(private companyService: CompanyService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.user = JSON.parse(localStorage.getItem("User"));
        this.RoleShort = localStorage.getItem("RoleShort");
        this.loadDropdown();
        this.loadTableColumns();
        this.loadTableEndClientColumns()
        this.LoadTableConfig();
    }

    ngOnInit(): void {
        this.LoadTypeList(1);
    }

    loadTableColumns() {
        this.cols = [
            { field: 'name', header: 'Company Name' },
            { field: 'companyType', header: 'Type' },
            { field: 'invoicingPeriod', header: 'Invoicing' },
            { field: 'paymentTerm', header: 'Payment Term' },
            { field: 'city', header: 'City' },
            { field: 'state', header: 'State' },
            { field: 'contactName', header: 'Contact Name' },
            { field: 'contactPhone', header: 'Phone' },
            { field: 'contactEmail', header: 'Email' }
        ];

        this.selectedColumns = this.cols;
    }
    loadTableEndClientColumns() {
        this.colsEndClient = [
            { field: 'name', header: 'Company Name' },
            { field: 'city', header: 'City' },
            { field: 'state', header: 'State' },
            { field: 'country', header: 'Country' }
        ];

        this.selectedEndClientColumns = this.colsEndClient;
    }

  loadDropdown() {
    this.lstCompanys = [{'ID': 1, Value: 'Company'},{'ID': 0, Value: 'End Client'}]
    this.company = 'Company';
    this.DefaultDwnDn1ID = this.lstCompanys.find(x => x.Value.toLowerCase() == this.company.toLowerCase()).ID
    }
    CompanyRolePermission: IRolePermissionDisplay;
    LoadTableConfig() {
        this.CompanyRolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.COMPANY);
        this.rowActions = [];
        if (this.CompanyRolePermission !== null && this.CompanyRolePermission !== undefined) {
            if (this.CompanyRolePermission.update === true) {
                var m1 = {
                    actionMethod: this.editCompany,
                    iconClass: 'pi pi-pencil',
                }
            this.rowActions.push(m1)
            }
            if (this.CompanyRolePermission != null && this.CompanyRolePermission.delete == true) {
                var m2 = {
                    actionMethod: this.delete,
                    styleClass: 'p-button-raised p-button-danger',
                    iconClass: 'pi pi-trash',
                }
                this.rowActions.push(m2);
            }
            if (this.CompanyRolePermission.add === true) {
                this.headerActions = [
                    {
                        actionMethod: this.EndClient,
                        hasIcon: false,
                        styleClass: 'btn btn-block btn-sm btn-info',
                        actionText: 'END CLIENT',
                        iconClass: 'fas fa-plus'

                    },
                    {
                        actionMethod: this.addCompany,
                        hasIcon: false,
                        styleClass: 'btn btn-block btn-sm btn-info',
                        actionText: 'COMPANY',
                        iconClass: 'fas fa-plus'

                    }
                ];
            } else {
                this.headerActions = [];
            }
        }

    }



    EndClientList: any[] = [];
     async  LoadTypeList(ID: number) {
        this.TypeId = ID;
        await this.LoadTableConfig();
         if (this.TypeId == 0) {
             await this.companyService.getEndClientList().subscribe(respRequest => {
                 if (respRequest['data'] !== null && respRequest['messageType'] === 1) {
                     this.EndClientList = respRequest['data'];
                     this.CompanyList = null;
                 }
             })
        }
        else {
            this.companyService.getCompanyList().subscribe(result => {
                if (result['data'] !== null && result['messageType'] === 1) {
                    this.CompanyList = result['data'];
                    this.EndClientList = null;
                }
            })
        }
    }

  @ViewChild('AddEditCompanyModal') AddEditCompanyModal: AddEditCompanyComponent;
    @ViewChild('AddEditEndClientModal') AddEditEndClientModal: AddEditCompanyComponent;

    addCompany = () => {
        this.AddEditCompanyModal.Show(0)
  }
    editCompany = (selected) => {
        if (this.TypeId === 0) {
            this.AddEditEndClientModal.Show(selected.endClientID)
        } else {
            this.AddEditCompanyModal.Show(selected.companyID)
        }
   
  }
  EndClient = () => {
      this.AddEditEndClientModal.Show(0)
  }
  OnCompanyTypeChange(event){
      console.log(event);
      this.DefaultDwnDn1ID = event;
      this.LoadTypeList(event)
  }
    delete = () => {

    }

    searchText: string;
    searchableList: any[] = ['name', 'companyType', 'title', 'invoicingPeriod', 'paymentTerm', 'city', 'state','contactName']
    searchableListEndClients: any[] = ['name', 'city', 'state','contactName']

    OnChangeCompany(event: any) {
        if (event !== undefined && event.value !== undefined) {
            this.OnCompanyTypeChange(event.value)
        }
    }

}
