import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TicketRoutingModule } from './ticket-routing.module';
import { TicketComponent } from './ticket.component';
import { SharedModule } from '../../shared/shared.module';
import { MessageService } from 'primeng/api';
import { TicketService } from './ticket.service';
import { AddEditTicketComponent } from './add-edit-ticket/add-edit-ticket.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CalendarModule } from 'primeng/calendar';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { CheckboxModule } from 'primeng/checkbox';
import { DropdownModule } from 'primeng/dropdown';
import { MultiSelectModule } from 'primeng/multiselect';
import { RadioButtonModule } from 'primeng/radiobutton';
import { NgxSpinnerModule } from 'ngx-spinner';
import { ButtonModule } from 'primeng/button';
import { InputTextareaModule } from 'primeng/inputtextarea';



@NgModule({
  declarations: [TicketComponent, AddEditTicketComponent],
  imports: [
    CommonModule,
      TicketRoutingModule,
      SharedModule,
      NgxSpinnerModule,
      ReactiveFormsModule,
      FormsModule,
      NgxSpinnerModule,
      ButtonModule,
      InputTextModule,
      ToastModule,
      CheckboxModule,
      CalendarModule,
      DropdownModule,
      InputTextModule,
      MultiSelectModule,
      RadioButtonModule,
      InputTextareaModule
    ],
    providers: [
        MessageService,
        TicketService
    ],
    exports: [
        AddEditTicketComponent
    ]
})
export class TicketModule { }
