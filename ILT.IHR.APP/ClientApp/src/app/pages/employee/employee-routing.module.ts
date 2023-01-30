import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EmployeeComponent } from './employee.component';
import { EmployeeDetailsComponent } from './employee-details/employee-details.component';

const routes: Routes = [
    { path: '', component: EmployeeComponent },
    { path: 'employees', component: EmployeeComponent },
    { path: 'employeeDetails', component: EmployeeDetailsComponent },
   { path: 'AddEmployee', component: EmployeeDetailsComponent }
];


@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class EmployeeRoutingModule { }
