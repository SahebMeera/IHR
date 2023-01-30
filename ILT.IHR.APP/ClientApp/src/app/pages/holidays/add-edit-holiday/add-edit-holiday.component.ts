import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { CommonUtils } from '../../../common/common-utils';
import { ErrorMsg } from '../../../constant';
import { ICountryDisplay } from '../../../core/interfaces/Country';
import { IHoliday, IHolidayDisplay } from '../../../core/interfaces/Holiday';
import { IModalPopupAlternateOptions } from '../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { HolidayService } from '../holidays.service';

@Component({
  selector: 'app-add-edit-holiday',
  templateUrl: './add-edit-holiday.component.html',
  styleUrls: ['./add-edit-holiday.component.scss']
})
export class AddEditHolidayComponent implements OnInit {
    commonUtils = new CommonUtils()
    @Input() yearId: number = 0;
    @Output() loadHolidayDetails = new EventEmitter<any>();
    @ViewChild('addEditHolidayModal') addEditHolidayModal: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;

    HolidayForm: FormGroup;
    user: any;

    ModalHeading: string = 'Add Holiday';
    HolidayId: number;
    submitted: boolean
    constructor(private fb: FormBuilder,
        private holidayService: HolidayService,
        private messageService: MessageService) {
        this.user = JSON.parse(localStorage.getItem("User"));
        this.buildHolidayForm({}, 'New');
    }

    ngOnInit(): void {
        this.LoadDropDown();
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
    CountryList: ICountryDisplay[] = [];
    HolidaysList: IHolidayDisplay[] = [];
    LoadDropDown() {
        this.holidayService.getCountry().subscribe(result => {
            if (result['data'] !== undefined && result['data'] !== null) {
                this.CountryList = result['data']
            }
        })
        this.holidayService.getHolidayList().subscribe(result => {
            if (result['data'] !== undefined && result['data'] !== null) {
                this.HolidaysList = result['data'];
            }
        })
    }

    show(Id: any) {
        this.isSaveButtonDisabled = false;
        this.HolidayId = Id;
       this.ResetDialog();
        if (this.HolidayId != 0) {
            this.isSaveButtonDisabled = false;
          this.GetDetails(this.HolidayId);
      }
      else {
          this.ModalHeading = "Add Holiday";
          let StartDate = this.commonUtils.defaultDateTimeLocalSet(new Date());
            this.HolidayForm.controls.StartDate.patchValue(new Date(StartDate));
         // holiday.StartDate = DateTime.Now;
          this.addEditHolidayModal.show();
      }
  }

    GetDetails(Id: number) {
        this.holidayService.getHolidayByIdAsync(Id).subscribe(result => {
            if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                this.buildHolidayForm(result['data'], 'Edit');
                this.ModalHeading = "Edit Holiday";
                this.addEditHolidayModal.show();
            }
        })
    }
    buildHolidayForm(data: any, keyName: string) {
        this.HolidayForm = this.fb.group({
            HolidayID: [keyName === 'New' ? 0 : data.holidayID],
            Name: [keyName === 'New' ? '' : data.name, Validators.required],
            StartDate: [keyName === 'New' ? this.commonUtils.formatDateDefault(new Date()) : new Date(data.startDate)],
            Country: [keyName === 'New' ? null : data.country, Validators.required],
            CreatedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : this.user.firstName + " " + this.user.lastName],
            CreatedDate: [keyName === 'New' ? new Date() : data.createdDate],
            ModifiedDate: [keyName === 'New' ? new Date() : data.modifiedDate],
            ModifiedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : data.modifiedBy],
            TimeStamp: ['']
        });
    }
    get addEditHolidayFormControls() { return this.HolidayForm.controls; }

    isSaveButtonDisabled: boolean = false;
    holiday: IHoliday;
    save  = () => {
        this.submitted = true;
        if (this.HolidayForm.invalid) {
            this.HolidayForm.markAllAsTouched();
            return;
        } else {
            if (this.isSaveButtonDisabled) {
                return;
            } else {
                this.isSaveButtonDisabled = false;
                if (this.HolidayId == 0) {
                    this.HolidayForm.value.StartDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.HolidayForm.value.StartDate));
                    this.holidayService.SaveHoliday(this.HolidayForm.value).subscribe(result => {
                        if (result['data'] !== null && result['messageType'] === 1) {
                            this.messageService.add({ severity: 'success', summary: 'Holiday saved successfully', detail: '' });
                            this.isSaveButtonDisabled = false;
                            this.loadModalOptions();
                            this.loadHolidayDetails.emit(this.yearId);
                        } else {
                            this.isSaveButtonDisabled = false;
                            this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                        }
                    })
                    this.submitted = false;
                    this.cancel();
                }
                else if (this.HolidayId > 0) {
                    this.HolidayForm.value.StartDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.HolidayForm.value.StartDate));
                    this.HolidayForm.value.ModifiedBy = this.user.firstName + " " + this.user.lastName;
                    this.HolidayForm.value.ModifiedDate = this.commonUtils.defaultDateTimeLocalSet(new Date());
                    this.holidayService.UpdateHoliday(this.HolidayForm.value).subscribe(result => {
                        if (result['data'] !== null && result['messageType'] === 1) {
                            this.messageService.add({ severity: 'success', summary: 'Holiday saved successfully', detail: '' });
                            this.isSaveButtonDisabled = false;
                            this.loadModalOptions();
                            this.loadHolidayDetails.emit(this.yearId);

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
    cancel = () => {
        this.submitted = false;
        this.HolidayId = -1;
        this.addEditHolidayModal.hide();
        this.buildHolidayForm({}, 'New');
    }
    ResetDialog() {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildHolidayForm({}, 'New');
    }
}
