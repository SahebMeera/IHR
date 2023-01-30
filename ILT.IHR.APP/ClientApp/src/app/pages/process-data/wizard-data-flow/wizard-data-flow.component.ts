import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import * as moment from 'moment';
import { MessageService } from 'primeng/api';
import { forkJoin } from 'rxjs';
import { CommonUtils } from '../../../common/common-utils';
import { Constants, Countries, ListTypeConstants, Settings, WizardStatusConstants } from '../../../constant';
import { IEmployeeDisplay } from '../../../core/interfaces/Employee';
import { IProcessData, IProcessDataDisplay } from '../../../core/interfaces/ProcessData';
import { IProcessDataTicketDisplay } from '../../../core/interfaces/ProcessDataTicket';
import { IFields } from '../../../core/interfaces/Processwizard';
import { IRolePermissionDisplay } from '../../../core/interfaces/RolePermission';
import { DataProvider } from '../../../core/providers/data.provider';
import { IModalPopupAlternateOptions } from '../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { ITableHeaderAction, ITableRowAction } from '../../../shared/ihr-table/table-options';
import { EmailApprovalService } from '../../employee/emailApproval.service';
import { EmployeeService } from '../../employee/employee.service';
import { HolidayService } from '../../holidays/holidays.service';
import { LookUpService } from '../../lookup/lookup.service';
import { RolePermissionService } from '../../role-permission/role-permission.service';
import { AddEditTicketComponent } from '../../ticket/add-edit-ticket/add-edit-ticket.component';
import { ProcessDataService } from '../process-data.service';
import { ProcessWizardService } from '../process-wizard.service';

@Component({
  selector: 'app-wizard-data-flow',
  templateUrl: './wizard-data-flow.component.html',
  styleUrls: ['./wizard-data-flow.component.scss']
})
export class WizardDataFlowComponent implements OnInit {

    countries = Countries;
    commonUtils = new CommonUtils()
    myDate = new Date();
    RolePermissions: IRolePermissionDisplay[] = [];
    EmployeeRolePermission: IRolePermissionDisplay;
    user: any;
    isEditMode: boolean = false;
    canUpdate: boolean;
    employee: IEmployeeDisplay
    EmployeeList: IEmployeeDisplay[] = [];
    EmpEmail: string = '';
    EmpWorkEmail: string = '';
    isShowW4I9Info: boolean = false;


    settings = new Settings()
    @Output() WizardDataUpdated = new EventEmitter<any>();
    @ViewChild('WizardDataFlowModal') WizardDataFlowModal: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;
    submitted: boolean = false;
    moment: any = moment;

    ProcessDataForm: FormGroup;
    EmployeeInfoRolePermission: IRolePermissionDisplay;
    ExpenseId: number;
    ModalHeading: string = '';
    isSaveButtonDisabled: boolean = false;
    isShow: boolean = true;
    dateTime = new Date();

    CountryList: any[] = [];
    EmployMentList: any[] = [];
    TitleList: any[] = [];
    InvoicingPeriodList: any[] = [];
    PaymentTermList: any[] = [];
    GenderList: any[] = [];
    WorkAuthorizationList: any[] = [];
    MaritalStautsList: any[] = [];
    PaymentTypeList: any[] = [];
    WizardStatusList: any[] = [];
    wizarddata: IProcessDataDisplay;

    isShowTickets: boolean = false;
    isControlDisable: boolean = false;
    isProcessBtnClick: boolean = false;
    stepErrorMessage: string = '';
    ClientID: string;
    wizarddataForSave: IProcessData;

    EmployeeAssigmentRolePermission: IRolePermissionDisplay;
    constructor(private fb: FormBuilder, private dataProvider: DataProvider,
        private holidayService: HolidayService,
        private emailApprovalService: EmailApprovalService,
        private lookupService: LookUpService,
        private messageService: MessageService,
        private rolePermissionService: RolePermissionService,
        private processDataService: ProcessDataService,
        private processWizardService: ProcessWizardService,
        private employeeService: EmployeeService) {
        this.user = JSON.parse(localStorage.getItem("User"));
        this.buildWizardForm({}, 'New')
        this.ClientID = localStorage.getItem("ClientID");
    }

