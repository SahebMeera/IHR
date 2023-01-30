import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { forkJoin } from 'rxjs';
import { CommonUtils } from '../../../common/common-utils';
import { AssetStatusConstants, Constants, ErrorMsg, ListTypeConstants, SessionConstants, Settings } from '../../../constant';
import { IAssetDisplay } from '../../../core/interfaces/Asset';
import { IEmployeeDisplay } from '../../../core/interfaces/Employee';
import { IRolePermissionDisplay } from '../../../core/interfaces/RolePermission';
import { IModalPopupAlternateOptions } from '../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { EmployeeService } from '../../employee/employee.service';
import { LookUpService } from '../../lookup/lookup.service';
import { AssetService } from '../asset.service';

@Component({
  selector: 'app-add-edit-asset',
  templateUrl: './add-edit-asset.component.html',
  styleUrls: ['./add-edit-asset.component.scss']
})
export class AddEditAssetComponent implements OnInit {
    commonUtils = new CommonUtils()
    @Output() AssetUpdated = new EventEmitter<any>();
    @ViewChild('addEditAssetModal') addEditAssetModal: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;

    AssetForm: FormGroup;
    user: any;
    ModalHeading: string = 'Add Asset';
    submitted: boolean
    RolePermissions: IRolePermissionDisplay[] = [];
    AssetInfoRolePermission: IRolePermissionDisplay;
    isSaveButtonDisabled: boolean = false;
    ClientID: string;
    AssetId: number;
    EmployeeID: number;
    isAssignedTemp: boolean = false;
    disabledvalue: boolean = false;


