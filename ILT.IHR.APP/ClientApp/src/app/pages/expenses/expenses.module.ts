import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ExpensesRoutingModule } from './expenses-routing.module';
import { ExpensesComponent } from './expenses.component';
import { SharedModule } from '../../shared/shared.module';
import { AddEditExpensesComponent } from './add-edit-expenses/add-edit-expenses.component';
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
import { MessageService } from 'primeng/api';
import { ExpenseService } from './expense.service';
import { InputNumberModule } from 'primeng/inputnumber';



@NgModule({
  declarations: [ExpensesComponent, AddEditExpensesComponent],
  imports: [
    CommonModule,
      ExpensesRoutingModule,
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
      InputTextareaModule,
      InputNumberModule
    ],
    providers: [
        MessageService,
        ExpenseService
    ]
})
export class ExpensesModule { }
