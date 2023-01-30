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


namespace ILT.IHR.UI.Pages.EmployeeLeaveRequest
{
    public class ApproveDenyLeaveBase: ComponentBase
    {
        [Inject]
        public IConfiguration Configuration { get; set; }
        [Inject]
        protected ISessionStorageService sessionStorage { get; set; }
        [Inject]
        protected IToastService toastService { get; set; } //Service
        [Inject]
        protected ILeaveService LeaveService { get; set; } //Service
        [Inject]
        public ILeaveBalanceService LeaveBalanceService { get; set; } //Service
        [Inject]
        protected IEmployeeService EmployeeService { get; set; } //Service
        [Inject]
        protected ILookupService LookupService { get; set; } //Service
        [Inject]
        public IRoleService RoleService { get; set; } //Service
        [Inject]
        public ICommonService CommonService { get; set; } //Service
        [Inject]
        public IEmailApprovalService EmailApprovalService { get; set; } //Service  
        protected DTO.User user { get; set; }
        [Parameter]
        public EventCallback<bool> UpdateLeaveList { get; set; }
        protected IEnumerable<DTO.Employee> EmployeeList { get; set; }
        protected List<ListValue> VacationTypeList { get; set; }
        protected List<ListValue> StatusList { get; set; }
        public List<Module> Modules { get; set; }
        protected bool ShowDialog { get; set; }
        protected int LeaveID { get; set; }
        protected string AccountsEmail { get; set; }
        protected Leave Leave = new Leave();
        ILT.IHR.DTO.EmailApproval emailapproval = new ILT.IHR.DTO.EmailApproval();

        public decimal BalanceLeaves { get; set; }
        protected bool isApproveDisabled { get; set; } = false;
        protected bool isCancelDisabled { get; set; } = false;
        protected bool isDenyDisabled { get; set; } = false;
        public string ClientID { get; set; }



