using Blazored.SessionStorage;
using Blazored.Toast.Services;
using ILT.IHR.UI.Service;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;

namespace ILT.IHR.UI.Pages.WorkFromHome
{
    public class ApproveDenyWFHBase: ComponentBase
    {
        [Inject]
        public IConfiguration Configuration { get; set; }
        [Inject]
        protected ISessionStorageService sessionStorage { get; set; }
        [Inject]
        protected IToastService toastService { get; set; } //Service
        [Inject]
        protected IWorkFromHomeService WorkFromHomeService { get; set; } //Service
      
        [Inject]
        protected IEmployeeService EmployeeService { get; set; } //Service
        [Inject]
        protected ILookupService LookupService { get; set; } //Service
        [Inject]
        public ICommonService CommonService { get; set; } //Service
        protected DTO.User user { get; set; }
        [Parameter]
        public EventCallback<bool> UpdateWFHList { get; set; }
        protected IEnumerable<DTO.Employee> EmployeeList { get; set; }
        protected List<ListValue> VacationTypeList { get; set; }
        protected List<ListValue> StatusList { get; set; }
        protected bool ShowDialog { get; set; }
        protected int WFHID { get; set; }
        protected string AccountsEmail { get; set; }
        protected WFH WFH = new WFH();
        public decimal BalanceWFHs { get; set; }
        public bool isApproveButtonDisabled { get; set; } = false;
        public bool isDenyButtonDisabled { get; set; } = false;
        public bool isCancelButtonDisabled { get; set; } = false;
        public List<Module> Modules { get; set; }
        [Inject]
        public IRoleService RoleService { get; set; } //Service
        ILT.IHR.DTO.EmailApproval emailapproval = new ILT.IHR.DTO.EmailApproval();
        [Inject]
        public IEmailApprovalService EmailApprovalService { get; set; } //Service 
        protected override async Task OnInitializedAsync()
        {
            user = await sessionStorage.GetItemAsync<DTO.User>("User");
            await LoadDropDown();
            Modules = (await RoleService.GetModules()).ToList();
        }
        private async Task LoadDropDown()
        {
            AccountsEmail = Configuration["EmailNotifications:" + this.user.ClientID.ToUpper() + ":WFHRequest"];
            Response<IEnumerable<ILT.IHR.DTO.Employee>> respEmployees = await EmployeeService.GetEmployees();
            if (respEmployees.MessageType == MessageType.Success)
                EmployeeList = respEmployees.Data;
            else
                toastService.ShowError("Error occured", "");
            Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());
            if (resp.MessageType == MessageType.Success)
            {
                VacationTypeList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.VACATIONTYPE).ToList();
                StatusList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.WFHSTATUS).ToList();
            }
        }
        
        public void Show(int Id)
        {
            WFHID = Id;
            ResetDialog();
            if (WFHID != 0)
            {
                GetDetails(WFHID);
            }
        }
        private async Task GetDetails(int Id)
        {
            Response<WFH> resp = new Response<WFH>();
            resp = await WorkFromHomeService.GetWFHByIdAsync(Id, 0, 0);
            WFH = resp.Data;
            ShowDialog = true;
            StateHasChanged();
        }
       
        protected async Task Approve()
        {
            if (isApproveButtonDisabled)
                return;
            isApproveButtonDisabled = true;
            string ClientID = await sessionStorage.GetItemAsync<string>("ClientID");
            Response<WFH> resp = new Response<WFH>();
            WFH.StatusID = StatusList.Find(x => x.ValueDesc.ToUpper() == "APPROVED").ListValueID;
            var reposnse = await WorkFromHomeService.UpdateWFH(WFHID, WFH);
            if (reposnse.MessageType == MessageType.Success)
            {
                Response<ILT.IHR.DTO.EmailApproval> respEmailApproval = new Response<ILT.IHR.DTO.EmailApproval>();
                DTO.EmailApproval emailApproval = new DTO.EmailApproval(); ;
                if (WFH.LinkID != Guid.Empty)
                {
                    respEmailApproval = await EmailApprovalService.GetEmailApprovalByIdAsync(WFH.LinkID) as Response<ILT.IHR.DTO.EmailApproval>;
                    emailApproval = respEmailApproval.Data;
                    emailApproval.ModifiedBy = user.FirstName + " " + user.LastName;
                    await EmailApprovalService.EamilApprovalAction(ClientID, WFH.LinkID, "APPROVED", "WFH");
                }
                else
                {
                    emailApproval.ModuleID = Modules.Find(m => m.ModuleShort.ToUpper() == "WFHREQUEST").ModuleID;
                    emailApproval.ID = WFH.WFHID;
                    emailApproval.ValidTime = DateTime.Now.AddDays(Configuration["EmailApprovalValidity"] == null ? 1 : Convert.ToInt32(Configuration["EmailApprovalValidity"]));
                    emailApproval.IsActive = true;
                    emailApproval.LinkID = Guid.NewGuid();
                    emailApproval.ApproverEmail = await GetEmail(WFH.ApproverID);
                    emailApproval.CreatedBy = user.FirstName + " " + user.LastName;
                }


                EmailFields emailFields = await prepareApprovedMail();
                emailApproval.EmailApprovalID = 0;
                emailApproval.LinkID = Guid.Empty;
                emailApproval.EmailSubject = emailFields.EmailSubject;
                emailApproval.EmailBody = emailFields.EmailBody;
                emailApproval.EmailCC = emailFields.EmailCC;
                emailApproval.EmailTo = emailFields.EmailTo;
                emailApproval.Value = null;
                emailApproval.IsActive = true;
                await EmailApprovalService.SaveEmailApproval(emailApproval);
                toastService.ShowSuccess("WFH Approved successfully", "");
                await UpdateWFHList.InvokeAsync(true);
                Cancel();
                // await sendApprovedMail();
            }

            isApproveButtonDisabled = false;
        }
        protected async Task WFHDeny()
        {
            if (isDenyButtonDisabled)
                return;
            isDenyButtonDisabled = true;
            string ClientID = await sessionStorage.GetItemAsync<string>("ClientID");
            Response<WFH> resp = new Response<WFH>();
            WFH.StatusID = StatusList.Find(x => x.ValueDesc.ToUpper() == "DENIED").ListValueID;
            var reposnse = await WorkFromHomeService.UpdateWFH(WFHID, WFH);
            if (reposnse.MessageType == MessageType.Success)
            {
                Response<ILT.IHR.DTO.EmailApproval> respEmailApproval = new Response<ILT.IHR.DTO.EmailApproval>();
                DTO.EmailApproval emailApproval = new DTO.EmailApproval();

                if (WFH.LinkID != Guid.Empty)
                {
                    respEmailApproval = await EmailApprovalService.GetEmailApprovalByIdAsync(WFH.LinkID) as Response<ILT.IHR.DTO.EmailApproval>;
                    emailApproval = respEmailApproval.Data;
                    emailApproval.ModifiedBy = user.FirstName + " " + user.LastName;
                    await EmailApprovalService.EamilApprovalAction(ClientID, WFH.LinkID, "DENIED", "WFH");
                }
                else
                {
                    emailApproval.ModuleID = Modules.Find(m => m.ModuleShort.ToUpper() == "WFHREQUEST").ModuleID;
                    emailApproval.ID = WFH.WFHID;
                    emailApproval.ValidTime = DateTime.Now.AddDays(Configuration["EmailApprovalValidity"] == null ? 1 : Convert.ToInt32(Configuration["EmailApprovalValidity"]));
                    emailApproval.IsActive = true;
                    emailApproval.LinkID = Guid.NewGuid();
                    emailApproval.ApproverEmail = await GetEmail(WFH.ApproverID);
                    emailApproval.CreatedBy = user.FirstName + " " + user.LastName;
                }


                EmailFields emailFields = await prepareDenyMail();
                emailApproval.EmailApprovalID = 0;
                emailApproval.LinkID = Guid.Empty;
                emailApproval.EmailSubject = emailFields.EmailSubject;
                emailApproval.EmailBody = emailFields.EmailBody;
                emailApproval.EmailCC = emailFields.EmailCC;
                emailApproval.EmailTo = emailFields.EmailTo;
                emailApproval.Value = null;
                emailApproval.IsActive = true;
                await EmailApprovalService.SaveEmailApproval(emailApproval);
                toastService.ShowSuccess("WFH Denied successfully", "");
                await UpdateWFHList.InvokeAsync(true);
                Cancel();
               // await sendDenyMail();
            }
            isDenyButtonDisabled = false;
        }

        protected async Task WFHCancel()
        {
            if (isCancelButtonDisabled)
                return;
            isCancelButtonDisabled = true;
            string ClientID = await sessionStorage.GetItemAsync<string>("ClientID");
            Response<WFH> resp = new Response<WFH>();
            WFH.StatusID = StatusList.Find(x => x.ValueDesc.ToUpper() == "CANCELLED").ListValueID;
            var reposnse = await WorkFromHomeService.UpdateWFH(WFHID, WFH);
            if (reposnse.MessageType == MessageType.Success)
            {
                Response<ILT.IHR.DTO.EmailApproval> respEmailApproval = new Response<ILT.IHR.DTO.EmailApproval>();
                DTO.EmailApproval emailApproval = new DTO.EmailApproval(); ;

                if (WFH.LinkID != Guid.Empty)
                {
                    respEmailApproval = await EmailApprovalService.GetEmailApprovalByIdAsync(WFH.LinkID) as Response<ILT.IHR.DTO.EmailApproval>;
                    emailApproval = respEmailApproval.Data;
                    emailApproval.ModifiedBy = user.FirstName + " " + user.LastName;
                    await EmailApprovalService.EamilApprovalAction(ClientID, WFH.LinkID, "CANCELLED", "WFH");
                }
                else
                {
                    emailApproval.ModuleID = Modules.Find(m => m.ModuleShort.ToUpper() == "WFHREQUEST").ModuleID;
                    emailApproval.ID = WFH.WFHID;
                    emailApproval.ValidTime = DateTime.Now.AddDays(Configuration["EmailApprovalValidity"] == null ? 1 : Convert.ToInt32(Configuration["EmailApprovalValidity"]));
                    emailApproval.IsActive = true;
                    emailApproval.LinkID = Guid.NewGuid();
                    emailApproval.ApproverEmail = await GetEmail(WFH.ApproverID);
                    emailApproval.CreatedBy = user.FirstName + " " + user.LastName;
                }


                EmailFields emailFields = await prepareCancelMail();
                emailApproval.EmailApprovalID = 0;
                emailApproval.LinkID = Guid.Empty;
                emailApproval.EmailSubject = emailFields.EmailSubject;
                emailApproval.EmailBody = emailFields.EmailBody;
                emailApproval.EmailCC = emailFields.EmailCC;
                emailApproval.EmailTo = emailFields.EmailTo;
                emailApproval.Value = null;
                emailApproval.IsActive = true;
                await EmailApprovalService.SaveEmailApproval(emailApproval);
                toastService.ShowSuccess("WFH cancelled successfully", "");
                await UpdateWFHList.InvokeAsync(true);
                Cancel();
                //await sendCancelMail();
            }
            isCancelButtonDisabled = false;
        }
        public void Cancel()
        {
            ShowDialog = false;
            StateHasChanged();
        }
        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }

        private void ResetDialog()
        {
            WFH = new WFH { };
        }
        public async Task<EmailFields> prepareApprovedMail()
        {
            EmailFields emailFields = new EmailFields();
            emailFields.isMultipleEmail = true;
            string RequesterEmail = await GetEmail(WFH.EmployeeID);
            string ApproverEmail = await GetEmail(WFH.ApproverID);
            emailFields.EmailTo = RequesterEmail;
            emailFields.EmailCCList = new List<string>();
            emailFields.EmailCCList.Add(AccountsEmail);
            emailFields.EmailCCList.Add(ApproverEmail);
            //emailFields.EmailCC = emailFields.EmailCCList.Aggregate((x, y) => x + ";" + y); // String.Join(";", emailFields.EmailCCList.ToArray());
            //common.EmailCC=AccountsEmail;
            emailFields.EmailSubject = "WFH request approved for "+ WFH.EmployeeName;
            emailFields.EmailBody = "WFH Request for " + WFH.EmployeeName +
                " has been approved.<br/>" +
                "<ul style='margin-bottom: 0px;'><li>Title: " + WFH.Title +
                "</li><li>Start Date: " + FormatDate(WFH.StartDate) +
                "</li><li>End Date: " + FormatDate(WFH.EndDate) +
                "</li>";
            if (!string.IsNullOrEmpty(WFH.Comment))
            {
                emailFields.EmailBody = emailFields.EmailBody + "</li><li>Comments: " + WFH.Comment + "</ul>";
            }
            return emailFields;
        }
        
        public async Task<EmailFields> prepareDenyMail()
        {
            EmailFields emailFields = new EmailFields();
            emailFields.EmailTo = await GetEmail(WFH.EmployeeID);
            emailFields.EmailCC = await GetEmail(WFH.ApproverID);
            emailFields.EmailSubject = "WFH request denied for " + WFH.EmployeeName;
            emailFields.EmailBody = "WFH request for " + WFH.EmployeeName +
                " has been denied.<br/>" +
                "<ul style='margin-bottom: 0px;'><li>Title: " + WFH.Title +
                "</li><li>Start Date: " + FormatDate(WFH.StartDate) +
                "</li><li>End Date: " + FormatDate(WFH.EndDate) +
                "</li>";
            if (!string.IsNullOrEmpty(WFH.Comment))
            {
                emailFields.EmailBody = emailFields.EmailBody + "</li><li>Comments: " + WFH.Comment + "</ul>";
            }
            return emailFields;
        }
        
        public async Task<EmailFields> prepareCancelMail()
        {
            EmailFields emailFields = new EmailFields();
            emailFields.isMultipleEmail = true;
            emailFields.EmailCCList = new List<string>();
            string RequesterEmail = await GetEmail(WFH.EmployeeID);
            string ApproverEmail = await GetEmail(WFH.ApproverID);
            emailFields.EmailTo = RequesterEmail;
            emailFields.EmailCCList = new List<string>();
            emailFields.EmailCCList.Add(AccountsEmail);
            emailFields.EmailCCList.Add(ApproverEmail);
           // emailFields.EmailCC = emailFields.EmailCCList.Aggregate((x, y) => x + ";" + y);
            //common.EmailCC=AccountsEmail;
            emailFields.EmailSubject = "Approved WFH Request Cancelled";
            emailFields.EmailBody = "Below approved WFH Request for " + WFH.EmployeeName +
                " has been cancelled.<br/>" +
                "<ul style='margin-bottom: 0px;'><li>Title: " + WFH.Title +
                "</li><li>Start Date: " + FormatDate(WFH.StartDate) +
                "</li><li>End Date: " + FormatDate(WFH.EndDate) +
                "</li>";
            if (!string.IsNullOrEmpty(WFH.Comment))
            {
                emailFields.EmailBody = emailFields.EmailBody + "</li><li>Comments: " + WFH.Comment + "</ul>";
            }
            return emailFields;
        }
       
        protected async Task<string> GetEmail(int? EmployeeID)
        {
            DTO.Employee emp = EmployeeList.ToList().Find(x => x.EmployeeID == EmployeeID);
            return !string.IsNullOrEmpty(emp.LoginEmail) ? emp.LoginEmail : emp.Email;
        }
        protected string FormatDate(DateTime? dateTime)
        {
            string formattedDate = "";
            if (dateTime.Value != null)
            {
                var date = dateTime.Value.ToString("dddd, MMMM dd");
                formattedDate = date;
            }
            return formattedDate;
        }
        [Inject] IJSRuntime JSRuntime { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            JSRuntime.InvokeVoidAsync("JSHelpers.setFocusByCSSClass");
        }
    }
}
