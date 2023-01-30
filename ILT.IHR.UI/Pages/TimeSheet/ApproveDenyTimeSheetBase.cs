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

namespace ILT.IHR.UI.Pages.TimeSheet
{
    public class ApproveDenyTimeSheetBase : ComponentBase
    {
        [Inject]
        public IConfiguration Configuration { get; set; }
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service    
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public ITimeSheetService TimeSheetService { get; set; } //Service   
        [Inject]
        public IEmailApprovalService EmailApprovalService { get; set; } //Service   
        public ITimeEntryService TimeEntryService { get; set; } //Service   
        [Inject]
        public IEmployeeService EmployeeService { get; set; } //Service  
        [Inject]
        public ILookupService LookupService { get; set; } //Service
        [Inject]
        public ICompanyService CompanyService { get; set; } //Service
        [Inject]
        public IUserService UserService { get; set; } //Service
        public List<ILT.IHR.DTO.User> UserList { get; set; } //Drop Down Api Data
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

        public bool disabledvalue;
        public string totalhours = "0";

        public ILT.IHR.DTO.User user;

        public bool entrydisabledvalue;
        public bool buttonsdisabledvalue;
        public bool isCommentExist { get; set; } = false;
        public List<ILT.IHR.DTO.TimeEntry> TimeEntryList { get; set; }  // Table APi Data
        protected ILT.IHR.DTO.TimeEntry selected;
        public ILT.IHR.DTO.TimeEntry timeentry = new ILT.IHR.DTO.TimeEntry();
        public ILT.IHR.DTO.EmailApproval emailapproval = new ILT.IHR.DTO.EmailApproval();

        public List<IRowActions> RowActions { get; set; } //Row Actions
        public List<ListValue> WeekEndingDayList { get; set; }

        public List<Assignment> EmployeeAssignments { get; set; }  // Table APi Data
        public Assignment EmployeeAssignment { get; set; }  // Table APi Data
        protected DTO.Employee employeeDetails { get; set; }
        protected string AccountsEmail { get; set; }
        public string ErrorMessage;
        public string ConfirmMessage;
        protected ILT.IHR.UI.Pages.DeleteConfirmation.ConfirmBase SubmitConfirmation { get; set; }


        protected override async Task OnInitializedAsync()
        {
            user = await sessionStorage.GetItemAsync<ILT.IHR.DTO.User>("User");
            AccountsEmail = Configuration["EmailNotifications:" + this.user.ClientID.ToUpper() + ":Timesheet"];
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

            List<ListValue> lstValues = new List<ListValue>();
            Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());
            if (resp.MessageType == MessageType.Success)
            {
                TimeSheetStatusList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.TIMESHEETSTATUS).ToList();
                WeekEndingDayList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.TIMESHEETTYPE).ToList();
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

        private async Task GetClientDetails(int clientid)
        {
            timesheet.ClientID = clientid;
            timesheet.ApprovedDate = DateTime.Now;
            timesheet.ClosedDate = null;
            timesheet.EmployeeID = EmployeeAssignment.EmployeeID;
            timesheet.TimeSheetTypeID = EmployeeAssignment.TimeSheetTypeID != null ? (int)EmployeeAssignment.TimeSheetTypeID : 0; ;
            timesheet.TimeSheetType = EmployeeAssignment.TimeSheetType;
            //timesheet.TimesheetApproverID = EmployeeAssignment.TimesheetApproverID != null ? (int)EmployeeAssignment.TimesheetApproverID : 0; ;
            timesheet.ApprovedByEmail = EmployeeAssignment.TSApproverEmail;
            //timesheet.TSApproverName = EmployeeAssignment.TimesheetApprover;
            timesheet.TSApproverEmail = EmployeeAssignment.TSApproverEmail;
            timesheet.AssignmentID = EmployeeAssignment.AssignmentID;
        }


