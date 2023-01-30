using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;
using ILT.IHR.UI.Service;
using Blazored.Toast.Services;
using Blazored.SessionStorage;

namespace ILT.IHR.UI.Pages.Help
{
    public class HelpBase : ComponentBase
    {
     
        public bool ShowDialog { get; set; }
        public bool showImageModal { get; set; }
        public bool isShowAllHelpDocumentation { get; set; }
        public bool isShowEmployees { get; set; }
        public bool isShowWFH { get; set; }
        public bool isShowHolidays { get; set; }
        public bool isShowUser { get; set; }
        public bool isShowExpenses { get; set; }
        public bool isShowCompany { get; set; }
        public bool isShowLookup { get; set; }
        public bool isShowLeaveRequest { get; set; }
        public bool isShowTimeSheet { get; set; }
        public bool isShowRolepermission { get; set; }
        public bool isShowManageLeave { get; set; }
        public bool isShowManageTimesheet { get; set; }
        public bool isShowAsset { get; set; }
        public bool isShowTicket { get; set; }
        public bool isShowAppraisal { get; set; }
        public bool isShowIHRProcess { get; set; }

        [Inject]
        public NavigationManager UrlNavigationManager { get; set; }
        public string CurrUrl { get; set; }

        protected override async Task OnInitializedAsync()
        {
            //CurrUrl = null;
        }


        public void Show()
        {
            var uri = UrlNavigationManager.ToAbsoluteUri(UrlNavigationManager.Uri);
            CurrUrl = UrlNavigationManager.ToBaseRelativePath(uri.ToString());
            if(CurrUrl != "" && CurrUrl != "#" && CurrUrl != null)
            {
                isShowAllHelpDocumentation = true;

                loadHelpDocument();
            }
            else
            {
                isShowAllHelpDocumentation = false;
                AllHelpDocumentationShow();
            }
           
            ShowDialog = true;
            StateHasChanged();
        }

