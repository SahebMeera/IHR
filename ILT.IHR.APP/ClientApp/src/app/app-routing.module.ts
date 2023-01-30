import { RouterModule, PreloadAllModules } from '@angular/router';
import {NgModule} from '@angular/core';
// import {DashboardDemoComponent} from './demo/view/dashboarddemo.component';

import {AppMainComponent} from './app.main.component';
import {AppNotfoundComponent} from './pages/app.notfound.component';
import {AppErrorComponent} from './pages/app.error.component';
import {AppAccessdeniedComponent} from './pages/app.accessdenied.component';
import {AppLoginComponent} from './pages/app.login.component';
import {AppCrudComponent} from './pages/app.crud.component';
import {AppCalendarComponent} from './pages/app.calendar.component';
import {AppInvoiceComponent} from './pages/app.invoice.component';
import {AppHelpComponent} from './pages/app.help.component';

import { AppTimelineDemoComponent } from './pages/app.timelinedemo.component';
import { LoginComponent } from './pages/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { ApproveAccountComponent } from './pages/registration/approve-account/approve-account.component';
import { AuthGuard } from './_helpers';
import { Role } from './_models/role';
import { MasterLayoutComponent } from './layouts/master-layout/master-layout.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { DummayComponent } from './pages/dummay/dummay.component';
import { EmailApprovalComponent } from './pages/emailapproval/emailapproval.component';

@NgModule({
    imports: [
        RouterModule.forRoot([
            { path: '', component: LoginComponent },
            {
                //path: '', component: AppMainComponent,
                 path: '', component: MasterLayoutComponent,
                //path: '', component: DummayComponent,
                children: [
                  //  { path: '', component: HomeComponent },
                    //{
                    //    path: 'dummay',
                    //    component: DummayComponent,
                    //    //canActivate: [AuthGuard],
                    //},
                    {
                        path: 'dashboard',
                        component: DashboardComponent,
                        canActivate: [AuthGuard],
                    },
                    {
                        path: 'admin',
                        loadChildren: () => import('./pages/admin/admin.module').then(m => m.AdminModule),
                        canActivate: [AuthGuard],
                    //    data: { roles: [Role.SuperAdmin, Role.Admin] }
                    },
                    {
                        path: 'employees',
                        loadChildren: () => import('./pages/employee/employee.module').then(m => m.EmployeeModule),
                        canActivate: [AuthGuard],
                    },
                    {
                        path: 'leaverequests',
                        loadChildren: () => import('./pages/leave-request/leave-request.module').then(m => m.LeaveRequestModule),
                        canActivate: [AuthGuard],
                    },
                    {
                      path: 'wfhrequests',
                      loadChildren: () => import('./pages/wfh-request/wfh-request.module').then(m => m.WFHRequestModule),
                    //  canActivate: [AuthGuard],
                    },
                    {
                      path: 'timesheet',
                      loadChildren: () => import('./pages/timesheet/timesheet.module').then(m => m.TimesheetModule),
                    //  canActivate: [AuthGuard],
                    },
                    {
                      path: 'holidays',
                      loadChildren: () => import('./pages/holidays/holidays.module').then(m => m.HolidaysModule),
                      canActivate: [AuthGuard],
                    },
                    {
                      path: 'company',
                      loadChildren: () => import('./pages/company/company.module').then(m => m.CompanyModule),
                    //  canActivate: [AuthGuard],
                    },
                    {
                      path: 'manageLeave',
                      loadChildren: () => import('./pages/manage-leave/manage-leave.module').then(m => m.ManageLeaveModule),
                    //  canActivate: [AuthGuard],
                    },
                    {
                      path: 'managetimesheet',
                      loadChildren: () => import('./pages/manage-timesheet/manage-timesheet.module').then(m => m.ManageTimesheetModule),
                    //  canActivate: [AuthGuard],
                    },
                    {
                      path: 'lookupTables',
                      loadChildren: () => import('./pages/lookup/lookup.module').then(m => m.LookupModule),
                      canActivate: [AuthGuard],
                    },
                    {
                      path: 'rolepermission',
                      loadChildren: () => import('./pages/role-permission/role-permission.module').then(m => m.RolePermissionModule),
                      canActivate: [AuthGuard],
                    },
                    {
                      path: 'users',
                      loadChildren: () => import('./pages/user/user.module').then(m => m.UserModule),
                      canActivate: [AuthGuard],
                    },
                    {
                      path: 'expenses',
                      loadChildren: () => import('./pages/expenses/expenses.module').then(m => m.ExpensesModule),
                      canActivate: [AuthGuard],
                    },
                    {
                      path: 'processdatas',
                      loadChildren: () => import('./pages/process-data/process-data.module').then(m => m.ProcessDataModule),
                      canActivate: [AuthGuard],
                    },
                    {
                      path: 'asset',
                      loadChildren: () => import('./pages/asset/asset.module').then(m => m.AssetModule),
                      canActivate: [AuthGuard],
                    },
                    {
                      path: 'ticket',
                      loadChildren: () => import('./pages/ticket/ticket.module').then(m => m.TicketModule),
                      canActivate: [AuthGuard],
                    },
                    {
                      path: 'reports',
                      loadChildren: () => import('./pages/reports/reports.module').then(m => m.ReportsModule),
                      canActivate: [AuthGuard],
                    },
                    {
                      path: 'appraisal',
                      loadChildren: () => import('./pages/appraisal/appraisal.module').then(m => m.AppraisalModule),
                      canActivate: [AuthGuard],
                    },
                   
                   
                ]
            },
            {
                path: 'emailapproval/:ClientID/:LinkID/:Value',
                component: EmailApprovalComponent
                //loadChildren: () => import('./pages/emailapproval/emailapproval.module').then(m => m.EmailApprovalModule),
                //canActivate: [AuthGuard],
            },
            {
                path: 'registration',
                loadChildren: () => import('./pages/registration/registration.module').then(m => m.RegistrationModule)
            },
            {path: 'error', component: AppErrorComponent},
            {path: 'access', component: AppAccessdeniedComponent},
            {path: 'notfound', component: AppNotfoundComponent},
            { path: 'login', component: LoginComponent},
            { path: 'Registration/ApproveAccount/:Guid', component: ApproveAccountComponent},
            {path: '**', redirectTo: '/notfound'},
        ],  {
            preloadingStrategy: PreloadAllModules
    })
    ],
    exports: [RouterModule]
})
export class AppRoutingModule {
}