    constructor(private fb: FormBuilder,
        private employeeService: EmployeeService,
        private messageService: MessageService,
        private assetService: AssetService,
        private lookupService: LookUpService) {
        this.ClientID = localStorage.getItem("ClientID");
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.AssetInfoRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.ASSET);
        this.user = JSON.parse(localStorage.getItem("User"));
        this.LoadDropDown();
        this.buildAssetForm({}, 'New');
    }

    ngOnInit(): void {
        this.loadModalOptions();
    }

    loadModalOptions() {
        this.modalOptions = {
            footerActions: [
                {
                    actionText: 'Save',
                    actionMethod: this.checkTagExist,
                    styleClass:'btn-width-height p-button-raised p-mr-2 p-mb-2',
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
    AssetList: IAssetDisplay[] = []
    AssetTypeList: any[] = []
    AssetStatusList: any[] = []
    AssetStatus2List: any[] = []
    LoadDropDown() {
        forkJoin(
            this.employeeService.GetEmployees(),
            this.lookupService.getListValues(),
            this.assetService.GetAsset()
        ).subscribe(resultSet => {
            if (resultSet !== undefined && resultSet !== null) {
                this.Employees = resultSet[0]['data'].filter(x => x.termDate === null);
                //this.Employees = lisEmp;
                this.AssetTypeList = resultSet[1]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.ASSETTYPE);
                this.AssetStatusList = resultSet[1]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.ASSETSTATUS);
                this.AssetStatus2List = resultSet[1]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.ASSETSTATUS);
                this.AssetList = resultSet[2]['data']
            }
        })
    }

    ErrorMessage: string;
    isAssignedRequired: boolean = false;
    Show(Id: number) {
        this.AssetId = Id;
         this.ResetDialog();
        if (this.AssetId != 0) {
            this.ErrorMessage = '';
            this.disabledvalue = true;
            this.LoadDropDown()
            this.GetDetails(this.AssetId);
        }
        else {
            this.ErrorMessage = ''
            this.disabledvalue = false;
            this.isAssignedTemp = false;
            this.isAssignedRequired = false;
            this.ModalHeading = "Add Asset";
            this.buildAssetForm({}, 'New');
            //this.TickeForm.controls.RequestedByID.patchValue(employeeId)
            //this.Ticket.RequestedByID = employeeId;
            //// Ticket.ResolvedDate = DateTime.Now;
            //Ticket.CreatedDate = DateTime.Now;
            this.loadModalOptions();
            this.addEditAssetModal.show();
        }
    }
    Asset: IAssetDisplay;
    isAssignedTempRequired: boolean = false;
    GetDetails(Id: number) {
        this.assetService.getAssetByIdAsync(Id).subscribe(result => {
            if (result !== null && result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                this.Asset = result['data'];
                this.AssetId = this.Asset.assetID;
                this.isAssignedRequired = false;
                this.isAssignedTempRequired = false;
                if (this.Asset.statusID != null) {
                    var AssetStatus = this.AssetStatus2List.find(x => x.listValueID === this.Asset.statusID);
                    if (AssetStatus != null && AssetStatus.value.toUpperCase() === AssetStatusConstants.ASSIGNEDTEMP) {
                        this.isAssignedTemp = true;
                    }
                    else {
                        this.isAssignedTemp = false;
                        if (AssetStatus != null && AssetStatus.value.toUpperCase() == AssetStatusConstants.ASSIGNED) {
                            this.AssetStatusList = this.AssetStatus2List.filter(x => x.value.toUpperCase() !== AssetStatusConstants.DECOMMISSIONED && x.value.toUpperCase() !== AssetStatusConstants.UNASSIGNED);
                        }
                        else {
                            this.AssetStatusList = this.AssetStatus2List.filter(x => x.value.toUpperCase() !== AssetStatusConstants.ASSIGNED);
                        }
                    }
                }
                this.buildAssetForm(result['data'], 'Edit')
                this.isSaveButtonDisabled = false;
                this.submitted = false;
                this.loadModalOptions();
                this.ModalHeading = "Edit Asset";
                this.loadModalOptions();
                // this.isCommentExist = false;
                this.addEditAssetModal.show();
            }
        })
    }

    buildAssetForm(data: any, keyName: string) {
        this.AssetForm = this.fb.group({
            AssetID: [keyName === 'New' ? 0 : data.assetID],
            AssetTypeID: [keyName === 'New' ? null : data.assetTypeID, Validators.required],
            AssetType: [keyName === 'New' ? '' : data.assetType],
            Tag: [keyName === 'New' ? '' : data.tag, Validators.required],
            Make: [keyName === 'New' ? null : data.make],
            Model: [keyName === 'New' ? '' : data.model],
            Configuration: [keyName === 'New' ? '' : data.configuration, Validators.required],
            WiFiMAC: [keyName === 'New' ? '' : data.wiFiMAC],
            LANMAC: [keyName === 'New' ? '' : data.lanmac],
            OS: [keyName === 'New' ? '' : data.os],
            AssignedToID: [keyName === 'New' ? null : data.assignedToID],
            AssignedTo: [keyName === 'New' ? '' : data.assignedTo],
            StatusID: [keyName === 'New' ? null : data.statusID, Validators.required],
            Status: [keyName === 'New' ? null : data.status],
            Comment: [keyName === 'New' ? '' : data.comment],
            PurchaseDate: [keyName === 'New' ? new Date() : data.purchaseDate !== null ? new Date(data.purchaseDate) : null],
            WarantyExpDate: [keyName === 'New' ? null : data.warantyExpDate !== null ? new Date(data.warantyExpDate) : null],
            CreatedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : this.user.firstName + " " + this.user.lastName],
            CreatedDate: [keyName === 'New' ? new Date() : new Date(data.createdDate)],
            ModifiedDate: [keyName === 'New' ? new Date() : data.modifiedDate],
            ModifiedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : data.modifiedBy],
            TimeStamp: ['']
        });
        if (keyName === 'New' && this.AssetStatus2List !== null && this.AssetStatus2List.length > 0) {
            this.AssetStatusList = this.AssetStatus2List.filter(x => x.value.toUpperCase() !== AssetStatusConstants.ASSIGNED);
            var status = this.AssetStatusList.find(x => x.valueDesc.toUpperCase() === AssetStatusConstants.UNASSIGNED);

            if (status !== undefined && status.listValueID !== null && status.listValueID !== undefined) {
                this.AssetForm.controls.StatusID.patchValue(status.listValueID)
            }
         }
    }
    get addEditTicketControls() { return this.AssetForm.controls; }


    onAssignedChange(e: any) {
        this.ErrorMessage = "";
        if (Number(e.value) != 0) {
            this.isSaveButtonDisabled = false;
            this.onAssetStatus(Number(e.value));
        }
    }

    onAssetStatus(status) {
        if (Number(status) != 0 && status != null) {
            this.isAssignedRequired = false;
            this.AssetStatusList = this.AssetStatus2List.filter(x => x.value.toUpperCase() !== AssetStatusConstants.DECOMMISSIONED && x.value.toUpperCase() !== AssetStatusConstants.UNASSIGNED);
            var assetStatus = this.AssetStatusList.find(x => x.valueDesc.toUpperCase() === AssetStatusConstants.ASSIGNED);
            if (assetStatus !== undefined && assetStatus.listValueID !== null && assetStatus.listValueID !== undefined) {
                this.AssetForm.controls.StatusID.patchValue(assetStatus.listValueID)
            }
        }
        else {
            this.AssetStatusList = this.AssetStatus2List.filter(x => x.value.toUpperCase() !== AssetStatusConstants.ASSIGNED);
            var assetStatus = this.AssetStatusList.find(x => x.valueDesc.toUpperCase() === AssetStatusConstants.UNASSIGNED).listValueID;
            if (assetStatus !== undefined && assetStatus.listValueID !== null && assetStatus.listValueID !== undefined) {
                this.AssetForm.controls.StatusID.patchValue(assetStatus.listValueID)
            }
        }
    }

    onAssignedTempChange() {
        var AssignedTo = this.AssetForm.value.AssignedTo;
        if (AssignedTo !== '' && AssignedTo !== undefined && AssignedTo !== null) {
            this.isAssignedTempRequired = false;
        }
        else {
            this.isAssignedTempRequired = true;
        }
    }
    onStatusChange(e: any) {
        if (Number(e.value) != 0 && e.value != null) {
            var status = Number(e.value);
            var AssetStatus = this.AssetStatusList.find(x => x.listValueID === status);
            if (AssetStatus != null && AssetStatus.value.toUpperCase() == AssetStatusConstants.ASSIGNEDTEMP) {
                this.isAssignedRequired = false;
                this.isAssignedTemp = true;
                this.isAssignedTempRequired = true;
                this.AssetForm.controls.AssignedTo.patchValue('')
                this.AssetForm.controls.AssignedToID.patchValue(null)
            }
            else {
                if (AssetStatus !== null && AssetStatus.value.toUpperCase() === AssetStatusConstants.ASSIGNED && this.Asset !== undefined && this.Asset.assignedToID == null) {
                    this.isAssignedRequired = true;
                   // Asset.AssignedTo = "";
                    this.AssetForm.controls.AssignedTo.patchValue('')
                    this.isAssignedTemp = false;
                    this.isAssignedTempRequired = false;
                } else {
                    this.AssetForm.controls.AssignedTo.patchValue('')
                   // Asset.AssignedTo = "";
                    this.isAssignedTemp = false;
                    this.isAssignedRequired = false;
                    this.isAssignedTempRequired = false;
                }
            }
        }
        else {
            this.isAssignedTemp = false;
            this.isAssignedTempRequired = false;
        }
    }

    checkTagExist = () => {
        this.ErrorMessage = '';
        this.submitted = true;
        if (this.AssetForm.invalid) {
            this.AssetForm.markAllAsTouched();
            return;
        } else {
            if (this.isSaveButtonDisabled) {
                return;
            } else {
                this.isSaveButtonDisabled = true;
                if (this.AssetId != 0) {
                    if (this.AssetForm.value.Tag !== '' && this.AssetForm.value.Tag !== null) {
                        if (this.AssetList.filter(x => x.assetID != this.AssetForm.value.AssetID && x.tag.toUpperCase() === this.AssetForm.value.Tag.toUpperCase()).length > 0) {
                            this.ErrorMessage = "Asset tag already exists in the system";
                            this.isSaveButtonDisabled = false;
                        }
                        else {
                            this.SaveAsset();
                        }
                    }
                }
                else {
                    if (this.AssetList.filter(x => x.tag.toUpperCase() == this.AssetForm.value.Tag.toUpperCase()).length > 0) {
                        this.ErrorMessage = "Asset Tag already exists in the system";
                        this.isSaveButtonDisabled = false;
                    }
                    else {
                       this.SaveAsset();
                    }
                }
                this.isSaveButtonDisabled = false;
            }
        }
    }
    SaveAsset() {
        if (this.AssetForm.value.PurchaseDate !== null) {
            this.AssetForm.value.PurchaseDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.AssetForm.value.PurchaseDate));
        }
        if (this.AssetForm.value.WarantyExpDate !== null) {
            this.AssetForm.value.WarantyExpDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.AssetForm.value.WarantyExpDate));
        }

        if (this.AssetId === 0) {
            this.assetService.SaveAsset(this.AssetForm.value).subscribe(result => {
                if (result['data'] !== null && result['messageType'] === 1) {
                    this.messageService.add({ severity: 'success', summary: 'Asset saved successfully', detail: '' });
                    this.isSaveButtonDisabled = true;
                    this.loadModalOptions();
                    this.AssetUpdated.emit();
                    this.Cancel();
                } else {
                    this.isSaveButtonDisabled = false;
                    this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                }
            })
        } else {
            this.assetService.UpdateAsset(this.AssetForm.value.AssetID, this.AssetForm.value).subscribe(result => {
                if (result['data'] !== null && result['messageType'] === 1) {
                 this.messageService.add({ severity: 'success', summary: 'Asset saved successfully', detail: '' });
                this.isSaveButtonDisabled = true;
                this.loadModalOptions();
                    this.AssetUpdated.emit();
                this.Cancel();
            } else {
                this.isSaveButtonDisabled = false;
                this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
            }
            })
        }
    }


    Cancel = () => {
        this.AssetId = -1
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildAssetForm({}, 'New');
        this.addEditAssetModal.hide();
    }

    ResetDialog() {
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildAssetForm({}, 'New');
        this.addEditAssetModal.hide();
    }

}
