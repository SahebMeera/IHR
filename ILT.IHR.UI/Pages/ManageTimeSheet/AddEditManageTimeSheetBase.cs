using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorTable;
using ILT.IHR.DTO;
using ILT.IHR.UI.Service;
using Blazored.Toast.Services;
using Blazored.SessionStorage;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using BlazorDownloadFile;
using System.IO;


namespace ILT.IHR.UI.Pages.ManageTimeSheet
{
    public class AddEditManageTimeSheetBase : ComponentBase
    {
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service
        [Inject]
        public IConfiguration Configuration { get; set; } //configuration  
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public ITimeSheetService TimeSheetService { get; set; } //Service   
        [Inject]
        public ITimeEntryService TimeEntryService { get; set; } //Service   
        [Inject]
        public IEmployeeService EmployeeService { get; set; } //Service  
        [Inject]
        public ILookupService LookupService { get; set; } //Service
        [Inject]
        public ICompanyService CompanyService { get; set; } //Service
        [Inject]
        public IUserService UserService { get; set; } //Service
        [Inject]
        public IRoleService RoleService { get; set; } //Service
        [Inject]
        public IEmailApprovalService EmailApprovalService { get; set; } //Service  
        public ILT.IHR.DTO.EmailApproval emailapproval = new ILT.IHR.DTO.EmailApproval();
        public List<Module> Modules { get; set; } //Drop Down Api Data
        public IEnumerable<ILT.IHR.DTO.User> UserList { get; set; } //Drop Down Api Data
        // public IEnumerable<ILT.IHR.DTO.TimeSheet> TimeSheets { get; set; } //Drop Down Api Data
        public IEnumerable<ILT.IHR.DTO.Employee> Employees { get; set; } //Drop Down Api Data    
        // public IEnumerable<ILT.IHR.DTO.Company> CompanyList { get; set; }  // Table APi Data
        // public IEnumerable<ILT.IHR.DTO.Company> ClientList { get; set; }  // Table APi Data
        public List<ListValue> TimeSheetStatusList { get; set; }
        private int TimeSheetId { get; set; }
        public int EmployeeId { get; set; }
        [Parameter]
        public EventCallback<bool> TimeSheetUpdated { get; set; }
        [Inject]
        public NavigationManager UrlNavigationManager { get; set; }
        [Inject]
        public ICommonService CommonService { get; set; } //Service

        protected string Title = "Add";
        public ILT.IHR.DTO.TimeSheet timesheet = new ILT.IHR.DTO.TimeSheet();
        public ILT.IHR.DTO.Employee employee = new ILT.IHR.DTO.Employee();
        public bool ShowDialog { get; set; }
        public bool isMonthly { get; set; }

        public bool disabledvalue;
        public string totalhours = "0";

        public ILT.IHR.DTO.User user;

        public bool entrydisabledvalue;
        public bool buttonsdisabledvalue;
        public bool resubmitbuttonsdisabledvalue;
        public List<ILT.IHR.DTO.TimeEntry> TimeEntryList { get; set; }  // Table APi Data
        protected ILT.IHR.DTO.TimeEntry selected;
        public ILT.IHR.DTO.TimeEntry timeentry = new ILT.IHR.DTO.TimeEntry();
        public List<IRowActions> RowActions { get; set; } //Row Actions
        public List<ListValue> WeekEndingDayList { get; set; }

        public List<Assignment> EmployeeAssignments { get; set; }  // Table APi Data
        public Assignment EmployeeAssignment { get; set; }  // Table APi Data
        protected DTO.Employee employeeDetails { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        public string ErrorMessage;
        protected ILT.IHR.UI.Pages.DeleteConfirmation.ConfirmBase SubmitConfirmation { get; set; }
        [Inject] IBlazorDownloadFileService BlazorDownloadFileService { get; set; }

        public bool isSaveButtonDisabled { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            user = await sessionStorage.GetItemAsync<ILT.IHR.DTO.User>("User");
            await LoadDropDown();
        }

        private async Task LoadDropDown()
        {
            var respUsers = (await UserService.GetUsers());
            if (respUsers.MessageType == MessageType.Success)
                UserList = respUsers.Data.ToList();
            else
                toastService.ShowError(ErrorMsg.ERRORMSG);

            var respEmployees = (await EmployeeService.GetEmployees());
            if (respEmployees.MessageType == MessageType.Success)
                Employees = respEmployees.Data;
            else
                toastService.ShowError(ErrorMsg.ERRORMSG);

            Modules = (await RoleService.GetModules()).ToList();
            List<ListValue> lstValues = new List<ListValue>();
            Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());
            if (resp.MessageType == MessageType.Success)
            {
                TimeSheetStatusList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.TIMESHEETSTATUS).ToList();
                WeekEndingDayList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.TIMESHEETTYPE).ToList();
            }

