import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import * as moment from 'moment';
import { MessageService } from 'primeng/api';
import { forkJoin } from 'rxjs';
import { CommonUtils } from '../../../../common/common-utils';
import { Constants, EmployeeAddresses, ErrorMsg, ListTypeConstants, SessionConstants } from '../../../../constant';
import { IEmployeeAddress, IEmployeeAddressDisplay } from '../../../../core/interfaces/EmployeeAddress';
import { IRolePermissionDisplay } from '../../../../core/interfaces/RolePermission';
import { IModalPopupAlternateOptions } from '../../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { HolidayService } from '../../../holidays/holidays.service';
import { LookUpService } from '../../../lookup/lookup.service';
import { EmployeeService } from '../../employee.service';
import { FormI9Service } from '../formI9.service';
import { I9DocumentService } from '../I9DocumentService';

@Component({
  selector: 'app-add-edit-formi9',
  templateUrl: './add-edit-formi9.component.html',
  styleUrls: ['./add-edit-formi9.component.scss']
})
export class AddEditFormi9Component implements OnInit {
    commonUtils = new CommonUtils()
    @Input() EmployeeName: string;
    @Output() ListValueUpdated = new EventEmitter<any>();
    @ViewChild('addEditEmployeeI9FormModal') addEditEmployeeI9FormModal: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;
    isSaveButtonDisabled: boolean = false;
    ModalHeading: string = 'Add Form I9';
    isShow: boolean;
    isViewPermissionForNPIRole: boolean = false;
    NPIPermission: IRolePermissionDisplay;


    withHoldingStausList: any[] = [];
    W4TypeList: any[] = [];
    CountryList: any[] = [];
    user: any;
    submitted: boolean = false;
    StateList: any[] = [];
    FormI9ID: number;

    FormI9: FormGroup;
    RolePermissions: IRolePermissionDisplay[] = [];
    EmployeeInfoRolePermission: IRolePermissionDisplay;


