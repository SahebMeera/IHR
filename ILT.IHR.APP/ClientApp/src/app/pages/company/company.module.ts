import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CompanyComponent } from './company.component';
import { CompanyRoutingModule } from './company-routing.module';
import { MessageService } from 'primeng/api';
import { CompanyService } from './company.service';
import { NgxSpinnerModule } from 'ngx-spinner';
import {ButtonModule} from 'primeng/button';
import { SharedModule } from 'src/app/shared/shared.module';
import { FormsModule, ReactiveFormsModule  } from '@angular/forms';
import { EndClientService } from './EndClientService';
import { AddEditCompanyComponent } from './add-edit-company/add-edit-company.component';
import { CardModule } from 'primeng/card';
import { TabViewModule } from 'primeng/tabview';
import { FieldsetModule } from 'primeng/fieldset';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { InputMaskModule } from 'primeng/inputmask';
import { InputNumberModule } from 'primeng/inputnumber';
import { CheckboxModule } from 'primeng/checkbox';
import { AddEditEndClientComponent } from './add-edit-end-client/add-edit-end-client.component';



@NgModule({
  declarations: [CompanyComponent, AddEditCompanyComponent, AddEditEndClientComponent],
  imports: [
    CommonModule,
    CompanyRoutingModule,
    SharedModule,
    NgxSpinnerModule,
    ButtonModule,
    ReactiveFormsModule,
      FormsModule,
      NgxSpinnerModule,
      ButtonModule,
      ReactiveFormsModule,
      FormsModule,
      CardModule,
      TabViewModule,
      FieldsetModule,
      CalendarModule,
      DropdownModule,
      InputTextModule,
      ToastModule,
      InputMaskModule,
      InputNumberModule,
      CheckboxModule
  ],
  providers: [
     MessageService,
     CompanyService,
     EndClientService
],
})
export class CompanyModule { }
