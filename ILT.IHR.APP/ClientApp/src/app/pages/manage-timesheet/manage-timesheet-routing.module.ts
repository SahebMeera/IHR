import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ManageTimesheetComponent } from './manage-timesheet.component';

const routes: Routes = [
  {
    path: '',
    component: ManageTimesheetComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ManageTimesheetRoutingModule { }
