import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import * as moment from 'moment';
import { MessageService } from 'primeng/api';
import { CommonUtils } from '../../../common/common-utils';
import { Constants, ErrorMsg, SessionConstants, Settings } from '../../../constant';
import { ICountryDisplay } from '../../../core/interfaces/Country';
import { ILeaveAccrualDisplay } from '../../../core/interfaces/LeaveAccrual';
import { IRolePermissionDisplay } from '../../../core/interfaces/RolePermission';
import { IModalPopupAlternateOptions } from '../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { EmployeeService } from '../../employee/employee.service';
import { HolidayService } from '../../holidays/holidays.service';
import { LeaveRequestService } from '../../leave-request/leave-request.service';
import { LookUpService } from '../../lookup/lookup.service';
import { LeaveAccrueService } from '../LeaveAccrueService';
interface LeaveMonth {
    ID: number;
    Month: Date;
    MonthName: string;
 }
@Component({
  selector: 'app-accrue-leave',
  templateUrl: './accrue-leave.component.html',
  styleUrls: ['./accrue-leave.component.scss']
})
export class AccrueLeaveComponent implements OnInit {
    settings = new Settings()
    commonUtils = new CommonUtils()
    @Output() UpdateLeaveList = new EventEmitter<any>();
    @ViewChild('addEditLeaveAccrueModal') addEditLeaveAccrueModal: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;
    user: any;
    submitted: boolean = false;
    moment: any = moment;
    ClientID: string;

    LeaveAccrueForm: FormGroup;
    RolePermissions: IRolePermissionDisplay[] = [];
    EmployeeInfoRolePermission: IRolePermissionDisplay;
    ModalHeading: string = 'Accrue Leave';
    isSaveButtonDisabled: boolean = false;
    isShow: boolean = true;
    dateTime = new Date();
    EmployeeID: number;

