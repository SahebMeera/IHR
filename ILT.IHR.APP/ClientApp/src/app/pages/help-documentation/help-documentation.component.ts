import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { HelpDocumentation } from '../../constant';

@Component({
  selector: 'app-help-documentation',
  templateUrl: './help-documentation.component.html',
  styleUrls: ['./help-documentation.component.scss']
})
export class HelpDocumentationComponent implements OnInit {
      // @ViewChild('helpDocumentation') helpDocumentation: IHRModalPopupComponent;
        ShowDialog:boolean = false
        showImageModal:boolean = false
        isShowAllHelpDocumentation:boolean = false
        isShowEmployees:boolean = false
        isShowWFH:boolean = false
        isShowHolidays:boolean = false
        isShowUser:boolean = false
        isShowExpenses:boolean = false
        isShowCompany:boolean = false
        isShowLookup:boolean = false
        isShowLeaveRequest:boolean = false
        isShowTimeSheet:boolean = false
        isShowRolepermission:boolean = false
        isShowManageLeave:boolean = false
        isShowManageTimesheet:boolean = false
        isShowAsset:boolean = false
        isShowTicket:boolean = false
        isShowAppraisal:boolean = false
    isShowIHRProcess: boolean = false
    public visibleAnimate = false;
    right: string = 'top-right'
    center: string = 'center'
    constructor(private router: Router) {
    }

  ngOnInit(): void {
  }
    CurrUrl: string;
    Show() {
        var uri = this.router.url;
        if (uri !== '' && uri !== null && uri !== undefined) {
            this.CurrUrl = this.router.url.split('/')[1];
            if (this.CurrUrl !== '' && this.CurrUrl !== null && this.CurrUrl !== undefined  && this.CurrUrl.toUpperCase() !== HelpDocumentation.DASHBOARD && this.CurrUrl != "" && this.CurrUrl.toUpperCase() !== HelpDocumentation.REPORTS) {
                this.isShowAllHelpDocumentation = true;
                this.loadHelpDocument();
            }
            else {
                this.isShowAllHelpDocumentation = false;
                this.AllHelpDocumentationShow();
            }
        }
        //ShowDialog = true;
        //StateHasChanged();
    }