            if (user.EmployeeID != null)
            {
                await GetEmployeeDetails((int)user.EmployeeID);
            }
        }

        private async Task GetEmployeeDetails(int employeeid)
        {
            Response<ILT.IHR.DTO.Employee> respEmployee = new Response<ILT.IHR.DTO.Employee>();
            respEmployee = await EmployeeService.GetEmployeeByIdAsync(employeeid);
            if (respEmployee.MessageType == MessageType.Success)
            {
                employeeDetails = respEmployee.Data;
                EmployeeAssignments = respEmployee.Data.Assignments;
            }
            EmployeeAssignment = EmployeeAssignments.Find(ea => ea.ClientID == timesheet.ClientID);
        }

        public async Task GetBlobContentPath()
        {
            var fileDownLoadresp = await TimeSheetService.DownloadFile(user.ClientID, timesheet.DocGuid);
            if (String.IsNullOrEmpty(fileDownLoadresp.Data.ErrorMessage))
            {
                var task = await BlazorDownloadFileService.DownloadFile(fileDownLoadresp.Data.FileName, fileDownLoadresp.Data.memoryStream, "application/octet-stream");
                if (task.Succeeded)
                {
                    toastService.ShowSuccess("Successful download!");
                }
                else
                {
                    toastService.ShowError(task.ErrorMessage);
                }
            }
            else
            {
                toastService.ShowError(fileDownLoadresp.Data.ErrorMessage);
            }
            //string baseUri = Configuration["ApiUrl"];
            //return baseUri + "Timesheet/DownloadFile?Client=" + user.ClientID + "&Doc=" + timesheet.DocGuid;
        }

        private async Task GetClientDetails(int clientid)
        {
            timesheet.ClientID = clientid;
            //timesheet.ClosedDate = DateTime.Now;
            timesheet.EmployeeID = EmployeeAssignment.EmployeeID;
            timesheet.TimeSheetTypeID = (int)EmployeeAssignment.TimeSheetTypeID;
            timesheet.TimeSheetType = EmployeeAssignment.TimeSheetType;
            //timesheet.TSApproverName = EmployeeAssignment.TimesheetApprover;
            timesheet.AssignmentID = EmployeeAssignment.AssignmentID;
        }
        protected async Task ValidTimeSheet()
        {
            if (timesheet.Status.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.SUBMITTED.ToUpper())
            {
                SubmitConfirmation.Show();
            }
            else if (timesheet.Status.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.CLOSED.ToUpper())
            {
                SubmitConfirmation.Show();
            }
            else
            {
                await SaveTimeSheet();
            }
        }

        protected async Task SaveTimeSheet()
        {
            ErrorMessage = string.Empty;

            if (timesheet.StatusID == TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.CLOSED.ToUpper()).ListValueID)
            {
                timesheet.ClosedBy = user.FirstName + " " + user.LastName;
                timesheet.ClosedByID = user.UserID;
                timesheet.ClosedDate = DateTime.Now;
            }
            timesheet.TimeEntries = TimeEntryList;
            timesheet.ClientName = EmployeeAssignments.Find(cl => cl.ClientID == timesheet.ClientID).Client;
            if (isSaveButtonDisabled)
                return;
            isSaveButtonDisabled = true;
            if (TimeSheetId == 0)
            {
                var result = await TimeSheetService.SaveTimeSheet(timesheet);
                if (result.MessageType == MessageType.Success)
                {
                    toastService.ShowSuccess("TimeSheet saved successfully", "");
                    TimeSheetUpdated.InvokeAsync(true);
                    Cancel();
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
            }
            else if (TimeSheetId > 0)
            {
                timesheet.ModifiedBy = user.FirstName + " " + user.LastName;
                var result = await TimeSheetService.UpdateTimeSheet(TimeSheetId, timesheet);
                if (result.MessageType == MessageType.Success)
                {
                    if (!resubmitbuttonsdisabledvalue)
                    {
                        //if (timesheet.StatusID == TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.CLOSED.ToUpper()).ListValueID)
                        //{
                        timesheet = result.Data;
                        Response<ILT.IHR.DTO.EmailApproval> respEmailApproval = new Response<ILT.IHR.DTO.EmailApproval>();
                        respEmailApproval = await EmailApprovalService.GetEmailApprovalByIdAsync(timesheet.LinkID) as Response<ILT.IHR.DTO.EmailApproval>;
                        emailapproval = respEmailApproval.Data;
                        emailapproval.ValidTime = DateTime.Now.AddDays(Configuration["EmailApprovalValidity"] == null ? 1 : Convert.ToInt32(Configuration["EmailApprovalValidity"]));
                        emailapproval.ModifiedBy = user.FirstName + " " + user.LastName;
                        emailapproval.SentCount = emailapproval.SentCount > 0 ? emailapproval.SentCount - 1 : 0;
                        var resultEmailApproval = await EmailApprovalService.SaveEmailApproval(emailapproval);
                        if (resultEmailApproval.Message == MessageType.Error.ToString())
                        {
                            ErrorMessage = ErrorMsg.ERRORMSG;
                        }
                    }
                    if (timesheet.StatusID == TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.VOID.ToUpper()).ListValueID)
                    {
                        string ClientID = await sessionStorage.GetItemAsync<string>("ClientID");
                        Guid LinkID = timesheet.LinkID;
                        string value = "VOID";
                        string message = await EmailApprovalService.EamilApprovalAction(ClientID, LinkID, value);
                        if (message == MessageType.Error.ToString())
                        {
                            ErrorMessage = ErrorMsg.ERRORMSG;
                        }
                    }

                    if (!String.IsNullOrEmpty(ErrorMessage))
                    {
                        toastService.ShowSuccess(ErrorMessage);
                    }
                    else
                    { 
                        toastService.ShowSuccess("TimeSheet saved successfully", "");
                    }
                    TimeSheetUpdated.InvokeAsync(true);
                    Cancel();
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
            }
            isSaveButtonDisabled = false;
        }

        public async Task sendMail()
        {
            var tableInfo = "";
            tableInfo = tableInfo + "<tr><th style='border: 1px solid black;'>Work Date</th><th style='border: 1px solid black;'>Project</th>" +
                "<th style='border: 1px solid black;'>Activity</th><th style='border: 1px solid black;'>Hours</th>";
            timesheet.TimeEntries.ForEach(te =>
            {
                tableInfo = tableInfo + "<tr><td style='border: 1px solid black;' width='15%'>&nbsp;" + te.WorkDate.ToString("MM/dd/yyyy") + "</td>" +
                                            "<td style='border: 1px solid black;' width='35%'>&nbsp;" + te.Project + "</td>" +
                                            "<td style='border: 1px solid black;' width='40%'>&nbsp;" + te.Activity + "</td>" +
                                            "<td style='border: 1px solid black;' width='10%' align='right'>" + te.Hours + "&nbsp;</td>";
            });

            Common common = new Common();
            common.isMultipleEmail = true;
            var managerName = "";
            var clientManager = "";
            if (EmployeeAssignment.TSApproverEmail != null)
            {
                //var approverEmail = UserList.Find(ul => ul.UserID == EmployeeAssignment.TimesheetApproverID).Email;
                var approverEmail = EmployeeAssignment.TSApproverEmail;
                clientManager = EmployeeAssignment.ClientManager;
                common.EmailTo = approverEmail;
                managerName = employeeDetails.Manager;
            }
            // common.EmailCC= employeeDetails.Email;
            if (isMonthly == true)
            {
                common.EmailSubject = employeeDetails.EmployeeName + " has submitted timesheet for month ending " + FormatDate(timesheet.WeekEnding);
            }
            else
            {
                common.EmailSubject = employeeDetails.EmployeeName + " has submitted timesheet for week ending " + FormatDate(timesheet.WeekEnding);
            }
            string uri = Configuration["EmailApprovalUrl:" + user.ClientID];
            var emailBody = "<table><tr><td><b>Employee</b></td>" + "<td>: " + employeeDetails.EmployeeName + "</td></tr>";
            if (timesheet.TimeSheetType.ToUpper() == TimeSheetType.MONTHLY)
            {
                emailBody = emailBody + "<tr><td><b>Month Ending</b></td>" + "<td>: " + FormatDate(timesheet.WeekEnding) + "</td></tr>";
            }
            else
            {
                emailBody = emailBody + "<tr><td><b>Week Ending</b></td>" + "<td>: " + FormatDate(timesheet.WeekEnding) + "</td></tr>";
            }
            emailBody = emailBody + "<tr><td><b>Client</b></td>" + "<td>: " + timesheet.ClientName + "</td></tr>";
            if (clientManager != null && clientManager != "")
            {
                emailBody = emailBody + "<tr><td><b>Client Manager</b></td>" + "<td>: " + clientManager + "</td></tr>";
            }
            if (managerName != null && managerName != "")
            {
                emailBody = emailBody + "<tr><td><b>Manager</b></td>" + "<td>: " + managerName + "</td></tr>";
            }
            emailBody = emailBody + "<tr><td><b>Status</b></td>" + "<td>: Submitted</td></tr>" +
            "<tr><td><b>Total Hours</b></td>" + "<td>: " + totalhours + "</td></tr></table><br/>" +
            "<table style='border: 1px solid black; border-collapse:collapse;' width='80%'>" + tableInfo + "</table><br/>" +
            "<table width='80%'><tr align='center'><td align='right'><a href='" + uri + "emailapproval?ClientID=" + user.ClientID + "&LinkID=" + emailapproval.LinkID + "&Value=APPROVE'><img src='" + Configuration["ImageURL:" + this.user.ClientID.ToUpper() + ":Approve"] + "' /></a></td><td align='left'><a href='" + uri + "emailapproval?ClientID=" + user.ClientID.ToUpper() + "&LinkID=" + emailapproval.LinkID + "&Value=REJECT'><img src='" + Configuration["ImageURL:" + this.user.ClientID.ToUpper() + ":Reject"] + "' /></a></td></tr></table>";
            common.EmailBody = emailBody;
            Cancel();
            var result = await CommonService.SendEmail(common);
        }

        protected async Task CloseTimeSheet()
        {
            ConfirmationMessage = "Timesheet should be closed after the invoice has been paid.\n Are you sure you want to close the TimeSheet ?";
            timesheet.StatusID = TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.CLOSED.ToUpper()).ListValueID;
            timesheet.Status = TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.CLOSED.ToUpper()).ValueDesc;
        }


        protected async Task VoidTimeSheet()
        {
            ConfirmationMessage = "Are you sure you want to void ?";
            timesheet.StatusID = TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.VOID.ToUpper()).ListValueID;
            timesheet.Status = TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.VOID.ToUpper()).ValueDesc;
        }


        protected async Task ReSubmitTimeSheet()
        {
            ConfirmationMessage = "Are you sure you want to Resubmit?";
            timesheet.StatusID = TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.SUBMITTED.ToUpper()).ListValueID;
            timesheet.Status = TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.SUBMITTED.ToUpper()).ValueDesc;
        }

        public void RowClick(ILT.IHR.DTO.TimeEntry data)
        {
            selected = data;
            StateHasChanged();
        }

        private async Task GetDetails(int Id)
        {
            Response<ILT.IHR.DTO.TimeSheet> resp = new Response<ILT.IHR.DTO.TimeSheet>();
            resp = await TimeSheetService.GetTimeSheetByIdAsync(Id) as Response<ILT.IHR.DTO.TimeSheet>;
            if (resp.MessageType == MessageType.Success)
            {
                timesheet = resp.Data;
            }

            TimeEntryList = timesheet.TimeEntries;
            await GetEmployeeDetails((int)timesheet.EmployeeID);
            totalhours = timesheet.TotalHours.ToString();
            //timesheet.ClosedDate = DateTime.Now;
            /*var tempTotalHours = 0;
           TimeEntryList.ForEach(tl =>
           {
               tempTotalHours = tempTotalHours + tl.Hours;
           });
           totalhours = tempTotalHours.ToString();*/
            //if (timesheet.StatusValue.ToUpper() != ListTypeConstants.TimeSheetStatusConstants.PENDING.ToUpper())
            //{
            entrydisabledvalue = true;
            //}
            //else
            //{
            //    entrydisabledvalue = false;
            //}
            if (timesheet.StatusValue.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.APPROVED.ToUpper())
            {
                buttonsdisabledvalue = false;
            }
            else
            {
                buttonsdisabledvalue = true;
            }
            if (timesheet.StatusValue.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.SUBMITTED.ToUpper())
            {
                resubmitbuttonsdisabledvalue = false;
            }
            else
            {
                resubmitbuttonsdisabledvalue = true;
            }
            if (timesheet.TimeSheetType.ToUpper() == TimeSheetType.MONTHLY)
            {
                isMonthly = true;
            }
            else
            {
                isMonthly = false;
            }
            Title = "Edit";
            ShowDialog = true;
            StateHasChanged();
        }

        public void Cancel()
        {
            TimeEntryList.Clear();
            ShowDialog = false;
            StateHasChanged();

        }
        public void Show(int Id)
        {
            ErrorMessage = "";
            TimeSheetId = Id;
            ResetDialog();
            if (TimeSheetId != 0)
            {
                totalhours = "0";
                disabledvalue = true;
                entrydisabledvalue = true;
                GetDetails(TimeSheetId);
            }
            else
            {
                TimeEntryList.Clear();
                totalhours = "0";
                disabledvalue = true;
                entrydisabledvalue = false;
                isMonthly = false;
                ShowDialog = true;
                StateHasChanged();
            }
        }

        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }
        private void ResetDialog()
        {
            timesheet = new DTO.TimeSheet { };
        }
        protected string FormatDate(DateTime? dateTime)
        {
            string formattedDate = "";
            if (dateTime.Value != null)
            {
                var date = dateTime.Value.ToString("MM/dd/yyyy");
                formattedDate = date;
            }

            return formattedDate;
        }
        public string ConfirmationMessage { get; set; } = "Are you sure you want to Resubmit?";

        protected async Task ConfirmSubmit_Click(bool submitConfirmed)
        {
            if (submitConfirmed)
            {
                await SaveTimeSheet();
            }
        }
    }
}
