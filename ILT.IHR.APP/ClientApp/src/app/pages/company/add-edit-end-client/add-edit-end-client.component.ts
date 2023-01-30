import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { forkJoin } from 'rxjs';
import { CommonUtils } from '../../../common/common-utils';
import { Constants, ErrorMsg, ListTypeConstants, SessionConstants } from '../../../constant';
import { ICompanyDisplay } from '../../../core/interfaces/company';
import { IEmployeeDisplay } from '../../../core/interfaces/Employee';
import { IRolePermissionDisplay } from '../../../core/interfaces/RolePermission';
import { IModalPopupAlternateOptions } from '../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { EmployeeService } from '../../employee/employee.service';
import { HolidayService } from '../../holidays/holidays.service';
import { LookUpService } from '../../lookup/lookup.service';
import { CompanyService } from '../company.service';
import { EndClientService } from '../EndClientService';

@Component({
  selector: 'app-add-edit-end-client',
  templateUrl: './add-edit-end-client.component.html',
  styleUrls: ['./add-edit-end-client.component.scss']
})
export class AddEditEndClientComponent implements OnInit {
    commonUtils = new CommonUtils()
    @Output() ListValueUpdated = new EventEmitter<any>();
    @ViewChild('addEditEndClientModal') addEditEndClientModal: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;

