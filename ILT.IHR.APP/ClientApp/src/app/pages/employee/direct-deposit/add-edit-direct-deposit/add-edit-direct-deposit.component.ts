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
import { EmerygencyService } from '../../employee-emergency/emerygency.service';
import { EmployeeService } from '../../employee.service';
import { DirectDepositService } from '../direct-deposit.service';

@Component({
  selector: 'app-add-edit-direct-deposit',
  templateUrl: './add-edit-direct-deposit.component.html',
  styleUrls: ['./add-edit-direct-deposit.component.scss']
})
export class AddEditDirectDepositComponent implements OnInit {
    commonUtils = new CommonUtils()
    @Input() EmployeeName: string;
    @Output() ListValueUpdated = new EventEmitter<any>();
    @ViewChild('addEditDirectDepositModal') addEditDirectDepositModal: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;
    isSaveButtonDisabled: boolean = false;
    DirectDepositId: number;
    ModalHeading: string = 'Add Direct Deposit';
    isShow: boolean;

    AccountTypeList: any[] = [];
    CountryList: any[] = [];
    user: any;
    submitted: boolean = false;
    StateList: any[] = [];

    DirectDepositForm: FormGroup;
    RolePermissions: IRolePermissionDisplay[] = [];
    EmployeeInfoRolePermission: IRolePermissionDisplay;


    constructor(private fb: FormBuilder,
        private LookupService: LookUpService,
        private emerygencyService: EmerygencyService,
        private messageService: MessageService,
        private directDepositService: DirectDepositService,
        private employeeService: EmployeeService,
        private holidayService: HolidayService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.EmployeeInfoRolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.EMPLOYEEINFO);
        this.user = JSON.parse(localStorage.getItem("User"));
        this.LoadDropDown();
        this.buildDirectDepositForm({}, 'New');
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
                this.AccountTypeList = resp['data'].filter(x => x.type.toUpperCase() === ListTypeConstants.ACCOUNTTYPE);
            }
        })
        this.holidayService.getCountry().subscribe(resp => {
            this.CountryList = resp['data'];
        })
    }
    getStateList(country: string) {
        this.GetStates(country);
    }
    Show(Id: number, employeeId: number) {
        this.DirectDepositId = Id;
        this.ResetDialog();
        if (this.DirectDepositId != 0) {
            this.isShow = this.EmployeeInfoRolePermission.update;
            this.loadModalOptions();
            this.GetDetails(this.DirectDepositId);
        }
        else {
            this.isShow = this.EmployeeInfoRolePermission.add;
            this.loadModalOptions();
            this.ModalHeading = "Add Direct Deposit";
            this.buildDirectDepositForm({}, 'New');
            this.loadEmployeeData(employeeId);
            this.DirectDepositForm.controls.EmployeeID.patchValue(employeeId)
            this.DirectDepositForm.controls.EmployeeName.patchValue(this.EmployeeName);
            this.addEditDirectDepositModal.show();
        }
    }
    loadEmployeeData(employeeID: number) {
        this.employeeService.getEmployeeByIdAsync(employeeID).subscribe(result => {
            if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                if (result['data'].employeeID !== 0) {
                    this.DirectDepositForm.controls.Country.patchValue(result['data'].country)
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


    buildDirectDepositForm(data: any, keyName: string) {
        this.DirectDepositForm = this.fb.group({
            DirectDepositID: [keyName === 'New' ? 0 : data.directDepositID],
            AccountTypeID: [keyName === 'New' ? null : data.accountTypeID, Validators.required],
            AccountType: [keyName === 'New' ? '' : data.accountType],
            EmployeeID: [keyName === 'New' ? null : data.employeeID],
            BankName: [keyName === 'New' ? '' : data.bankName, Validators.required],
            EmployeeName: [keyName === 'New' ? '' : data.employeeName],
            RoutingNumber: [keyName === 'New' ? '' : data.routingNumber, Validators.required],
            AccountNumber: [keyName === 'New' ? '' : data.accountNumber, Validators.required],
            Amount: [keyName === 'New' ? 0 : data.amount, Validators.required],
            State: [keyName === 'New' ? null : data.state],
            Country: [keyName === 'New' ? null : data.country, Validators.required],
            CreatedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : this.user.firstName + " " + this.user.lastName],
            CreatedDate: [keyName === 'New' ? new Date() : data.createdDate],
            ModifiedDate: [keyName === 'New' ? new Date() : data.modifiedDate],
            ModifiedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : data.modifiedBy],
            IsPrimary: false,
            TimeStamp: ['']
        });
    }
    get addEditDirectDepositControls() { return this.DirectDepositForm.controls; }

    save = () => {
        this.submitted = true;
        if (this.DirectDepositForm.invalid) {
            this.DirectDepositForm.markAllAsTouched();
            return;
        } else {
            if (this.isSaveButtonDisabled) {
                return;
            } else {
                this.isSaveButtonDisabled = true;
                if (this.DirectDepositId === 0) {
                    this.directDepositService.SaveDirectdeposit(this.DirectDepositForm.value).subscribe(result => {
                        if (result['data'] !== null && result['messageType'] === 1) {
                            this.messageService.add({ severity: 'success', summary: 'Direct Deposit saved successfully', detail: '' });
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
                else if (this.DirectDepositId > 0) {
                    this.directDepositService.UpdateDirectdeposit(this.DirectDepositForm.value).subscribe(result => {
                        if (result['data'] !== null && result['messageType'] === 1) {
                            this.messageService.add({ severity: 'success', summary: 'Direct Deposit saved successfully', detail: '' });
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

    GetDetails(Id: number) {
        this.directDepositService.getDirectDepositByIdAsync(Id).subscribe(result => {
            if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                this.buildDirectDepositForm(result['data'], 'Edit');
                this.ModalHeading = "Edit Direct Deposit";
                this.DirectDepositId = result['data'].directDepositID;
                this.EmployeeName = result['data'].employeeName;
                if (result['data'].country != null) {
                    this.GetStates(result['data'].country);
                } else {
                    this.StateList = [];
                }
                this.addEditDirectDepositModal.show();
            }
        })
    }

    cancel = () => {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildDirectDepositForm({}, 'New');
        this.addEditDirectDepositModal.hide();
    }
    ResetDialog() {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildDirectDepositForm({}, 'New');
        this.addEditDirectDepositModal.hide();
    }

}
