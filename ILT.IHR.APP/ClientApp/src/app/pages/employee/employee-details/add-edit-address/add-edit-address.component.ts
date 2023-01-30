import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CommonUtils } from '../../../../common/common-utils';
import { EmployeeAddresses, ListTypeConstants } from '../../../../constant';
import { IEmployeeAddress, IEmployeeAddressDisplay } from '../../../../core/interfaces/EmployeeAddress';
import { IModalPopupAlternateOptions } from '../../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { HolidayService } from '../../../holidays/holidays.service';
import { LookUpService } from '../../../lookup/lookup.service';

@Component({
  selector: 'app-add-edit-address',
  templateUrl: './add-edit-address.component.html',
  styleUrls: ['./add-edit-address.component.scss']
})
export class AddEditAddressComponent implements OnInit {
    commonUtils = new CommonUtils()
    @Input() EmployeeName: string;
    @Output() updatedEmpAddress = new EventEmitter<any>();
    @ViewChild('addEditAddressModal') addEditAddressModal: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;
    AddressTypeList: any[] = [];
    CountryList: any[] = [];
    user: any;

    EmployeeAddressForm: FormGroup;

    constructor(private fb: FormBuilder,private LookupService: LookUpService, private holidayService: HolidayService) {
        this.user = JSON.parse(localStorage.getItem("User"));
        this.LoadDropDown();
        this.buildEmployeeAddressForm({}, 'New');
    }
    isSaveButtonDisabled: boolean = false;
    ngOnInit(): void {
        this.loadModalOptions();
  }
    loadModalOptions() {
        this.modalOptions = {
            footerActions: [
                {
                    actionText: 'Save',
                    actionMethod: this.save,
                    styleClass: 'p-button-raised p-mr-2 p-mb-2',
                    iconClass: 'mdi mdi-content-save',
                    disabled: this.isSaveButtonDisabled
                },
                {
                    actionText: 'Cancel',
                    actionMethod: this.cancel,
                    styleClass: 'p-button-raised p-button-danger  p-mb-2',
                    iconClass: 'p-button-raised p-button-danger'
                }
            ]
        }
    }
    copyFromCurrent: boolean = false;
    ModalHeading: string = 'Add Address';
    addTypeID: number;
    employeeAddress: IEmployeeAddress;
    countryId: number;
    StateList: any[] = [];
    submitted: boolean = false;
    currentAddress: IEmployeeAddress;
    Show(ID: number, AddressTypeId: number, empAddress: IEmployeeAddress, AddressType: string) {
        this.copyFromCurrent = false;
        if (ID == 0) {
            if (Object.keys(empAddress).length === 0 || empAddress.Address1 === '' || empAddress.Address1 === null) {
                this.ResetDialog();
            } else {
                this.buildEmployeeAddressForm(empAddress, 'Edit')
            }
            this.ModalHeading = "Add Address";
            // ShowDialog = true;
            this.addTypeID = AddressTypeId;
            if (AddressType != "" && AddressType.toUpperCase() !== EmployeeAddresses.CURRADD) {
                this.copyFromCurrent = true;
            }
            this.EmployeeAddressForm.controls.AddressTypeID.patchValue(AddressTypeId)
            this.EmployeeAddressForm.controls.EmployeeName.patchValue(this.EmployeeName);
            this.addEditAddressModal.show();
        }
        else {
            this.ModalHeading = "Edit Address";
            this.employeeAddress = empAddress;
            if (this.employeeAddress.Country !== null) {
                this.getStateList(this.employeeAddress.Country);
            }
            else {
                this.StateList =  [];
            }
            this.buildEmployeeAddressForm(empAddress, 'Edit');
            console.log(this.EmployeeName);
            this.EmployeeAddressForm.controls.EmployeeName.patchValue(this.EmployeeName);
            this.addEditAddressModal.show();
        }
    }

