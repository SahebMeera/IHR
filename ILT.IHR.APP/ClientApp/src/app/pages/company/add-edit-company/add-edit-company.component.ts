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
import { AssetService } from '../../asset/asset.service';
import { EmployeeService } from '../../employee/employee.service';
import { HolidayService } from '../../holidays/holidays.service';
import { LookUpService } from '../../lookup/lookup.service';
import { CompanyService } from '../company.service';
import { EndClientService } from '../EndClientService';

@Component({
  selector: 'app-add-edit-company',
  templateUrl: './add-edit-company.component.html',
  styleUrls: ['./add-edit-company.component.scss']
})
export class AddEditCompanyComponent implements OnInit {
    commonUtils = new CommonUtils()
    @Output() ListValueUpdated = new EventEmitter<any>();
    @ViewChild('addEditCompanyModal') addEditCompanyModal: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;

    CompanyForm: FormGroup;
    user: any;
    ModalHeading: string = 'Add Asset';
    submitted: boolean
    RolePermissions: IRolePermissionDisplay[] = [];
    AssetInfoRolePermission: IRolePermissionDisplay;
    isSaveButtonDisabled: boolean = false;
    ClientID: string;
    EmployeeID: number;
    isAssignedTemp: boolean = false;
    disabledvalue: boolean = false;
    constructor(private fb: FormBuilder,
        private employeeService: EmployeeService,
        private messageService: MessageService,
        private holidayService: HolidayService,
        private assetService: AssetService,
        private companyService: CompanyService,
        private endClientService: EndClientService,
        private lookupService: LookUpService) {
        this.ClientID = localStorage.getItem("ClientID");
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.AssetInfoRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.COMPANY);
        this.user = JSON.parse(localStorage.getItem("User"));
        this.LoadDropDown();
        this.buildCompanyForm({}, 'New');
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

