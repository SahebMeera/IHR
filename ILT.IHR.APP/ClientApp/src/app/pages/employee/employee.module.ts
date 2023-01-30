import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EmployeeComponent } from './employee.component';
import { EmployeeDetailsComponent } from './employee-details/employee-details.component';
import { EmployeeRoutingModule } from './employee-routing.module';
import { MessageService } from 'primeng/api';
import { EmployeeService } from './employee.service';
import { SharedModule } from 'src/app/shared/shared.module';
import { NgxSpinnerModule } from 'ngx-spinner';
import {ButtonModule} from 'primeng/button';
import { FormsModule, ReactiveFormsModule  } from '@angular/forms';
import { AddEditEmployeeComponent } from './add-edit-employee/add-edit-employee.component';
import { CardModule } from 'primeng/card';
import { TabViewModule } from 'primeng/tabview';
import { FieldsetModule } from 'primeng/fieldset';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { InputMaskModule } from 'primeng/inputmask';
import { AddEditAddressComponent } from './employee-details/add-edit-address/add-edit-address.component';
import { EmployeeEmergencyComponent } from './employee-emergency/employee-emergency.component';
import { EmerygencyService } from './employee-emergency/emerygency.service';
import { AddEditContactComponent } from './employee-emergency/add-edit-contact/add-edit-contact.component';
import { DirectDepositComponent } from './direct-deposit/direct-deposit.component';
import { AddEditDirectDepositComponent } from './direct-deposit/add-edit-direct-deposit/add-edit-direct-deposit.component';
import { DirectDepositService } from './direct-deposit/direct-deposit.service';
import { InputNumberModule } from 'primeng/inputnumber';
import { EmployeeDependentComponent } from './employee-dependent/employee-dependent.component';
import { DependentService } from './employee-dependent/dependent.service';
import { AddEditDependentComponent } from './employee-dependent/add-edit-dependent/add-edit-dependent.component';
import { EmployeeW4Component } from './employee-w4/employee-w4.component';
import { EmployeeW4Service } from './employee-w4/w4.service';
import { AddEditEmployeew4Component } from './employee-w4/add-edit-employeew4/add-edit-employeew4.component';
import { CheckboxModule } from 'primeng/checkbox';
import { EmployeeAssignmentComponent } from './employee-assignment/employee-assignment.component';
import { EmployeeAssignmentService } from './employee-assignment/assignment.service';
import { AddEditAssignmentComponent } from './employee-assignment/add-edit-assignment/add-edit-assignment.component';
import { AddEditAssignmentrateComponent } from './employee-assignment/add-edit-assignmentrate/add-edit-assignmentrate.component';
import { FormI9Component } from './form-i9/form-i9.component';
import { FormI9Service } from './form-i9/formI9.service';
import { EmployeeSkillComponent } from './employee-skill/employee-skill.component';
import { SkillService } from './employee-skill/skill.service';
import { AddEditSkillComponent } from './employee-skill/add-edit-skill/add-edit-skill.component';
import { AddEditFormi9Component } from './form-i9/add-edit-formi9/add-edit-formi9.component';
import { I9DocumentService } from './form-i9/I9DocumentService';
import { EmployeeSalaryComponent } from './employee-salary/employee-salary.component';
import { AddEditSalaryComponent } from './employee-salary/add-edit-salary/add-edit-salary.component';
import { EmployeeSalaryService } from './employee-salary/employee-salary.service';
import { EmailApprovalService } from './emailApproval.service';
import { EmployeeNotificationModalComponent } from './employee-notification-modal/employee-notification-modal.component';
import { I9formChangesetComponent } from './form-i9/i9form-changeset/i9form-changeset.component';
import { MultiSelectModule } from 'primeng/multiselect';
import { Ng2SearchPipeModule } from 'ng2-search-filter';
import { TableModule } from 'primeng/table';



@NgModule({
    declarations: [
        EmployeeComponent,
        EmployeeDetailsComponent,
        AddEditEmployeeComponent,
        AddEditAddressComponent,
        EmployeeEmergencyComponent,
        AddEditContactComponent,
        DirectDepositComponent,
        AddEditDirectDepositComponent,
        EmployeeDependentComponent,
        AddEditDependentComponent,
        EmployeeW4Component,
        AddEditEmployeew4Component,
        EmployeeAssignmentComponent,
        AddEditAssignmentComponent,
        AddEditAssignmentrateComponent,
        FormI9Component,
        EmployeeSkillComponent,
        AddEditSkillComponent,
        AddEditFormi9Component,
        EmployeeSalaryComponent,
        AddEditSalaryComponent,
        EmployeeNotificationModalComponent,
        I9formChangesetComponent,
        
    ],
    imports: [
        CommonModule,
        EmployeeRoutingModule,
        SharedModule,
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
        CheckboxModule,
        MultiSelectModule,
        Ng2SearchPipeModule,
        TableModule
    ],
  providers: [
      MessageService,
      EmployeeService,
      EmerygencyService,
      DirectDepositService,
      DependentService,
      EmployeeW4Service,
      EmployeeAssignmentService,
      FormI9Service,
      SkillService,
      I9DocumentService,
      EmployeeSalaryService,
      EmailApprovalService
    ],
  exports: [EmployeeAssignmentComponent]
})
export class EmployeeModule { }
