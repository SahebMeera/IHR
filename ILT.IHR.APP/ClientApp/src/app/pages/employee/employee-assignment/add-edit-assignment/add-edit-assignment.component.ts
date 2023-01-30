import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import * as moment from 'moment';
import { MessageService } from 'primeng/api';
import { forkJoin } from 'rxjs';
import { CommonUtils } from '../../../../common/common-utils';
import { Constants, ErrorMsg, ListTypeConstants, SessionConstants } from '../../../../constant';
import { IAssignmentRate, IAssignmentRateDisplay } from '../../../../core/interfaces/AssignmentRate';
import { IRolePermissionDisplay } from '../../../../core/interfaces/RolePermission';
import { IModalPopupAlternateOptions } from '../../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { CompanyService } from '../../../company/company.service';
import { EndClientService } from '../../../company/EndClientService';
import { HolidayService } from '../../../holidays/holidays.service';
import { LookUpService } from '../../../lookup/lookup.service';
import { EmerygencyService } from '../../employee-emergency/emerygency.service';
import { EmployeeService } from '../../employee.service';
import { EmployeeAssignmentService } from '../assignment.service';

interface SubClient {
    Text: string;
    isValidSubClient: boolean;
}
@Component({
  selector: 'app-add-edit-assignment',
  templateUrl: './add-edit-assignment.component.html',
  styleUrls: ['./add-edit-assignment.component.scss']
})
export class AddEditAssignmentComponent implements OnInit {
    commonUtils = new CommonUtils()
    @Input() EmployeeName: string;
    @Input() EmployeeId: number;
    @Output() ListValueUpdated = new EventEmitter<any>();
    @ViewChild('addEditAssignmentModal') addEditAssignmentModal: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;
    ContactTypeList: any[] = [];
    CountryList: any[] = [];
    user: any;
    submitted: boolean = false;
    StateList: any[] = [];

    AssignmentForm: FormGroup;
    RolePermissions: IRolePermissionDisplay[] = [];
    AssigmentsRolePermission: IRolePermissionDisplay;

