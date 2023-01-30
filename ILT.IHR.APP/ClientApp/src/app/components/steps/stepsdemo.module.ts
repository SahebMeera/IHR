import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {StepsDemo} from './stepsdemo';
import { StepsDemoRoutingModule } from './stepsdemo-routing.module';
import { StepsModule } from 'primeng/steps';
import { TabViewModule } from 'primeng/tabview';
import { AppCodeModule } from '../../app.code.component';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { InputMaskModule } from 'primeng/inputmask';
import { CheckboxModule } from 'primeng/checkbox';
import { ToastModule } from 'primeng/toast';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TicketService } from './ticketservice';
import { RegistrationComponent } from './registration/registration.component';
import { ConfirmationComponent } from './confirmation/confirmation.component';
import { LoginComponent } from './login/login.component';
import {InputNumberModule} from 'primeng/inputnumber';
import {InputSwitchModule} from 'primeng/inputswitch';
import {PasswordModule} from 'primeng/password';

@NgModule({
	imports: [
		CommonModule,
		StepsDemoRoutingModule,
        StepsModule,
		TabViewModule,
		AppCodeModule,
		ButtonModule,
		CardModule,
		InputTextModule,
		DropdownModule,
		InputMaskModule,
		CheckboxModule,
		ToastModule,
		FormsModule,
		ReactiveFormsModule,
		InputNumberModule,
		InputSwitchModule,
		PasswordModule
	],
	declarations: [
		StepsDemo,
		LoginComponent,
  RegistrationComponent,
  ConfirmationComponent
	],
	providers: [
		TicketService
	]
})
export class StepsDemoModule {}
