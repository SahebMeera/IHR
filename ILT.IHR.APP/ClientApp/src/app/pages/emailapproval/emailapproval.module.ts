import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from 'src/app/shared/shared.module';
import { NgxSpinnerModule } from 'ngx-spinner';
import { ButtonModule } from 'primeng/button';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { DropdownModule } from 'primeng/dropdown';
import { EmailApprovalComponent } from './emailapproval.component';
import { EmailApprovalRoutingModule } from './emailapproval-routing.module';



@NgModule({
    declarations: [],
  imports: [
    CommonModule,
      EmailApprovalRoutingModule,
      SharedModule,
      ReactiveFormsModule,
      FormsModule,
      NgxSpinnerModule,
      ButtonModule,
      InputTextModule,
      ToastModule,
      CheckboxModule,
      DropdownModule
    ],
    providers: [
        MessageService
    ]
})
export class EmailApprovalModule { }
