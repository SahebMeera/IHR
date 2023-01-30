import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import * as moment from 'moment';
import { CommonUtils } from '../../../common/common-utils';
import { ErrorMsg } from '../../../constant';
import { INotification } from '../../../core/interfaces/Notification';
import { IModalPopupAlternateOptions } from '../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { ITableHeaderAction, ITableRowAction } from '../../../shared/ihr-table/table-options';
import { EmployeeService } from '../employee.service';

@Component({
  selector: 'app-employee-notification-modal',
  templateUrl: './employee-notification-modal.component.html',
  styleUrls: ['./employee-notification-modal.component.scss']
})
export class EmployeeNotificationModalComponent implements OnInit {
    empID: number;
    employeeChangeSet: any[] = [];
    commonUtils = new CommonUtils()
    @Output() EmployeeUpdated = new EventEmitter<any>();
    @ViewChild('employeeNotificationModal') employeeNotificationModal: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;
    isSaveButtonDisabled: boolean = false;
    ModalHeading: string = 'Employee Changes';
    isShow: boolean;

    cols: any[] = [];
    selectedColumns: any[];
    isPagination: boolean = true;
    loading: boolean = false;
    showCurrentPageReport: boolean = true;
    rowActions: ITableRowAction[] = [];
    headerActions: ITableHeaderAction[] = [];
    patientLanding: string = 'patientLanding';
    globalFilterFields = ['employeeCode', 'employeeName', 'title', 'hireDate', 'department', 'workAuthorization', 'email', 'phone']

    constructor(private employeeService: EmployeeService) { }

    ngOnInit(): void {
        this.cols = [
            { field: 'employeeCode', header: 'Employee Code' },
            { field: 'firstName', header: 'First Name' },
            { field: 'middleName', header: 'Middle Name' },
            { field: 'lastName', header: 'Last Name' },
            { field: 'country', header: 'Country' },
            { field: 'title', header: 'Title' },
            { field: 'department', header: 'Department' },
            { field: 'birthDate', header: 'Birth Date' },
            { field: 'hireDate', header: 'Hire Date' },
            { field: 'termDate', header: 'Term Date' },
            { field: 'workAuthorization', header: 'Work Authorization' },
            { field: 'salary', header: 'Salary' },
            { field: 'variablePay', header: 'Variable Pay' },
            { field: 'maritalStatus', header: 'Marital Status' },
            { field: 'manager', header: 'Manager' },
            { field: 'employmentType', header: 'Employment Type' },
            { field: 'modifiedBy', header: 'Modified By' },
            { field: 'modifiedDate', header: 'Modified Date' },
        ];

        this.selectedColumns = this.cols;
        this.modalOptions = {
            footerActions: []
        }
        this.headerActions = [
            {
                actionMethod: this.Acknowledge,
                hasIcon: false,
                styleClass: 'btn-width-height btn btn-block btn-sm btn-info first-letter-caps',
                actionText: 'Acknowledge',
                iconClass: ''

            }
        ];
    }
    notification: INotification;
    Acknowledge = () => {
        this.notification = {
            TableName: "Employee",
            RecordID: this.empID,
            ChangeSetID: 0,
            NotificationID: 0,
            UserID: 0,
            IsAck: false
        }
        this.employeeService.SaveNotification(0, this.notification).subscribe(result => {
            if (result['data'] !== null && result['messageType'] === 1) {
               // this.messageService.add({ severity: 'success', summary: 'EmployeeW4 saved successfully', detail: '' });
                this.isSaveButtonDisabled = true;
                this.EmployeeUpdated.emit();
                this.employeeNotificationModal.hide();
            } else {
                this.isSaveButtonDisabled = false;
               // this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
            }
        })


    }

   show(employeeid: number) {
       this.empID = employeeid;
       this.employeeChangeSet = [];
       if (employeeid !== undefined && employeeid !== null) {
           this.employeeService.GetEmployeeChangeset(employeeid).subscribe(result => {
          
               if (result['data'] !== null && result['messageType'] === 1) {
                   result['data'].forEach(d => {
                       d.birthDate = moment(d.birthDate).format("MM/DD/YYYY")
                       d.hireDate = moment(d.hireDate).format("MM/DD/YYYY")
                       if (d.termDate !== null) {
                           d.termDate = moment(d.termDate).format("MM/DD/YYYY")
                       }
                       if (d.modifiedDate !== null) {
                           d.modifiedDate = this.commonUtils.formatDateTimeWithAmPM(d.modifiedDate);
                       }
                   })
                   this.employeeChangeSet = result['data'];

                   this.employeeNotificationModal.show();
               }
           })
       }
    }

}
