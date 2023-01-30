import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppraisalRoutingModule } from './appraisal-routing.module';
import { AppraisalComponent } from './appraisal.component';
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
import { SharedModule } from '../../shared/shared.module';
import { TableModule } from 'primeng/table';
import { AppraisalService } from './appraisal.service';
import { AppraisalDetailsComponent } from './appraisal-details/appraisal-details.component';
import { CardModule } from 'primeng/card';
import { RatingModule } from 'primeng/rating';




@NgModule({
  declarations: [AppraisalComponent, AppraisalDetailsComponent],
  imports: [
    CommonModule,
      AppraisalRoutingModule,
      CommonModule,
      TableModule,
      RatingModule,
      SharedModule,
      ReactiveFormsModule,
      FormsModule,
      NgxSpinnerModule,
      ButtonModule,
      InputTextModule,
      ToastModule,
      CardModule,
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
        AppraisalService
    ]
})
export class AppraisalModule { }