    constructor(private fb: FormBuilder,
        private lookupService: LookUpService,
        private holidayService: HolidayService,
        private messageService: MessageService,
        private leaveAccrueService: LeaveAccrueService,
        private leaveRequestService: LeaveRequestService,
        private employeeService: EmployeeService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.EmployeeInfoRolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.EMPLOYEEINFO);
        this.user = JSON.parse(localStorage.getItem("User"));
        this.EmployeeID = this.user.employeeID
        this.ClientID = localStorage.getItem("ClientID");
        this.dateTime.setDate(this.dateTime.getDate());
        this.LoadDropDown();
        this.buildLeaveAccrueForm({}, 'New');
    }

    ngOnInit(): void {
        this.isDisableAccrual = true;
        this.loadModalOptions();
  }
    CountryList: ICountryDisplay[] = [];

    LoadDropDown() {
        this.holidayService.getCountry().subscribe(result => {
            if (result['data'] !== undefined && result['data'] !== null) {
                this.CountryList = result['data']
            }
        })
    }

    loadModalOptions() {
        this.modalOptions = {
            footerActions: [
                {
                    actionText: 'Accrue',
                    actionMethod: this.save,
                    styleClass: 'p-button-raised p-mr-2 p-mb-2',
                    iconClass: 'mdi mdi-content-save',
                    disabled: (this.isDisableAccrual || this.isSaveButtonDisabled)
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

    buildLeaveAccrueForm(data: any, keyName: string) {
        this.LeaveAccrueForm = this.fb.group({
            LeaveAccrualID: [keyName === 'New' ? 0 : data.leaveAccrualID],
            Country: [keyName === 'New' ? null : data.country, Validators.required],
            MonthID: [keyName === 'New' ? null : data.monthID, Validators.required],
            AccruedValue: [keyName === 'New' ? 0 : data.accruedValue !== null ? parseFloat(data.accruedValue) : 0],
            AccruedDate: [keyName === 'New' ? null : data.AccruedDate !== null ? new Date(data.AccruedDate) : null],
            CreatedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : this.user.firstName + " " + this.user.lastName],
            CreatedDate: [keyName === 'New' ? new Date() : data.createdDate],
            ModifiedDate: [keyName === 'New' ? new Date() : data.modifiedDate],
            ModifiedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : data.modifiedBy],
            TimeStamp: ['']
        });
       
    }
    get addEditWFHFormControls() { return this.LeaveAccrueForm.controls; }

    onChangeCountry(e: any) {
        var country = e.value.toString();
        if (country !== null && country !== "") {
           // this.GetStates(country);
            this.LoadLeaveAccrual(country)
        }
        else {
            //this.StateList = [];
        }
    }
    isAccruvalValue: boolean = false;
    Show() {
        //this.ResetDialog();
        this.isSaveButtonDisabled = false;
        this.isDisableAccrual = true;
        this.loadModalOptions();
        this.isAccruvalValue = false;
        this.buildLeaveAccrueForm({}, 'New')
        this.addEditLeaveAccrueModal.show()
    }

    LeaveAccrualList: ILeaveAccrualDisplay[] = [];
    lstLeaveAccrual: ILeaveAccrualDisplay[] = [];
    LoadLeaveAccrual(Country: string) {
        this.leaveAccrueService.GetLeaveAccrual(Country).subscribe(respLeaveAccrual => {
            if (respLeaveAccrual !== undefined && respLeaveAccrual['data'] !== null && respLeaveAccrual['data'] !== undefined) {
                this.LeaveAccrualList = respLeaveAccrual['data'];
                this.loadMonthDropDown();
            }
        })
    }

    LeaveMonthList: LeaveMonth[] = [];

    lstMonth: any[] = [];
    loadMonthDropDown() {
        this.LeaveMonthList = [];
        // var i = 0;
        this.lstLeaveAccrual = [];
        if (this.LeaveAccrualList !== undefined && this.LeaveAccrualList !== null && this.LeaveAccrualList.length > 0) {
            this.lstLeaveAccrual = this.LeaveAccrualList.filter(x => x.leaveAccrualID === 0)
            if (this.lstLeaveAccrual !== undefined && this.lstLeaveAccrual !== null && this.lstLeaveAccrual.length > 0) {
                this.lstLeaveAccrual.forEach((accrual: any, i) => {
                    //  const LeaveMonth  = new LeaveMonth();
                    this.LeaveMonthList.push({ ID: i + 1, Month: accrual.accruedDate, MonthName: new Date(accrual.accruedDate).getFullYear() + ' - ' + moment(accrual.accruedDate).format('MMMM') })
                })
                this.lstMonth = []
                if (this.lstLeaveAccrual.length === this.LeaveMonthList.length) {
                    this.LeaveMonthList.forEach(x => {
                        this.lstMonth.push({ ID: x.ID, Value: x.MonthName });
                    })
                }
            }
        }
        
    }
    MonthID: number;
    accrualDate: Date;
    isDisableAccrual: boolean = false;
    onMonthChange(e: any) {
        this.MonthID = Number(e.value);

        if (this.MonthID !== 0) {
            this.accrualDate = this.LeaveMonthList.find(x => x.ID == this.MonthID).Month;
           // accrualMonth = accrualDate.ToString("MMMM") + " " + accrualDate.Year.ToString();
            this.isDisableAccrual = (this.LeaveAccrualList.find(x => x.accruedDate == this.accrualDate)) != null ? false : true;
            if (this.isDisableAccrual === false) {
                if (this.LeaveAccrueForm.value.AccruedValue === 0) {
                    this.isDisableAccrual = true;
                    this.loadModalOptions();
                }
            }
        }
        else {
            this.isDisableAccrual = true;
        }
    }

    isMonthSelected: boolean = false;
    onAccrualValueChange(e: any) {
        var value = Number(this.LeaveAccrueForm.value.AccruedValue);

        if (value < 1) {
            this.isAccruvalValue = true;
            this.isDisableAccrual = true;

        }
        else if (this.MonthID == 0) {
            this.isAccruvalValue = true;
            this.isDisableAccrual = true;
            this.isMonthSelected = true;

        } else {
            this.isAccruvalValue = false;
            this.isDisableAccrual = false;
            this.isMonthSelected = false;
        }
        this.loadModalOptions();
    }


    save = () => {
        this.submitted = true;
        if (this.LeaveAccrueForm.invalid) {
            this.LeaveAccrueForm.markAllAsTouched();
            return;
        } else {
            if (this.isSaveButtonDisabled) {
                return;
            } else {
                this.isSaveButtonDisabled = true;
                this.LeaveAccrueForm.controls.AccruedDate.patchValue(this.commonUtils.defaultDateTimeLocalSet(new Date(this.accrualDate)));
                this.leaveAccrueService.SaveLeaveAccrual(this.LeaveAccrueForm.value).subscribe(async result => {
                    if (result['data'] !== null && result['messageType'] === 1) {
                        this.isDisableAccrual = true;
                        this.messageService.add({ severity: 'success', summary: 'Leave Accrued saved successfully', detail: '' });
                        await this.UpdateLeaveList.emit(true);
                        this.cancel();
                    } else {
                        this.isSaveButtonDisabled = false;
                        this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                    }
                })
            }
        }
    }

    cancel = () => {
        this.submitted = false;
        this.isDisableAccrual = false;
        this.isMonthSelected = false;
        this.isAccruvalValue = false;
        this.buildLeaveAccrueForm({}, 'New');
        this.addEditLeaveAccrueModal.hide()
    }

}
