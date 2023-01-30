import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UtilityRoutingModule } from './utility-routing.module';
import { UtilityComponent } from './utility.component';


@NgModule({
  declarations: [
    UtilityComponent
  ],
  imports: [
    CommonModule,
    UtilityRoutingModule
  ]
})
export class UtilityModule { }