    constructor(private fb: FormBuilder,
        private LookupService: LookUpService,
        private messageService: MessageService,
        private formI9Service: FormI9Service,
        private holidayService: HolidayService,
        private i9DocumentService: I9DocumentService,
        private employeeService: EmployeeService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.EmployeeInfoRolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.I9INFO);
        this.NPIPermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.NPI);

        this.user = JSON.parse(localStorage.getItem("User"));
        this.LoadDropDown();
        this.buildEmployeeI9Form({}, 'New');
    }

    ngOnInit(): void {
        this.loadModalOptions();
    }


    loadModalOptions() {
        this.modalOptions = {
            footerActions: [
                {
                    actionText: 'Save',
                    actionMethod: this.save,
                    styleClass: this.isShow ? 'btn-width-height p-button-raised p-mr-2 p-mb-2' : 'btn-width-height p-button-raised p-mr-2 p-mb-2 display-none',
                    iconClass: 'mdi mdi-content-save',
                    disabled: this.isSaveButtonDisabled || !this.isShow
                },
                {
                    actionText: 'Cancel',
                    actionMethod: this.cancel,
                    styleClass: 'btn-width-height p-button-raised p-button-danger  p-mb-2',
                    iconClass: 'p-button-raised p-button-danger'
                }
            ]
        }
    }
    Show(Id: number, employeeId: number) {
        this.FormI9ID = Id;
        //this.ResetDialog();
        if (this.FormI9ID !== 0) {
            this.isShow = this.EmployeeInfoRolePermission.update;
            this.loadModalOptions();
            this.GetDetails(this.FormI9ID, employeeId);
        }
        else {
            this.isShow = this.EmployeeInfoRolePermission.add;
            this.loadModalOptions();
            this.ModalHeading = "Add FormI9";
            this.buildEmployeeI9Form({}, 'New');
            this.LoadDropDown();
            this.loadEmployeeData(employeeId);
            this.addEditEmployeeI9FormModal.show();
        }
    }
    WorkAuthorization: string = '';
    GetDetails(Id: number, employeeId: number) {
        this.isViewPermissionForNPIRole = false;
        this.formI9Service.getFormI9ByIdAsync(Id).subscribe(result => {
            if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                this.ModalHeading = "Edit FormI9";
                this.CountryName = "";
                this.StateName = '';
                this.isViewPermissionForNPIRole = this.NPIPermission.view;
                this.buildEmployeeI9Form(result['data'], 'Edit');
                if (result['data'].Country != null) {
                    this.CountryName = result['data'].Country;
                    this.StateName = result['data'].state;
                    this.WorkAuthorization = result['data'].WorkAuthorization;
                    this.GetStates(result['data'].Country);
                }
                else {
                    this.StateList = [];
                }
                this.FormI9ID = result['data'].formI9ID;
                this.EmployeeName = result['data'].employeeName;
                this.LoadI9Documents(result['data'].workAuthorizationID);
                this.EmployeeID = employeeId;
                this.LoadList();
                this.addEditEmployeeI9FormModal.show();
            }
        })
    }

    formI9List: any[] = [];
    lstFormI9List: any[] = [];
    EmployeeID: number;
    LoadList() {
        if (this.EmployeeID !== undefined && this.EmployeeID !== 0) {
            this.formI9Service.getFormI9s(this.EmployeeID).subscribe(result => {
                if (result !== undefined && result['data'] !== null && result['data'] !== undefined && result['data'].length > 0) {
                    result['data'].forEach((d) => {
                        if (d.i94ExpiryDate !== null) {
                            d.i94ExpiryDate = moment(d.i94ExpiryDate).format("MM/DD/YYYY")
                        }
                        if (d.listAExpirationDate !== null) {
                            d.listAExpirationDate = moment(d.listAExpirationDate).format("MM/DD/YYYY")
                        }
                        if (d.listBExpirationDate !== null) {
                            d.listBExpirationDate = moment(d.listBExpirationDate).format("MM/DD/YYYY")
                        }
                        if (d.listCExpirationDate !== null) {
                            d.listCExpirationDate = moment(d.listCExpirationDate).format("MM/DD/YYYY")
                        }
                    })
                    
                    this.formI9List = result['data'];
                    this.lstFormI9List = this.formI9List;
                    var index = this.lstFormI9List.findIndex(x => x.formI9ID == this.FormI9ID);
                    if (this.lstFormI9List.length > 1 && index == this.lstFormI9List.length - 1) {
                        this.isShow = true;
                        this.loadModalOptions();
                    }
                    else {
                        this.isShow = false;
                        this.loadModalOptions();
                    }
                }
            }, error => {
                //toastService.ShowError();
            })
        }
    }
    AddressTypeList: any[] = [];
    WorkAuthorizationList: any[] = [];
    ListValues: any[] = [];
    I9DocumentsList: any[] = [];
    LoadDropDown() {
        forkJoin(
            this.LookupService.getListValues(),
            this.holidayService.getCountry(),
            this.i9DocumentService.GetI9Documents()
        ).subscribe(resultSet => {
            if (resultSet !== undefined && resultSet !== null) {
                this.AddressTypeList = resultSet[0]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.ADDRESSTYPE);
                this.WorkAuthorizationList = resultSet[0]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.WORKAUTHORIZATION);
                this.ListValues = resultSet[0]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.I9DOCUMENTTYPE);
                this.CountryList = resultSet[1]['data'];
                this.I9DocumentsList = resultSet[2]['data'];
            }
        })
    }



    buildEmployeeI9Form(data: any, keyName: string) {
        this.FormI9 = this.fb.group({
            FormI9ID: [keyName === 'New' ? 0 : data.formI9ID],
            EmployeeID: [keyName === 'New' ? null : data.employeeID],
            FirstName: [keyName === 'New' ? '' : data.firstName],
            MiddleName: [keyName === 'New' ? '' : data.middleName],
            LastName: [keyName === 'New' ? '' : data.lastName],
            SSN: [keyName === 'New' ? '' : data.ssn,],
            Phone: [keyName === 'New' ? null : data.phone],
            EmployeeName: [keyName === 'New' ? '' : data.employeeName],
            StartDate: [keyName === 'New' ? null : data.startDate !== null ? new Date(data.startDate) : null],
            EndDate: [keyName === 'New' ? null : data.endDate !== null ? new Date(data.endDate) : null],
            Address1: [keyName === 'New' ? '' : data.address1, Validators.required],
            Address2: [keyName === 'New' ? '' : data.address2],
            City: [keyName === 'New' ? '' : data.city],
            State: [keyName === 'New' ? '' : data.state],
            StateName: [keyName === 'New' ? '' : data.state],
            Country: [keyName === 'New' ? '' : data.country],
            CountryName: [keyName === 'New' ? '' : data.country],
            ZipCode: [keyName === 'New' ? '' : data.zipCode],
            BirthDate: [keyName === 'New' ? null : data.birthDate !== null ? new Date(data.birthDate) : null],
            Email: [keyName === 'New' ? '' : data.email],
            WorkAuthorizationID: [keyName === 'New' ? null : data.workAuthorizationID],
            WorkAuthorization: [keyName === 'New' ? '' : data.workAuthorization],
            ANumber: [keyName === 'New' ? null : data.aNumber],
            USCISNumber: [keyName === 'New' ? null : data.uscisNumber],
            I94Number: [keyName === 'New' ? '' : data.i94Number],
            I94ExpiryDate: [keyName === 'New' ? null : data.i94ExpiryDate !== null ? new Date(data.i94ExpiryDate) : null],
            PassportNumber: [keyName === 'New' ? null : data.passportNumber],
            PassportCountry: [keyName === 'New' ? null : data.passportCountry],
            HireDate: [keyName === 'New' ? null : data.hireDate !== null ? new Date(data.hireDate) : null],
            ListADocumentTitleID: [keyName === 'New' ? null : data.listADocumentTitleID],
            ListADocumentTitle: [keyName === 'New' ? '' : data.listADocumentTitle],
            ListAIssuingAuthority: [keyName === 'New' ? '' : data.listAIssuingAuthority],
            ListADocumentNumber: [keyName === 'New' ? '' : data.listADocumentNumber],
            ListAStartDate: [keyName === 'New' ? null : data.listAStartDate !== null ? new Date(data.listAStartDate) : null],
            ListAExpirationDate: [keyName === 'New' ? null : data.listAExpirationDate !== null ? new Date(data.listAExpirationDate) : null],
            ListBDocumentTitleID: [keyName === 'New' ? null : data.listBDocumentTitleID],
            ListBDocumentTitle: [keyName === 'New' ? '' : data.listBDocumentTitle],
            ListBIssuingAuthority: [keyName === 'New' ? '' : data.listBIssuingAuthority],
            ListBDocumentNumber: [keyName === 'New' ? '' : data.listBDocumentNumber],
            ListBStartDate: [keyName === 'New' ? null : data.listBStartDate !== null ? new Date(data.listBStartDate) : null],
            ListBExpirationDate: [keyName === 'New' ? null : data.listBExpirationDate !== null ? new Date(data.listBExpirationDate) : null],
            ListCDocumentTitleID: [keyName === 'New' ? null : data.listCDocumentTitleID],
            ListCDocumentTitle: [keyName === 'New' ? '' : data.listCDocumentTitle],
            ListCIssuingAuthority: [keyName === 'New' ? '' : data.listCIssuingAuthority],
            ListCDocumentNumber: [keyName === 'New' ? '' : data.listCDocumentNumber],
            ListCStartDate: [keyName === 'New' ? null : data.listCStartDate !== null ? new Date(data.listCStartDate) : null],
            ListCExpirationDate: [keyName === 'New' ? null : data.listCExpirationDate !== null ? new Date(data.listCExpirationDate) : null],
            IsDeleted: false,
            InputDate: null,
            CreatedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : this.user.firstName + " " + this.user.lastName],
            CreatedDate: [keyName === 'New' ? new Date() : data.createdDate],
            ModifiedDate: [keyName === 'New' ? new Date() : data.modifiedDate],
            ModifiedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : data.modifiedBy],
            TimeStamp: ['']
        });
    }
    get addEditEmployeeFormI9Controls() { return this.FormI9.controls; }

    ssnRequired: boolean = false;
    StateName: string = '';
    CountryName: string = '';
    loadEmployeeData(employeeID: number) {
        var currentAddress: IEmployeeAddressDisplay
        this.CountryName = '';
        this.StateName =  '';
        this.employeeService.getEmployeeByIdAsync(employeeID).subscribe(result => {
            if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                if (result['data'].employeeID !== 0) {
                    var Employee = result['data'];
                    this.FormI9.controls.EmployeeID.patchValue(Employee.employeeID);
                    this.FormI9.controls.FirstName.patchValue(Employee.firstName);
                    this.FormI9.controls.LastName.patchValue(Employee.lastName);
                    this.FormI9.controls.EmployeeName.patchValue(Employee.employeeName);
                    this.FormI9.controls.MiddleName.patchValue(Employee.middleName);
                    this.FormI9.controls.BirthDate.patchValue(new Date(Employee.birthDate));
                    this.FormI9.controls.SSN.patchValue(Employee.ssn);
                    this.FormI9.controls.WorkAuthorizationID.patchValue(Employee.workAuthorizationID);
                    this.FormI9.controls.WorkAuthorization.patchValue(Employee.workAuthorization);
                    this.FormI9.controls.HireDate.patchValue(new Date(Employee.hireDate));
                    this.FormI9.controls.Phone.patchValue(Employee.phone);
                    this.FormI9.controls.Email.patchValue(Employee.email);
                    if (this.AddressTypeList != null) {
                        var currentAddressTypeID = this.AddressTypeList.find(x => x.value === EmployeeAddresses.CURRADD).listValueID;
                        var currentAdd = Employee.employeeAddresses.find(x => x.addressTypeID === currentAddressTypeID);
                        if (currentAdd != null) {
                            currentAddress = currentAdd;
                            this.FormI9.controls.Address1.patchValue(currentAddress.address1);
                            this.FormI9.controls.Address2.patchValue(currentAddress.address2);
                            this.FormI9.controls.City.patchValue(currentAddress.city);
                            this.FormI9.controls.Country.patchValue(currentAddress.country);
                            this.FormI9.controls.ZipCode.patchValue(currentAddress.zipCode);
                            if (this.FormI9.value.Country !== null && this.FormI9.value.Country !== '') {
                                this.CountryName = currentAddress.country;
                                this.FormI9.controls.CountryName.patchValue(this.CountryName);
                                this.GetStates(this.FormI9.value.Country);
                                this.FormI9.controls.State.patchValue(currentAddress.state);
                               // this.FormI9.value.State = currentAddress.state;
                                this.StateName = currentAddress.state;
                                this.FormI9.controls.StateName.patchValue(this.StateName);
                            }
                            else {
                                this.StateList = [];
                            }
                        }
                        this.LoadI9Documents(this.FormI9.value.WorkAuthorizationID);
                        this.ErrorMessage = "";
                        if (this.FormI9.value.SSN === '' || this.FormI9.value.SSN === null) {
                            this.ErrorMessage = "SSN Number is required. Please update SSN from Employee tab";
                            this.ssnRequired = true;
                            this.isSaveButtonDisabled = true;
                        } else {
                            this.ssnRequired = false;
                        }
                    }
                }
            }
        }, error => {
            //toastService.ShowError();
        })
    }

    onChangeCountry(e: any) {
        var country = e.value.toString();
        if (country !== null && country !== "") {
            this.GetStates(country);
        }
        else {
            this.StateList = [];
        }
    }
    countryId: number;
    GetStates(country: string) {
        if (this.CountryList != null) {
            this.countryId = this.CountryList.find(x => x.countryDesc.toUpperCase() == country.toUpperCase()).countryID;
            if (this.countryId != 0 && this.countryId != null) {
                this.holidayService.GetCountryByIdAsync(this.countryId).subscribe(result => {
                    if (result !== null && result['data'] !== null && result['data'] !== undefined) {
                        this.StateList = result['data'].states;
                    }
                })
            }
        }
    }
    I9ANumber: boolean = false;
    I9USCISNumber: boolean = false;
    I9I94Number: boolean = false;
    I9I94ExpiryDate: boolean = false;
    onANumberChange(e: any) {
        if (this.WorkAuthorizationList.filter(x => (x.value === ListTypeConstants.GREENCARD || x.value === ListTypeConstants.H1B || x.value === ListTypeConstants.E3 || x.value === ListTypeConstants.TN || x.value === ListTypeConstants.H4EAD) && x.listValueID === this.FormI9.value.WorkAuthorizationID).length > 0 && (e.value === '' || e.value === null)) {
            this.I9ANumber = true;
        }
        else {
            this.I9ANumber = false;
        }
    }

    onUSCISNumberChange(e: any) {
        if (this.WorkAuthorizationList.filter(x => (x.value === ListTypeConstants.H1B || x.value === ListTypeConstants.E3 || x.value === ListTypeConstants.TN || x.value === ListTypeConstants.H4EAD) && x.listValueID === this.FormI9.value.WorkAuthorizationID).length > 0 && (e.value === '' || e.value === null)) {
            this.I9USCISNumber = true;
        }
        else {
            this.I9USCISNumber = false;
        }
    }
    onI94NumberChange(e: any) {
        if (this.WorkAuthorizationList.filter(x => (x.value === ListTypeConstants.H1B || x.value === ListTypeConstants.E3 || x.value == ListTypeConstants.TN || x.value == ListTypeConstants.H4EAD) && x.listValueID === this.FormI9.value.WorkAuthorizationID).length > 0 && (e.target.value === '' || e.target.value === null || e.target.value === undefined)) {
            this.I9I94Number = true;
        }
        else {
            this.I9I94Number = false;
        }
    }
    onI94ExpiryDateChange(e: any) {
        if (e.value === null) {
            this.I9I94ExpiryDate = true;
        }
        else {
            this.I9I94ExpiryDate = false;
        }
    }

    ListADocumentList: any[] = [];
    ListBDocumentList: any[] = [];
    ListCDocumentList: any[] = [];
    ErrorMessage: string;
    I9USCISNumberDisabled: boolean = false;
    I9ANumberDisabled: boolean = false;
    I94FieldsShow: boolean = false;

    I9ListAIssAuth: boolean = false;
    I9ListADocNum: boolean = false;
    I9ListAExpDate: boolean = false;
    I9ListBIssAuth: boolean = false;
    I9ListBDocNum: boolean = false;
    I9ListBExpDate: boolean = false;
    I9ListCIssAuth: boolean = false;
    I9ListCDocNum: boolean = false;
    I9ListCExpDate: boolean = false;

    LoadI9Documents(intWorkAuthId: number) {
        this.ErrorMessage = "";
        if (this.WorkAuthorizationList.filter(x => x.value === ListTypeConstants.USCITIZEN && x.listValueID === intWorkAuthId).length > 0) {
            this.I9USCISNumberDisabled = true;
            this.I9ANumberDisabled = true;
        }
        else {
            this.I9USCISNumberDisabled = false;
            this.I9ANumberDisabled = false;
        }

        if (this.WorkAuthorizationList.filter(x => (x.value === ListTypeConstants.H1B || x.value === ListTypeConstants.E3 || x.value === ListTypeConstants.TN || x.value === ListTypeConstants.H4EAD) && x.listValueID === intWorkAuthId).length > 0) {
            this.I94FieldsShow = true;
        }
        else {
            this.I94FieldsShow = false;
        }


        if (this.WorkAuthorizationList.filter(x => (x.value === ListTypeConstants.H1B || x.value === ListTypeConstants.E3 || x.value === ListTypeConstants.TN || x.value === ListTypeConstants.H4EAD) && x.listValueID === intWorkAuthId).length > 0 && (this.FormI9.value.I94Number === null || this.FormI9.value.I94Number === '')) {
            this.I9I94Number = true;
        } else {
            this.I9I94Number = false;
        }

        if (this.WorkAuthorizationList.filter(x => (x.value === ListTypeConstants.H1B || x.value === ListTypeConstants.E3 || x.value === ListTypeConstants.TN || x.value === ListTypeConstants.H4EAD) && x.listValueID === intWorkAuthId).length > 0 && (this.FormI9.value.I94ExpiryDate == null)) {
            this.I9I94ExpiryDate = true;
        }
        else {
            this.I9I94ExpiryDate = false;
        }
        this.ListADocumentList = this.I9DocumentsList.filter(x => x.i9DocTypeID === this.ListValues.find(y => y.value.toUpperCase() === ListTypeConstants.I9LISTADOCUMENTS).listValueID && x.workAuthID === intWorkAuthId);
        this.ListBDocumentList = this.I9DocumentsList.filter(x => x.i9DocTypeID === this.ListValues.find(y => y.value.toUpperCase() === ListTypeConstants.I9LISTBDOCUMENTS).listValueID && x.workAuthID === intWorkAuthId);
        this.ListCDocumentList = this.I9DocumentsList.filter(x => x.i9DocTypeID === this.ListValues.find(y => y.value.toUpperCase() === ListTypeConstants.I9LISTCDOCUMENTS).listValueID && x.workAuthID === intWorkAuthId);
       
        if (this.FormI9.value.ListADocumentTitleID != null && this.FormI9.value.ListADocumentTitleID !== 0) {
            if (this.FormI9.value.ListAIssuingAuthority === null || this.FormI9.value.ListAIssuingAuthority === '' || this.FormI9.value.ListAIssuingAuthority === undefined) {
                this.I9ListAIssAuth = true;
            }
            else {
                this.I9ListAIssAuth = false;
            }
            if (this.FormI9.value.ListADocumentNumber === null || this.FormI9.value.ListADocumentNumber === undefined) {
                this.I9ListADocNum = true;
            }
            else {
                this.I9ListADocNum = false;
            }
            if (this.FormI9.value.ListAExpirationDate === null || this.FormI9.value.ListAExpirationDate === undefined) {
                this.I9ListAExpDate = true;
            }
            else {
                this.I9ListAExpDate = false;
            }
        }
        if (this.FormI9.value.ListBDocumentTitleID != null && this.FormI9.value.ListBDocumentTitleID !== 0) {
            if (this.FormI9.value.ListBIssuingAuthority === null || this.FormI9.value.ListBIssuingAuthority === '' || this.FormI9.value.ListBIssuingAuthority === undefined) {
                this.I9ListBIssAuth = true;
            }
            else {
                this.I9ListBIssAuth = false;
            }
            if (this.FormI9.value.ListBDocumentNumber === null || this.FormI9.value.ListBDocumentNumber === undefined) {
                this.I9ListBDocNum = true;
            }
            else {
                this.I9ListBDocNum = false;
            }
            if (this.FormI9.value.ListBExpirationDate === null) {
                this.I9ListBExpDate = true;
            }
            else {
                this.I9ListBExpDate = false;
            }
        }
        if (this.FormI9.value.ListCDocumentTitleID != null && this.FormI9.value.ListCDocumentTitleID !== 0) {
            if (this.FormI9.value.ListCIssuingAuthority === null || this.FormI9.value.ListCIssuingAuthority === '' || this.FormI9.value.ListCIssuingAuthority === undefined) {
                this.I9ListCIssAuth = true;
            }
            else {
                this.I9ListCIssAuth = false;
            }
            if (this.FormI9.value.ListCDocumentNumber === null || this.FormI9.value.ListCDocumentNumber === undefined) {
                this.I9ListCDocNum = true;
            }
            else {
                this.I9ListCDocNum = false;
            }
            if (this.FormI9.value.ListCExpirationDate == null) {
                this.I9ListCExpDate = true;
            }
            else {
                this.I9ListCExpDate = false;
            }
        }
    }

    onChangeListADocumentTitle(e: any) {
        
        if (Number(e.value) != 0 && e.value != null && this.ListADocumentList.length > 0) {
            var listADocumentTitle = Number(e.value);
            this.FormI9.value.ListADocumentTitle = this.ListADocumentList.find(x => x.i9DocumentID === listADocumentTitle).i9DocName;

            if (this.FormI9.value.ListAIssuingAuthority === '' || this.FormI9.value.ListAIssuingAuthority === null) {
                this.I9ListAIssAuth = true;
            }
            else {
                this.I9ListAIssAuth = false;
            }
            if (this.FormI9.value.ListADocumentNumber === '' || this.FormI9.value.ListADocumentNumber === null) {
                this.I9ListADocNum = true;
            }
            else {
                this.I9ListADocNum = false;
            }
            if (this.FormI9.value.ListAExpirationDate === null) {
                this.I9ListAExpDate = true;
            }
            else {
                this.I9ListAExpDate = false;
            }
        }
        else {
            this.FormI9.controls.ListADocumentTitle.patchValue('');
            this.FormI9.controls.ListAIssuingAuthority.patchValue('');
            this.FormI9.controls.ListADocumentNumber.patchValue('');
            this.FormI9.controls.ListAExpirationDate.patchValue(null);
            this.I9ListAIssAuth = false;
            this.I9ListADocNum = false;
            this.I9ListAExpDate = false;
        }
    }


    onChangeListBDocumentTitle(e: any) {
        if (Number(e.value) != 0 && e.value != null && this.ListBDocumentList.length > 0) {
            var listBDocumentTitle = Number(e.value);
            this.FormI9.value.ListBDocumentTitle = this.ListBDocumentList.find(x => x.i9DocumentID === listBDocumentTitle).i9DocName;

            if (this.FormI9.value.ListBIssuingAuthority === '' || this.FormI9.value.ListBIssuingAuthority === null) {
                this.I9ListBIssAuth = true;
            }
            else {
                this.I9ListBIssAuth = false;
            }
            if (this.FormI9.value.ListBDocumentNumber === '' || this.FormI9.value.ListBDocumentNumber === null) {
                this.I9ListBDocNum = true;
            }
            else {
                this.I9ListBDocNum = false;
            }
            if (this.FormI9.value.ListBExpirationDate === null) {
                this.I9ListBExpDate = true;
            }
            else {
                this.I9ListBExpDate = false;
            }
        }
        else {
            this.FormI9.controls.ListBDocumentTitle.patchValue('');
            this.FormI9.controls.ListBIssuingAuthority.patchValue('');
            this.FormI9.controls.ListBDocumentNumber.patchValue('');
            this.FormI9.controls.ListBExpirationDate.patchValue(null);
            this.I9ListBIssAuth = false;
            this.I9ListBDocNum = false;
            this.I9ListBExpDate = false;
        }
    }
    isListCExpirationDate: boolean = false;
    onChangeListCDocumentTitle(e: any) {
        if (Number(e.value) != 0 && e.value != null && this.ListCDocumentList.length > 0) {
            var listCDocumentTitle = Number(e.value);
            this.FormI9.value.ListCDocumentTitle = this.ListCDocumentList.find(x => x.i9DocumentID === listCDocumentTitle).i9DocName;
            this.isListCExpirationDate = false;
            if (this.FormI9.value.ListCDocumentTitle.toLowerCase() === "Social Security Account Number Card".toString().toLowerCase() || this.FormI9.value.ListCDocumentTitle.toLowerCase() === "Birth Certificate".toString().toLowerCase()) {
                this.FormI9.controls.ListCExpirationDate.patchValue('12/31/9999');
                this.isListCExpirationDate = true;
            } else {
                this.FormI9.controls.ListCExpirationDate.patchValue(null);
                this.isListCExpirationDate = false;
            }

            if (this.FormI9.value.ListCIssuingAuthority === '' || this.FormI9.value.ListCIssuingAuthority === null) {
                this.I9ListCIssAuth = true;
            }
            else {
                this.I9ListCIssAuth = false;
            }
            if (this.FormI9.value.ListCDocumentNumber === '' || this.FormI9.value.ListCDocumentNumber === null) {
                this.I9ListCDocNum = true;
            }
            else {
                this.I9ListCDocNum = false;
            }
            if (this.FormI9.value.ListCExpirationDate === null) {
                this.I9ListCExpDate = true;
            }
            else {
                this.I9ListCExpDate = false;
            }
        }
        else {
            this.FormI9.controls.ListCDocumentTitle.patchValue('');
            this.FormI9.controls.ListCIssuingAuthority.patchValue('');
            this.FormI9.controls.ListCDocumentNumber.patchValue('');
            this.FormI9.controls.ListCExpirationDate.patchValue(null);
            this.I9ListCIssAuth = false;
            this.I9ListCDocNum = false;
            this.I9ListCExpDate = false;
        }
    }

    onListAIssuAuthChange(e: any) {
        if (e.value === null || e.value === '') {
            this.I9ListAIssAuth = true;
        }
        else {
            this.I9ListAIssAuth = false;
        }
    }

    onListADocNumChange(e: any) {
        if (e.value === null || e.value === '') {
            this.I9ListADocNum = true;
        }
        else {
            this.I9ListADocNum = false;
        }
    }

    onListAExpDateChange(e: any) {
        if (e.value === null) {
            this.I9ListAExpDate = true;
        }
        else {
            this.I9ListAExpDate = false;
        }
    }

    onListBIssuAuthChange(e: any) {
        if (e.value === null || e.value === '') {
            this.I9ListBIssAuth = true;
        }
        else {
            this.I9ListBIssAuth = false;
        }
    }

    onListBDocNumChange(e: any) {
        if (e.value === null || e.value === '') {
            this.I9ListBDocNum = true;
        }
        else {
            this.I9ListBDocNum = false;
        }
    }

    onListBExpDateChange(e: any) {
        if (e.value === null) {
            this.I9ListBExpDate = true;
        }
        else {
            this.I9ListBExpDate = false;
        }
    }


  onListCIssuAuthChange(e: any) {
        if (e.value === null || e.value === '') {
            this.I9ListCIssAuth = true;
        }
        else {
            this.I9ListCIssAuth = false;
        }
    }

    onListCDocNumChange(e: any) {
        if (e.value === null || e.value === '') {
            this.I9ListCDocNum = true;
        }
        else {
            this.I9ListCDocNum = false;
        }
    }

    onListCExpDateChange(e: any) {
        if (e.value === null) {
            this.I9ListCExpDate = true;
        }
        else {
            this.I9ListCExpDate = false;
        }
    }


    save = () => {
        this.submitted = true;
        if (this.FormI9.invalid) {
            this.FormI9.markAllAsTouched();
            return;
        } else {
            if (this.I9ANumber === true || this.I9USCISNumber === true || this.I9I94Number === true || this.I9ListAIssAuth === true || this.I9ListADocNum === true || this.I9ListAExpDate === true || this.I9ListBIssAuth === true || this.I9ListBDocNum === true || this.I9ListBExpDate === true || this.I9ListCIssAuth === true || this.I9ListCDocNum === true || this.I9ListCExpDate === true || this.I9I94ExpiryDate === true) {
                return
            } else {
                if (this.WorkAuthorizationList.filter(x => x.value == ListTypeConstants.GREENCARD && x.listValueID == this.FormI9.value.WorkAuthorizationID).length > 0 && (this.FormI9.value.ANumber === '' || this.FormI9.value.ANumber === null) && (this.FormI9.value.USCISNumber === '' || this.FormI9.value.USCISNumber === null)) {
                    this.ErrorMessage = "A Number or USCIS Number required";
                    return;
                }
                if (this.FormI9.value.ListADocumentTitleID === null || this.FormI9.value.ListADocumentTitleID === 0) {
                    if ((this.FormI9.value.ListBDocumentTitleID === null || this.FormI9.value.ListBDocumentTitleID === 0) || (this.FormI9.value.ListCDocumentTitleID === null || this.FormI9.value.ListCDocumentTitleID === 0)) {
                        this.ErrorMessage = "List A or List (B and C) Documents required";
                        return;
                    }
                }
                if (this.isSaveButtonDisabled) {
                    return;
                } else {
                    this.isSaveButtonDisabled = true
                    if (this.FormI9.value.I94ExpiryDate !== null) {
                        this.FormI9.value.I94ExpiryDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.FormI9.value.I94ExpiryDate));
                    }
                    if (this.FormI9.value.ListAExpirationDate !== null) {
                        this.FormI9.value.ListAExpirationDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.FormI9.value.ListAExpirationDate));
                    }
                    if (this.FormI9.value.ListBExpirationDate !== null) {
                        this.FormI9.value.ListBExpirationDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.FormI9.value.ListBExpirationDate));
                    }
                    if (this.FormI9.value.ListCExpirationDate !== null) {
                        this.FormI9.value.ListCExpirationDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.FormI9.value.ListCExpirationDate));
                    }
                    if (this.FormI9.value.BirthDate !== null) {
                        this.FormI9.value.BirthDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.FormI9.value.BirthDate));
                    }
                    if (this.FormI9.value.HireDate !== null) {
                        this.FormI9.value.HireDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.FormI9.value.HireDate));
                    }
                    if (this.FormI9ID === 0) {
                        console.log(this.FormI9.value);
                        this.formI9Service.SaveFormI9(this.FormI9.value).subscribe(result => {
                            if (result['data'] !== null && result['messageType'] === 1) {
                                this.messageService.add({ severity: 'success', summary: 'FormI9 saved successfully', detail: '' });
                                this.isSaveButtonDisabled = false;
                                this.loadModalOptions();
                                this.ListValueUpdated.emit();
                            } else {
                                this.isSaveButtonDisabled = false;
                                this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                            }
                        })
                        this.submitted = false;
                        this.cancel();
                    }
                    else if (this.FormI9ID > 0) {
                        this.formI9Service.UpdateFormI9(this.FormI9.value).subscribe(result => {
                            if (result['data'] !== null && result['messageType'] === 1) {
                                this.messageService.add({ severity: 'success', summary: 'FormI9 saved successfully', detail: '' });
                                this.isSaveButtonDisabled = false;
                                this.loadModalOptions();
                                this.ListValueUpdated.emit();

                            } else {
                                this.isSaveButtonDisabled = false;
                                this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                            }
                        })
                        this.submitted = false;
                        this.cancel();
                    }
                }
                this.isSaveButtonDisabled = false
            }
        }

    }

    cancel = () => {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildEmployeeI9Form({}, 'New');
        this.addEditEmployeeI9FormModal.hide();
    }


}
