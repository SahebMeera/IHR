import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RolePermissionComponent } from './role-permission.component';
import { RolePermissionRoutingModule } from './role-permission-routing.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { NgxSpinnerModule } from 'ngx-spinner';
import { ButtonModule } from 'primeng/button';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { RolePermissionService } from './role-permission.service';
import { AddEditRolepermissionComponent } from './add-edit-rolepermission/add-edit-rolepermission.component';
import { ToastModule } from 'primeng/toast';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { DropdownModule } from 'primeng/dropdown';


@NgModule({
  declarations: [RolePermissionComponent, AddEditRolepermissionComponent],
  imports: [
    CommonModule,
      RolePermissionRoutingModule,
      SharedModule,
      NgxSpinnerModule,
      ButtonModule,
      ReactiveFormsModule,
      FormsModule,
      InputTextModule,
      ToastModule,
      DropdownModule,
      CheckboxModule
    ],
    providers: [
        MessageService,
        RolePermissionService
    ]
})
export class RolePermissionModule { }
