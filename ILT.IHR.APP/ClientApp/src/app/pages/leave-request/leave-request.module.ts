import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LeaveRequestComponent } from './leave-request.component';
import { LeaveRequestRoutingModule } from './leave-request-routing.module';
import { MessageService } from 'primeng/api';
import { SharedModule } from '../../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxSpinnerModule } from 'ngx-spinner';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { CheckboxModule } from 'primeng/checkbox';
import { TabViewModule } from 'primeng/tabview';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { LeaveRequestService } from './leave-request.service';
import { LeaveBalanceService } from './leave-balance.service';
import { ApprovalDenyLeaveComponent } from './LeaveApproval/approval-deny-leave/approval-deny-leave.component';
import { AddEditLeaveComponent } from './add-edit-leave/add-edit-leave.component';
import { InputNumberModule } from 'primeng/inputnumber';
import { LeaveApproveDenyComponent } from './LeaveApproval/approval-deny-leave/leave-approve-deny/leave-approve-deny.component';
import { MenuModule } from 'primeng/menu';



@NgModule({
    declarations: [LeaveRequestComponent, ApprovalDenyLeaveComponent, AddEditLeaveComponent, LeaveApproveDenyComponent],
    imports: [
        CommonModule,
        LeaveRequestRoutingModule,
        SharedModule,
        ReactiveFormsModule,
        FormsModule,
        NgxSpinnerModule,
        ButtonModule,
        InputTextModule,
        ToastModule,
        CheckboxModule,
        TabViewModule,
        CalendarModule,
        DropdownModule,
        InputTextModule,
        InputTextareaModule,
        InputNumberModule,
        MenuModule
    ],
    providers: [
        MessageService,
        LeaveRequestService,
        LeaveBalanceService
    ]
})
export class LeaveRequestModule { }
