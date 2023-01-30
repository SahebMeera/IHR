import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { WFHRequestComponent } from './wfh-request.component';

const routes: Routes = [
  {
    path: '',
    component: WFHRequestComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class WFHRequestRoutingModule { }