    EndClientForm: FormGroup;
    user: any;
    ModalHeading: string = 'Add EndClient';
    submitted: boolean
    RolePermissions: IRolePermissionDisplay[] = [];
    AssetInfoRolePermission: IRolePermissionDisplay;
    isSaveButtonDisabled: boolean = false;
    ClientID: string;
    EmployeeID: number;
    constructor(private fb: FormBuilder,
        private employeeService: EmployeeService,
        private messageService: MessageService,
        private holidayService: HolidayService,
        private companyService: CompanyService,
        private endClientService: EndClientService,
        private lookupService: LookUpService) {
        this.ClientID = localStorage.getItem("ClientID");
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.AssetInfoRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.COMPANY);
        this.user = JSON.parse(localStorage.getItem("User"));
        this.LoadDropDown();
        this.buildEndClientForm({}, 'New');
    }

    ngOnInit(): void {
        this.loadModalOptions();
    }
    loadModalOptions() {
        this.modalOptions = {
            footerActions: [
                {
                    actionText: 'Save',
                    actionMethod: this.checkNameExist,
                    styleClass: 'btn-width-height p-button-raised p-mr-2 p-mb-2',
                    iconClass: 'mdi mdi-content-save',
                    disabled: this.isSaveButtonDisabled,
                },
                {
                    actionText: 'Cancel',
                    actionMethod: this.Cancel,
                    styleClass: 'btn-width-height p-button-raised p-button-danger  p-mb-2',
                    iconClass: 'p-button-raised p-button-danger'
                }
            ]
        }
    }

    EndClientId: number = 0;
    ErrorMessage: string = '';
   Show(Id: number) {
        this.EndClientId = Id;
       this.ResetDialog();
       this.loadModalOptions();
        if (this.EndClientId != 0) {
            this.GetDetails(this.EndClientId);
        }
        else {
            this.ModalHeading = "Add End Client";
            this.ErrorMessage = "";
            this.buildEndClientForm({}, 'New')
            this.addEditEndClientModal.show();
         
        }
   }


    Employees: IEmployeeDisplay[] = []
    CountryList: any[] = []
    PaymentTermList: any[] = []
    CompanyTypeList: any[] = []
    InvoicingPeriodList: any[] = []
    LoadDropDown() {
        forkJoin(
            this.lookupService.getListValues(),
            this.holidayService.getCountry()
        ).subscribe(resultSet => {
            if (resultSet !== undefined && resultSet !== null) {
                //this.Employees = lisEmp;
                this.CompanyTypeList = resultSet[0]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.COMPANYTYPE);
                this.CountryList = resultSet[1]['data']
            }
        })
    }
    EndClientList: any[] = [];
    LoadList() {
        this.endClientService.GetEndClients().subscribe(result => {
            if (result['messageType'] !== null && result['messageType'] === 1) {
                this.EndClientList = result['data'];
            }
        })
    }

    endclient: any;
    EndClients: any[] = [];
    async GetDetails(Id: number) {
        this.EndClients = [];
        this.endClientService.GetEndClientByIdAsync(Id).subscribe(async result => {
            if (result !== null && result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                this.endclient = result['data'];
                if (this.endclient.country != null) {
                    await this.GetStates(this.endclient.country);
                }
                else {
                    this.StateList = [];
                }
             
                this.ModalHeading = "Edit End Client";
                this.buildEndClientForm(this.endclient, 'Edit');
                this.addEditEndClientModal.show();


            }
        })
    }

    StateList: any[] = []
    countryId: number;
    onChangeCountry(e: any) {
        var country = e.value.toString();
        if (country !== null && country !== "") {
            this.GetStates(country);
        }
        else {
            this.StateList = [];
        }
    }
    GetStates(country: string) {
        if (this.CountryList != null && this.CountryList.find(x => x.countryDesc.toUpperCase() === country.toUpperCase()) !== undefined) {
            this.countryId = this.CountryList.find(x => x.countryDesc.toUpperCase() === country.toUpperCase()).countryID;
            if (this.countryId != 0 && this.countryId != null) {
                this.holidayService.GetCountryByIdAsync(this.countryId).subscribe(result => {
                    if (result !== null && result['data'] !== null && result['data'] !== undefined) {
                        this.StateList = result['data'].states;
                    }
                })
            }
        }
    }

    buildEndClientForm(data: any, keyName: string) {
        this.EndClientForm = this.fb.group({
            EndClientID: [keyName === 'New' ? 0 : data.endClientID],
            CompanyID: [keyName === 'New' ? 0 : data.companyID],
            Name: [keyName === 'New' ? '' : data.name, Validators.required],
            Address1: [keyName === 'New' ? null : data.address1, Validators.required],
            Address2: [keyName === 'New' ? '' : data.address2],
            City: [keyName === 'New' ? '' : data.city, Validators.required],
            State: [keyName === 'New' ? '' : data.state, Validators.required],
            Country: [keyName === 'New' ? '' : data.country, Validators.required],
            ZipCode: [keyName === 'New' ? '' : data.zipCode, Validators.required],
            TaxID: [keyName === 'New' ? '' : data.taxID],
            CreatedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : this.user.firstName + " " + this.user.lastName],
            CreatedDate: [keyName === 'New' ? new Date() : new Date(data.createdDate)],
            ModifiedDate: [keyName === 'New' ? new Date() : data.modifiedDate],
            ModifiedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : data.modifiedBy],
            TimeStamp: ['']
        });
        //if (keyName === 'New' && this.AssetStatus2List !== null && this.AssetStatus2List.length > 0) {
        //    this.AssetStatusList = this.AssetStatus2List.filter(x => x.value.toUpperCase() !== AssetStatusConstants.ASSIGNED);
        //    var status = this.AssetStatusList.find(x => x.valueDesc.toUpperCase() === AssetStatusConstants.UNASSIGNED);

        //    if (status !== undefined && status.listValueID !== null && status.listValueID !== undefined) {
        //        this.AssetForm.controls.StatusID.patchValue(status.listValueID)
        //    }
        //}
    }

    get addEditCompanyControls() { return this.EndClientForm.controls; }

    checkNameExist = () => {
        this.ErrorMessage = '';
        this.submitted = true;
        if (this.EndClientForm.invalid) {
            this.EndClientForm.markAllAsTouched();
            return;
        } else {
            if (this.isSaveButtonDisabled) {
                return;
            } else {
                this.isSaveButtonDisabled = true;
                this.ErrorMessage = "";
                if (this.EndClientId != 0) {
                    if (this.EndClientForm.value.Name !== '' && this.EndClientForm.value.Name !== null) {
                        if ((this.EndClientList.filter(x => x.endClientID != this.EndClientForm.value.EndClientID && x.name.toUpperCase() === this.EndClientForm.value.Name.toUpperCase()).length > 0)) {
                            this.ErrorMessage = "Company Name already exists in the system";
                        }
                        else {
                            this.SaveCompany();
                        }
                    }
                }
                else {
                    if (this.EndClientList.filter(x => x.Name.toUpperCase() == this.EndClientForm.value.Name.toUpperCase()).length > 0) {
                        this.ErrorMessage = "Company Name already exists in the system";
                    }
                    else {
                        this.SaveCompany();
                    }
                }
            }
        }
    }

    SaveCompany() {
        if (this.EndClientId === 0) {
            console.log(this.EndClientForm.value)
            this.endClientService.SaveEndClient(this.EndClientForm.value).subscribe(result => {
                if (result['data'] !== null && result['messageType'] === 1) {
                    this.messageService.add({ severity: 'success', summary: 'End Client saved successfully', detail: '' });
                    this.isSaveButtonDisabled = true;
                    this.loadModalOptions();
                    this.ListValueUpdated.emit(true);
                    this.Cancel();
                } else {
                    this.isSaveButtonDisabled = false;
                    this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                }
            })
        } else {
            this.endClientService.UpdateEndClient(this.EndClientForm.value.EndClientID, this.EndClientForm.value).subscribe(result => {
                if (result['data'] !== null && result['messageType'] === 1) {
                    this.messageService.add({ severity: 'success', summary: 'End Client saved successfully', detail: '' });
                    this.isSaveButtonDisabled = true;
                    this.loadModalOptions();
                    this.ListValueUpdated.emit();
                    this.Cancel();
                } else {
                    this.isSaveButtonDisabled = false;
                    this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                }
            })
        }
    }

    Cancel = () => {
        this.EndClientId = -1
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildEndClientForm({}, 'New');
        this.addEditEndClientModal.hide();
    }
    ResetDialog() {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildEndClientForm({}, 'New');
        this.addEditEndClientModal.hide();
    }

}
