import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { CommonUtils } from '../../../common/common-utils';
import { AssetStatusConstants } from '../../../constant';
import { IModalPopupAlternateOptions } from '../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { ITableHeaderAction, ITableRowAction } from '../../../shared/ihr-table/table-options';
import { AssetService } from '../asset.service';

@Component({
  selector: 'app-asset-changeset',
  templateUrl: './asset-changeset.component.html',
  styleUrls: ['./asset-changeset.component.scss']
})
export class AssetChangesetComponent implements OnInit {
    assetChangeSet: any[] = [];
    commonUtils = new CommonUtils()
    @ViewChild('notificationModal') notificationModal: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;
    isSaveButtonDisabled: boolean = false;
    ModalHeading: string = 'Asset Changes';
    isShow: boolean;

    cols: any[] = [];
    selectedColumns: any[];
    isPagination: boolean = true;
    loading: boolean = false;
    showCurrentPageReport: boolean = true;
    rowActions: ITableRowAction[] = [];
    headerActions: ITableHeaderAction[] = [];
    patientLanding: string = 'patientLanding';
    globalFilterFields = ['assetType','assignedTo', 'status']

    constructor(private assetService: AssetService) { }

    ngOnInit(): void {
        this.cols = [
            { field: 'assetType', header: 'Asset Type' },
            { field: 'assignedTo', header: 'Assigned To' },
            { field: 'status', header: 'Status' },
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
        this.assetService.GetAssetChangeset(assetID).subscribe(result => {
            if (result['data'] !== null && result['messageType'] === 1) {
                result['data'].forEach(d => {
                    if (d.modifiedDate !== null) {
                        d.modifiedDate = this.commonUtils.formatDateTimeWithAmPM(d.modifiedDate);
                    }
                })
                this.assetChangeSet = result['data'];
                this.notificationModal.show();
            }
        })
    }
}
