import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TimesheetComponent } from './timesheet.component';
import { TimesheetRoutingModule } from './timsheet-routing.module';
import { MessageService } from 'primeng/api';
import { TimesheetService } from './timesheet.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxSpinnerModule } from 'ngx-spinner';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { CheckboxModule } from 'primeng/checkbox';
import { TabViewModule } from 'primeng/tabview';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { InputNumberModule } from 'primeng/inputnumber';
import { SharedModule } from '../../shared/shared.module';
import { AddEditTimesheetComponent } from './add-edit-timesheet/add-edit-timesheet.component';
import { TableModule } from 'primeng/table';
import { ApprovalDenyTimesheetComponent } from './TimesheetApproval/approval-deny-timesheet/approval-deny-timesheet.component';
import { TimesheetApprovalDenyComponent } from './TimesheetApproval/approval-deny-timesheet/timesheet-approval-deny/timesheet-approval-deny.component';




@NgModule({
  declarations: [TimesheetComponent, AddEditTimesheetComponent, ApprovalDenyTimesheetComponent, TimesheetApprovalDenyComponent],
  imports: [
      CommonModule,
      TableModule,
      TimesheetRoutingModule,
      SharedModule,
      ReactiveFormsModule,
      FormsModule,
      NgxSpinnerModule,
      ButtonModule,
      InputTextModule,
      ToastModule,
      CheckboxModule,
      TabViewModule,
      CalendarModule,
      DropdownModule,
      InputTextModule,
      InputTextareaModule,
      InputNumberModule,
    ],
    providers: [
        MessageService,
        TimesheetService
    ]
})
export class TimesheetModule { }
