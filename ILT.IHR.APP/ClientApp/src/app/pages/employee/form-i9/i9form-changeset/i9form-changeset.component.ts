import { Component, OnInit, ViewChild } from '@angular/core';
import * as moment from 'moment';
import { CommonUtils } from '../../../../common/common-utils';
import { IModalPopupAlternateOptions } from '../../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { ITableHeaderAction, ITableRowAction } from '../../../../shared/ihr-table/table-options';
import { FormI9Service } from '../formI9.service';

@Component({
  selector: 'app-i9form-changeset',
  templateUrl: './i9form-changeset.component.html',
  styleUrls: ['./i9form-changeset.component.scss']
})
export class I9formChangesetComponent implements OnInit {
    assetChangeSet: any[] = [];
    commonUtils = new CommonUtils()
    @ViewChild('notificationModal') notificationModal: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;
    isSaveButtonDisabled: boolean = false;
    ModalHeading: string = 'FormI9 Changes';
    isShow: boolean;

    cols: any[] = [];
    selectedColumns: any[];
    isPagination: boolean = true;
    loading: boolean = false;
    showCurrentPageReport: boolean = true;
    rowActions: ITableRowAction[] = [];
    headerActions: ITableHeaderAction[] = [];
    patientLanding: string = 'patientLanding';
    globalFilterFields = ['FirstName', 'assignedTo', 'status']

    constructor(private formI9Service: FormI9Service) { }

    ngOnInit(): void {
        this.cols = [
            { field: 'firstName', header: 'First Name' },
            { field: 'middleName', header: 'Middle Name' },
            { field: 'lastName', header: 'Last Name' },
            { field: 'address1', header: 'Address1' },
            { field: 'address2', header: 'Address2' },
            { field: 'city', header: 'City' },
            { field: 'state', header: 'State' },
            { field: 'country', header: 'Country' },
            { field: 'zipCode', header: 'ZipCode' },
            { field: 'birthDate', header: 'BirthDate' },
            { field: 'ssn', header: 'SSN' },
            { field: 'phone', header: 'Phone' },
            { field: 'email', header: 'Email' },
            { field: 'aNumber', header: 'ANumber' },
            { field: 'USCISNumber', header: 'USCISNumber' },
            { field: 'i94Number', header: 'I94Number' },
            { field: 'i94ExpiryDate', header: 'I94ExpiryDate' },
            { field: 'hireDate', header: 'HireDate' },
            { field: 'listADocumentTitle', header: 'ListA Document Title' },
            { field: 'listAIssuingAuthority', header: 'ListA Issuing Authority' },
            { field: 'listADocumentNumber', header: 'ListA Document Number' },
            { field: 'listAStartDate', header: 'ListA StartDate' },
            { field: 'listAExpirationDate', header: 'ListA Expiration Date' },
            { field: 'listBDocumentTitle', header: 'ListB DocumentTitle' },
            { field: 'listBIssuingAuthority', header: 'ListB Issuing Authority' },
            { field: 'listBDocumentNumber', header: 'ListB Document Number' },
            { field: 'listBStartDate', header: 'ListB Start Date' },
            { field: 'listBExpirationDate', header: 'ListB Expiration Date' },
            { field: 'listCDocumentTitle', header: 'ListC DocumentTitle' },
            { field: 'listCIssuingAuthority', header: 'ListC Issuing Authority' },
            { field: 'listCDocumentNumber', header: 'ListC Document Number' },
            { field: 'listCStartDate', header: 'ListC Start Date' },
            { field: 'listCExpirationDate', header: 'ListC Expiration Date' },
            { field: 'modifiedBy', header: 'Modified By' },
            { field: 'modifiedDate', header: 'Modified Date' },
        ];

        this.selectedColumns = this.cols;
        this.modalOptions = {
            footerActions: []
        }
        this.headerActions = [];
  }

    show(assetID: number) {
        this.assetChangeSet = [];
        this.formI9Service.GetFormI9Changeset(assetID).subscribe(result => {
            if (result['data'] !== null && result['messageType'] === 1) {
                result['data'].forEach(d => {
                    if (d.i94ExpiryDate !== null) {
                        d.i94ExpiryDate = moment(d.i94ExpiryDate).format("MM/DD/YYYY")
                    }

                    if (d.listAExpirationDate !== null) {
                        d.listAExpirationDate = moment(d.listAExpirationDate).format("MM/DD/YYYY")
                    }
                    if (d.listAStartDate !== null) {
                        d.listAStartDate = moment(d.listAStartDate).format("MM/DD/YYYY")
                    }
                    if (d.listBExpirationDate !== null) {
                        d.listBExpirationDate = moment(d.listBExpirationDate).format("MM/DD/YYYY")
                    }
                    if (d.listBStartDate !== null) {
                        d.listBStartDate = moment(d.listBStartDate).format("MM/DD/YYYY")
                    }

                    if (d.listCExpirationDate !== null) {
                        d.listCExpirationDate = moment(d.listCExpirationDate).format("MM/DD/YYYY")
                    }
                    if (d.listCStartDate !== null) {
                        d.listCStartDate = moment(d.listCStartDate).format("MM/DD/YYYY")
                    }

                    if (d.modifiedDate !== null) {
                        d.modifiedDate = this.commonUtils.formatDateTimeWithAmPM(d.modifiedDate);
                    }
                    if (d.birthDate !== null) {
                        d.birthDate = moment(d.birthDate).format("MM/DD/YYYY")
                    }
                    if (d.hireDate !== null) {
                        d.hireDate = moment(d.hireDate).format("MM/DD/YYYY")
                    }
                })
                this.assetChangeSet = result['data'];
                this.notificationModal.show();
            }
        })
    }

}
