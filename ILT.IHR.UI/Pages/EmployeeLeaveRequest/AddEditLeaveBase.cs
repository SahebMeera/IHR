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

namespace ILT.IHR.UI.Pages.EmployeeLeaveRequest
{
    public class AddEditLeaveBase : ComponentBase
    {
        [Inject]
        public ISessionStorageService sessionStorage { get; set; }
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public ILeaveService LeaveService { get; set; } //Service
        [Inject]
        public ILeaveBalanceService LeaveBalanceService { get; set; } //Service
        [Inject]
        public IEmployeeService EmployeeService { get; set; } //Service
        [Inject]
        public ILookupService LookupService { get; set; } //Service
        [Inject]
        public ICommonService CommonService { get; set; } //Service
        [Inject]
        public IRoleService RoleService { get; set; } //Service
        [Inject]
        public IEmailApprovalService EmailApprovalService { get; set; } //Service  
        [Inject]
        public IConfiguration Configuration { get; set; } //configuration  

        protected DTO.User user { get; set; }
        [Parameter]
        public EventCallback<bool> UpdateLeaveList { get; set; }
        protected IEnumerable<DTO.Employee> EmployeeList { get; set; }
        protected IEnumerable<DTO.Employee> lstEmployees { get; set; }
        protected List<ListValue> VacationTypeList { get; set; }
        protected List<ListValue> StatusList { get; set; }
        protected List<Leave> lstEmployeeLeave { get; set; }
        public List<Module> Modules { get; set; } 

        public bool ShowDialog { get; set; }
        private int LeaveID { get; set; }
        private int EmployeeID { get; set; }
        protected bool isDisabled { get; set; }
        protected bool isCancel { get; set; }
        protected bool isShowComments { get; set; }
        protected bool isSaveButtonDisabled { get; set; } = false;
        public string startDate { get; set; }
        public string endDate { get; set; }
        public decimal BalanceLeaves { get; set; }
        protected DTO.Employee employeeDetails { get; set; }
        protected DTO.Employee employeeManagerDetails { get; set; }
        public Leave Leave = new Leave();
        ILT.IHR.DTO.EmailApproval emailapproval = new ILT.IHR.DTO.EmailApproval();
        public string ClientID { get; set; }

