import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ManageLeaveComponent } from './manage-leave.component';

const routes: Routes = [
  {
    path: '',
    component: ManageLeaveComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ManageLeaveRoutingModule { }
