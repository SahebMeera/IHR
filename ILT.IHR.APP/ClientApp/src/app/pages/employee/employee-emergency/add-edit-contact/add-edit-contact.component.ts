import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { CommonUtils } from '../../../../common/common-utils';
import { Constants, ErrorMsg, ListTypeConstants, SessionConstants } from '../../../../constant';
import { IRolePermissionDisplay } from '../../../../core/interfaces/RolePermission';
import { IModalPopupAlternateOptions } from '../../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { HolidayService } from '../../../holidays/holidays.service';
import { LookUpService } from '../../../lookup/lookup.service';
import { EmployeeService } from '../../employee.service';
import { EmerygencyService } from '../emerygency.service';

@Component({
  selector: 'app-add-edit-contact',
  templateUrl: './add-edit-contact.component.html',
  styleUrls: ['./add-edit-contact.component.scss']
})
export class AddEditContactComponent implements OnInit {
    commonUtils = new CommonUtils()
    @Input() EmployeeName: string;
    @Output() ListValueUpdated = new EventEmitter<any>();
    @ViewChild('addEditContactModal') addEditContactModal: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;
    ContactTypeList: any[] = [];
    CountryList: any[] = [];
    user: any;
    submitted: boolean = false;
    StateList: any[] = [];

    ContactForm: FormGroup;
    RolePermissions: IRolePermissionDisplay[] = [];
    EmployeeInfoRolePermission: IRolePermissionDisplay;

    constructor(private fb: FormBuilder,
        private LookupService: LookUpService,
        private emerygencyService: EmerygencyService,
        private messageService: MessageService,
        private employeeService: EmployeeService,
        private holidayService: HolidayService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.EmployeeInfoRolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.EMPLOYEEINFO);
        this.user = JSON.parse(localStorage.getItem("User"));
        this.LoadDropDown();
        this.buildContactForm({}, 'New');
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
                    styleClass: this.isShow ? 'btn-width-height p-button-raised p-mr-2 p-mb-2' : 'btn-width-height p-button-raised p-mr-2 p-mb-2 display-none',
                    iconClass: 'mdi mdi-content-save',
                    disabled: this.isSaveButtonDisabled
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
    LoadDropDown() {
        this.LookupService.getListValues().subscribe(resp => {
            if (resp['data'] !== undefined && resp['data'] !== null) {
                this.ContactTypeList = resp['data'].filter(x => x.type.toUpperCase() === ListTypeConstants.CONTACTTYPE);
            }
        })
        this.holidayService.getCountry().subscribe(resp => {
            this.CountryList = resp['data'];
        })
    }
    getStateList(country: string) {
        this.GetStates(country);
    }
    ContactId: number;
    ModalHeading: string = 'Add Contact';
    isShow: boolean;
    Show(Id: number, employeeId: number) {
        this.ContactId = Id;
        this.ResetDialog();
        if (this.ContactId != 0) {
            this.isShow = this.EmployeeInfoRolePermission.update;
          this.loadModalOptions();
          this.GetDetails(this.ContactId, employeeId);
        }
        else {
            this.isShow = this.EmployeeInfoRolePermission.add;
            this.loadModalOptions();
            this.ModalHeading = "Add Contact";
            this.buildContactForm({}, 'New');
            this.loadEmployeeData(employeeId);
            this.ContactForm.controls.EmployeeID.patchValue(employeeId)
            this.ContactForm.controls.EmployeeName.patchValue(this.EmployeeName);
            this.addEditContactModal.show();
        }
    }
    loadEmployeeData(employeeID: number) {
        this.employeeService.getEmployeeByIdAsync(employeeID).subscribe(result => {
            if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                if (result['data'].employeeID !== 0) {
                    this.ContactForm.controls.Country.patchValue(result['data'].country)
                    if (result['data'].country !== null) {
                        this.GetStates(result['data'].country);
                    }
                    else {
                        this.StateList = [];
                    }
                }
            }
        }, error => {
            //toastService.ShowError();
        })
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
    countryId: number;
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

    buildContactForm(data: any, keyName: string) {
        this.ContactForm = this.fb.group({
            ContactID: [keyName === 'New' ? 0 : data.contactID],
            ContactTypeID: [keyName === 'New' ? null : data.contactTypeID, Validators.required],
            ContactType: [keyName === 'New' ? '' : data.contactType],
            EmployeeID: [keyName === 'New' ? null : data.employeeID],
            FirstName: [keyName === 'New' ? '' : data.firstName, Validators.required],
            EmployeeName: [keyName === 'New' ? '' : data.employeeName],
            LastName: [keyName === 'New' ? '' : data.lastName, Validators.required],
            Phone: [keyName === 'New' ? '' : data.phone, Validators.required],
            Email: [keyName === 'New' ? '' : data.email, [Validators.required, Validators.email]],
            Address1: [keyName === 'New' ? '' : data.address1, Validators.required],
            Address2: [keyName === 'New' ? '' : data.address2],
            City: [keyName === 'New' ? '' : data.city, Validators.required],
            State: [keyName === 'New' ? null : data.state, Validators.required],
            Country: [keyName === 'New' ? null : data.country, Validators.required],
            ZipCode: [keyName === 'New' ? '' : data.zipCode, Validators.required],
            CreatedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : this.user.firstName + " " + this.user.lastName],
            CreatedDate: [keyName === 'New' ? new Date() : data.createdDate],
            ModifiedDate: [keyName === 'New' ? new Date() : data.modifiedDate],
            ModifiedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : data.modifiedBy],
            IsDeleted: false,
            TimeStamp: ['']
        });
    }
    get addEditContactControls() { return this.ContactForm.controls; }
    save = () => {
        this.submitted = true;
        if (this.ContactForm.invalid) {
            this.ContactForm.markAllAsTouched();
            return;
        } else {
            if (this.isSaveButtonDisabled) {
                return;
            } else {
                this.isSaveButtonDisabled = true;
                if (this.ContactId == 0) {
                    this.emerygencyService.SaveContact(this.ContactForm.value).subscribe(result => {
                        if (result['data'] !== null && result['messageType'] === 1) {
                            this.messageService.add({ severity: 'success', summary: 'Contact saved successfully', detail: '' });
                            this.isSaveButtonDisabled = true;
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
                else if (this.ContactId > 0) {
                    this.emerygencyService.UpdateContact(this.ContactForm.value).subscribe(result => {
                        if (result['data'] !== null && result['messageType'] === 1) {
                            this.messageService.add({ severity: 'success', summary: 'Contact saved successfully', detail: '' });
                            this.isSaveButtonDisabled = true;
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
            this.isSaveButtonDisabled = false;
        }
    }

  GetDetails(Id: number, employeeId: number) {
      this.emerygencyService.getcontactByIdAsync(Id, employeeId).subscribe(result => {
          if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
              this.buildContactForm(result['data'], 'Edit');
              this.ModalHeading = "Edit Contact";
              this.ContactId = result['data'].contactID;
              this.EmployeeName = result['data'].employeeName;
              if (result['data'].country != null) {
                  this.GetStates(result['data'].country);
              } else {
                  this.StateList = [];
              }
              this.addEditContactModal.show();
          }
      })
    }

    cancel = () => {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildContactForm({}, 'New');
        this.addEditContactModal.hide();
    }
    ResetDialog() {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildContactForm({}, 'New');
        this.addEditContactModal.hide();
    }

}