    Employees: IEmployeeDisplay[] = []
    CountryList: any[] = []
    PaymentTermList: any[] = []
    CompanyTypeList: any[] = []
    InvoicingPeriodList: any[] = []
    LoadDropDown() {
        forkJoin(
            this.employeeService.GetEmployees(),
            this.lookupService.getListValues(),
            this.holidayService.getCountry()
        ).subscribe(resultSet => {
            if (resultSet !== undefined && resultSet !== null) {
                this.Employees = resultSet[0]['data'].filter(x => x.termDate === null);
                //this.Employees = lisEmp;
                this.PaymentTermList = resultSet[1]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.PAYMENTTERM);
                this.CompanyTypeList = resultSet[1]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.COMPANYTYPE);
                this.InvoicingPeriodList = resultSet[1]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.INVOICINGPERIOD);
                this.CountryList = resultSet[2]['data']
            }
        })
    }
    CompanyList: any[] = [];
    LoadList() {
        this.companyService.getCompanyList().subscribe(result => {
            if (result['messageType'] !== null && result['messageType'] === 1) {
                this.CompanyList = result['data'];
            }
        })
    }
    CompanyId: number = 0;
    isEndClientdisable: boolean = false;
    ErrorMessage: string = '';
    Show(Id: number) {
        this.CompanyId = Id;
        //ResetDialog();
        this.isEndClientdisable = false;
        if (this.CompanyId != 0) {
            this.GetDetails(this.CompanyId);
        }
        else {
            this.ModalHeading = 'Add Company';
            this.ErrorMessage = "";
            this.buildCompanyForm({}, 'New');
            this.addEditCompanyModal.show()
        }
    }
    company: ICompanyDisplay;
    EndClients: any[] = [];
    async GetDetails(Id: number) {
        this.EndClients = [];
        this.companyService.GetCompanyByIdAsync(Id).subscribe(async result => {
            if (result !== null && result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                this.company = result['data'];
                if (this.company.country != null) {
                    await this.GetStates(this.company.country);
                }
                else {
                    this.StateList = [];
                }
                if (this.CompanyTypeList.find(x => x.listValueID == this.company.companyTypeID).value == "CLIENT") {
                    this.isEndClientdisable = false;
                }
                else {
                    this.isEndClientdisable = true;
                }

                if (this.company.isEndClient) {
                    this.endClientService.GetEndClients().subscribe(reponses => {
                        if (reponses !== null && reponses !== undefined && reponses['data'] !== null && reponses['data'] !== undefined) {
                            this.EndClients = reponses['data'];
                            this.endclient = this.EndClients.find(ec => ec.companyID === this.company.companyID);
                           // console.log('this.endclient', this.endclient)
                            if (this.endclient == null) {
                                this.endclient = null
                            }
                        }
                        this.isEndClientdisable = true;
                    })
                }
                this.ModalHeading = "Edit Company";
                this.buildCompanyForm(this.company, 'Edit');
                this.addEditCompanyModal.show();


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

    buildCompanyForm(data: any, keyName: string) {
        this.CompanyForm = this.fb.group({
            CompanyID: [keyName === 'New' ? 0 : data.companyID],
            CompanyTypeID: [keyName === 'New' ? null : data.companyTypeID, Validators.required],
            CompanyType: [keyName === 'New' ? '' : data.companyType],
            Name: [keyName === 'New' ? '' : data.name, Validators.required],
            Address1: [keyName === 'New' ? null : data.address1, Validators.required],
            Address2: [keyName === 'New' ? '' : data.address2],
            City: [keyName === 'New' ? '' : data.city, Validators.required],
            State: [keyName === 'New' ? '' : data.state, Validators.required],
            Country: [keyName === 'New' ? '' : data.country, Validators.required],
            ZipCode: [keyName === 'New' ? '' : data.zipCode, Validators.required],
            ContactName: [keyName === 'New' ? '' : data.contactName, Validators.required],
            ContactEmail: [keyName === 'New' ? '' : data.contactEmail, Validators.required],
            ContactPhone: [keyName === 'New' ? '' : data.contactPhone, Validators.required],
            AlternateContactName: [keyName === 'New' ? '' : data.alternateContactName],
            AlternateContactEmail: [keyName === 'New' ? '' : data.alternateContactEmail],
            AlternateContactPhone: [keyName === 'New' ? '' : data.alternateContactPhone],
            InvoiceContactName: [keyName === 'New' ? '' : data.invoiceContactName, Validators.required],
            InvoiceContactEmail: [keyName === 'New' ? '' : data.invoiceContactEmail, Validators.required],
            InvoiceContactPhone: [keyName === 'New' ? '' : data.invoiceContactPhone, Validators.required],
            AlternateInvoiceContactName: [keyName === 'New' ? '' : data.alternateInvoiceContactName],
            AlternateInvoiceContactEmail: [keyName === 'New' ? '' : data.alternateInvoiceContactEmail],
            AlternateInvoiceContactPhone: [keyName === 'New' ? '' : data.alternateInvoiceContactPhone],
            InvoicingPeriodID: [keyName === 'New' ? null : data.invoicingPeriodID, Validators.required],
            InvoicingPeriod: [keyName === 'New' ? '' : data.invoicingPeriod],
            PaymentTermID: [keyName === 'New' ? null : data.paymentTermID, Validators.required],
            PaymentTerm: [keyName === 'New' ? '' : data.paymentTerm],
            TaxID: [keyName === 'New' ? '' : data.taxID, Validators.required],
            IsEndClient: [keyName === 'New' ? false : data.isEndClient],
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

    get addEditCompanyControls() { return this.CompanyForm.controls; }

    onChangeCompanyType(e: any) {
        if (e !== undefined && e.value !== null && e.value !== undefined) {
            if (this.CompanyTypeList.find(x => x.listValueID == Number(e.value)).value === "CLIENT") {
                this.CompanyForm.controls.CompanyType.patchValue(this.CompanyTypeList.find(x => x.listValueID == Number(e.value)).valueDesc);
                this.isEndClientdisable = false;
            }
            else {
                this.CompanyForm.controls.CompanyType.patchValue(this.CompanyTypeList.find(x => x.listValueID  == Number(e.value)).valueDesc);
                this.isEndClientdisable = true;
            }
        }
        else {
            this.isEndClientdisable = true;
        }
    }

    onChangePaymentTerm(e: any) {
        if (Number(e.value) != 0 && e.value != null && this.PaymentTermList != null && this.PaymentTermList.length > 0) {
            var paymentTerm = Number(e.value);
            this.CompanyForm.controls.PaymentTerm.patchValue(this.PaymentTermList.find(x => x.listValueID == paymentTerm).valueDesc);
        }
    }
    onChangeInvoicingPeriod(e: any) {
        if (Number(e.value) != 0 && e.value != null && this.InvoicingPeriodList != null && this.InvoicingPeriodList.length > 0) {
            var invoicingPeriod = Number(e.value);
            this.CompanyForm.controls.InvoicingPeriod.patchValue(this.InvoicingPeriodList.find(x => x.listValueID == invoicingPeriod).valueDesc);
        }
    }

    checkNameExist = () => {
        this.ErrorMessage = '';
        this.submitted = true;
        if (this.CompanyForm.invalid) {
            this.CompanyForm.markAllAsTouched();
            return;
        } else {
            if (this.isSaveButtonDisabled) {
                return;
            } else {
                this.isSaveButtonDisabled = true;
                this.ErrorMessage = "";
                if (this.CompanyId != 0) {
                    if (this.CompanyForm.value.Name !== '' && this.CompanyForm.value.Name !== null) {
                        if ((this.CompanyList.filter(x => x.companyID != this.CompanyForm.value.CompanyID && x.name.toUpperCase() === this.CompanyForm.value.Name.toUpperCase()).length > 0)) {
                            this.ErrorMessage = "Company Name already exists in the system";
                        }
                        else {
                            this.SaveCompany();
                        }
                    }
                }
                else {
                    if (this.CompanyList.filter(x => x.Name.toUpperCase() == this.CompanyForm.value.Name.toUpperCase()).length> 0) {
                        this.ErrorMessage = "Company Name already exists in the system";
                    }
                    else {
                        this.SaveCompany();
                    }
                }
            }
        }
    }
    endclient: any;
    SaveCompany() {
        if (this.CompanyTypeList.find(x => x.listValueID == this.CompanyForm.value.CompanyTypeID).value !== "CLIENT") {
            this.CompanyForm.value.IsEndClient = false;
        }

        if (this.CompanyId == 0) {
            //company.CreatedBy = "Admin";
            // var abc = Newtonsoft.Json.JsonConvert.SerializeObject(company); 
            this.companyService.SaveCompany(this.CompanyForm.value).subscribe(async result => {
                if (result['data'] !== null && result['messageType'] === 1) {
                    if (this.CompanyForm.value.IsEndClient) {
                        this.endclient = {};
                        this.endclient.CompanyID = result['data'].companyID;
                        this.endclient.Name = this.CompanyForm.value.Name;
                        this.endclient.TaxID = this.CompanyForm.value.TaxID;
                        this.endclient.Address1 = this.CompanyForm.value.Address1;
                        this.endclient.Address2 = this.CompanyForm.value.Address2;
                        this.endclient.City = this.CompanyForm.value.City;
                        this.endclient.State = this.CompanyForm.value.State;
                        this.endclient.Country = this.CompanyForm.value.Country;
                        this.endclient.ZipCode = this.CompanyForm.value.ZipCode;
                        this.CompanyForm.value.CreatedBy = this.CompanyForm.value.FirstName + " " + this.CompanyForm.value.LastName;
                        await this.endClientService.SaveEndClient(this.endclient).subscribe(resultEndClient => {
                            if (resultEndClient['data'] !== null && resultEndClient['data'] === 1) {
                               this.messageService.add({ severity: 'success', summary: 'Company saved successfully', detail: '' });
                               this.ListValueUpdated.emit();
                               this.Cancel();
                           }
                           else {
                                this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                            }
                        })
                    }
                else {
                            this.messageService.add({ severity: 'success', summary: 'Company saved successfully', detail: '' });
                            this.ListValueUpdated.emit();
                            this.Cancel();
                        }
                    }
                    else {
                        this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                    }
            })
        }
        else if (this.CompanyId != 0) {
            this.CompanyForm.value.CreatedBy = "Admin";
            // company.ModifiedBy = user.FirstName + " " + user.LastName;
            this.companyService.UpdateCompany(this.CompanyId, this.CompanyForm.value).subscribe(async result => {
                if (result['data'] !== null && result['messageType'] === 1) {
                    if (this.endclient !== undefined && this.endclient !== null && this.endclient.endClientID !== 0) {
                        const EndClientID = this.endclient.endClientID;
                            this.endclient = {};
                            this.endclient.CompanyID = result['data'].companyID;
                           this.endclient.EndClientID = EndClientID;
                            this.endclient.Name = this.CompanyForm.value.Name;
                            this.endclient.TaxID = this.CompanyForm.value.TaxID;
                            this.endclient.Address1 = this.CompanyForm.value.Address1;
                            this.endclient.Address2 = this.CompanyForm.value.Address2;
                            this.endclient.City = this.CompanyForm.value.City;
                            this.endclient.State = this.CompanyForm.value.State;
                            this.endclient.Country = this.CompanyForm.value.Country;
                            this.endclient.ZipCode = this.CompanyForm.value.ZipCode;
                            this.endclient.ModifiedBy = this.user.FirstName + " " + this.user.LastName;
                        await this.endClientService.UpdateEndClient(this.endclient.EndClientID, this.endclient).subscribe(resultEndClient => {
                            if (resultEndClient['data'] !== null && resultEndClient['messageType'] === 1) {
                                    this.messageService.add({ severity: 'success', summary: 'Company saved successfully', detail: '' });
                                    this.ListValueUpdated.emit();
                                    this.Cancel();
                                }
                                else {
                                    this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                                }
                            })
                        }
                    else if (this.CompanyForm.value.IsEndClient) {
                            this.endclient = {};
                            this.endclient.CompanyID = result['data'].companyID;
                            this.endclient.Name = this.CompanyForm.value.Name;
                            this.endclient.TaxID = this.CompanyForm.value.TaxID;
                            this.endclient.Address1 = this.CompanyForm.value.Address1;
                            this.endclient.Address2 = this.CompanyForm.value.Address2;
                            this.endclient.City = this.CompanyForm.value.City;
                            this.endclient.State = this.CompanyForm.value.State;
                            this.endclient.Country = this.CompanyForm.value.Country;
                            this.endclient.ZipCode = this.CompanyForm.value.ZipCode;
                            this.endclient.CreatedBy = this.user.FirstName + " " + this.user.LastName;
                            this.endClientService.SaveEndClient(this.endclient).subscribe(resp => {
                                if (resp['data'] !== null && resp['messageType'] === 1) {
                                    this.messageService.add({ severity: 'success', summary: 'Company saved successfully', detail: '' });
                                    this.ListValueUpdated.emit();
                                    this.Cancel();
                                }
                                else {
                                    this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                                    //toastService.ShowError(ErrorMsg.ERRORMSG);
                                }
                            })
                        }
                        else {
                            this.messageService.add({ severity: 'success', summary: 'Company saved successfully', detail: '' });
                            this.ListValueUpdated.emit(true);
                            this.Cancel();
                        }
                }
                else {
                    this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                    //toastService.ShowError(ErrorMsg.ERRORMSG);
                }
            })
        }
        this.isSaveButtonDisabled = false;
    }

    Cancel = () => {
        this.CompanyId = -1
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildCompanyForm({}, 'New');
        this.addEditCompanyModal.hide();
    }
    ResetDialog() {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildCompanyForm({}, 'New');
        this.addEditCompanyModal.hide();
    }


}