    buildWizardForm(data: any, keyName: string) {
        this.ProcessDataForm = this.fb.group({
            TimeSheetID: [keyName === 'New' ? 0 : data.timeSheetID]
        })
    }

    ngOnInit(): void {
        this.loadModalOptions();
        this.LoadDropDown();
    }

    LoadDropDown() {
        forkJoin(
            this.holidayService.getCountry(),
            this.employeeService.GetEmployees(),
            this.lookupService.getListValues(),
        ).subscribe(async resultSet => {
            if (resultSet !== undefined && resultSet !== null) {
                this.CountryList = resultSet[0]['data'];
                this.EmployeeList = resultSet[1]['data'].filter(x => x.termDate === null || this.commonUtils.defaultDateTimeLocalSet(this.myDate) <= this.commonUtils.defaultDateTimeLocalSet(x.termDate))
                this.GenderList = resultSet[2]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.GENDER);
                this.EmployMentList = resultSet[2]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.EMPLOYMENTTYPE);
                this.TitleList = resultSet[2]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.TITLE);
                this.WorkAuthorizationList = resultSet[2]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.WORKAUTHORIZATION);
                this.MaritalStautsList = resultSet[2]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.MARITALSTATUS);
                this.InvoicingPeriodList = resultSet[2]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.INVOICINGPERIOD);
                this.PaymentTermList = resultSet[2]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.PAYMENTTERM);
                this.PaymentTypeList = resultSet[2]['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.PAYMENTTYPE);
                this.WizardStatusList = resultSet[2]['data'].filter(x => x.type.toUpperCase() == Constants.WIZARDSTATUS);
            }
        })
    }

    processID: number;
    wizardDataID: number;
    selectedProcessID: number;
    Show(processDataID: number, isDisable: boolean, selectedProcessID: number) {
        this.selectedProcessID = selectedProcessID;
        this.isDisable = isDisable;
        this.loadModalOptions();
        this.isShowTickets = false;
        this.processID = processDataID;
        if (processDataID !== undefined) {
            this.loadgetWizardDataByIdAsync(processDataID)
        }
        this.WizardDataFlowModal.show();
    }
    isDisable: boolean = false;
    isSaveProcessBtnDisable: boolean = false;
    
    WizardSteps: any[] = [];
    loadgetWizardDataByIdAsync(processDataID: number) {
        this.processDataService.GetWizardDataByIdAsync(processDataID).subscribe(result => {
            if (result['data'] !== null && result['messageType'] === 1) {
                this.wizarddata = result['data'];
                this.ModalHeading = this.wizarddata.process 
                if (this.isDisable) {
                    this.isControlDisable = true;
                    this.isProcessBtnClick = true;
                } else {
                    this.isControlDisable = false;
                    this.isProcessBtnClick = false;
                }
                if (this.wizarddata.status.toUpperCase() != WizardStatusConstants.PENDING) {
                    this.isControlDisable = true;
                    this.isSaveProcessBtnDisable = true;
                } else {
                    this.isSaveProcessBtnDisable = false;
                }
                this.loadProcessDataList(processDataID)
                this.loadModalOptions();
            }
        })
    }

    loadProcessDataList(processID: number) {
        this.processDataService.GetProcessDataList(processID).subscribe(reponsesData => {
            if (reponsesData['data'] !== null && reponsesData['messageType'] === 1) {
                this.WizardSteps = [];
                this.WizardSteps = reponsesData['data'];
                 if (this.WizardSteps !== null && this.WizardSteps !== undefined && this.WizardSteps.length > 0) {
                    this.WizardSteps.forEach(step => {
                        if (step.fields != null && step.fields !== undefined && step.fields.length > 0) {
                            step.fields.forEach(element => {
                                if (element.elementType.toUpperCase() === 'DateInput'.toUpperCase()) {
                                    if (element.value !== '' && element.value !== null && element.value !== undefined) {
                                        element.value = new Date(element.value);
                                    }
                                }
                                if (element.elementType.toUpperCase() === 'SelectInput'.toUpperCase() && element.name.toUpperCase() !== 'Employee'.toUpperCase()) {
                                    if (element.value !== '' && element.value !== null && element.value !== undefined) {
                                        element.value = Number(element.value);
                                    }
                                }
                            })
                        }
                    })
                }
            }
        })
    }

    isResolveButtonDisabled: boolean = false;
    loadModalOptions() {
        this.modalOptions = {
            footerActions: [
                {
                    actionText: 'Process',
                    actionMethod: this.process,
                    styleClass: this.isProcessBtnClick ? 'btn-width-height p-button-raised p-mr-2 p-mb-2' : 'btn-width-height p-button-raised p-mr-2 p-mb-2  display-none',
                    iconClass: 'mdi mdi-content-save',
                    disabled: !this.isProcessBtnClick  || this.isSaveProcessBtnDisable,
                },
                {
                    actionText: 'Save',
                    actionMethod: this.Save,
                    styleClass: this.isProcessBtnClick === false ? 'btn-width-height p-button-raised p-mr-2 p-mb-2 ' : 'btn-width-height p-button-raised p-mr-2 p-mb-2 display-none',
                    iconClass: 'mdi mdi-content-save',
                    disabled: this.isSaveProcessBtnDisable || this.isProcessBtnClick,
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

    ValidateWizardData(Fields: IFields[]) {
        if (Fields !== null && Fields !== undefined && Fields.length > 0) {
            const Elements = Fields;
            for (let element of Elements) {
                if ( element.Required.toString().toUpperCase() == "TRUE" && (element.Value == "" || element.Value == null)) {
                    this.stepErrorMessage =  element.Label + " Cannot be blank"
                    return true;
                } else if ( element.ElementType == "EmailInput" && element.Required.toString().toUpperCase() == "TRUE") {
                    var email = element.Value;
                    var filter = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
                    //let regex = new RegExp('[a-z0-9]+@[a-z]+\.[a-z]{2,3}');
                    if (!filter.test(email)) {
                        this.stepErrorMessage = element.Label + " not valid"
                        return true;
                    }
                }
            }
            return false;
        }
        return false;
    }


    process = () => {
        const processID = this.wizarddata.processDataID;
        var username = this.wizarddata.createdBy;
        this.wizarddataForSave = new IProcessData();
        if (this.WizardSteps !== null && this.WizardSteps !== undefined && this.WizardSteps.length > 0) {
            this.WizardSteps.forEach(step => {
                if (step !== null && step !== undefined && step.name !== undefined && step.name !== null && step.name !== '' && step.fields !== null && step.fields !== undefined && step.fields.length > 0) {
                    step.fields.forEach(element => {
                        if (element.position !== undefined && element.position !== '' && element.position !== null && step.name === element.position) {
                            if (element.name !== null && element.name !== undefined && element.name.toUpperCase() == "EMPLOYEE" && this.EmployeeList.length > 0) {
                                var empId = this.EmployeeList.find(x => x.employeeName == element.value) !== undefined ? this.EmployeeList.find(x => x.employeeName == element.value).employeeID : element.value;
                                if (empId != null && empId !== undefined) {
                                    element['EmployeeID'] = empId
                                }
                            }
                        }
                    })
                }
            })
        }
        if (this.WizardStatusList !== null && this.WizardStatusList !== undefined && this.WizardStatusList.length > 0) {
                const statusID = Number(this.WizardStatusList.find(x => x.value.toUpperCase() === WizardStatusConstants.INPROCESS).listValueID);
                this.wizarddataForSave.StatusId = statusID;
        }
        this.wizarddataForSave.EmailApprovalValidity = 7;
        var EmailTo = this.settings.EmailNotifications[this.ClientID]['ChangeNotification']
        this.wizarddataForSave.ChangeNotificationEmailId = EmailTo;
        this.wizarddataForSave.ProcessDataID = processID;
        this.wizarddataForSave.ProcessWizardID = this.wizarddata.processWizardID;
        var data = JSON.stringify(this.WizardSteps);
        this.wizarddataForSave.Data = data;
        this.wizarddataForSave.Process = this.wizarddata.process;
        this.wizarddataForSave.CreatedBy = this.wizarddata.createdBy;
        this.wizarddataForSave.CreatedDate = this.wizarddata.createdDate;
        this.wizarddataForSave.ModifiedDate = new Date();
        this.wizarddataForSave.ModifiedBy = this.user.firstName + " " + this.user.lastName;
        this.processDataService.SaveWizardData(this.wizarddataForSave).subscribe(result => {
            if (result['data'] !== null && result['messageType'] === 1) {
                this.emailApprovalService.GetEmailApprovals().subscribe(result => {
                })
                this.messageService.add({ severity: 'success', summary: 'Process data saved successfully', detail: '' });
                this.WizardDataUpdated.emit(this.selectedProcessID)
                this.WizardDataFlowModal.hide();
            } else {
                this.messageService.add({ severity: 'error', summary: 'Error occured', detail: '' });
            }
        })
    }



    Save = (event: any) =>  {
        const processID = this.wizarddata.processDataID;
        this.wizarddataForSave = new IProcessData();

        if (this.WizardSteps !== null && this.WizardSteps !== undefined && this.WizardSteps.length > 0) {
            this.WizardSteps.forEach(step => {
                if (step !== null && step !== undefined && step.name !== undefined && step.name !== null && step.name !== '' && step.fields !== null && step.fields !== undefined && step.fields.length > 0) {
                    step.fields.forEach(element => {
                        if (element.position !== undefined && element.position !== '' && element.position !== null && step.name === element.position) {
                            if (element.name !== null && element.name !== undefined && element.name.toUpperCase() == "EMPLOYEE" && this.EmployeeList.length > 0) {
                                var empId = this.EmployeeList.find(x => x.employeeName == element.value) !== undefined ? this.EmployeeList.find(x => x.employeeName == element.value).employeeID : element.value;
                                if (empId != null && empId !== undefined) {
                                    element['EmployeeID'] = empId
                                }
                            }
                        }
                    })
                }
            })
        }
        if (this.WizardStatusList !== null && this.WizardStatusList !== undefined && this.WizardStatusList.length > 0) {
            const statusID = Number(this.WizardStatusList.find(x => x.value.toUpperCase() === WizardStatusConstants.PENDING).listValueID);
            this.wizarddataForSave.StatusId = statusID;
        }
        this.wizarddataForSave.ProcessDataID = processID;
        this.wizarddataForSave.ProcessWizardID = this.wizarddata.processWizardID;
        var data = JSON.stringify(this.WizardSteps);
        this.wizarddataForSave.Data = data;
        this.wizarddataForSave.CreatedBy = this.wizarddata.createdBy;
        this.wizarddataForSave.CreatedDate = this.wizarddata.createdDate;
        this.wizarddataForSave.ModifiedDate = new Date();
        this.wizarddataForSave.ModifiedBy = this.user.firstName + " " + this.user.lastName;
        this.processDataService.SaveWizardData(this.wizarddataForSave).subscribe(result => {
            if (result['data'] !== null && result['messageType'] === 1) {
                this.messageService.add({ severity: 'success', summary: 'Process data saved successfully', detail: '' });
                this.WizardDataUpdated.emit(this.selectedProcessID)
                this.WizardDataFlowModal.hide();
            } else {
                this.messageService.add({ severity: 'error', summary: 'Error occured', detail: '' });
            }
        })
    }


    WizardDataID: number;
    showTickets(wizardDataID: number) {
        this.isShowTickets = true;
        this.loadTicketTableColumns();
        this.WizardDataID = wizardDataID;
        if (this.WizardDataID !== undefined) {
            this.loadWizardTicketsAsync(wizardDataID)
        }
        this.WizardDataFlowModal.show();
    }

    SplitByString(source, splitBy): any {
        let splitter = splitBy.split('');
        splitter.push([source]); //Push initial value

        return splitter.reduceRight(function (accumulator, curValue) {
            let k = [];
            accumulator.forEach(v => k = [...k, ...v.split(curValue)]);
            return k;
        });
    }

    wizardDataTickets: IProcessDataTicketDisplay[] = []
    loadWizardTicketsAsync(processDataID: number) {
        this.processDataService.GetWizardDataByIdAsync(processDataID).subscribe(result => {
            if (result['data'] !== null && result['messageType'] === 1) {
                this.wizarddata = result['data'];

                let fullHeading = '';
                if (this.wizarddata.dataColumns !== null && this.wizarddata.dataColumns !== undefined && this.wizarddata.dataColumns !== '') {
                    if (this.wizarddata.dataColumns.includes('First Name')) {
                        let name = this.SplitByString(this.wizarddata.dataColumns, ':,')
                        fullHeading = name !== null ? this.wizarddata.process + ' ('+name[1].toString().trim() + name[3] + ')' : this.wizarddata.process
                    }
                    if (this.wizarddata.dataColumns.includes('Employee Name')) {
                        let name = this.SplitByString(this.wizarddata.dataColumns, ':,')
                        fullHeading = name !== null ? this.wizarddata.process + ' (' +name[1].toString().trim() + ')' : this.wizarddata.process
                    }
                    this.ModalHeading = fullHeading
                } else {
                    this.ModalHeading = this.wizarddata.process;
                }
                
                this.loadProcessDataList(processDataID)
                if (this.wizarddata.processDataTickets !== null && this.wizarddata.processDataTickets !== undefined && this.wizarddata.processDataTickets.length > 0) {
                    this.wizarddata.processDataTickets.forEach((d) => {
                        if (d.createdDate !== null) {
                            d['createdDate'] = moment(d.createdDate).format("MM/DD/YYYY")
                        }
                        if (d.resolvedDate !== null) {
                            d['resolvedDate'] = moment(d.resolvedDate).format("MM/DD/YYYY")
                        }
                    })
                }
              
                this.wizardDataTickets = this.wizarddata.processDataTickets;

                this.modalOptions = {
                    footerActions: [
                    ]
                }
                
            }
        })
    }


    cols: any[] = [];
    selectedColumns: any[];
    isPagination: boolean = true;
    loading: boolean = false;
    showCurrentPageReport: boolean = true;
    rowActions: ITableRowAction[] = [];
    headerActions: ITableHeaderAction[] = [];
    patientLanding: string = 'patientLanding';
    globalFilterFields = ['employeeCode', 'firstName', 'lastName', 'email', 'roleName', 'companyName', 'isOAuth', 'isActive']


    loadTicketTableColumns() {
            this.cols = [
                { field: 'ticketID', header: 'Ticket ID', hasChange: true },
                { field: 'title', header: 'Title' },
                { field: 'assignedToUser', header: 'Assigned To' },
                { field: 'status', header: 'Status' },
                { field: 'createdDate', header: 'Created' },
                { field: 'resolvedDate', header: 'Resolved' }
            ];
            this.selectedColumns = this.cols;
            this.rowActions = [];
        this.headerActions = [];
    }
    cancel = () => {
        this.WizardDataFlowModal.hide();
    }

    @ViewChild('AddEditTicketModal') AddEditTicketModal: AddEditTicketComponent;
    ProcessTicket(data:any) {
        this.AddEditTicketModal.Show(data.ticketID, this.user.employeeID, this.WizardDataID);
    }

    getValueParse(value: any) {
        return Number(value);
    }

}
