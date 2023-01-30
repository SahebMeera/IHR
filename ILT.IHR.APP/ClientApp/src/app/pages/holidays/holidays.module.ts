import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HolidaysComponent } from './holidays.component';
import { HolidaysRoutingModule } from './holidays-routing.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { NgxSpinnerModule } from 'ngx-spinner';
import {ButtonModule} from 'primeng/button';
import { FormsModule, ReactiveFormsModule  } from '@angular/forms';
import { HolidayService } from './holidays.service';
import { MessageService } from 'primeng/api';
import { AddEditHolidayComponent } from './add-edit-holiday/add-edit-holiday.component';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';




@NgModule({
  declarations: [HolidaysComponent, AddEditHolidayComponent],
  imports: [
    CommonModule,
    HolidaysRoutingModule,
    SharedModule,
    NgxSpinnerModule,
    ButtonModule,
    ReactiveFormsModule,
      FormsModule,
      CalendarModule,
      DropdownModule,
      InputTextModule,
      ToastModule
    ],
    providers: [
        MessageService,
        HolidayService
    ]
})
export class HolidaysModule { }
