import {NgModule} from '@angular/core';
import {RouterModule} from '@angular/router'
import { StepsDemo } from './stepsdemo';

import { RegistrationComponent } from './registration/registration.component';
import { ConfirmationComponent } from './confirmation/confirmation.component';
import { LoginComponent } from './login/login.component';

@NgModule({
	imports: [
		RouterModule.forChild([
				{path:'',component: StepsDemo, children:[
				{path:'', redirectTo: 'login', pathMatch: 'full'},
				{path: 'login', component: LoginComponent},
				{path: 'registration', component: RegistrationComponent},
				{path: 'confirmation', component: ConfirmationComponent}
			]}
		])
	],
	exports: [
		RouterModule
	]
})
export class StepsDemoRoutingModule {}
