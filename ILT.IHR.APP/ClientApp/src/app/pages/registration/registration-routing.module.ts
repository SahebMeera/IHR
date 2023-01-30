import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegistrationComponent } from './registration.component';
import { ConfirmationComponent } from './confirmation/confirmation.component';
import { AddressComponent } from './address/address.component';
import { PersonalDetailsComponent } from './personal-details/personal-details.component';
import { ApproveAccountComponent } from './approve-account/approve-account.component';


@NgModule({

  imports: [
		RouterModule.forChild([
			{path:'',component: RegistrationComponent, children:[
				{path:'', redirectTo: 'personalDetails', pathMatch: 'full'},
				{path: 'personalDetails', component: PersonalDetailsComponent},
				{path: 'address', component: AddressComponent},
				{path: 'confirmation', component: ConfirmationComponent},
            ]
            },

		])
	],
	exports: [
		RouterModule
	]
})
export class RegistrationRoutingModule { }