        public void loadHelpDocument()
        {
            if(CurrUrl != "" && CurrUrl.ToUpper() == HelpDocumentation.EMPLOYEES)
            {
                isShowEmployees = true;
                isShowWFH = false;
                isShowHolidays = false;
                isShowUser = false;
                isShowExpenses = false;
                isShowLookup = false;
                isShowCompany = false;
                isShowRolepermission = false;
                isShowLeaveRequest = false;
                isShowTimeSheet = false;
                isShowManageTimesheet = false;
                isShowManageLeave = false;
                isShowAsset = false;
                isShowTicket = false;
                isShowAppraisal = false;
                isShowIHRProcess = false;
            }
            if (CurrUrl != "" && CurrUrl.ToUpper() == HelpDocumentation.WFHREQUESTS)
            {
                isShowEmployees = false;
                isShowWFH = true;
                isShowHolidays = false;
                isShowUser = false;
                isShowExpenses = false;
                isShowLookup = false;
                 isShowCompany = false;
                isShowRolepermission = false;
                isShowLeaveRequest = false;
                isShowManageTimesheet = false;
                isShowManageLeave = false;
                isShowAsset = false;
                isShowTicket = false;
                isShowAppraisal = false;
                isShowIHRProcess = false;
            }
            if (CurrUrl != "" && CurrUrl.ToUpper() == HelpDocumentation.HOLIDAYS)
            {
                isShowEmployees = false;
                isShowWFH = false;
                isShowHolidays = true;
                isShowUser = false;
                isShowExpenses = false;
                isShowLookup = false;
                isShowRolepermission = false;
                isShowLeaveRequest = false;
                isShowTimeSheet = false;
                isShowManageTimesheet = false;
                isShowManageLeave = false;
                isShowAsset = false;
                isShowTicket = false;
                isShowAppraisal = false;
                isShowIHRProcess = false;
            }
            if (CurrUrl != "" && CurrUrl.ToUpper() == HelpDocumentation.USERS)
            {
                isShowEmployees = false;
                isShowWFH = false;
                isShowHolidays = false;
                isShowUser = true;
                isShowExpenses = false;
                isShowLookup = false;
                isShowCompany = false;
                isShowRolepermission = false;
                isShowLeaveRequest = false;
                isShowTimeSheet = false;
                isShowManageTimesheet = false;
                isShowManageLeave = false;
                isShowAsset = false;
                isShowTicket = false;
                isShowAppraisal = false;
                isShowIHRProcess = false;
            }
            if (CurrUrl != "" && CurrUrl.ToUpper() == HelpDocumentation.EXPENSES)
            {
                isShowEmployees = false;
                isShowWFH = false;
                isShowHolidays = false;
                isShowUser = false;
                isShowExpenses = true;
                isShowCompany = false;
                isShowLookup = false;
                isShowRolepermission = false;
                isShowLeaveRequest = false;
                isShowTimeSheet = false;
                isShowManageTimesheet = false;
                isShowManageLeave = false;
                isShowAsset = false;
                isShowTicket = false;
                isShowAppraisal = false;
                isShowIHRProcess = false;
            }
            if (CurrUrl != "" && CurrUrl.ToUpper() == HelpDocumentation.LOOKUPTABLES)
            {
                isShowEmployees = false;
                isShowWFH = false;
                isShowHolidays = false;
                isShowUser = false;
                isShowExpenses = false;
                isShowLookup = true;
                isShowRolepermission = false;
                isShowCompany = false;
                isShowLeaveRequest = false;
                isShowManageTimesheet = false;
                isShowManageLeave = false;
                isShowAsset = false;
                isShowTicket = false;
                isShowAppraisal = false;
                isShowIHRProcess = false;
            }
            if (CurrUrl != "" && CurrUrl.ToUpper() == HelpDocumentation.ROLEPERMISSION)
            {
                isShowEmployees = false;
                isShowWFH = false;
                isShowHolidays = false;
                isShowUser = false;
                isShowExpenses = false;
                isShowLookup = false;
                isShowRolepermission = true;
                isShowCompany = false;
                isShowLeaveRequest = false;
                isShowTimeSheet = false;
                isShowManageTimesheet = false;
                isShowManageLeave = false;
                isShowAsset = false;
                isShowTicket = false;
                isShowAppraisal = false;
                isShowIHRProcess = false;
            }
            if (CurrUrl != "" && CurrUrl.ToUpper() == HelpDocumentation.COMPANY)
            {
                isShowEmployees = false;
                isShowWFH = false;
                isShowHolidays = false;
                isShowUser = false;
                isShowExpenses = false;
                isShowLookup = false;
                isShowRolepermission = false;
                isShowCompany = true;
                isShowLeaveRequest = false;
                isShowTimeSheet = false;
                isShowManageTimesheet = false;
                isShowManageLeave = false;
                isShowAsset = false;
                isShowTicket = false;
                isShowAppraisal = false;
                isShowIHRProcess = false;
            }
           
            if (CurrUrl != "" && CurrUrl.ToUpper() == HelpDocumentation.LEAVEREQUESTS)
            {
                isShowEmployees = false;
                isShowWFH = false;
                isShowHolidays = false;
                isShowUser = false;
                isShowExpenses = false;
                isShowLookup = false;
                isShowRolepermission = false;
                isShowCompany = false;
                isShowLeaveRequest = true;
                isShowTimeSheet = false;
                isShowManageTimesheet = false;
                isShowManageLeave = false;
                isShowAsset = false;
                isShowTicket = false;
                isShowAppraisal = false;
                isShowIHRProcess = false;
            }
            if (CurrUrl != "" && CurrUrl.ToUpper() == HelpDocumentation.TIMESHEET)
            {
                isShowEmployees = false;
                isShowWFH = false;
                isShowHolidays = false;
                isShowUser = false;
                isShowExpenses = false;
                isShowLookup = false;
                isShowRolepermission = false;
                isShowCompany = false;
                isShowLeaveRequest = false;
                isShowTimeSheet = true;
                isShowManageTimesheet = false;
                isShowManageLeave = false;
                isShowAsset = false;
                isShowAppraisal = false;
                isShowTicket = false;
                isShowIHRProcess = false;
            }
            if (CurrUrl != "" && CurrUrl.ToUpper() == HelpDocumentation.MANAGETIMESHEET)
            {
                isShowEmployees = false;
                isShowWFH = false;
                isShowHolidays = false;
                isShowUser = false;
                isShowExpenses = false;
                isShowLookup = false;
                isShowCompany = false;
                isShowRolepermission = false;
                isShowLeaveRequest = false;
                isShowTimeSheet = false;
                isShowManageTimesheet = true;
                isShowManageLeave = false;
                isShowAsset = false;
                isShowTicket = false;
                isShowAppraisal = false;
                isShowIHRProcess = false;
            }
            if (CurrUrl != "" && CurrUrl.ToUpper() == HelpDocumentation.MANAGELEAVE)
            {
                isShowEmployees = false;
                isShowWFH = false;
                isShowHolidays = false;
                isShowUser = false;
                isShowExpenses = false;
                isShowLookup = false;
                isShowCompany = false;
                isShowRolepermission = false;
                isShowLeaveRequest = false;
                isShowTimeSheet = false;
                isShowManageTimesheet = false;
                isShowManageLeave = true;
                isShowAsset = false;
                isShowTicket = false;
                isShowAppraisal = false;
                isShowIHRProcess = false;
            }
            if (CurrUrl != "" && CurrUrl.ToUpper() == HelpDocumentation.ASSET)
            {
                isShowEmployees = false;
                isShowWFH = false;
                isShowHolidays = false;
                isShowUser = false;
                isShowExpenses = false;
                isShowLookup = false;
                isShowCompany = false;
                isShowRolepermission = false;
                isShowLeaveRequest = false;
                isShowTimeSheet = false;
                isShowManageTimesheet = false;
                isShowManageLeave = false;
                isShowAsset = true;
                isShowTicket = false;
                isShowAppraisal = false;
                isShowIHRProcess = false;
            }
            if (CurrUrl != "" && CurrUrl.ToUpper() == HelpDocumentation.TICKET)
            {
                isShowEmployees = false;
                isShowWFH = false;
                isShowHolidays = false;
                isShowUser = false;
                isShowExpenses = false;
                isShowLookup = false;
                isShowCompany = false;
                isShowRolepermission = false;
                isShowLeaveRequest = false;
                isShowTimeSheet = false;
                isShowManageTimesheet = false;
                isShowManageLeave = false;
                isShowAsset = false;
                isShowTicket = true;
                isShowAppraisal = false;
                isShowIHRProcess = false;
            }

            if (CurrUrl != "" && CurrUrl.ToUpper() == HelpDocumentation.APPRAISAL)
            {
                isShowEmployees = false;
                isShowWFH = false;
                isShowHolidays = false;
                isShowUser = false;
                isShowExpenses = false;
                isShowLookup = false;
                isShowCompany = false;
                isShowRolepermission = false;
                isShowLeaveRequest = false;
                isShowTimeSheet = false;
                isShowManageTimesheet = false;
                isShowManageLeave = false;
                isShowAsset = false;
                isShowTicket = false;
                isShowAppraisal = true;
                isShowIHRProcess = false;
            }
            if (CurrUrl != "" && CurrUrl.ToUpper() == HelpDocumentation.PROCESSDATAS)
            {
                isShowEmployees = false;
                isShowWFH = false;
                isShowHolidays = false;
                isShowUser = false;
                isShowExpenses = false;
                isShowLookup = false;
                isShowCompany = false;
                isShowRolepermission = false;
                isShowLeaveRequest = false;
                isShowTimeSheet = false;
                isShowManageTimesheet = false;
                isShowManageLeave = false;
                isShowAsset = false;
                isShowTicket = false;
                isShowAppraisal = false;
                isShowIHRProcess = true;
            }


            StateHasChanged();
        }

