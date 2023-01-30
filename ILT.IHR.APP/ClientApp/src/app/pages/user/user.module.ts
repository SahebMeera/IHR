import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserRoutingModule } from './user-routing.module';
import { UserComponent } from './user.component';
import { MessageService } from 'primeng/api';
import { UserService } from './user.service';
import { NgxSpinnerModule } from 'ngx-spinner';
import {ButtonModule} from 'primeng/button';
import { SharedModule } from 'src/app/shared/shared.module';
import { FormsModule, ReactiveFormsModule  } from '@angular/forms';
import { AddEditUserComponent } from './add-edit-user/add-edit-user.component';
import { CalendarModule } from 'primeng/calendar';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { CheckboxModule } from 'primeng/checkbox';
import { DropdownModule } from 'primeng/dropdown';
import { MultiSelectModule } from 'primeng/multiselect';
import { RadioButtonModule } from 'primeng/radiobutton';



@NgModule({
  declarations: [UserComponent, AddEditUserComponent],
  imports: [
    CommonModule,
    UserRoutingModule,
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
      RadioButtonModule
  ],
  providers: [
    MessageService,
    UserService
    ],
    exports: [
        AddEditUserComponent
    ]
})
export class UserModule { }
