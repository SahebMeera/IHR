import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ManageTimesheetComponent } from './manage-timesheet.component';
import { ManageTimesheetRoutingModule } from './manage-timesheet-routing.module';
import { SharedModule } from '../../shared/shared.module';
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
import { AddEditManageTimesheetComponent } from './add-edit-manage-timesheet/add-edit-manage-timesheet.component';
import { TableModule } from 'primeng/table';



@NgModule({
  declarations: [ManageTimesheetComponent, AddEditManageTimesheetComponent],
  imports: [
    CommonModule,
      ManageTimesheetRoutingModule,
      SharedModule,
      TableModule,
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
  ]
})
export class ManageTimesheetModule { }
