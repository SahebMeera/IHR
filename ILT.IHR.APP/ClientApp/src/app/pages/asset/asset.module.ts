import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AssetRoutingModule } from './asset-routing.module';
import { AssetComponent } from './asset.component';
import { SharedModule } from '../../shared/shared.module';
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
import { AssetService } from './asset.service';
import { AddEditAssetComponent } from './add-edit-asset/add-edit-asset.component';
import { AssetChangesetComponent } from './asset-changeset/asset-changeset.component';



@NgModule({
  declarations: [AssetComponent, AddEditAssetComponent, AssetChangesetComponent],
  imports: [
    CommonModule,
      AssetRoutingModule,
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
      InputTextareaModule
    ],
    providers: [
        MessageService,
        AssetService
    ]
})
export class AssetModule { }
