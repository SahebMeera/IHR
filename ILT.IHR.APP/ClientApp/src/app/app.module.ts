import {NgModule} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import {BrowserModule} from '@angular/platform-browser';
import {LocationStrategy, HashLocationStrategy, CommonModule} from '@angular/common';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {AppRoutingModule} from './app-routing.module';

import {AccordionModule} from 'primeng/accordion';
import {AutoCompleteModule} from 'primeng/autocomplete';
import {AvatarModule} from 'primeng/avatar';
import {AvatarGroupModule} from 'primeng/avatargroup';
import {BadgeModule} from 'primeng/badge';
import {BreadcrumbModule} from 'primeng/breadcrumb';
import {ButtonModule} from 'primeng/button';
import {CalendarModule} from 'primeng/calendar';
import {CardModule} from 'primeng/card';
import {CarouselModule} from 'primeng/carousel';
import {CascadeSelectModule} from 'primeng/cascadeselect';
import {ChartModule} from 'primeng/chart';
import {CheckboxModule} from 'primeng/checkbox';
import {ChipModule} from 'primeng/chip';
import {ChipsModule} from 'primeng/chips';
import {CodeHighlighterModule} from 'primeng/codehighlighter';
import {ConfirmDialogModule} from 'primeng/confirmdialog';
import {ConfirmPopupModule} from 'primeng/confirmpopup';
import {ColorPickerModule} from 'primeng/colorpicker';
import {ContextMenuModule} from 'primeng/contextmenu';
import {DataViewModule} from 'primeng/dataview';
import {DialogModule} from 'primeng/dialog';
import {DividerModule} from 'primeng/divider';
import {DropdownModule} from 'primeng/dropdown';
import {FieldsetModule} from 'primeng/fieldset';
import {FileUploadModule} from 'primeng/fileupload';
import {FullCalendarModule} from '@fullcalendar/angular';
import {GalleriaModule} from 'primeng/galleria';
import {ImageModule} from 'primeng/image';
import {InplaceModule} from 'primeng/inplace';
import {InputNumberModule} from 'primeng/inputnumber';
import {InputMaskModule} from 'primeng/inputmask';
import {InputSwitchModule} from 'primeng/inputswitch';
import {InputTextModule} from 'primeng/inputtext';
import {InputTextareaModule} from 'primeng/inputtextarea';
import {KnobModule} from 'primeng/knob';
import {LightboxModule} from 'primeng/lightbox';
import {ListboxModule} from 'primeng/listbox';
import {MegaMenuModule} from 'primeng/megamenu';
import {MenuModule} from 'primeng/menu';
import {MenubarModule} from 'primeng/menubar';
import {MessagesModule} from 'primeng/messages';
import {MessageModule} from 'primeng/message';
import {MultiSelectModule} from 'primeng/multiselect';
import {OrderListModule} from 'primeng/orderlist';
import {OrganizationChartModule} from 'primeng/organizationchart';
import {OverlayPanelModule} from 'primeng/overlaypanel';
import {PaginatorModule} from 'primeng/paginator';
import {PanelModule} from 'primeng/panel';
import {PanelMenuModule} from 'primeng/panelmenu';
import {PasswordModule} from 'primeng/password';
import {PickListModule} from 'primeng/picklist';
import {ProgressBarModule} from 'primeng/progressbar';
import {RadioButtonModule} from 'primeng/radiobutton';
import {RatingModule} from 'primeng/rating';
import {RippleModule} from 'primeng/ripple';
import {ScrollPanelModule} from 'primeng/scrollpanel';
import {ScrollTopModule} from 'primeng/scrolltop';
import {SelectButtonModule} from 'primeng/selectbutton';
import {SidebarModule} from 'primeng/sidebar';
import {SkeletonModule} from 'primeng/skeleton';
import {SlideMenuModule} from 'primeng/slidemenu';
import {SliderModule} from 'primeng/slider';
import {SplitButtonModule} from 'primeng/splitbutton';
import {SplitterModule} from 'primeng/splitter';
import {StepsModule} from 'primeng/steps';
import {TabMenuModule} from 'primeng/tabmenu';
import {TableModule} from 'primeng/table';
import {TabViewModule} from 'primeng/tabview';
import {TagModule} from 'primeng/tag';
import {TerminalModule} from 'primeng/terminal';
import {TieredMenuModule} from 'primeng/tieredmenu';
import {TimelineModule} from 'primeng/timeline';
import {ToastModule} from 'primeng/toast';
import {ToggleButtonModule} from 'primeng/togglebutton';
import {ToolbarModule} from 'primeng/toolbar';
import {TooltipModule} from 'primeng/tooltip';
import {TreeModule} from 'primeng/tree';
import {TreeTableModule} from 'primeng/treetable';
import {VirtualScrollerModule} from 'primeng/virtualscroller';

