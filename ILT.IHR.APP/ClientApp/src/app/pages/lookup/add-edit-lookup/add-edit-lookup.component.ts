import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { CommonUtils } from '../../../common/common-utils';
import { ErrorMsg } from '../../../constant';
import { IModalPopupAlternateOptions } from '../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { LookUpService } from '../lookup.service';

@Component({
  selector: 'app-add-edit-lookup',
  templateUrl: './add-edit-lookup.component.html',
  styleUrls: ['./add-edit-lookup.component.scss']
})
export class AddEditLookupComponent implements OnInit {
    commonUtils = new CommonUtils()
    @Input() lookupId: number = 0;
    @Output() loadLookupList = new EventEmitter<any>();
    @ViewChild('addEditLookupModal') addEditLookupModalpopup: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;

    LookupForm: FormGroup;
    user: any;

    ModalHeading: string = 'Add Lookup';
    submitted: boolean

    isSaveButtonDisabled: boolean = false;

    constructor(private fb: FormBuilder, private lookupService: LookUpService,
        private messageService: MessageService) {
        this.user = JSON.parse(localStorage.getItem("User"));
        this.buildLookupForm({}, 'New');}

    ngOnInit(): void {
       // this.LoadDropDown();
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
    ListValueId: number;
    lookupType: string = '';
    Show(Id: number, lookId: number, lookupType: string) {
        this.ListValueId = Id;
        this.ResetDialog();
        this.isSaveButtonDisabled = false;
        if (this.ListValueId != 0) {
            this.isSaveButtonDisabled = false;
            this.GetDetails(this.ListValueId);
        } else {
            this.ModalHeading = "Add Lookup";
            this.LookupForm.controls.ListTypeID.patchValue(lookId)
           /* this.LookupForm.value.ListTypeID = lookId*/
            this.lookupType = lookupType;
            this.addEditLookupModalpopup.show();
        }
    }
    GetDetails(Id: number) {
        this.lookupService.getListValueByIdAsync(Id).subscribe(result => {
            if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                this.lookupType = result['data']['typeDesc'];
                this.buildLookupForm(result['data'], 'Edit');
                this.ModalHeading = "Edit Lookup";
                this.addEditLookupModalpopup.show();
            }
        })
    }

    buildLookupForm(data: any, keyName: string) {
        this.LookupForm = this.fb.group({
            ListValueID: [keyName === 'New' ? 0 : data.listValueID],
            ListTypeID: [keyName === 'New' ? 0 : data.listTypeID],
            Value: [keyName === 'New' ? '' : data.value, Validators.required],
            ValueDesc: [keyName === 'New' ? '' : data.valueDesc, Validators.required],
            IsActive: [keyName === 'New' ? true : data.isActive],
            CreatedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : this.user.firstName + " " + this.user.lastName],
            CreatedDate: [keyName === 'New' ? new Date() : data.createdDate],
            ModifiedDate: [keyName === 'New' ? new Date() : data.modifiedDate],
            ModifiedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : data.modifiedBy],
            TimeStamp: ['']
        });
    }

    get addEditLookupFormControls() { return this.LookupForm.controls; }

    save = () => {
        this.submitted = true;
        if (this.LookupForm.invalid) {
            this.LookupForm.markAllAsTouched();
            return;
        } else {
            if (this.isSaveButtonDisabled) {
                return;
            } else {
                this.isSaveButtonDisabled = true;
                if (this.ListValueId == 0) {
                   this.lookupService.SaveLookup(this.LookupForm.value).subscribe(result => {
                        if (result['data'] !== null && result['messageType'] === 1) {
                            this.messageService.add({ severity: 'success', summary: 'Lookup saved successfully', detail: '' });
                            this.isSaveButtonDisabled = false;
                            this.loadModalOptions();
                            this.loadLookupList.emit(this.lookupId);
                        } else {
                            this.isSaveButtonDisabled = false;
                            this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                        }
                    })
                    this.submitted = false;
                    this.cancel();
                }
                else if (this.ListValueId > 0) {
                    this.LookupForm.value.ModifiedBy = this.user.firstName + " " + this.user.lastName;
                    this.LookupForm.value.ModifiedDate = this.commonUtils.defaultDateTimeLocalSet(new Date());
                    this.lookupService.UpdateLookup(this.LookupForm.value).subscribe(result => {
                        if (result['data'] !== null && result['messageType'] === 1) {
                            this.messageService.add({ severity: 'success', summary: 'Lookup saved successfully', detail: '' });
                            this.isSaveButtonDisabled = false;
                            this.loadModalOptions();
                            this.loadLookupList.emit(this.lookupId);

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
        this.ListValueId = -1;
        this.isSaveButtonDisabled = false;
        this.addEditLookupModalpopup.hide();
        this.buildLookupForm({}, 'New');
    }
    ResetDialog() {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildLookupForm({}, 'New');
    }

}