    loadHelpDocument() {
        if (this.CurrUrl != "" && this.CurrUrl.toUpperCase() == HelpDocumentation.EMPLOYEES) {
            this.isShowEmployees = true;
            this.isShowWFH = false;
            this.isShowHolidays = false;
            this.isShowUser = false;
            this.isShowExpenses = false;
            this.isShowLookup = false;
            this.isShowCompany = false;
            this.isShowRolepermission = false;
            this.isShowLeaveRequest = false;
            this.isShowTimeSheet = false;
            this.isShowManageTimesheet = false;
            this.isShowManageLeave = false;
            this.isShowAsset = false;
            this.isShowTicket = false;
            this.isShowAppraisal = false;
            this.isShowIHRProcess = false;
            this.visibleAnimate = true;
        }
        if (this.CurrUrl != "" && this.CurrUrl.toUpperCase() == HelpDocumentation.WFHREQUESTS) {
            this.isShowEmployees = false;
            this.isShowWFH = true;
            this.isShowHolidays = false;
            this.isShowUser = false;
            this.isShowExpenses = false;
            this.isShowLookup = false;
            this.isShowCompany = false;
            this.isShowRolepermission = false;
            this.isShowLeaveRequest = false;
            this.isShowTimeSheet = false;
            this.isShowManageTimesheet = false;
            this.isShowManageLeave = false;
            this.isShowAsset = false;
            this.isShowTicket = false;
            this.isShowAppraisal = false;
            this.isShowIHRProcess = false;
            this.visibleAnimate = true;
        }
        if (this.CurrUrl != "" && this.CurrUrl.toUpperCase() == HelpDocumentation.HOLIDAYS) {
            this.isShowEmployees = false;
            this.isShowWFH = false;
            this.isShowHolidays = true;
            this.isShowUser = false;
            this.isShowExpenses = false;
            this.isShowLookup = false;
            this.isShowCompany = false;
            this.isShowRolepermission = false;
            this.isShowLeaveRequest = false;
            this.isShowTimeSheet = false;
            this.isShowManageTimesheet = false;
            this.isShowManageLeave = false;
            this.isShowAsset = false;
            this.isShowTicket = false;
            this.isShowAppraisal = false;
            this.isShowIHRProcess = false;
            this.visibleAnimate = true;
        }
        if (this.CurrUrl != "" && this.CurrUrl.toUpperCase() == HelpDocumentation.USERS) {
            this.isShowEmployees = false;
            this.isShowWFH = false;
            this.isShowHolidays = false;
            this.isShowUser = true;
            this.isShowExpenses = false;
            this.isShowLookup = false;
            this.isShowCompany = false;
            this.isShowRolepermission = false;
            this.isShowLeaveRequest = false;
            this.isShowTimeSheet = false;
            this.isShowManageTimesheet = false;
            this.isShowManageLeave = false;
            this.isShowAsset = false;
            this.isShowTicket = false;
            this.isShowAppraisal = false;
            this.isShowIHRProcess = false;
            this.visibleAnimate = true;
        }
        if (this.CurrUrl != "" && this.CurrUrl.toUpperCase() == HelpDocumentation.EXPENSES) {
            this.isShowEmployees = false;
            this.isShowWFH = false;
            this.isShowHolidays = false;
            this.isShowUser = false;
            this.isShowExpenses = true;
            this.isShowLookup = false;
            this.isShowCompany = false;
            this.isShowRolepermission = false;
            this.isShowLeaveRequest = false;
            this.isShowTimeSheet = false;
            this.isShowManageTimesheet = false;
            this.isShowManageLeave = false;
            this.isShowAsset = false;
            this.isShowTicket = false;
            this.isShowAppraisal = false;
            this.isShowIHRProcess = false;
            this.visibleAnimate = true;
        }

        if (this.CurrUrl != "" && this.CurrUrl.toUpperCase() == HelpDocumentation.LOOKUPTABLES) {
            this.isShowEmployees = false;
            this.isShowWFH = false;
            this.isShowHolidays = false;
            this.isShowUser = false;
            this.isShowExpenses = false;
            this.isShowLookup = true;
            this.isShowCompany = false;
            this.isShowRolepermission = false;
            this.isShowLeaveRequest = false;
            this.isShowTimeSheet = false;
            this.isShowManageTimesheet = false;
            this.isShowManageLeave = false;
            this.isShowAsset = false;
            this.isShowTicket = false;
            this.isShowAppraisal = false;
            this.isShowIHRProcess = false;
            this.visibleAnimate = true;
        }

        if (this.CurrUrl != "" && this.CurrUrl.toUpperCase() == HelpDocumentation.ROLEPERMISSION) {
            this.isShowEmployees = false;
            this.isShowWFH = false;
            this.isShowHolidays = false;
            this.isShowUser = false;
            this.isShowExpenses = false;
            this.isShowLookup = false;
            this.isShowCompany = false;
            this.isShowRolepermission = true;
            this.isShowLeaveRequest = false;
            this.isShowTimeSheet = false;
            this.isShowManageTimesheet = false;
            this.isShowManageLeave = false;
            this.isShowAsset = false;
            this.isShowTicket = false;
            this.isShowAppraisal = false;
            this.isShowIHRProcess = false;
            this.visibleAnimate = true;
        }

        if (this.CurrUrl != "" && this.CurrUrl.toUpperCase() == HelpDocumentation.COMPANY) {
            this.isShowEmployees = false;
            this.isShowWFH = false;
            this.isShowHolidays = false;
            this.isShowUser = false;
            this.isShowExpenses = false;
            this.isShowLookup = false;
            this.isShowCompany = true;
            this.isShowRolepermission = false;
            this.isShowLeaveRequest = false;
            this.isShowTimeSheet = false;
            this.isShowManageTimesheet = false;
            this.isShowManageLeave = false;
            this.isShowAsset = false;
            this.isShowTicket = false;
            this.isShowAppraisal = false;
            this.isShowIHRProcess = false;
            this.visibleAnimate = true;
        }
        if (this.CurrUrl != "" && this.CurrUrl.toUpperCase() == HelpDocumentation.LEAVEREQUESTS) {
            this.isShowEmployees = false;
            this.isShowWFH = false;
            this.isShowHolidays = false;
            this.isShowUser = false;
            this.isShowExpenses = false;
            this.isShowLookup = false;
            this.isShowCompany = false;
            this.isShowRolepermission = false;
            this.isShowLeaveRequest = true;
            this.isShowTimeSheet = false;
            this.isShowManageTimesheet = false;
            this.isShowManageLeave = false;
            this.isShowAsset = false;
            this.isShowTicket = false;
            this.isShowAppraisal = false;
            this.isShowIHRProcess = false;
            this.visibleAnimate = true;
        }
        if (this.CurrUrl != "" && this.CurrUrl.toUpperCase() == HelpDocumentation.TIMESHEET) {
            this.isShowEmployees = false;
            this.isShowWFH = false;
            this.isShowHolidays = false;
            this.isShowUser = false;
            this.isShowExpenses = false;
            this.isShowLookup = false;
            this.isShowCompany = false;
            this.isShowRolepermission = false;
            this.isShowLeaveRequest = false;
            this.isShowTimeSheet = true;
            this.isShowManageTimesheet = false;
            this.isShowManageLeave = false;
            this.isShowAsset = false;
            this.isShowTicket = false;
            this.isShowAppraisal = false;
            this.isShowIHRProcess = false;
            this.visibleAnimate = true;
        }

        if (this.CurrUrl != "" && this.CurrUrl.toUpperCase() == HelpDocumentation.MANAGELEAVE) {
            this.isShowEmployees = false;
            this.isShowWFH = false;
            this.isShowHolidays = false;
            this.isShowUser = false;
            this.isShowExpenses = false;
            this.isShowLookup = false;
            this.isShowCompany = false;
            this.isShowRolepermission = false;
            this.isShowLeaveRequest = false;
            this.isShowTimeSheet = false;
            this.isShowManageTimesheet = false;
            this.isShowManageLeave = true;
            this.isShowAsset = false;
            this.isShowTicket = false;
            this.isShowAppraisal = false;
            this.isShowIHRProcess = false;
            this.visibleAnimate = true;
        }
        if (this.CurrUrl != "" && this.CurrUrl.toUpperCase() == HelpDocumentation.MANAGETIMESHEET) {
            this.isShowEmployees = false;
            this.isShowWFH = false;
            this.isShowHolidays = false;
            this.isShowUser = false;
            this.isShowExpenses = false;
            this.isShowLookup = false;
            this.isShowCompany = false;
            this.isShowRolepermission = false;
            this.isShowLeaveRequest = false;
            this.isShowTimeSheet = false;
            this.isShowManageTimesheet = true;
            this.isShowManageLeave = false;
            this.isShowAsset = false;
            this.isShowTicket = false;
            this.isShowAppraisal = false;
            this.isShowIHRProcess = false;
            this.visibleAnimate = true;
        }

        if (this.CurrUrl != "" && this.CurrUrl.toUpperCase() == HelpDocumentation.ASSET) {
            this.isShowEmployees = false;
            this.isShowWFH = false;
            this.isShowHolidays = false;
            this.isShowUser = false;
            this.isShowExpenses = false;
            this.isShowLookup = false;
            this.isShowCompany = false;
            this.isShowRolepermission = false;
            this.isShowLeaveRequest = false;
            this.isShowTimeSheet = false;
            this.isShowManageTimesheet = false;
            this.isShowManageLeave = false;
            this.isShowAsset = true;
            this.isShowTicket = false;
            this.isShowAppraisal = false;
            this.isShowIHRProcess = false;
            this.visibleAnimate = true;
        }

        if (this.CurrUrl != "" && this.CurrUrl.toUpperCase() == HelpDocumentation.TICKET) {
            this.isShowEmployees = false;
            this.isShowWFH = false;
            this.isShowHolidays = false;
            this.isShowUser = false;
            this.isShowExpenses = false;
            this.isShowLookup = false;
            this.isShowCompany = false;
            this.isShowRolepermission = false;
            this.isShowLeaveRequest = false;
            this.isShowTimeSheet = false;
            this.isShowManageTimesheet = false;
            this.isShowManageLeave = false;
            this.isShowAsset = false;
            this.isShowTicket = true;
            this.isShowAppraisal = false;
            this.isShowIHRProcess = false;
            this.visibleAnimate = true;
        }

        if (this.CurrUrl != "" && this.CurrUrl.toUpperCase() == HelpDocumentation.APPRAISAL) {
            this.isShowEmployees = false;
            this.isShowWFH = false;
            this.isShowHolidays = false;
            this.isShowUser = false;
            this.isShowExpenses = false;
            this.isShowLookup = false;
            this.isShowCompany = false;
            this.isShowRolepermission = false;
            this.isShowLeaveRequest = false;
            this.isShowTimeSheet = false;
            this.isShowManageTimesheet = false;
            this.isShowManageLeave = false;
            this.isShowAsset = false;
            this.isShowTicket = false;
            this.isShowAppraisal = true;
            this.isShowIHRProcess = false;
            this.visibleAnimate = true;
        }

        if (this.CurrUrl != "" && this.CurrUrl.toUpperCase() == HelpDocumentation.PROCESSDATAS) {
            this.isShowEmployees = false;
            this.isShowWFH = false;
            this.isShowHolidays = false;
            this.isShowUser = false;
            this.isShowExpenses = false;
            this.isShowLookup = false;
            this.isShowCompany = false;
            this.isShowRolepermission = false;
            this.isShowLeaveRequest = false;
            this.isShowTimeSheet = false;
            this.isShowManageTimesheet = false;
            this.isShowManageLeave = false;
            this.isShowAsset = false;
            this.isShowTicket = false;
            this.isShowAppraisal = false;
            this.isShowIHRProcess = true;
            this.visibleAnimate = true;
        }
        this.showImageModal = false;
    }

    AllHelpDocumentationShow() {
        this.showImageModal = false;
        this.isShowEmployees = true;
        this.isShowWFH = true;
        this.isShowHolidays = true;
        this.isShowUser = true;
        this.isShowExpenses = true;
        this.isShowLookup = true;
        this.isShowCompany = true;
        this.isShowRolepermission = true;
        this.isShowLeaveRequest = true;
        this.isShowTimeSheet = true;
        this.isShowManageTimesheet = true;
        this.isShowManageLeave = true;
        this.isShowAsset = true;
        this.isShowTicket = true;
        this.isShowAppraisal = true;
        this.isShowIHRProcess = true;
        this.visibleAnimate = true;
    }

    Close() {
        this.showImageModal = false;
        this.visibleAnimate = false;
    }

    ImgeUrlPath: string;
    HelpHeaderText: string
    openImageModalPopUp(e: any, image: string, helpHeaderText: string) {
        this.HelpHeaderText = "";
        this.ImgeUrlPath = "";
        var ImagePath = "";
        ImagePath = image;
        this.ImgeUrlPath = image;
        this.HelpHeaderText = helpHeaderText;
        this.showImageModal = true;
    }
    closeImageModel() {
        this.showImageModal = false;
    }


}
