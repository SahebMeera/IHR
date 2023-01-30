using Blazored.SessionStorage;
using Blazored.Toast.Services;
using ILT.IHR.DTO;
using ILT.IHR.UI.Service;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.Extensions.Configuration;

namespace ILT.IHR.UI.Pages.WorkFromHome
{
    public class AddEditWFHBase : ComponentBase
    {
        [Inject]
        public ISessionStorageService sessionStorage { get; set; }
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public  IWorkFromHomeService WorkFromHomeService { get; set; } //Service

        [Inject]
        public IEmployeeService EmployeeService { get; set; } //Service
        [Inject]
        public ILookupService LookupService { get; set; } //Service
        [Inject]
        public ICommonService CommonService { get; set; } //Service
        protected DTO.User user { get; set; }
        [Parameter]
        public EventCallback<bool> UpdateWFHList { get; set; }
        protected IEnumerable<DTO.Employee> EmployeeList { get; set; }
        protected IEnumerable<DTO.Employee> lstEmployees { get; set; }
        protected List<ListValue> StatusList { get; set; }
        public bool ShowDialog { get; set; }
        private int WFHID { get; set; }
        private int EmployeeID { get; set; }
        protected bool isDisabled { get; set; }
        protected bool isCancel { get; set; }
        protected bool isShowComments { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public List<Module> Modules { get; set; }
        protected DTO.Employee employeeDetails { get; set; }
        [Inject]
        public IRoleService RoleService { get; set; } //Service
        [Inject]
        public IConfiguration Configuration { get; set; } //configuration 
        [Inject]
        public IEmailApprovalService EmailApprovalService { get; set; } //Service 
        public bool isSaveButtonDisabled { get; set; } = false;
        public WFH WFH = new WFH();
        ILT.IHR.DTO.EmailApproval emailapproval = new ILT.IHR.DTO.EmailApproval();
        protected override async Task OnInitializedAsync()
        {
            int year = DateTime.Now.Year;
            endDate = new DateTime(year, 12, 31).ToString("yyyy-MM-dd");
            startDate = new DateTime(year, 01, 01).ToString("yyyy-MM-dd");
            user = await sessionStorage.GetItemAsync<DTO.User>("User");
            await GetEmployeeDetails(user.EmployeeID);
            WFH.RequesterID = Convert.ToInt32(user.EmployeeID);
            await LoadDropDown();
            Modules = (await RoleService.GetModules()).ToList();
        }
        private async Task LoadDropDown()
        {
            Response<IEnumerable<ILT.IHR.DTO.Employee>> respEmployees = await EmployeeService.GetEmployees();
            if (respEmployees.MessageType == MessageType.Success)
                EmployeeList = respEmployees.Data;
            else
                toastService.ShowError("Error occured", "");
            Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());
            if (resp.MessageType == MessageType.Success)
            {
                StatusList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.WFHSTATUS).ToList();
            }
        }
        protected async Task onEmployeeChange(ChangeEventArgs e)
        {
            if(e.Value != "")
            {
                await GetEmployeeDetails(Convert.ToInt32(e.Value));
            }
            
        }

        protected  async Task GetEmployeeDetails(int? employeeId)
        {
            if(employeeId != null)
            {
                var response = await EmployeeService.GetEmployeeByIdAsync(Convert.ToInt32(employeeId));
                if (response.MessageType == MessageType.Success)
                {
                    employeeDetails = response.Data;
                }
            }
            
        }
        public void Show(int Id, int employeeId, bool cancel = false)
        {
            isCancel = cancel;
            WFHID = Id;
            EmployeeID = employeeId;
            isDisabled = false;
            isShowComments = false;
            lstEmployees = EmployeeList.Where(x => x.ManagerID == employeeId || x.EmployeeID == employeeId);
            ResetDialog();
            if (WFHID != 0)
            {
                GetDetails(WFHID);
            }
            else if (employeeId != 0)
            {
                var currentEmployee = EmployeeList.ToList().Find(x => x.EmployeeID == employeeId);
                if (currentEmployee.ManagerID != null && currentEmployee.ManagerID != 0)
                {
                    WFH.EmployeeID = employeeId;
                    WFH.RequesterID = employeeId;
                    WFH.StartDate = DateTime.Now;
                    WFH.EndDate = DateTime.Now;
                    WFH.StatusID = StatusList.Find(x => x.ValueDesc.ToUpper() == "PENDING").ListValueID;
                    WFH.StatusValue = StatusList.Find(x => x.ListValueID == WFH.StatusID).ValueDesc;
                    isfirstElementFocus = true;
                    ShowDialog = true;
                    StateHasChanged();
                }
                else
                {
                    toastService.ShowError("WFH request can't be created for Employee with no manager", "");
                }
            }
            else
            {
                toastService.ShowError("WFH request can't be created for non-Employees", "");
            }
        }
        