        protected override async Task OnInitializedAsync()
        {
            int year = DateTime.Now.Year;
            endDate = new DateTime(year, 12, 31).ToString("yyyy-MM-dd");
            startDate = new DateTime(year, 01, 01).ToString("yyyy-MM-dd");
            user = await sessionStorage.GetItemAsync<DTO.User>("User");
            ClientID = await sessionStorage.GetItemAsync<string>("ClientID");
            await GetEmployeeDetails(user.EmployeeID);
            Leave.RequesterID = Convert.ToInt32(user.EmployeeID);
            await LoadDropDown();
            Modules = (await RoleService.GetModules()).ToList();
        }
        private async Task LoadDropDown()
        {
            Response<IEnumerable<ILT.IHR.DTO.Employee>> respEmployees = await EmployeeService.GetEmployees();
            if (respEmployees.MessageType == MessageType.Success)
                EmployeeList = respEmployees.Data;
            else
                toastService.ShowError(ErrorMsg.ERRORMSG);
            Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());
            if (resp.MessageType == MessageType.Success)
            {
               var  vacList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.VACATIONTYPE).ToList();
                VacationTypeList = vacList.Where(x => x.Value.ToUpper() != ListTypeConstants.LWP.ToUpper()).ToList();
                StatusList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.LEAVEREQUESTSTATUS).ToList();
            }
        }
        protected async Task onEmployeeChange(ChangeEventArgs e)
        {
            if(e.Value != "")
            {
                await GetEmployeeDetails(Convert.ToInt32(e.Value));
                await getBalanceLeaves(Convert.ToInt32(e.Value));
                await getEmployeeLeaves(Convert.ToInt32(e.Value));
            }
            
        }

        protected async Task getEmployeeLeaves(int employeeId)
        {
            var respLeaveRequest = (await LeaveService.GetLeave("EmployeeID", employeeId));
            if (respLeaveRequest.MessageType == MessageType.Success)
            {
                lstEmployeeLeave = respLeaveRequest.Data.ToList();
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
                    var Managerresponse = await EmployeeService.GetEmployeeByIdAsync(Convert.ToInt32(employeeDetails.ManagerID));
                    if (response.MessageType == MessageType.Success)
                    {
                        employeeManagerDetails = Managerresponse.Data;
                        //if (employeeManagerDetails != null && (employeeManagerDetails.TermDate == null || DateTime.Now <= employeeManagerDetails.TermDate))
                        //{
                        //    //SubmitLeave();
                        //}
                        //else
                        //{
                        //    ErrorMessage = "Your Approver is not active";
                        //    isSaveButtonDisabled = true;
                        //}
                    }
                }
            }
            
        }
        public void Show(int Id, int employeeId, List<Leave> leaves,bool cancel = false)
        {
            lstEmployeeLeave = leaves;
            isCancel = cancel;
            LeaveID = Id;
            EmployeeID = employeeId;
            isDisabled = false;
            isShowComments = false;
            lstEmployees = EmployeeList.Where(x => x.ManagerID == employeeId || x.EmployeeID == employeeId);
                ResetDialog();
            if (LeaveID != 0)
            {
                GetDetails(LeaveID);
            }
            else if (employeeId != 0)
            {
                    var currentEmployee = EmployeeList.ToList().Find(x => x.EmployeeID == employeeId);
                    if (currentEmployee.ManagerID != null && currentEmployee.ManagerID != 0)
                    {
                        if (VacationTypeList != null && VacationTypeList.Count == 1)
                        {
                            Leave.LeaveTypeID = VacationTypeList[0].ListValueID;
                        }
                        Leave.EmployeeID = employeeId;
                        Leave.RequesterID = employeeId;
                        Leave.StartDate = DateTime.Now;
                        Leave.EndDate = DateTime.Now;
                        Leave.StatusID = StatusList.Find(x => x.ValueDesc.ToUpper() == "PENDING").ListValueID;
                        Leave.StatusValue = StatusList.Find(x => x.ListValueID == Leave.StatusID).ValueDesc;
                        getBalanceLeaves(Leave.EmployeeID);
                        duration();
                        isfirstElementFocus = true;
                        ShowDialog = true;
                        StateHasChanged();
                    }
                    else
                    {
                        toastService.ShowError("Leave request can't be created for Employee with no manager", "");
                    }
            }
            else
            {
                toastService.ShowError("Leave request can't be created for non-Employees", "");
            }
        }
        private async Task getBalanceLeaves(int? EmployeeID)
        {
            var balanceLeaveResp = await LeaveBalanceService.GetLeaveBalance(EmployeeID);
            if(balanceLeaveResp.MessageType == MessageType.Success)
            {
                var VacationBalance = balanceLeaveResp.Data.Where(x => x.LeaveYear == DateTime.Now.Year).ToList();
                BalanceLeaves = VacationBalance.FirstOrDefault().VacationBalance;
                StateHasChanged();
            }
        }
        private async Task GetDetails(int Id)
        {
            Response<Leave> resp = new Response<Leave>();
            resp = await LeaveService.GetLeaveByIdAsync(Id, 0, 0);
            if(resp.MessageType == MessageType.Success)
            {
                Leave = resp.Data;
                getBalanceLeaves(Leave.EmployeeID);
                if (Leave.Status.ToLower() != "Pending".ToLower())
                {
                    isDisabled = true;
                }
                else
                    isDisabled = false;
                if (isCancel == true)
                {
                    Leave.StatusID = StatusList.Find(x => x.ValueDesc.ToUpper() == "CANCELLED").ListValueID;
                    Leave.StatusValue = StatusList.Find(x => x.ListValueID == Leave.StatusID).ValueDesc;
                }
                if (Leave.Status.ToLower() == "Cancelled".ToLower() || Leave.Status.ToLower() == "Approved".ToLower() || Leave.Status.ToLower() == "Denied".ToLower())
                    isShowComments = true;
                isfirstElementFocus= true;
                ShowDialog = true;
                StateHasChanged();
            }
            
        }
        protected async Task SubmitLeave()
        {
            if (LeaveID == 0)
            {
                if (Leave.StartDate.Date <= Leave.EndDate.Date)
                {
                    int existingLeaveIndex = lstEmployeeLeave.FindIndex(x => ((x.StartDate <= Leave.EndDate && Leave.StartDate <= x.EndDate) ||
               (x.StartDate >= Leave.StartDate && x.EndDate <= Leave.EndDate)) && (x.StatusValue != LeaveStatus.CANCELLED) && (x.StatusValue != LeaveStatus.DENIED));
                    if (existingLeaveIndex == -1)
                    {
                        Leave.CreatedBy = user.FirstName + " " + user.LastName;
                        Leave.ApproverID = employeeDetails.ManagerID;
                        Leave.Approver = employeeDetails.Manager;
                        Leave.EmployeeName = employeeDetails.EmployeeName;
                        Leave.LeaveType = VacationTypeList.Find(x => x.ListValueID == Leave.LeaveTypeID).ValueDesc;
                        var result = await LeaveService.SaveLeave(Leave);
                        Leave = result.Data;

                        if (result.MessageType == MessageType.Success && Leave.LeaveID != 0)
                        {
                            var leaveResp = await LeaveService.GetLeaveByIdAsync(result.Data.RecordID, 0, 0);
                            if (leaveResp.MessageType == MessageType.Success)
                            {
                                Leave = leaveResp.Data;
                                EmailFields emailFields = new EmailFields();
                                emailapproval.EmailApprovalID = 0;
                                emailapproval.ModuleID = Modules.Find(m => m.ModuleShort.ToUpper() == "LEAVEREQUEST").ModuleID;
                                emailapproval.ID = Leave.LeaveID;
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
                            await UpdateLeaveList.InvokeAsync(true);
                            toastService.ShowSuccess("Leave Requested successfully", "");
                            Cancel();
                        }
                        else
                        {
                            toastService.ShowError(ErrorMsg.ERRORMSG);
                        }
                    }
                    else
                    {
                        toastService.ShowError("Leave exists for current range");
                    }
                }
                else
                {
                    toastService.ShowError("End date must be greater than start date", "");
                }
            }
            else if (LeaveID != 0)
            {
                Leave.ModifiedBy = user.FirstName + " " + user.LastName;
                var result = await LeaveService.UpdateLeave(LeaveID, Leave);
                if (result.MessageType == MessageType.Success)
                {
                    Leave = result.Data;
                    EmailFields emailFields = new EmailFields();
                    Response<ILT.IHR.DTO.EmailApproval> respEmailApproval = new Response<ILT.IHR.DTO.EmailApproval>();
                    if (Leave.LinkID != Guid.Empty)
                    {
                        respEmailApproval = await EmailApprovalService.GetEmailApprovalByIdAsync(Leave.LinkID) as Response<ILT.IHR.DTO.EmailApproval>;
                        emailapproval = respEmailApproval.Data;
                        emailapproval.ModifiedBy = user.FirstName + " " + user.LastName;
                        emailapproval.SentCount = emailapproval.SentCount > 0 ? emailapproval.SentCount - 1 : 0;
                    }
                    else
                    {
                        emailapproval.EmailApprovalID = 0;
                        emailapproval.ModuleID = Modules.Find(m => m.ModuleShort.ToUpper() == "LEAVEREQUEST").ModuleID;
                        emailapproval.ID = Leave.LeaveID;
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
                    toastService.ShowSuccess("Leave Requested successfully", "");
                    await UpdateLeaveList.InvokeAsync(true);
                    //if (isCancel)
                    //{
                    //    await sendCancelMail();
                    //}
                    Cancel();
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
            }
        }
        public string ErrorMessage;
        protected async Task SaveLeave()
        {
            if (isSaveButtonDisabled)
                return;
            isSaveButtonDisabled = true;
            if (employeeManagerDetails != null && (employeeManagerDetails.TermDate == null || DateTime.Now <= employeeManagerDetails.TermDate))
            {
                await SubmitLeave();
            } else
            {
                toastService.ShowError("Manager (Leave Approver) is inactive", "");
                isSaveButtonDisabled = false;
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
            Leave = new Leave { };
        }

        private async Task<EmailFields> prepareEmail() 
        {
            EmailFields emailFields = new EmailFields();
            string uri = Configuration["EmailApprovalUrl:" + user.ClientID];
            Common common = new Common();
            emailFields.EmailTo = employeeDetails.ManagerEmail;
            emailFields.EmailSubject = "Leave request submitted for " + employeeDetails.EmployeeName;
            emailFields.EmailBody = "There is a leave request from " + employeeDetails.EmployeeName +
                " pending your approval<br/>" +
                "<ul style='margin-bottom: 0px;'><li>Type: " + Leave.LeaveType + "<br/>" +
                "</li><li>Title: " + Leave.Title +
                "</li><li>Description: " + Leave.Detail +
                "</li><li>Start Date: " + FormatDate(Leave.StartDate) +
                "</li><li>End Date: " + FormatDate(Leave.EndDate) +
                "</li><li> Duration: " + Leave.Duration + " Days" +
                "</ul>";

            return emailFields;
        }

        public async Task<EmailFields> prepareCancelMail()
        {
            EmailFields emailFields = new EmailFields();
            emailFields.EmailTo = employeeDetails.ManagerEmail;
            emailFields.EmailSubject = "Leave Request Cancelled";
            emailFields.EmailBody = employeeDetails.EmployeeName + " cancelled his leave" +
                "<ul style='margin-bottom: 0px;'><li>Type: " + Leave.LeaveType + "<br/>" +
                "</li><li>Title: " + Leave.Title +
                "</li><li>Description: " + Leave.Detail +
                "</li><li>Start Date: " + FormatDate(Leave.StartDate) +
                "</li><li>End Date: " + FormatDate(Leave.EndDate) +
                "</li><li> Duration: " + Leave.Duration + " Days" +
                "</ul>";
            return emailFields; 
            //var result = await CommonService.SendEmail(common);
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
            if (!string.IsNullOrEmpty(e.Value.ToString()))
            {
                Leave.StartDate = Convert.ToDateTime(e.Value);
                Leave.EndDate = Convert.ToDateTime(e.Value);
                duration();
            }
        }

        protected async Task duration()
        {
            if (Leave.StartDate.Date <= Leave.EndDate.Date || Leave.StartDate.Date == Leave.EndDate.Date)
            {
                var leaveResp = await LeaveService.GetLeaveDays(ClientID, Convert.ToInt32(Leave.EmployeeID), Leave.StartDate, Leave.EndDate, Leave.IncludesHalfDay);
                if (leaveResp.MessageType == MessageType.Success)
                {
                    Leave.Duration = leaveResp.Data.Duration;
                    StateHasChanged();
                }
            }
            else
            {
                toastService.ShowError("End date must be greater than start date", "");
            }

        }

        public void endDateChange(ChangeEventArgs e)
        {
            if(!string.IsNullOrEmpty(e.Value.ToString()))
            Leave.EndDate = Convert.ToDateTime(e.Value);
            duration();
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
        protected async Task onLeaveTypeChange(ChangeEventArgs e)
        {
            if (Convert.ToInt32(e.Value) != 0 && e.Value != null && VacationTypeList != null)
            {
                var vacationType = Convert.ToInt32(e.Value);
                Leave.LeaveType = VacationTypeList.Find(x => x.ListValueID == vacationType).ValueDesc;
            }
        }



    }

   

}
