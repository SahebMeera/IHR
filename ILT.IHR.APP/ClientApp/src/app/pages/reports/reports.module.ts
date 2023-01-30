import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReportsComponent } from './reports.component';
import { ReportsRoutingModule } from './reports-routing.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { NgxSpinnerModule } from 'ngx-spinner';
import { ButtonModule } from 'primeng/button';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { TableModule } from 'primeng/table';
import { CardModule } from 'primeng/card';
import { MultiSelectModule } from 'primeng/multiselect';
import { AssetReportComponent } from './asset-report/asset-report.component';
import { AuditReportComponent } from './audit-report/audit-report.component';
import { EmployeeDetailReportComponent } from './employee-detail-report/employee-detail-report.component';
import { I9expiryFormComponent } from './i9expiry-form/i9expiry-form.component';
import { PendingLeaveComponent } from './pending-leave/pending-leave.component';
import { LeaveReportComponent } from './leave-report/leave-report.component';




@NgModule({
  declarations: [ReportsComponent, AssetReportComponent, AuditReportComponent, EmployeeDetailReportComponent, I9expiryFormComponent, PendingLeaveComponent, LeaveReportComponent],
  imports: [
    CommonModule,
      ReportsRoutingModule,
      CommonModule,
      SharedModule,
      NgxSpinnerModule,
      ButtonModule,
      ReactiveFormsModule,
      FormsModule,
      CalendarModule,
      DropdownModule,
      InputTextModule,
      ToastModule,
      TableModule,
      CardModule,
      MultiSelectModule
  ]
})
export class ReportsModule { }