        private async Task GetDetails(int Id)
        {
            Response<WFH> resp = new Response<WFH>();
            resp = await WorkFromHomeService.GetWFHByIdAsync(Id, 0, 0);
            if(resp.MessageType == MessageType.Success)
            {
                WFH = resp.Data;
                if (WFH.Status.ToLower() != "Pending".ToLower())
                {
                    isDisabled = true;
                }
                else
                    isDisabled = false;
                if (isCancel == true)
                {
                    WFH.StatusID = StatusList.Find(x => x.ValueDesc.ToUpper() == "CANCELLED").ListValueID;
                    WFH.StatusValue = StatusList.Find(x => x.ListValueID == WFH.StatusID).ValueDesc;

                }
                if (WFH.Status.ToLower() == "Cancelled".ToLower() || WFH.Status.ToLower() == "Approved".ToLower() || WFH.Status.ToLower() == "Denied".ToLower())
                    isShowComments = true;
                isfirstElementFocus = true;
                ShowDialog = true;
                StateHasChanged();
            }
            
        }
        protected async Task SaveWFH()
        {
            if (isSaveButtonDisabled)
                return;
            isSaveButtonDisabled = true;
            if (WFHID == 0)
            {
                if (WFH.StartDate.Date <= WFH.EndDate.Date)
                {
                    WFH.CreatedBy = user.FirstName + " " + user.LastName;
                    WFH.ApproverID = employeeDetails.ManagerID;
                    WFH.Approver = employeeDetails.Manager;
                    var result = await WorkFromHomeService.SaveWFH(WFH);
                        if (result.MessageType == MessageType.Success)
                    {
                        var WFHResp = await WorkFromHomeService.GetWFHByIdAsync(result.Data.RecordID,0,0);
                        if(WFHResp.MessageType == MessageType.Success)
                        {
                            WFH = WFHResp.Data;
                            EmailFields emailFields = new EmailFields();
                            emailapproval.EmailApprovalID = 0;
                            emailapproval.ModuleID = Modules.Find(m => m.ModuleShort.ToUpper() == "WFHREQUEST").ModuleID;
                            emailapproval.ID = WFH.WFHID;
                            emailapproval.ValidTime = DateTime.Now.AddDays(Configuration["EmailApprovalValidity"] == null ? 1 : Convert.ToInt32(Configuration["EmailApprovalValidity"]));
                            emailapproval.IsActive = true;
                            emailapproval.LinkID = Guid.NewGuid();
                            emailapproval.ApproverEmail = employeeDetails.ManagerEmail;
                            emailapproval.CreatedBy = user.FirstName + " " + user.LastName;
                            emailFields = await prepareEmail();
                            emailapproval.EmailSubject = emailFields.EmailSubject;
                            emailapproval.EmailBody = emailFields.EmailBody;
                            emailapproval.EmailFrom = emailFields.EmailFrom;
                            emailapproval.EmailTo = emailFields.EmailTo;
                            var resultEmailApproval = await EmailApprovalService.SaveEmailApproval(emailapproval);
                        }
                        await UpdateWFHList.InvokeAsync(true);
                        toastService.ShowSuccess("WFH Requested successfully", "");
                        Cancel();
                    }
                    else
                    {
                        toastService.ShowSuccess("Error occured", "");
                    }
                }
                else
                {
                    toastService.ShowError("End date must be greater than start date", "");
                }
            }
            else if (WFHID != 0)
            {
                WFH.ModifiedBy = user.FirstName + " " + user.LastName;
                var result = await WorkFromHomeService.UpdateWFH(WFHID, WFH);
                if (result.MessageType == MessageType.Success)
                {
                    WFH = result.Data;
                    EmailFields emailFields = new EmailFields();
                    Response<ILT.IHR.DTO.EmailApproval> respEmailApproval = new Response<ILT.IHR.DTO.EmailApproval>();
                    if (WFH.LinkID != Guid.Empty)
                    {
                        respEmailApproval = await EmailApprovalService.GetEmailApprovalByIdAsync(WFH.LinkID) as Response<ILT.IHR.DTO.EmailApproval>;
                        emailapproval = respEmailApproval.Data;
                        emailapproval.ModifiedBy = user.FirstName + " " + user.LastName;
                        emailapproval.SentCount = emailapproval.SentCount > 0 ? emailapproval.SentCount - 1 : 0;
                    }
                    else
                    {
                        emailapproval.EmailApprovalID = 0;
                        emailapproval.ModuleID = Modules.Find(m => m.ModuleShort.ToUpper() == "WFHREQUEST").ModuleID;
                        emailapproval.ID = WFH.WFHID;
                        emailapproval.IsActive = true;
                        emailapproval.LinkID = Guid.NewGuid();
                        emailapproval.ApproverEmail = employeeDetails.ManagerEmail;
                        emailapproval.CreatedBy = user.FirstName + " " + user.LastName;
                    }
                    emailapproval.ValidTime = DateTime.Now.AddDays(Configuration["EmailApprovalValidity"] == null ? 1 : Convert.ToInt32(Configuration["EmailApprovalValidity"]));
                    if (isCancel)
                    {
                        emailapproval.LinkID = Guid.Empty;
                        emailFields = await prepareCancelMail();
                    }
                    else
                    {
                        emailFields = await prepareEmail();
                    }
                    emailapproval.EmailSubject = emailFields.EmailSubject;
                    emailapproval.EmailBody = emailFields.EmailBody;
                    emailapproval.EmailFrom = emailFields.EmailFrom;
                    emailapproval.EmailTo = emailFields.EmailTo;
                    var resultEmailApproval = await EmailApprovalService.SaveEmailApproval(emailapproval);
                    toastService.ShowSuccess("WFH Requested successfully", "");
                    await UpdateWFHList.InvokeAsync(true);
                    //if (isCancel)
                    //{
                    //    await sendCancelMail();
                    //}
                    Cancel();
                }
                else
                {
                    toastService.ShowSuccess("Error occured", "");
                }
            }
            isSaveButtonDisabled = false;
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

        private async Task<EmailFields> prepareEmail()
        {
            EmailFields emailFields = new EmailFields();
            string uri = Configuration["EmailApprovalUrl:" + user.ClientID];
            Common common = new Common();
            emailFields.EmailTo = employeeDetails.ManagerEmail;
            emailFields.EmailSubject = "WFH request submitted for " + employeeDetails.EmployeeName;
            emailFields.EmailBody = "There is a WFH request from " + employeeDetails.EmployeeName +
                " pending for your approval<br/>" +
                "<ul style='margin-bottom: 0px;'><li>Title: " + WFH.Title +
                "</li><li>Start Date: " + FormatDate(WFH.StartDate) +
                "</li><li>End Date: " + FormatDate(WFH.EndDate) +
                "</li></ul>";

            return emailFields;
        }

        public async Task<EmailFields> prepareCancelMail()
        {
            EmailFields emailFields = new EmailFields();
            emailFields.EmailTo = employeeDetails.ManagerEmail;
            emailFields.EmailSubject = "WFH request cancelled for " + employeeDetails.EmployeeName;
            emailFields.EmailBody = employeeDetails.EmployeeName + " cancelled his WFH" +
                "<ul style='margin-bottom: 0px;'><li>Title: " + WFH.Title +
                "</li><li>Start Date: " + FormatDate(WFH.StartDate) +
                "</li><li>End Date: " + FormatDate(WFH.EndDate) +
                "</li></ul>";
            return emailFields;
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

        public void startDateChange(ChangeEventArgs e)
        {
            WFH.EndDate = Convert.ToDateTime(e.Value);
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
