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
import { IFields, IProcessWizardDisplay } from '../../../core/interfaces/Processwizard';
import { IRolePermissionDisplay } from '../../../core/interfaces/RolePermission';
import { DataProvider } from '../../../core/providers/data.provider';
import { IModalPopupAlternateOptions } from '../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { EmployeeService } from '../../employee/employee.service';
import { HolidayService } from '../../holidays/holidays.service';
import { LookUpService } from '../../lookup/lookup.service';
import { RolePermissionService } from '../../role-permission/role-permission.service';
import { ProcessDataService } from '../process-data.service';
import { ProcessWizardService } from '../process-wizard.service';
import { WizardComponent } from './wizard.component';

@Component({
  selector: 'app-wizard-flow',
  templateUrl: './wizard-flow.component.html',
  styleUrls: ['./wizard-flow.component.scss']
})
export class WizardFlowComponent implements OnInit {
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
    @ViewChild('AddWizardFlowModal') AddWizardFlowModal: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;
    submitted: boolean = false;
    moment: any = moment;

    TimesheetForm: FormGroup;
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
    wizarddata: IProcessDataDisplay;
    WizardStatusList: any[] = []

    isShowTickets: boolean = false;
    isControlDisable: boolean = false;
    isProcessBtnClick: boolean = false;

    EmployeeAssigmentRolePermission: IRolePermissionDisplay;
    constructor(private fb: FormBuilder, private dataProvider: DataProvider,
        private holidayService: HolidayService,
        private lookupService: LookUpService,
        private messageService: MessageService,
        private router: Router,
        private activeRoute: ActivatedRoute,
        private rolePermissionService: RolePermissionService,
        private processDataService: ProcessDataService,
        private processWizardService: ProcessWizardService,
        private employeeService: EmployeeService) {
        this.user = JSON.parse(localStorage.getItem("User"));
        this.buildWizardForm({}, 'New')
       // this.LoadDropDown();
    }

    buildWizardForm(data: any, keyName: string) {
        this.TimesheetForm = this.fb.group({
            TimeSheetID: [keyName === 'New' ? 0 : data.timeSheetID]
        })
    }

    ngOnInit(): void {
        this.LoadDropDown();
        this.loadModalOptions();
    }


    //myDate = new Date();
    LoadDropDown() {
        forkJoin(
            this.holidayService.getCountry(),
            this.employeeService.GetEmployees(),
            this.lookupService.getListValues(),
            this.processWizardService.GetWizards()
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
                this.WizardList = resultSet[3]['data'];
            }
        })
    }

    WizardList: any[] = []
    WizardSteps: any[] = []
    wizard: any;
    loadProcessData(processID: number) {
        this.processWizardService.GetProcessWizardList(processID).subscribe(reponsesData => {
            if (reponsesData['data'] !== null && reponsesData['messageType'] === 1) {
                this.wizard = this.WizardList.find(x => x.processWizardID == processID);
                this.ModalHeading = this.wizard.process
                this.stepErrorMessage = '';
                //this.WizardSteps = [];
                this.WizardSteps = reponsesData['data'];
                this.isWizardComponentVisiable = true;
                
            }
        })
    }
    processID: number;
    isWizardComponentVisiable: boolean = false;
    async Show(processID: number) {
        this.processID = processID;
        this.stepErrorMessage = '';
        await this.LoadDropDown();
        await this.loadProcessData(processID)
        this.isWizardComponentVisiable = false;
        this.buildWizardForm({}, 'New');
        await this.AddWizardFlowModal.show();
    }

    onStep1Next(event, step: IProcessWizardDisplay) {
    }
    stepErrorMessage: string = '';
    fieldValidationMessage(event: any) {
        console.log('Hehehe')
        console.log('Hehehe', event)
        this.stepErrorMessage = event;
    }

    @ViewChild('wizardStep') wizardStep: WizardComponent;
    wizarddataForSave: IProcessData;
    SaveWizardData(event: any) {
        const wizardID = this.wizard.processWizardID;
        var username = this.wizard.createdBy;
        console.log(this.WizardSteps)
        this.wizarddataForSave = new IProcessData();

        if (this.WizardSteps !== null && this.WizardSteps !== undefined && this.WizardSteps.length > 0) {
            this.WizardSteps.forEach(step => {
                if (step !== null && step !== undefined && step.name !== undefined && step.name !== null && step.name !== '' && step.fields !== null && step.fields !== undefined && step.fields.length > 0) {
                    step.fields.forEach(element => {
                        if (element.position !== undefined && element.position !== '' && element.position !== null && step.name === element.position) {
                            if (element.name !== null && element.name !== undefined && element.name.toUpperCase() == "EMPLOYEE" && this.EmployeeList.length > 0) {
                                var empId = this.EmployeeList.find(x => x.employeeName == element.value).employeeID;
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
        this.wizarddataForSave.ProcessWizardID = wizardID;
        console.log(JSON.stringify(this.WizardSteps))
        var data = JSON.stringify(this.WizardSteps);
        this.wizarddataForSave.Data = data;
        this.wizarddataForSave.CreatedBy = username;
        this.wizarddataForSave.CreatedDate = new Date();
        console.log(JSON.stringify(this.wizarddataForSave))
        //this.AddWizardFlowModal.hide();
        this.processDataService.SaveWizardData(this.wizarddataForSave).subscribe(result => {
            if (result['data'] !== null && result['messageType'] === 1) {
                this.messageService.add({ severity: 'success', summary: 'Process data saved successfully', detail: '' });
                this.WizardDataUpdated.emit(this.processID)
                this.AddWizardFlowModal.hide();
            } else {
                this.messageService.add({ severity: 'error', summary: 'Error occured', detail: '' });
            }
        })
    }
    loadModalOptions() {
        this.modalOptions = {
            footerActions: [
            ]
        }
    }

}