import { EditorModule } from '@tinymce/tinymce-angular';


import {AppCodeModule} from './app.code.component';
import {AppComponent} from './app.component';
import {AppMainComponent} from './app.main.component';
import {AppTopBarComponent} from './app.topbar.component';
import {AppConfigComponent} from './app.config.component';
import {AppMenuComponent} from './app.menu.component';
import {AppMenuitemComponent} from './app.menuitem.component';

import {AppCrudComponent} from './pages/app.crud.component';
import {AppCalendarComponent} from './pages/app.calendar.component';
import {AppInvoiceComponent} from './pages/app.invoice.component';
import {AppHelpComponent} from './pages/app.help.component';
import {AppNotfoundComponent} from './pages/app.notfound.component';
import {AppErrorComponent} from './pages/app.error.component';
import {AppTimelineDemoComponent} from './pages/app.timelinedemo.component';
import {AppAccessdeniedComponent} from './pages/app.accessdenied.component';
import {AppLoginComponent} from './pages/app.login.component';

//import {  } from '@txtextcontrol/tx-ng-document-viewer';


import {CountryService} from './demo/service/countryservice';
import {CustomerService} from './demo/service/customerservice';
import {EventService} from './demo/service/eventservice';
import {IconService} from './demo/service/iconservice';
import {NodeService} from './demo/service/nodeservice';
import {PhotoService} from './demo/service/photoservice';
import {ProductService} from './demo/service/productservice';
import {BreadcrumbService} from './app.breadcrumb.service';
import {MenuService} from './app.menu.service';

import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';

import { UtilityModule } from './pages/utility/utility.module';
import { DataProvider } from './core/providers/data.provider';
import { RegistrationModule } from './pages/registration/registration.module';
import { LoginComponent } from './pages/login/login.component';
import { HomeComponent } from './pages/home/home.component';

import { Md5 } from 'ts-md5';

import { AdminComponent } from './pages/admin/admin.component'
import { AdminModule } from './pages/admin/admin.module';

import { ErrorInterceptor, JwtInterceptor } from './_helpers';


import { PdfViewerModule } from 'ng2-pdf-viewer';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MasterLayoutComponent } from './layouts/master-layout/master-layout.component';
import { EmployeeModule } from './pages/employee/employee.module';
import { LeaveRequestModule } from './pages/leave-request/leave-request.module';
import { TimesheetModule } from './pages/timesheet/timesheet.module';
import { HolidaysModule } from './pages/holidays/holidays.module';
import { CompanyComponent } from './pages/company/company.component';
import { CompanyModule } from './pages/company/company.module';
import { WFHRequestComponent } from './pages/wfh-request/wfh-request.component';
import { ManageLeaveComponent } from './pages/manage-leave/manage-leave.component';
import { ManageTimesheetComponent } from './pages/manage-timesheet/manage-timesheet.component';
import { LookupComponent } from './pages/lookup/lookup.component';
import { RolePermissionComponent } from './pages/role-permission/role-permission.component';
import { LookupModule } from './pages/lookup/lookup.module';
import { ManageLeaveModule } from './pages/manage-leave/manage-leave.module';
import { ManageTimesheetModule } from './pages/manage-timesheet/manage-timesheet.module';
import { RolePermissionModule } from './pages/role-permission/role-permission.module';
import { WFHRequestModule } from './pages/wfh-request/wfh-request.module';
import { UserComponent } from './pages/user/user.component';
import { ExpensesComponent } from './pages/expenses/expenses.component';
import { ProcessDataComponent } from './pages/process-data/process-data.component';
import { AssetComponent } from './pages/asset/asset.component';
import { TicketComponent } from './pages/ticket/ticket.component';
import { ReportsComponent } from './pages/reports/reports.component';
import { AppraisalComponent } from './pages/appraisal/appraisal.component';
import { UserModule } from './pages/user/user.module';
import { TicketModule } from './pages/ticket/ticket.module';
import { ReportsModule } from './pages/reports/reports.module';
import { ProcessDataModule } from './pages/process-data/process-data.module';
import { ExpensesModule } from './pages/expenses/expenses.module';
import { AssetModule } from './pages/asset/asset.module';
import { AppraisalModule } from './pages/appraisal/appraisal.module';
import { SharedModule } from './shared/shared.module';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { DashboardService } from './pages/dashboard/dashboard.service';
import { DummayComponent } from './pages/dummay/dummay.component';
import { HelpDocumentationComponent } from './pages/help-documentation/help-documentation.component';
import { UserNotificationComponent } from './pages/user-notification/user-notification.component';
import { UserNotificationService } from './pages/user-notification/UserNotificationService';
import { EmailApprovalModule } from './pages/emailapproval/emailapproval.module';
import { EmailApprovalComponent } from './pages/emailapproval/emailapproval.component';
import { EmailApprovalService } from './pages/employee/emailApproval.service';