        protected  async Task AllHelpDocumentationShow()
        {
            isShowAllHelpDocumentation = false;
            isShowEmployees = true;
            isShowWFH = true;
            isShowHolidays = true;
            isShowUser = true;
            isShowLookup = true;
            isShowRolepermission = true;
            isShowExpenses = true;
            ShowDialog = true;
            isShowCompany = true;
            isShowLeaveRequest = true;
            isShowTimeSheet = true;
            isShowManageTimesheet = true;
            isShowManageLeave = true;
            isShowAsset = true;
            isShowTicket = true;
            isShowAppraisal = true;
            isShowIHRProcess = true;
            UrlNavigationManager.NavigateTo(UrlNavigationManager.Uri);
            StateHasChanged();
        }

        public void Close() 
        {
            ShowDialog = false;
            StateHasChanged();
        }
        public void CloseImageModal() 
        {
            showImageModal = false;
        }
        public string ImgeUrlPath { get; set; }
        public string HelpHeaderText { get; set; }
        protected void  openImageModalPopUp(EventArgs e, string image, string helpHeaderText)
        {
            HelpHeaderText = "";
            ImgeUrlPath = "";
            var ImagePath = "";
            ImagePath = image;
            ImgeUrlPath = image;
            HelpHeaderText = helpHeaderText;
            showImageModal = true;
            StateHasChanged();
        }
    }



}
