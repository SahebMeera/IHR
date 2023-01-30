import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { RegistrationRoutingModule } from './registration-routing.module';
import { RegistrationComponent } from './registration.component';
import { PersonalDetailsComponent } from './personal-details/personal-details.component';
import { AddressComponent } from './address/address.component';
import { ConfirmationComponent } from './confirmation/confirmation.component';

import {InputNumberModule} from 'primeng/inputnumber';
import {InputSwitchModule} from 'primeng/inputswitch';
import {PasswordModule} from 'primeng/password';
import { StepsModule } from 'primeng/steps';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { InputMaskModule } from 'primeng/inputmask';
import { CheckboxModule } from 'primeng/checkbox';
import { ToastModule } from 'primeng/toast';
import { RegistrationService } from './registration.service';
import { ToolbarModule } from 'primeng/toolbar';
import { ApproveAccountComponent } from './approve-account/approve-account.component';


@NgModule({
  declarations: [
    RegistrationComponent,
    PersonalDetailsComponent,
    AddressComponent,
    ConfirmationComponent,
    ApproveAccountComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,
    StepsModule,
    ButtonModule,
    CardModule,
    DropdownModule,
    InputTextModule,
    InputMaskModule,
    CheckboxModule,
    RegistrationRoutingModule,
    ReactiveFormsModule,
		InputNumberModule,
		InputSwitchModule,
      PasswordModule,
      ToolbarModule
  ],
  providers: [
    RegistrationService
],
})
export class RegistrationModule { }
