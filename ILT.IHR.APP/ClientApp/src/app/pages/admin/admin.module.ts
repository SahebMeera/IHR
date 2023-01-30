import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {TableModule} from 'primeng/table';


//import { ReviewRegistrationComponent } from './review-registration.component';
import { MessageService } from 'primeng/api';
import { IconService } from 'src/app/demo/service/iconservice';
import { InputTextModule } from 'primeng/inputtext';
import {DialogModule} from 'primeng/dialog';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import {InputNumberModule} from 'primeng/inputnumber';
import {InputSwitchModule} from 'primeng/inputswitch';
import {PasswordModule} from 'primeng/password';
import { CheckboxModule } from 'primeng/checkbox';
import { InputMaskModule } from 'primeng/inputmask';
import { DropdownModule } from 'primeng/dropdown';
import { AdminRoutingModule } from './admin-routing.module';

import { ToolbarModule } from 'primeng/toolbar';
import { MultiSelectModule } from 'primeng/multiselect';
import { ToastModule } from 'primeng/toast';
import { AdminComponent } from './admin.component';
import { CalendarModule } from 'primeng/calendar';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { AutoCompleteModule } from 'primeng/autocomplete';



@NgModule({
    declarations: [
  ],
  imports: [
    CommonModule,
    AdminRoutingModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    DialogModule,
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    FormsModule,
    ButtonModule,
    CardModule,
    DropdownModule,
    InputTextModule,
    InputMaskModule,
    CheckboxModule,
    ReactiveFormsModule,
	InputNumberModule,
	InputSwitchModule,
    PasswordModule,
    ToolbarModule,
    MultiSelectModule,
      CardModule,
      CalendarModule,
      InputTextareaModule,
      AutoCompleteModule,
      ToastModule,
  ],
   providers: [
    IconService,
       MessageService,
]
})
export class AdminModule { }
