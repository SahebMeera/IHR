import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ManageLeaveComponent } from './manage-leave.component';
import { ManageLeaveRoutingModule } from './manage-leave-routing.module';
import { LeaveBalanceService } from './LeaveBalanceService';
import { MessageService } from 'primeng/api';
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
import { TableModule } from 'primeng/table';
import { SharedModule } from '../../shared/shared.module';
import { AddEditManageLeaveComponent } from './add-edit-manage-leave/add-edit-manage-leave.component';
import { AddEditLwpComponent } from './add-edit-lwp/add-edit-lwp.component';
import { AccrueLeaveComponent } from './accrue-leave/accrue-leave.component';
import { LeaveAccrueService } from './LeaveAccrueService';
import { CalculateLwpComponent } from './calculate-lwp/calculate-lwp.component';




@NgModule({
  declarations: [ManageLeaveComponent, AddEditManageLeaveComponent, AddEditLwpComponent, AccrueLeaveComponent, CalculateLwpComponent],
  imports: [
    CommonModule,
      ManageLeaveRoutingModule,
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
    ],
    providers: [
        MessageService,
        LeaveBalanceService,
        LeaveAccrueService
    ]
})
export class ManageLeaveModule { }