FullCalendarModule.registerPlugins([
    dayGridPlugin,
    timeGridPlugin,
    interactionPlugin
]);

@NgModule({
    imports: [
        BrowserModule,
        FormsModule,
        ReactiveFormsModule,
        AppRoutingModule,
        HttpClientModule,
        BrowserAnimationsModule,
        AccordionModule,
        AutoCompleteModule,
        AvatarModule,
        AvatarGroupModule,
        BadgeModule,
        BreadcrumbModule,
        ButtonModule,
        CalendarModule,
        CardModule,
        CarouselModule,
        CascadeSelectModule,
        ChartModule,
        CheckboxModule,
        ChipsModule,
        ChipModule,
        CodeHighlighterModule,
        ConfirmDialogModule,
        ConfirmPopupModule,
        ColorPickerModule,
        ContextMenuModule,
        DataViewModule,
        DialogModule,
        DividerModule,
        DropdownModule,
        FieldsetModule,
        FileUploadModule,
        FullCalendarModule,
        GalleriaModule,
        ImageModule,
        InplaceModule,
        InputNumberModule,
        InputMaskModule,
        InputSwitchModule,
        InputTextModule,
        InputTextareaModule,
        KnobModule,
        LightboxModule,
        ListboxModule,
        MegaMenuModule,
        MenuModule,
        MenubarModule,
        MessageModule,
        MessagesModule,
        MultiSelectModule,
        OrderListModule,
        OrganizationChartModule,
        OverlayPanelModule,
        PaginatorModule,
        PanelModule,
        PanelMenuModule,
        PasswordModule,
        PickListModule,
        ProgressBarModule,
        RadioButtonModule,
        RatingModule,
        RippleModule,
        ScrollPanelModule,
        ScrollTopModule,
        SelectButtonModule,
        SidebarModule,
        SkeletonModule,
        SlideMenuModule,
        SliderModule,
        SplitButtonModule,
        SplitterModule,
        StepsModule,
        TagModule,
        TableModule,
        TabMenuModule,
        TabViewModule,
        TerminalModule,
        TieredMenuModule,
        TimelineModule,
        ToastModule,
        ToggleButtonModule,
        ToolbarModule,
        TooltipModule,
        TreeModule,
        TreeTableModule,
        VirtualScrollerModule,
        AppCodeModule,
        UtilityModule,
        RegistrationModule,
        AdminModule,
        EditorModule,
        PdfViewerModule,
        NgbModule,
        EmployeeModule,
        LeaveRequestModule,
        TimesheetModule,
        HolidaysModule,
        CompanyModule,
        LookupModule,
        ManageLeaveModule,
        ManageTimesheetModule,
        RolePermissionModule,
        WFHRequestModule,
        UserModule,
        TicketModule,
        ReportsModule,
        ProcessDataModule,
        ExpensesModule,
        AssetModule,
        AppraisalModule,
        SharedModule,
        CommonModule,
      // EmailApprovalModule
    ],
    declarations: [
        AppComponent,
        AppMainComponent,
        AppTopBarComponent,
        AppConfigComponent,
        AppMenuComponent,
        AppMenuitemComponent,
        DashboardComponent,
        AppCrudComponent,
        AppCalendarComponent,
        AppLoginComponent,
        AppInvoiceComponent,
        AppHelpComponent,
        AppNotfoundComponent,
        AppErrorComponent,
        AppTimelineDemoComponent,
        AppAccessdeniedComponent,
        LoginComponent,
        HomeComponent,
        AdminComponent,
        MasterLayoutComponent,
        DummayComponent,
        HelpDocumentationComponent,
       UserNotificationComponent,
        EmailApprovalComponent
        
    ],
    providers: [
        { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
        DashboardService,
        Md5,
        DataProvider,CountryService, CustomerService, EventService, IconService, NodeService,
        PhotoService, ProductService, MenuService, BreadcrumbService,
        UserNotificationService,
        EmailApprovalService
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
