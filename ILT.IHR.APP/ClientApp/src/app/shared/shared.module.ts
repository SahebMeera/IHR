import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
//import { ReviewRegistrationComponent } from './review-registration.component';
import { MessageService } from 'primeng/api';
import { IconService } from 'src/app/demo/service/iconservice';
import { InputTextModule } from 'primeng/inputtext';
import { DialogModule } from 'primeng/dialog';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputSwitchModule } from 'primeng/inputswitch';
import { PasswordModule } from 'primeng/password';
import { CheckboxModule } from 'primeng/checkbox';
import { InputMaskModule } from 'primeng/inputmask';
import { DropdownModule } from 'primeng/dropdown';
import { ToolbarModule } from 'primeng/toolbar';
import { MultiSelectModule } from 'primeng/multiselect';
import { NavigationComponent } from './header-navigation/navigation.component';
import { NgxSpinnerModule } from 'ngx-spinner';
import { IHRModalPopupComponent } from './ihr-modal-popup/ihr-modal-popup.component';
import { IHRTableComponent } from './ihr-table/ihr-table.component';
import { EmployeeAssignmentService } from '../pages/employee/employee-assignment/assignment.service';
import { DisallowSpecialDirective } from './directive/DisallowSpecialDirective';
import { FilterPipe } from './Pipes/FilterPipe';
import { OutsideClickDirective } from './directive/OutsideClickDirective';





@NgModule({
  declarations: [
        NavigationComponent,
        IHRModalPopupComponent,
        IHRTableComponent,
        DisallowSpecialDirective,
        FilterPipe,
        OutsideClickDirective
  ],
  imports: [
      CommonModule,
      CommonModule,
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
      NgxSpinnerModule
    ],
    exports: [
        NavigationComponent,
        IHRTableComponent,
        DisallowSpecialDirective,
        OutsideClickDirective,
        IHRModalPopupComponent,
        FilterPipe
    ],
     providers: [
        IconService,
         MessageService,
         EmployeeAssignmentService
    ]
})
export class SharedModule {}