    LoadDropDown() {
        this.LookupService.getListValues().subscribe(resp => {
            if (resp['data'] !== undefined && resp['data'] !== null) {
                this.AddressTypeList = resp['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.ADDRESSTYPE);
            }
        })
        this.holidayService.getCountry().subscribe(resp => {
            this.CountryList = resp['data'];
        })
    }
    getStateList(country: string) {
        this.GetStates(country);
    }
    onChangeCountry(e: any) {
        console.log(e.value)
        var country = e.value.toString();
        if (country !== null && country !== "") {
            this.GetStates(country);
        }
        else {
            this.StateList = [];
        }
    }
    GetStates(country: string) {
        if (this.CountryList != null) {
            this.countryId = this.CountryList.find(x => x.countryDesc.toUpperCase() == country.toUpperCase()).countryID;
            if (this.countryId != 0 && this.countryId != null) {
                this.holidayService.GetCountryByIdAsync(this.countryId).subscribe(result => {
                    if (result !== null && result['data'] !== null && result['data'] !== undefined) {
                        console.log(result['data']);
                        this.StateList = result['data'].states;
                    }
                })
            }
        }
    }

    buildEmployeeAddressForm(data: any, keyName: string) {
        this.EmployeeAddressForm = this.fb.group({
            EmployeeAddressID: [keyName === 'New' ? 0 : data.EmployeeAddressID],
            EmployeeID: [keyName === 'New' ? 0 : data.EmployeeID],
            EmployeeName: [keyName === 'New' ? 0 : data.EmployeeName],
            AddressTypeID: [keyName === 'New' ? 0 : data.AddressTypeID],
            Address1: [keyName === 'New' ? '' : data.Address1, Validators.required],
            Address2: [keyName === 'New' ? '' : data.Address2],
            City: [keyName === 'New' ? '' : data.City, Validators.required],
            State: [keyName === 'New' ? null : data.State, Validators.required],
            Country: [keyName === 'New' ? null : data.Country, Validators.required],
            ZipCode: [keyName === 'New' ? '' : data.ZipCode, Validators.required],
            StartDate: [keyName === 'New' ? null : new Date(data.StartDate), Validators.required],
            EndDate: [keyName === 'New' ? null : data.EndDate === null ? null : new Date(data.EndDate)],
            CreatedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : this.user.firstName + " " + this.user.lastName],
            CreatedDate: [keyName === 'New' ? new Date() : data.CreatedDate],
            ModifiedDate: [keyName === 'New' ? new Date() : data.ModifiedDate],
            ModifiedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : data.modifiedBy],
            TimeStamp: ['']
        });
    }
    get addEditAddressFormControls() { return this.EmployeeAddressForm.controls; }

    save = () => {
        this.submitted = true;
        if (this.EmployeeAddressForm.invalid) {
            this.EmployeeAddressForm.markAllAsTouched();
            return;
        } else {
            if (this.isSaveButtonDisabled) {
                return;
            } else {
                this.isSaveButtonDisabled = true;
                this.employeeAddress = this.EmployeeAddressForm.value;
                //this.employeeAddress.EmployeeName = EmployeeName;
                this.updatedEmpAddress.emit(this.employeeAddress);
                this.submitted = false;
                this.isSaveButtonDisabled = false;
                this.addEditAddressModal.hide();
            }
        }
        //employeeAddress.EmployeeName = EmployeeName;
        //updatedEmpAddress.InvokeAsync(employeeAddress);
        //ShowDialog = false;
    }
    cancel = () => {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildEmployeeAddressForm({}, 'New');
        this.addEditAddressModal.hide();
    }
    ResetDialog() {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildEmployeeAddressForm({}, 'New');
        this.addEditAddressModal.hide();
    }
    CopyFromCurrent() {
        if (Object.keys(this.currentAddress).length === 0) {
            this.buildEmployeeAddressForm(this.currentAddress, 'New');
        } else {
            this.buildEmployeeAddressForm(this.currentAddress, 'Edit');
            this.EmployeeAddressForm.controls.AddressTypeID.patchValue(this.addTypeID)
            this.EmployeeAddressForm.controls.EmployeeAddressID.patchValue(0)

        }
    }
}
