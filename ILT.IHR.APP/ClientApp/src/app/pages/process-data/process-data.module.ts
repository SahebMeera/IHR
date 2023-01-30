import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProcessDataRoutingModule } from './peocess-data-routing.module';
import { ProcessDataComponent } from './process-data.component';
import { ProcessDataService } from './process-data.service';
import { ProcessWizardService } from './process-wizard.service';
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
import { WizardFlowComponent } from './wizard-flow/wizard-flow.component';
import { WizardStepComponent } from './wizard-flow/step.component';
import { WizardComponent } from './wizard-flow/wizard.component';
import { InputMaskModule } from 'primeng/inputmask';
import { WizardDataFlowComponent } from './wizard-data-flow/wizard-data-flow.component';
import { FieldsetModule } from 'primeng/fieldset';
import { TicketModule } from '../ticket/ticket.module';
import { MessageService } from 'primeng/api';
import { MultiSelectModule } from 'primeng/multiselect';
import { MenuModule } from 'primeng/menu';



@NgModule({
   declarations: [ProcessDataComponent, WizardFlowComponent, WizardStepComponent, WizardComponent, WizardDataFlowComponent],
  imports: [
    CommonModule,
      ProcessDataRoutingModule,
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
      InputMaskModule,
      FieldsetModule,
      TicketModule,
      MultiSelectModule,
      MenuModule
    ],
    providers: [
        ProcessDataService,
        ProcessWizardService,
        MessageService
    ]
})
export class ProcessDataModule { }
