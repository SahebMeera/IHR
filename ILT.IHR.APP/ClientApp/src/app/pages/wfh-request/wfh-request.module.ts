import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WFHRequestComponent } from './wfh-request.component';
import { WFHRequestRoutingModule } from './wfh-request-routing.module';
import { SharedModule } from '../../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxSpinnerModule } from 'ngx-spinner';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { CheckboxModule } from 'primeng/checkbox';
import { MessageService } from 'primeng/api';
import { WFHService } from './wfh.service';
import { TabViewModule } from 'primeng/tabview';
import { WFHApprovalComponent } from './wfhapproval/wfhapproval.component';
import { AddEditWfhComponent } from './add-edit-wfh/add-edit-wfh.component';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { ApproveDenyWfhComponent } from './wfhapproval/approve-deny-wfh/approve-deny-wfh.component';
import { MenuModule } from 'primeng/menu';



@NgModule({
  declarations: [WFHRequestComponent, WFHApprovalComponent, AddEditWfhComponent, ApproveDenyWfhComponent],
  imports: [
      CommonModule,
      WFHRequestRoutingModule,
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
      MenuModule
    ],
     providers: [
        MessageService,
         WFHService
    ]
})
export class WFHRequestModule { }
