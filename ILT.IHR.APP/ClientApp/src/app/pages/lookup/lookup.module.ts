import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LookupComponent } from './lookup.component';
import { LookupRoutingModule } from './lookup-routing.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { NgxSpinnerModule } from 'ngx-spinner';
import { ButtonModule } from 'primeng/button';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { LookUpService } from './lookup.service';
import { AddEditLookupComponent } from './add-edit-lookup/add-edit-lookup.component';
import { ToastModule } from 'primeng/toast';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { DropdownModule } from 'primeng/dropdown';



@NgModule({
  declarations: [LookupComponent, AddEditLookupComponent],
  imports: [
    CommonModule,
      LookupRoutingModule,
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
        MessageService,
        LookUpService
    ]
})
export class LookupModule { }