        protected override async Task OnInitializedAsync()
        {
            user = await sessionStorage.GetItemAsync<DTO.User>("User");
            Modules = (await RoleService.GetModules()).ToList();
            await LoadDropDown();
        }
        private async Task LoadDropDown()
        {
            AccountsEmail = Configuration["EmailNotifications:" + this.user.ClientID.ToUpper() + ":LeaveRequest"];
            Response<IEnumerable<ILT.IHR.DTO.Employee>> respEmployees = await EmployeeService.GetEmployees();
            if (respEmployees.MessageType == MessageType.Success)
                EmployeeList = respEmployees.Data;
            else
                toastService.ShowError(ErrorMsg.ERRORMSG);
            Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());
            if (resp.MessageType == MessageType.Success)
            {
                VacationTypeList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.VACATIONTYPE).ToList();
                StatusList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.LEAVEREQUESTSTATUS).ToList();
            }
        }
        
        public void Show(int Id)
        {
            LeaveID = Id;
            ResetDialog();
            if (LeaveID != 0)
            {
                GetDetails(LeaveID);
            }
        }
        private async Task GetDetails(int Id)
        {
            Response<Leave> resp = new Response<Leave>();
            resp = await LeaveService.GetLeaveByIdAsync(Id, 0, 0);
            Leave = resp.Data;
            getBalanceLeaves(Leave.EmployeeID);
            isfirstElementFocus = true;
            ShowDialog = true;
            StateHasChanged();
        }        

        private async Task getBalanceLeaves(int? EmployeeID)
        {
            var balanceLeaveResp = await LeaveBalanceService.GetLeaveBalance(EmployeeID);
            if (balanceLeaveResp.MessageType == MessageType.Success)
            {
                var VacationBalance = balanceLeaveResp.Data.Where(x => x.LeaveYear == DateTime.Now.Year).ToList();
                BalanceLeaves = VacationBalance.FirstOrDefault().VacationBalance;
                StateHasChanged();
            }
        }
        protected async Task Approve()
        {
            if (isApproveDisabled)
                return;
            isApproveDisabled = true;
            string ClientID = await sessionStorage.GetItemAsync<string>("ClientID");
            Response<Leave> resp = new Response<Leave>();
            Leave.ModifiedBy = user.FirstName + " " + user.LastName;
            Leave.StatusID = StatusList.Find(x => x.ValueDesc.ToUpper() == "APPROVED").ListValueID;
            var reposnse = await LeaveService.UpdateLeave(LeaveID, Leave);
            if (reposnse.MessageType == MessageType.Success)
            {
                Response<ILT.IHR.DTO.EmailApproval> respEmailApproval = new Response<ILT.IHR.DTO.EmailApproval>();
                DTO.EmailApproval emailApproval = new DTO.EmailApproval(); ;
                if (Leave.LinkID != Guid.Empty)
                {
                    respEmailApproval = await EmailApprovalService.GetEmailApprovalByIdAsync(Leave.LinkID) as Response<ILT.IHR.DTO.EmailApproval>;
                    emailApproval = respEmailApproval.Data;
                    emailApproval.ModifiedBy = user.FirstName + " " + user.LastName;
                    await EmailApprovalService.EamilApprovalAction(ClientID, Leave.LinkID, "APPROVED", "LEAVE");
                }
                else
                {
                    emailApproval.ModuleID = Modules.Find(m => m.ModuleShort.ToUpper() == "LEAVEREQUEST").ModuleID;
                    emailApproval.ID = Leave.LeaveID;
                    emailApproval.ValidTime = DateTime.Now.AddDays(Configuration["EmailApprovalValidity"] == null ? 1 : Convert.ToInt32(Configuration["EmailApprovalValidity"]));
                    emailApproval.IsActive = true;
                    emailApproval.LinkID = Guid.NewGuid();
                    emailApproval.ApproverEmail = await GetEmail(Leave.ApproverID);
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

                toastService.ShowSuccess("Leave Approved successfully", "");
                await UpdateLeaveList.InvokeAsync(true);
                Cancel();
            }
            isApproveDisabled = false;
        }
        protected async Task LeaveDeny()
        {
            if (isDenyDisabled)
                return;
            isDenyDisabled = true;
            string ClientID = await sessionStorage.GetItemAsync<string>("ClientID");
            Response<Leave> resp = new Response<Leave>();
            Leave.ModifiedBy = user.FirstName + " " + user.LastName;
            Leave.StatusID = StatusList.Find(x => x.ValueDesc.ToUpper() == "DENIED").ListValueID;
            var reposnse = await LeaveService.UpdateLeave(LeaveID, Leave);
            if (reposnse.MessageType == MessageType.Success)
            {

                Response<ILT.IHR.DTO.EmailApproval> respEmailApproval = new Response<ILT.IHR.DTO.EmailApproval>();
                DTO.EmailApproval emailApproval = new DTO.EmailApproval();

                if (Leave.LinkID != Guid.Empty)
                {
                    respEmailApproval = await EmailApprovalService.GetEmailApprovalByIdAsync(Leave.LinkID) as Response<ILT.IHR.DTO.EmailApproval>;
                    emailApproval = respEmailApproval.Data;
                    emailApproval.ModifiedBy = user.FirstName + " " + user.LastName;
                    await EmailApprovalService.EamilApprovalAction(ClientID, Leave.LinkID, "DENIED", "LEAVE");
                }
                else
                {
                    emailApproval.ModuleID = Modules.Find(m => m.ModuleShort.ToUpper() == "LEAVEREQUEST").ModuleID;
                    emailApproval.ID = Leave.LeaveID;
                    emailApproval.ValidTime = DateTime.Now.AddDays(Configuration["EmailApprovalValidity"] == null ? 1 : Convert.ToInt32(Configuration["EmailApprovalValidity"]));
                    emailApproval.IsActive = true;
                    emailApproval.LinkID = Guid.NewGuid();
                    emailApproval.ApproverEmail = await GetEmail(Leave.ApproverID);
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

                toastService.ShowSuccess("Leave Denied successfully", "");
                await UpdateLeaveList.InvokeAsync(true);
                Cancel();
            }
            isDenyDisabled = false;
        }

        protected async Task LeaveCancel()
        {
            if (isCancelDisabled)
                return;
            isCancelDisabled = true;
            string ClientID = await sessionStorage.GetItemAsync<string>("ClientID");
            Response<Leave> resp = new Response<Leave>();
            Leave.ModifiedBy = user.FirstName + " " + user.LastName;
            Leave.StatusID = StatusList.Find(x => x.ValueDesc.ToUpper() == "CANCELLED").ListValueID;
            var reposnse = await LeaveService.UpdateLeave(LeaveID, Leave);
            if (reposnse.MessageType == MessageType.Success)
            {
                Response<ILT.IHR.DTO.EmailApproval> respEmailApproval = new Response<ILT.IHR.DTO.EmailApproval>();
                DTO.EmailApproval emailApproval = new DTO.EmailApproval(); ;

                if (Leave.LinkID != Guid.Empty)
                {
                    respEmailApproval = await EmailApprovalService.GetEmailApprovalByIdAsync(Leave.LinkID) as Response<ILT.IHR.DTO.EmailApproval>;
                    emailApproval = respEmailApproval.Data;
                    emailApproval.ModifiedBy = user.FirstName + " " + user.LastName;
                    await EmailApprovalService.EamilApprovalAction(ClientID, Leave.LinkID, "CANCELLED", "LEAVE");
                }
                else
                {
                    emailApproval.ModuleID = Modules.Find(m => m.ModuleShort.ToUpper() == "LEAVEREQUEST").ModuleID;
                    emailApproval.ID = Leave.LeaveID;
                    emailApproval.ValidTime = DateTime.Now.AddDays(Configuration["EmailApprovalValidity"] == null ? 1 : Convert.ToInt32(Configuration["EmailApprovalValidity"]));
                    emailApproval.IsActive = true;
                    emailApproval.LinkID = Guid.NewGuid();
                    emailApproval.ApproverEmail = await GetEmail(Leave.ApproverID);
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
                toastService.ShowSuccess("Leave cancelled successfully", "");
                await UpdateLeaveList.InvokeAsync(true);
                Cancel();
            }
            isCancelDisabled = false;
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
            Leave = new Leave { };
        }

        public async Task<EmailFields> prepareApprovedMail()
        {
            EmailFields emailFields = new EmailFields();
            emailFields.isMultipleEmail = true;
            string RequesterEmail = await GetEmail(Leave.EmployeeID);
            string ApproverEmail = await GetEmail(Leave.ApproverID);
            emailFields.EmailTo = RequesterEmail;
            emailFields.EmailCCList = new List<string>();
            emailFields.EmailCCList.Add(AccountsEmail);
            emailFields.EmailCCList.Add(ApproverEmail);
            emailFields.EmailCC = emailFields.EmailCCList.Aggregate((x, y) => x+";"+y); // String.Join(";", emailFields.EmailCCList.ToArray());
            //common.EmailCC=AccountsEmail;
            emailFields.EmailSubject = "Leave request approved for " + Leave.EmployeeName;
            emailFields.EmailBody = "Leave Request for " + Leave.EmployeeName +
                " has been approved.<br/>" +
                "<ul style='margin-bottom: 0px;'><li>Type: " + Leave.LeaveType + "<br/>" +
                "</li><li>Title: " + Leave.Title +
                "</li><li>Description: " + Leave.Detail +
                "</li><li>Start Date: " + FormatDate(Leave.StartDate) +
                "</li><li>End Date: " + FormatDate(Leave.EndDate) +
                "</li><li>Duration: " + Leave.Duration + " Days";
            if (!string.IsNullOrEmpty(Leave.Comment))
            {
                emailFields.EmailBody = emailFields.EmailBody + "</li><li>Comments: " + Leave.Comment + "</ul>";
            }
            else
            {
                emailFields.EmailBody += "</ul>";
            }
            return emailFields;
        }

        public async Task<EmailFields> prepareDenyMail()
        {
            EmailFields emailFields = new EmailFields();
            emailFields.EmailTo = await GetEmail(Leave.EmployeeID);
            emailFields.EmailCC = await GetEmail(Leave.ApproverID);
            emailFields.EmailSubject = "Leave request denied for " + Leave.EmployeeName ;
            emailFields.EmailBody = "Leave Request for " + Leave.EmployeeName +
                " has been denied.<br/>" +
                "<ul style='margin-bottom: 0px;'><li>Type: " + Leave.LeaveType + "<br/>" +
                "</li><li>Title: " + Leave.Title +
                "</li><li>Description: " + Leave.Detail +
                "</li><li>Start Date: " + FormatDate(Leave.StartDate) +
                "</li><li>End Date: " + FormatDate(Leave.EndDate) +
                "</li><li>Duration: " + Leave.Duration + " Days";
            if (!string.IsNullOrEmpty(Leave.Comment))
            {
                emailFields.EmailBody = emailFields.EmailBody + "</li><li>Comments: " + Leave.Comment + "</ul>";
            }
            else
            {
                emailFields.EmailBody += "</ul>";
            }
            return emailFields;
        }

        public async Task<EmailFields> prepareCancelMail()
        {
            EmailFields emailFields = new EmailFields();
            emailFields.isMultipleEmail = true;
            emailFields.EmailCCList = new List<string>();
            string RequesterEmail = await GetEmail(Leave.EmployeeID);
            string ApproverEmail = await GetEmail(Leave.ApproverID);
            emailFields.EmailTo = RequesterEmail;
            emailFields.EmailCCList = new List<string>();
            emailFields.EmailCCList.Add(AccountsEmail);
            emailFields.EmailCCList.Add(ApproverEmail);
            emailFields.EmailCC = emailFields.EmailCCList.Aggregate((x, y) => x + ";" + y);
            //common.EmailCC=AccountsEmail;
            emailFields.EmailSubject = "Leave request cancelled for " + Leave.EmployeeName;
            emailFields.EmailBody = "Below approved Leave Request for " + Leave.EmployeeName +
                " has been cancelled.<br/>" +
                "<ul style='margin-bottom: 0px;'><li>Type: " + Leave.LeaveType + "<br/>" +
                "</li><li>Title: " + Leave.Title +
                "</li><li>Description: " + Leave.Detail +
                "</li><li>Start Date: " + FormatDate(Leave.StartDate) +
                "</li><li>End Date: " + FormatDate(Leave.EndDate) +
                "</li><li>Duration: " + Leave.Duration + " Days";
            if (!string.IsNullOrEmpty(Leave.Comment))
            {
                emailFields.EmailBody = emailFields.EmailBody + "</li><li>Comments: " + Leave.Comment + "</ul>";
            } else
            {
                emailFields.EmailBody += "</ul>";
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
                var date = dateTime.Value.ToString("dd MMM yyy");
                formattedDate = date;
            }
            return formattedDate;
        }
        [Inject] IJSRuntime JSRuntime { get; set; }
        public bool isfirstElementFocus { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (isfirstElementFocus)
            {
                JSRuntime.InvokeVoidAsync("JSHelpers.setFocusByCSSClass");
                isfirstElementFocus = false;
            }
        }
    }
}