    constructor(private fb: FormBuilder,
        private LookupService: LookUpService,
        private emerygencyService: EmerygencyService,
        private messageService: MessageService,
        private employeeService: EmployeeService,
        private companyService: CompanyService,
        private endClientService: EndClientService,
        private assignmentService: EmployeeAssignmentService,
        private holidayService: HolidayService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.AssigmentsRolePermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.ASSIGNMENT);
        this.user = JSON.parse(localStorage.getItem("User"));
        this.LoadDropDown();
        this.buildAssignmentForm({}, 'New');
    }

    isSaveButtonDisabled: boolean = false;
    isShow: boolean = false;
    ngOnInit(): void {
        this.loadModalOptions();
    }

    loadModalOptions() {
        this.modalOptions = {
            footerActions: [
                {
                    actionText: 'Save',
                    actionMethod: this.isCheckSubClient,
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
    WeekEndingDayList: any[] =[];
    TitleList: any[] = [];
    PaymentTypeList: any[] = [];
    CompanyList: any[] = [];
    VendorList: any[] = [];
    ClientList: any[] = [];
    endClientList: any[] = [];
    LoadDropDown() {
            forkJoin(
                this.LookupService.getListValues(),
                this.holidayService.getCountry(),
                this.companyService.getCompanyList(),
                this.endClientService.GetEndClients()
            ).subscribe(resultSet => {
                if (resultSet !== undefined && resultSet !== null) {
                    this.WeekEndingDayList = resultSet[0]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.TIMESHEETTYPE);
                    this.TitleList = resultSet[0]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.TITLE);
                    this.PaymentTypeList = resultSet[0]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.PAYMENTTYPE);
                    this.CountryList = resultSet[1]['data'];
                    this.CompanyList = resultSet[2]['data'];
                    this.VendorList = this.CompanyList.filter(x => x.companyType.toUpperCase() === "VENDOR" || x.companyType.toUpperCase() === "SELF" || x.companyType.toUpperCase() === "CLIENT/VENDOR");
                    this.ClientList = this.CompanyList.filter(x => x.companyType.toUpperCase() === "CLIENT" || x.companyType.toUpperCase() === "SELF" || x.companyType.toUpperCase() === "CLIENT/VENDOR");
                    this.endClientList = resultSet[3]['data'];
                }
            })
    }

    AssignmentId: number;
    ModalHeading: string = '';
    subClients: SubClient[] = [];
    selectedClient: string = '';
    selectedEndClient: string = '';
    Show(Id: number) {
        this.AssignmentId = Id;
        this.ResetDialog();
        if (this.AssignmentId != 0) {
            this.isShow = this.AssigmentsRolePermission.update;
            this.loadModalOptions();
            this.GetDetails(this.AssignmentId);
        }
        else {
            this.isShow = this.AssigmentsRolePermission.add;
            this.loadModalOptions();
            this.ModalHeading = "Add Assignment";
            this.subClients = [];
            this.selectedClient = "";
            this.selectedEndClient = "";
            this.buildAssignmentForm({}, 'New');
            this.AssignmentForm.controls.EmployeeID.patchValue(this.EmployeeId)
            this.AssignmentForm.controls.EmployeeName.patchValue(this.EmployeeName);
            this.loadEmployeeData(this.EmployeeId);
            this.addEditAssignmentModal.show();
        }
    }
    buildAssignmentForm(data: any, keyName: string) {
        this.AssignmentForm = this.fb.group({
            AssignmentID: [keyName === 'New' ? 0 : data.assignmentID],
            VendorID: [keyName === 'New' ? null : data.vendorID],
            Vendor: [keyName === 'New' ? '' : data.vendor],
            ClientID: [keyName === 'New' ? null : data.clientID, Validators.required],
            Client: [keyName === 'New' ? '' : data.client],
            EndClientID: [keyName === 'New' ? null : data.endClientID, Validators.required],
            PaymentTypeID: [keyName === 'New' ? null : data.paymentTypeID, Validators.required],
            PaymentType: [keyName === 'New' ? '' : data.paymentType],
            TimesheetTypeID: [keyName === 'New' ? null : data.timeSheetTypeID],
            TimesheetType: [keyName === 'New' ? '' : data.timeSheetType],
            EmployeeID: [keyName === 'New' ? null : data.employeeID],
            StartDate: [keyName === 'New' ? new Date() : new Date(data.startDate), Validators.required],
            EndDate: [keyName === 'New' ? null : data.endDate !== null ? new Date(data.endDate) : null],
            ClientManager: [keyName === 'New' ? '' : data.clientManager],
            EmployeeName: [keyName === 'New' ? '' : data.employeeName],
            Title: [keyName === 'New' ? '' : data.title],
            Address1: [keyName === 'New' ? '' : data.address1, Validators.required],
            Address2: [keyName === 'New' ? '' : data.address2],
            City: [keyName === 'New' ? '' : data.city, Validators.required],
            State: [keyName === 'New' ? '' : data.state, Validators.required],
            Country: [keyName === 'New' ? '' : data.country, Validators.required],
            ZipCode: [keyName === 'New' ? '' : data.zipCode, Validators.required],
            SubClient: [keyName === 'New' ? '' : data.subClient],
            Comments: [keyName === 'New' ? '' : data.comments],
            TSApproverEmail: [keyName === 'New' ? '' : data.tSApproverEmail],
            ApprovedEmailTo: [keyName === 'New' ? '' : data.approvedEmailTo],
            CreatedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : this.user.firstName + " " + this.user.lastName],
            CreatedDate: [keyName === 'New' ? new Date() : data.createdDate],
            ModifiedDate: [keyName === 'New' ? new Date() : data.modifiedDate],
            ModifiedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : data.modifiedBy],
            TimeStamp: ['']
        });
    }
    get addEditAssignmentControls() { return this.AssignmentForm.controls; }
    Employee: any;
    loadEmployeeData(employeeID: number) {
        this.employeeService.getEmployeeByIdAsync(employeeID).subscribe(result => {
            if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                if (result['data'].employeeID !== 0) {
                    this.Employee = result['data'];
                    this.AssignmentForm.controls.Country.patchValue(result['data'].country)
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
        if (this.CountryList != null && this.CountryList.length > 0) {
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

   onChangeClient(e: any) {
        var client: number = Number(e.value);
        this.subClients = [];
        this.selectedClient = "";
        this.selectedEndClient = "";
        //assignment.TimesheetApproverID = null;
        if (client != 0) {
            var endClient = this.ClientList.find(x => x.isEndClient === true && x.companyID === client);
            if (endClient !== null && endClient !== undefined) {
                this.selectedClient = endClient.name;
                this.AssignmentForm.controls.Client.patchValue(this.selectedClient);
                var isCheck = this.endClientList.find(x => x.name.toUpperCase() === endClient.name.toUpperCase());
                this.selectedEndClient = isCheck !== undefined && isCheck.name !== undefined ? isCheck.name : "";
                if (isCheck !== null && isCheck !== undefined) {
                    this.AssignmentForm.controls.EndClientID.patchValue(isCheck.endClientID);
                    //assignment.EndClientID = isCheck.EndClientID;
                    //assignment.SubClient = "";
                }
            } else {
                var clientObj = this.ClientList.find(x => x.companyID === client);
                this.selectedClient = clientObj !== undefined ? clientObj.name : '';
                this.AssignmentForm.controls.Client.patchValue(this.selectedClient);
                //assignment.Client = selectedClient;
                this.AssignmentForm.controls.EndClientID.patchValue(null);
                //assignment.EndClientID = 0;
                //assignment.SubClient = "";
            }


            //await GetTimeSheetApproverList(client);
        } else {
            //timesheetApproverList = new List<DTO.User> { };
        }
   }

    onChangeEndClient(e: any) {
        var endclient = Number(e.value);
        if (endclient != 0) {
            var isCheck = this.endClientList.find(x => x.endClientID === endclient);
            if (isCheck !== undefined) {
                this.selectedEndClient = isCheck.name;
            }
        }
    }

 
    addSubClient(isInsertAtTop: boolean, sub: SubClient) {
        if (isInsertAtTop === true) {
            if (this.selectedClient != "" && this.selectedEndClient != "") {
                //if(selectedClient.ToUpper() != selectedEndClient.ToUpper())
                //{
                this.subClients.splice(0, 0, sub);
                //}

            }
        } else {
            if (sub !== undefined) {
                var index = this.subClients.findIndex(x => x.Text === sub.Text);
                this.subClients.splice(index + 1, 0, { Text: null, isValidSubClient: false});
            }
        }

    }
    removeSubClient(subClient: SubClient) {
        var index = this.subClients.findIndex(x => x.Text === subClient.Text);
        this.subClients.splice(index, 1);
    }

    isEndClient(): boolean {
        var client = this.ClientList.find(x => x.isEndClient == true && x.companyID == this.AssignmentForm.value.ClientID);
        if (client != null) {
            var isCheck = this.endClientList.find(x => x.name.toUpperCase() === this.selectedEndClient.toUpperCase() && x.companyID !== 0 && x.companyID === client.companyID);
            if (isCheck !== null && isCheck !== undefined) {
                return false;
            } else {
                return true;
            }
        }
        else {
            return true;
        }
    }

    EmployeeAssignmentRates: IAssignmentRateDisplay[] = [];
    GetDetails(Id: number) {
        this.selectedClient = "";
        this.selectedEndClient = "";
        this.subClients = [];
        this.EmployeeAssignmentRates = [];
        this.assignmentService.getEmployeeAssignmentById(Id).subscribe(result => {
            if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                this.buildAssignmentForm(result['data'], "Edit");
                var assignment = result['data'];
                this.AssignmentId = assignment.assignmentID;
                this.EmployeeAssignmentRates = assignment.assignmentRates !== null || assignment.assignmentRates.length > 0 ? assignment.assignmentRates : [];
                 var client = this.ClientList != null ? this.ClientList.find(x => x.companyID === assignment.clientID) : null;
                this.selectedClient = client != null ? client.name : "";
                var endClient = this.endClientList != null ? this.endClientList.find(x => x.endClientID === assignment.endClientID) : null;
                this.selectedEndClient = endClient != null ? endClient.name : "";
                console.log(assignment.subClient)
                if (assignment.subClient !== null && assignment.subClient !== '' && assignment.subClient !== '') {
                    var subClientList = assignment.subClient.split(',');
                    if (subClientList !== undefined && subClientList !== null) {
                        subClientList.forEach(x => {
                            this.subClients.push({ Text: x, isValidSubClient: false})
                        })
                    }
                }
                if (assignment.country != null) {
                    this.GetStates(assignment.country);
                }
                else {
                    this.StateList = [];
                }
                this.ModalHeading = 'Edit Assignment';
                this.addEditAssignmentModal.show();
            }
        })
    }

     AEndDateCheck: boolean = false;
    onchangeAssignmentEndDate(e: any) {
        this.AEndDateCheck = false;
        this.ErrorMessage = "";
        if (e !== undefined) {
            var assignmentDate = moment(e, 'yyyy-mm-dd');
            var employeeTermDate = this.Employee !== null && this.Employee !== undefined && this.Employee.termDate !== null && this.Employee.termDate !== undefined ? moment(this.Employee.termDate, 'yyyy-mm-dd') : null;
            if (employeeTermDate !== null && assignmentDate !== null && assignmentDate !== undefined) {
                if (assignmentDate >= employeeTermDate) {
                    this.AEndDateCheck = true;
                    this.ErrorMessage = "Employee has active Assignments. Please terminate Assignments before terminating Employee.";
                    this.AssignmentForm.controls.EndDate.patchValue(null);
                } else {
                    this.AEndDateCheck = false;
                    this.ErrorMessage = '';
                }
            } else {
                this.AEndDateCheck = false;
                this.ErrorMessage = '';
            }
        }
    }

    isCheckSubClient = () => {
        this.submitted = true;
        if (this.AssignmentForm.invalid) {
            this.AssignmentForm.markAllAsTouched();
            return;
        } else {
            if (this.isSaveButtonDisabled) {
                return;
            } else {
                this.isSaveButtonDisabled = true;
                if (this.subClients !== null && this.subClients !== undefined && this.subClients.length > 0) {
                    var subClient = this.subClients.find(x => x.Text === null || x.Text === undefined || x.Text === '')
                    if (subClient === undefined) {
                        this.SaveAssigment();
                    } else {
                        this.messageService.add({ severity: 'error', summary: 'SubClient open please fill', detail: '' });
                    }
                } else {
                    this.SaveAssigment();
                }
            }
        }
    }


    SaveAssigment() {
        this.isSaveButtonDisabled = true;
        if (this.subClients != null && this.subClients.length > 0) {
            var subClient = this.subClients.map(u => u.Text).join(', ')
            this.AssignmentForm.controls.SubClient.patchValue(subClient);
        }
        if (this.AssignmentForm.value.EndDate !== null && this.AEndDateCheck === false) {
            this.ErrorMessage = "";
            var EmployeeAssignmentRate = this.EmployeeAssignmentRates.length > 0 ? this.EmployeeAssignmentRates.find(x => x.endDate == null) : null;
            if (EmployeeAssignmentRate !== null && EmployeeAssignmentRate !== undefined) {
                this.ErrorMessage = "Assignment has active AssignmentRates. Please terminate AssignmentRates before terminating Assignment.";
            }
            else {
                var assignmentEndDate = this.AssignmentForm.value.EndDate !== null ? new Date(moment(this.AssignmentForm.value.EndDate).format('MM/DD/YYYY').toString()) : null;
                var IscheckAssignmentRate = this.EmployeeAssignmentRates.length > 0 ? this.EmployeeAssignmentRates.find(x => new Date(x.endDate) > assignmentEndDate) : null;
                if (IscheckAssignmentRate != null && IscheckAssignmentRate !== undefined) {
                    this.ErrorMessage = "End date must be greater than or equal to end dates of AssignmentRate.";

                } else {
                    this.AssignmentSave();
                }
            }
        }
        else {
            this.AssignmentSave();
        }
        this.isSaveButtonDisabled = false;
    }

    AssignmentSave() {
        this.AssignmentForm.value.StartDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.AssignmentForm.value.StartDate));
        if (this.AssignmentForm.value.EndDate !== null) {
            this.AssignmentForm.value.EndDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.AssignmentForm.value.EndDate));
        }
        if (this.AssignmentForm.value.EndDate === null || this.AssignmentForm.value.StartDate <= this.AssignmentForm.value.EndDate) {
            if (this.AssignmentId === 0) {
                this.assignmentService.SaveEmployeeAssignment(this.AssignmentForm.value).subscribe(result => {
                    if (result['data'] !== null && result['messageType'] === 1) {
                        this.messageService.add({ severity: 'success', summary: 'Assignment saved successfully', detail: '' });
                        this.isSaveButtonDisabled = true;
                        this.loadModalOptions();
                        this.ListValueUpdated.emit();
                        this.cancel();
                    } else {
                        this.isSaveButtonDisabled = false;
                        this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                    }
                })
              
            }
            else if (this.AssignmentId !== 0) {
                this.assignmentService.UpdateEmployeeAssignment(this.AssignmentForm.value).subscribe(result => {
                    if (result['data'] !== null && result['messageType'] === 1) {
                        this.messageService.add({ severity: 'success', summary: 'Assignment saved successfully', detail: '' });
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
        } else {
            this.ErrorMessage = "End date must be greater than start date";
            // toastService.ShowError("End date must be greater than start date", "");
        }
    
    }
    cancel = () => {
        this.submitted = false;
        this.ErrorMessage = '';
        this.isSaveButtonDisabled = false;
        this.buildAssignmentForm({}, 'New');
        this.addEditAssignmentModal.hide();
    }
    ErrorMessage: string = '';
    ResetDialog() {
        this.submitted = false;
        this.ErrorMessage = '';
        this.isSaveButtonDisabled = false;
        this.buildAssignmentForm({}, 'New');
        this.addEditAssignmentModal.hide();
    }

}