        protected async Task ValidTimeSheet()
        {
            if (isCommentExist != true)
            {
                SubmitConfirmation.Show();
            }
        }
        protected async Task SaveTimeSheet()
        {
            if (buttonsdisabledvalue)
                return;
            buttonsdisabledvalue = true;
            ErrorMessage = "";
            timesheet.ApprovedByEmail = user.Email;
            timesheet.TimeEntries = TimeEntryList;
            timesheet.ClientName = EmployeeAssignments.Find(cl => cl.ClientID == timesheet.ClientID).Client;

            string ClientID = await sessionStorage.GetItemAsync<string>("ClientID");
            Guid LinkID = timesheet.LinkID;
            string value = String.Empty;

            if (timesheet.StatusID == TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.APPROVED.ToUpper()).ListValueID || timesheet.StatusID == TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.REJECTED.ToUpper()).ListValueID)
            {
                if (timesheet.Status.ToUpper() == "APPROVED")
                    value = "APPROVE";
                else
                    value = "REJECT";
                string message = await EmailApprovalService.EamilApprovalAction(ClientID, LinkID, value);
                if (message == MessageType.Success.ToString())
                    toastService.ShowSuccess("TimeSheet saved successfully", "");
                else
                    toastService.ShowError(ErrorMsg.ERRORMSG);
            }

            TimeSheetUpdated.InvokeAsync(true);
            Cancel();
            buttonsdisabledvalue = false;
        }

        protected async Task RejectTimeSheet()
        {

            ConfirmMessage = "Are you sure you want to reject?";
            if (timesheet != null && timesheet.Comment != null && timesheet.Comment != "")
            {
                timesheet.StatusID = TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.REJECTED.ToUpper()).ListValueID;
                timesheet.Status = TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.REJECTED.ToUpper()).ValueDesc;
                isCommentExist = false;
            }
            else
            {
                isCommentExist = true;
                ConfirmSubmit_Click(false);
            }

        }

        protected async Task ApproveTimeSheet()
        {
            isCommentExist = false;
            ConfirmMessage = "Are you sure you want to approve?";
            timesheet.StatusID = TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.APPROVED.ToUpper()).ListValueID;
            timesheet.Status = TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.APPROVED.ToUpper()).ValueDesc;
        }

        public async Task<EmailFields> PrepareEmail()
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

            EmailFields emailFields = new EmailFields();
            //Common common = new Common();
            emailFields.isMultipleEmail = true;
            emailFields.EmailCCList = new List<string>();
            var approverEmail = timesheet.TSApproverEmail;
            var managerName = employeeDetails.Manager;
            emailFields.EmailTo = employeeDetails.Email;
            emailFields.EmailCCList.Add(AccountsEmail);
            emailFields.EmailCCList.Add(approverEmail);
            if (!string.IsNullOrEmpty(timesheet.ApprovedEmailTo))
            {
                List<string> ApprovedEmailTo = timesheet.ApprovedEmailTo.Split(';').ToList();
                emailFields.EmailCCList.AddRange(ApprovedEmailTo);
            }
            var tempstatus = "";
            if (timesheet.StatusID == TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.APPROVED.ToUpper()).ListValueID)
            {
                tempstatus = TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.APPROVED.ToUpper()).ValueDesc;
                emailFields.EmailSubject = "Timesheet approved for " + employeeDetails.EmployeeName + " for week ending " + FormatDate(timesheet.WeekEnding);
            }
            else if (timesheet.StatusID == TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.REJECTED.ToUpper()).ListValueID)
            {
                tempstatus = TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.REJECTED.ToUpper()).ValueDesc;
                emailFields.EmailSubject = "Timesheet rejected for " + employeeDetails.EmployeeName + " for week ending " + FormatDate(timesheet.WeekEnding);
            }
            var emailBody = "<table><tr><td><b>Employee</b></td>" + "<td>: " + employeeDetails.EmployeeName + "</td></tr>" +
                                "<tr><td><b>Week Ending</b></td>" + "<td>: " + FormatDate(timesheet.WeekEnding) + "</td></tr>" +
                                "<tr><td><b>Client</b></td>" + "<td>: " + timesheet.ClientName + "</td></tr>";
            if (timesheet.ClientManager != null && timesheet.ClientManager != "")
            {
                emailBody = emailBody + "<tr><td><b>Client Manager</b></td>" + "<td>: " + timesheet.ClientManager + "</td></tr>";
            }
            if (managerName != null && managerName != "")
            {
                emailBody = emailBody + "<tr><td><b>Manager</b></td>" + "<td>: " + managerName + "</td></tr>";
            }
            emailBody = emailBody + "<tr><td><b>Status</b></td>" + "<td>: " + tempstatus + "</td></tr>";
            if (timesheet != null && timesheet.Comment != "" && timesheet.Comment != null)
            {
                emailBody = emailBody + "<tr><td><b>Comment</b></td>" + "<td>: " + timesheet.Comment + "</td></tr>";
            }
            emailBody = emailBody + "<tr><td><b>Total Hours</b></td>" + "<td>: " + totalhours + "</td></tr></table><br/><br/>" +
            "<table style='border: 1px solid black; border-collapse:collapse;' width='80%'>" + tableInfo + "</table><br/>" +
            "<table><tr><td>Timesheet " + timesheet.Status + " by " + timesheet.TSApproverEmail + " on " + DateTime.Now.ToString("dd MMM yyy HH:mm:ss") + " GMT</td></tr></table>";
            emailFields.EmailBody = emailBody;
            return emailFields;
            // Cancel();
            //  var result = await CommonService.SendEmail(common);
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
            common.EmailCCList = new List<string>();
            var approverEmail = timesheet.TSApproverEmail;
            var managerName = employeeDetails.Manager;
            common.EmailTo = employeeDetails.Email;
            common.EmailCCList.Add(AccountsEmail);
            common.EmailCCList.Add(approverEmail);
            if (!string.IsNullOrEmpty(timesheet.ApprovedEmailTo))
            {
                List<string> ApprovedEmailTo = timesheet.ApprovedEmailTo.Split(';').ToList();
                common.EmailCCList.AddRange(ApprovedEmailTo);
            }
            var tempstatus = "";
            if (timesheet.StatusID == TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.APPROVED.ToUpper()).ListValueID)
            {
                tempstatus = TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.APPROVED.ToUpper()).ValueDesc;
                common.EmailSubject = "Timesheet approved for " + employeeDetails.EmployeeName + " for week ending " + FormatDate(timesheet.WeekEnding);
            }
            else if (timesheet.StatusID == TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.REJECTED.ToUpper()).ListValueID)
            {
                tempstatus = TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.REJECTED.ToUpper()).ValueDesc;
                common.EmailSubject = "Timesheet rejected for " + employeeDetails.EmployeeName + " for week ending " + FormatDate(timesheet.WeekEnding);
            }
            var emailBody = "<table><tr><td><b>Employee</b></td>" + "<td>: " + employeeDetails.EmployeeName + "</td></tr>" +
                                "<tr><td><b>Week Ending</b></td>" + "<td>: " + FormatDate(timesheet.WeekEnding) + "</td></tr>" +
                                "<tr><td><b>Client</b></td>" + "<td>: " + timesheet.ClientName + "</td></tr>";
            if (timesheet.ClientManager != null && timesheet.ClientManager != "")
            {
                emailBody = emailBody + "<tr><td><b>Client Manager</b></td>" + "<td>: " + timesheet.ClientManager + "</td></tr>";
            }
            if (managerName != null && managerName != "")
            {
                emailBody = emailBody + "<tr><td><b>Manager</b></td>" + "<td>: " + managerName + "</td></tr>";
            }
            emailBody = emailBody + "<tr><td><b>Status</b></td>" + "<td>: " + tempstatus + "</td></tr>";
            if (timesheet != null && timesheet.Comment != "" && timesheet.Comment != null)
            {
                emailBody = emailBody + "<tr><td><b>Comment</b></td>" + "<td>: " + timesheet.Comment + "</td></tr>";
            }
            emailBody = emailBody + "<tr><td><b>Total Hours</b></td>" + "<td>: " + totalhours + "</td></tr></table><br/><br/>" +
            "<table style='border: 1px solid black; border-collapse:collapse;' width='80%'>" + tableInfo + "</table><br/>" +
            "<table><tr><td>Timesheet " + timesheet.Status + " by " + timesheet.TSApproverEmail + " on " + DateTime.Now.ToString("dd MMM yyy HH:mm:ss") + " GMT</td></tr></table>";
            common.EmailBody = emailBody;
            Cancel();
            var result = await CommonService.SendEmail(common);
        }

        public void RowClick(ILT.IHR.DTO.TimeEntry data)
        {
            selected = data;
            StateHasChanged();
        }

        public string GetBlobContentPath()
        {
            string baseUri = Configuration["ApiUrl"];
            return baseUri + "Timesheet/DownloadFile?Client=" + user.ClientID + "&Doc=" + timesheet.DocGuid;
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
            timesheet.ApprovedDate = DateTime.Now;
            timesheet.ClosedDate = null;
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
            if (timesheet.StatusValue.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.SUBMITTED.ToUpper())
            {
                buttonsdisabledvalue = false;
            }
            else
            {
                buttonsdisabledvalue = true;
            }
            Title = "Edit";
            ShowDialog = true;
            StateHasChanged();
        }

        public void Cancel()
        {
            TimeEntryList.Clear();
            isCommentExist = false;
            ShowDialog = false;
            StateHasChanged();

        }
        public void Show(int Id)
        {
            ErrorMessage = "";
            TimeSheetId = Id;
            isCommentExist = false;
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

        protected async Task ConfirmSubmit_Click(bool submitConfirmed)
        {
            if (submitConfirmed)
            {
                await SaveTimeSheet();
            }
        }
    }
}
