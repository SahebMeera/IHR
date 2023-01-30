import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppraisalDetailsComponent } from './appraisal-details/appraisal-details.component';
import { AppraisalComponent } from './appraisal.component';

const routes: Routes = [
   { path: '', component: AppraisalComponent },
    { path: 'appraisal', component: AppraisalComponent },
  { path: 'appraisalDetails', component: AppraisalDetailsComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AppraisalRoutingModule { }
