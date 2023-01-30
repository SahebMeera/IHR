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
using Azure.Storage.Blobs;
using System.IO;
using Azure.Storage.Blobs.Models;
using System.Globalization;
using BlazorInputFile;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using BlazorDownloadFile;

namespace ILT.IHR.UI.Pages.TimeSheet
{
    public class AddEditTimeSheetBase : ComponentBase
    {
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service    
        [Inject]
        public IConfiguration Configuration { get; set; } //Service         
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public NavigationManager NavManager { get; set; }
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
        public IEmailApprovalService EmailApprovalService { get; set; } //Service   
        [Inject]
        public IRoleService RoleService { get; set; } //Service
        public List<ILT.IHR.DTO.User> UserList { get; set; } //Drop Down Api Data
        // public IEnumerable<ILT.IHR.DTO.TimeSheet> TimeSheets { get; set; } //Drop Down Api Data
        public IEnumerable<ILT.IHR.DTO.Employee> Employees { get; set; } //Drop Down Api Data    
        // public IEnumerable<ILT.IHR.DTO.Company> CompanyList { get; set; }  // Table APi Data
        // public List<ILT.IHR.DTO.Company> ClientList { get; set; }  // Table APi Data
        public List<Module> Modules { get; set; } //Drop Down Api Data
        public List<ListValue> TimeSheetStatusList { get; set; }
        private int TimeSheetId { get; set; }
        public int EmployeeId { get; set; }
        [Parameter]
        public EventCallback<bool> TimeSheetUpdated { get; set; }
        [Inject]
        public NavigationManager UrlNavigationManager { get; set; }
        [Inject]
        public ICommonService CommonService { get; set; } //Service
        [Inject] 
        IBlazorDownloadFileService BlazorDownloadFileService { get; set; }

        protected string Title = "Add";
        public ILT.IHR.DTO.TimeSheet timesheet = new ILT.IHR.DTO.TimeSheet();
        public ILT.IHR.DTO.EmailApproval emailapproval = new ILT.IHR.DTO.EmailApproval();
        public ILT.IHR.DTO.Employee employee = new ILT.IHR.DTO.Employee();
        public bool ShowDialog { get; set; }

        public bool disabledvalue;
        public string totalhours = "0";

        public ILT.IHR.DTO.User user;

        public bool entrydisabledvalue;
        public bool buttonsdisabledvalue;
        public bool isMonthly { get; set; }
        public List<ILT.IHR.DTO.TimeEntry> TimeEntryList { get; set; }  // Table APi Data
        protected ILT.IHR.DTO.TimeEntry selected;
        public ILT.IHR.DTO.TimeEntry timeentry = new ILT.IHR.DTO.TimeEntry();
        public List<IRowActions> RowActions { get; set; } //Row Actions
        public List<ListValue> WeekEndingDayList { get; set; }

        public List<Assignment> EmployeeAssignments { get; set; }  // Table APi Data
        public Assignment EmployeeAssignment { get; set; }  // Table APi Data
        protected DTO.Employee employeeDetails { get; set; }
        public string ErrorMessage;
        public string ConfirmMessage;
        public List<ILT.IHR.DTO.TimeSheet> TimeSheetsList { get; set; }  // Table APi Data
        protected ILT.IHR.UI.Pages.DeleteConfirmation.ConfirmBase SubmitConfirmation { get; set; }

        protected IFileListEntry file;


        protected override async Task OnInitializedAsync()
        {
            // file = null;
            user = await sessionStorage.GetItemAsync<ILT.IHR.DTO.User>("User");
            await LoadDropDown();
        }

        private async Task LoadDropDown()
        {
            await LoadTableConfig();

            var respUsers = (await UserService.GetUsers());
            if (respUsers.MessageType == MessageType.Success)
                UserList = respUsers.Data.ToList();
            else
                toastService.ShowError(ErrorMsg.ERRORMSG);

            var respEmployees = (await EmployeeService.GetEmployees());
            if (respEmployees.MessageType == MessageType.Success)
            {
                if (user.EmployeeID != null)
                {
                    Employees = respEmployees.Data.Where(x => x.ManagerID == user.EmployeeID || x.EmployeeID == user.EmployeeID);
                }
                else
                {
                    Employees = respEmployees.Data;
                }
            }
            else
            {
                toastService.ShowError(ErrorMsg.ERRORMSG);
            }
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

        protected async Task changeemployee(ChangeEventArgs e)
        {
            var employeeid = Convert.ToInt32(e.Value);
            await GetEmployeeDetails(employeeid);
            await GetClientDetails(0);
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

        protected async Task changeclient(ChangeEventArgs e)
        {
            var clientid = Convert.ToInt32(e.Value);
            await GetEmployeeDetails((int)timesheet.EmployeeID);
            EmployeeAssignment = EmployeeAssignments.Find(ea => ea.ClientID == clientid);
            await GetClientDetails(clientid);
        }

        private async Task GetClientDetails(int clientid)
        {
            ErrorMessage = "";
            TimeEntryList.Clear();
            timesheet.WeekEnding = null;
            isMonthly = false;
            if (clientid == 0)
            {
                timesheet.ClientID = 0;
                timesheet.StatusID = 0;
                timesheet.Status = "";
                timesheet.TimeSheetTypeID = 0;
                timesheet.TimeSheetType = "";
                //timesheet.TSApproverName = "";
                timesheet.TSApproverEmail = "";
                timesheet.AssignmentID = 0;
                totalhours = "0";
            }
            else
            {
                timesheet.ClientID = clientid;
                timesheet.StatusID = TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.PENDING.ToUpper()).ListValueID;
                timesheet.Status = TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.PENDING.ToUpper()).ValueDesc;
                timesheet.SubmittedDate = DateTime.Now;
                timesheet.ApprovedDate = null;
                timesheet.ClosedDate = null;
                if (EmployeeAssignment != null)
                {
                    timesheet.EmployeeID = EmployeeAssignment.EmployeeID;
                    timesheet.TimeSheetTypeID = EmployeeAssignment.TimeSheetTypeID != null ? (int)EmployeeAssignment.TimeSheetTypeID : 0;
                    timesheet.TimeSheetType = EmployeeAssignment.TimeSheetType;
                    if (timesheet.TimeSheetType.ToUpper() == TimeSheetType.MONTHLY)
                    {
                        isMonthly = true;
                    }
                    else
                    {
                        isMonthly = false;
                    }
                    //timesheet.TimesheetApproverID = EmployeeAssignment.TimesheetApproverID != null ? (int)EmployeeAssignment.TimesheetApproverID : 0;
                    //timesheet.TSApproverName = EmployeeAssignment.TimesheetApprover;
                    timesheet.TSApproverEmail = EmployeeAssignment.TSApproverEmail;
                    timesheet.AssignmentID = EmployeeAssignment.AssignmentID;
                }
            }

        }

        private async Task LoadTableConfig()
        {
            IRowActions m1 = new IRowActions
            {
                IconClass = "oi oi-plus",
                ActionMethod = AddRow,
                ButtonClass = "btn-primary"
            };

            IRowActions m2 = new IRowActions
            {
                IconClass = "oi oi-minus",
                ActionMethod = DeleteRow,
                ButtonClass = "btn-danger"
            };


            RowActions = new List<IRowActions> { m1, m2 };

            TimeEntryList = new List<ILT.IHR.DTO.TimeEntry> { };
        }

        public void AddRow()
        {
            ErrorMessage = "";
            var date = selected.WorkDate;
            int index = TimeEntryList.IndexOf(selected);
            if (selected != null && selected.Project.ToUpper() != "CLIENT HOLIDAY" && selected.Project.ToUpper() != "PERSONAL LEAVE" && selected.Project.ToUpper() != "NON-BILLABLE")
            {
                DTO.TimeEntry temptimeentry = new DTO.TimeEntry { };
                temptimeentry.WorkDate = date;
                TimeEntryList.Insert(index + 1, temptimeentry);
            }
        }

        public void DeleteRow()
        {
            ErrorMessage = "";
            var date = selected.WorkDate;
            int index = TimeEntryList.IndexOf(selected);
            if (TimeEntryList.FindAll(x => x.WorkDate == date).Count > 1)
            {
                var temphours = Convert.ToInt32(totalhours) - selected.Hours;
                totalhours = temphours.ToString();
                TimeEntryList.RemoveAt(index);
            }
            else
            {
                ErrorMessage = "Cannot remove default row";
            }

        }

        protected async Task ValidTimeSheet()
        {
            ErrorMessage = "";
            var isTableValues = true;
            if (TimeSheetId != -1)
            {
                if (TimeEntryList.Count > 0)
                {
                    TimeEntryList.ForEach(tel =>
                    {
                        if (isTableValues)
                        {
                            var workdate = Convert.ToDateTime(tel.WorkDate);
                            DateTime currentworkdate = workdate;
                            if (currentworkdate.ToString("dddd").ToUpper() != "SATURDAY" && currentworkdate.ToString("dddd").ToUpper() != "SUNDAY")
                            {
                                if (tel.Project == "" || tel.Project == null)
                                {
                                    ErrorMessage = "Please populate Project or Select from Projection options";
                                    isTableValues = false;
                                }
                                else if ((tel.Activity == "" || tel.Activity == null) && tel.Project.ToUpper().Trim() != "CLIENT HOLIDAY" && tel.Project.ToUpper().Trim() != "PERSONAL LEAVE")
                                {
                                    ErrorMessage = "Please populate Activity";
                                    isTableValues = false;
                                }
                                else if (tel.Project.ToUpper().Trim() != "CLIENT HOLIDAY" && tel.Project.ToUpper().Trim() != "PERSONAL LEAVE" && tel.Project.ToUpper().Trim() != "NON-BILLABLE" && tel.Hours == 0)
                                {
                                    ErrorMessage = "Please populate Hours";
                                    isTableValues = false;
                                }
                            }
                        }
                    });

                    if (timesheet.TSApproverEmail == null || timesheet.TSApproverEmail == "" || timesheet.TimeSheetTypeID == 0)
                    {
                        ErrorMessage = "Timesheet Type or Approver not defined";
                        isTableValues = false;
                    }
                }
                //else if(timesheet.TimesheetApproverID == 0 || timesheet.TimeSheetTypeID == 0)
                else
                {
                    ErrorMessage = "Time Entries cannot be empty, please select valid Week Ending";
                    isTableValues = false;
                }
            }

            if (isTableValues)
            {
                if (timesheet.Status.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.SUBMITTED.ToUpper())
                {
                    SubmitConfirmation.Show();
                }
                else
                {
                    await SaveTimeSheet();
                }
            }
        }

        protected async Task SaveTimeSheet()
        {
            if (buttonsdisabledvalue)
                return;
            buttonsdisabledvalue = true;
            int totalhours = 0;
            timesheet.CreatedBy = user.FirstName + " " + user.LastName;
            timesheet.EmployeeName = employeeDetails.EmployeeName;
            timesheet.SubmittedByID = user.UserID;
            timesheet.TimeEntries = TimeEntryList;
            TimeEntryList.ForEach(te =>
            {
                totalhours = totalhours + te.Hours;
            });
            timesheet.TotalHours = totalhours;
            timesheet.ClientName = EmployeeAssignments.Find(cl => cl.ClientID == timesheet.ClientID).Client;
            // file upload
            string timesheetdate = "";
            if (timesheet.WeekEnding == null)
            {
                timesheetdate = Guid.NewGuid().ToString();
            }
            else
            {
                if (timesheet.TimeSheetType.ToUpper() == TimeSheetType.MONTHLY)
                {
                    timesheetdate = ((DateTime)timesheet.WeekEnding).ToString("yyyyMMdd");
                }
            }

            string filePath = "";
            if (file != null)
            {
                filePath = file.Name;
            }

            if (TimeSheetId == 0)
            {
                if (filePath != "")
                {
                    string fileName = await UploadFile(filePath, timesheetdate, timesheet.EmployeeName);
                    timesheet.FileName = fileName;
                }

                var result = await TimeSheetService.SaveTimeSheet(timesheet);
                if (result.MessageType == MessageType.Success)
                {
                    timesheet = result.Data;
                    emailapproval.EmailApprovalID = 0;
                    emailapproval.ModuleID = Modules.Find(m => m.ModuleName.ToUpper() == "TIMESHEET").ModuleID;
                    emailapproval.ID = timesheet.TimeSheetID;
                    emailapproval.ValidTime = DateTime.Now.AddDays(Configuration["EmailApprovalValidity"] == null ? 1 : Convert.ToInt32(Configuration["EmailApprovalValidity"]));
                    emailapproval.Value = timesheet.StatusValue;
                    emailapproval.LinkID = Guid.NewGuid();
                    emailapproval.ApproverEmail = EmployeeAssignment.TSApproverEmail;
                    emailapproval.CreatedBy = user.FirstName + " " + user.LastName;
                    EmailFields emailFields = await PrepareEmail();
                    emailapproval.EmailSubject = emailFields.EmailSubject;
                    emailapproval.EmailBody = emailFields.EmailBody;
                    emailapproval.EmailFrom = emailFields.EmailFrom;
                    emailapproval.EmailTo = emailFields.EmailTo;

                    if (timesheet.StatusID == TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.SUBMITTED.ToUpper()).ListValueID)
                    {
                        emailapproval.IsActive = true;
                        var resultEmailApproval = await EmailApprovalService.SaveEmailApproval(emailapproval);
                        toastService.ShowSuccess("TimeSheet submitted successfully", "");
                    }
                    else {
                        // emailapproval.IsActive = false;
                        // var resultEmailApproval = await EmailApprovalService.SaveEmailApproval(emailapproval);
                        toastService.ShowSuccess("TimeSheet saved successfully", "");
                    }

                    await TimeSheetUpdated.InvokeAsync(true);

                    Cancel();
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
            }
            else if (TimeSheetId > 0)
            {
                if (filePath != "")
                {
                    string fileName = await UploadFile(filePath, timesheetdate, timesheet.EmployeeName);
                    timesheet.FileName = fileName;
                }

                var result = await TimeSheetService.UpdateTimeSheet(TimeSheetId, timesheet);
                if (result.MessageType == MessageType.Success)
                {
                    timesheet = result.Data;
                    Response<ILT.IHR.DTO.EmailApproval> respEmailApproval = new Response<ILT.IHR.DTO.EmailApproval>();

                    if (timesheet.LinkID != Guid.Empty)
                    {
                        respEmailApproval = await EmailApprovalService.GetEmailApprovalByIdAsync(timesheet.LinkID) as Response<ILT.IHR.DTO.EmailApproval>;
                        emailapproval = respEmailApproval.Data;
                        emailapproval.ModifiedBy = user.FirstName + " " + user.LastName;
                    }
                    else
                    {
                        emailapproval.EmailApprovalID = 0;
                        emailapproval.ModuleID = Modules.Find(m => m.ModuleName.ToUpper() == "TIMESHEET").ModuleID;
                        emailapproval.ID = timesheet.TimeSheetID;
                        emailapproval.LinkID = Guid.NewGuid();
                        emailapproval.ApproverEmail = EmployeeAssignment.TSApproverEmail;
                        emailapproval.CreatedBy = user.FirstName + " " + user.LastName;
                    }
                    emailapproval.ValidTime = DateTime.Now.AddDays(Configuration["EmailApprovalValidity"] == null ? 1 : Convert.ToInt32(Configuration["EmailApprovalValidity"]));
                    EmailFields emailFields = await PrepareEmail();
                    emailapproval.EmailSubject = emailFields.EmailSubject;
                    emailapproval.EmailBody = emailFields.EmailBody;
                    emailapproval.EmailFrom = emailFields.EmailFrom;
                    emailapproval.EmailTo = emailFields.EmailTo;
                    
                    if (timesheet.StatusID == TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.SUBMITTED.ToUpper()).ListValueID)
                    {
                        emailapproval.IsActive = true;
                        var resultEmailApproval = await EmailApprovalService.SaveEmailApproval(emailapproval);
                        toastService.ShowSuccess("TimeSheet submitted successfully", "");
                    }
                    else {
                        // var resultEmailApproval = await EmailApprovalService.SaveEmailApproval(emailapproval);
                        toastService.ShowSuccess("TimeSheet saved successfully", "");
                    }
                    await TimeSheetUpdated.InvokeAsync(true);
                    Cancel();
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
            }
            buttonsdisabledvalue = true;
        }

        private async Task<string> UploadFile(string filePath, string timesheetdate, string employeeName)
        {
            if (employeeName == null)
                employeeName = "";
            string fileName = "";
            if (filePath != "")
            {
                BlobServiceClient _blobServiceClient;
                string blobContainerName = Configuration["TimeSheetBlobContainer:" + this.user.ClientID.ToUpper()];
                Stream content = file.Data; // System.IO.File.OpenRead(filePath); //From the OpenDialog box
                string fileExtension = Path.GetExtension(filePath);
                employeeName = employeeName.Replace(" ", "");
                fileName = "TS" + (employeeName.Length <= 15 ? employeeName : employeeName.Substring(0, 15)) + timesheetdate + fileExtension;
                string contentType = content.GetType().ToString();

                _blobServiceClient = new BlobServiceClient(Configuration["AzureBlobConnectionString"]); //Azure ConnectionString
                var containerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
                var blobClient = containerClient.GetBlobClient(fileName);
                await blobClient.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType });
            }
            return fileName;
        }

        public void HandleFileSelected(IFileListEntry[] files)
        {
            file = files.FirstOrDefault();
        }

        public void removeAttachment()
        {
            file = null;
        }

        public void removeAttachmentPending()
        {
            timesheet.FileName = null;
        }


        public async Task GetBlobContentPath()
        {
            //string baseUri = Configuration["ApiUrl"];
            //return baseUri + "Timesheet/DownloadFile?Client=" + user.ClientID + "&Doc=" + timesheet.DocGuid;
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
            emailFields.isMultipleEmail = true;
            var managerName = "";
            var clientManager = "";
            if (EmployeeAssignment.TSApproverEmail != null)
            {
                //var approverEmail = UserList.Find(ul => ul.UserID == EmployeeAssignment.TimesheetApproverID).Email;
                var approverEmail = EmployeeAssignment.TSApproverEmail;
                clientManager = EmployeeAssignment.ClientManager;
                emailFields.EmailTo = approverEmail;
                managerName = employeeDetails.Manager;
            }
            // common.EmailCC= employeeDetails.Email;
            if (isMonthly == true)
            {
                emailFields.EmailSubject = employeeDetails.EmployeeName + " has submitted timesheet for month ending " + FormatDate(timesheet.WeekEnding);
            }
            else
            {
                emailFields.EmailSubject = employeeDetails.EmployeeName + " has submitted timesheet for week ending " + FormatDate(timesheet.WeekEnding);
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
            "<table width='80%'><tr align='center'><td align='right'><a href='" + uri + "emailapproval?ClientID=" + user.ClientID + "&LinkID=" + emailapproval.LinkID + "&Value=APPROVE'><img src='" + Configuration["ImageURL:" + this.user.ClientID.ToUpper() + ":Approve"] + "' /></a></td><td align='left'><a href='" + uri + "emailapproval?ClientID=" + user.ClientID.ToUpper() + "&LinkID=" + emailapproval.LinkID + "&Value=REJECT'><img src='" + Configuration["ImageURL:" + this.user.ClientID.ToUpper() + ":Reject"] + "' /></a></td></tr></table>" +
            "<br/><table style='border: 1px solid black; border-collapse:collapse;' width='80%'>" + tableInfo + "</table>";
            emailFields.EmailBody = emailBody;
            return emailFields;
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

        protected async Task PendingTimeSheet()
        {
            ConfirmMessage = "Are you sure you want to save?";
            timesheet.StatusID = TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.PENDING.ToUpper()).ListValueID;
            timesheet.Status = TimeSheetStatusList.Find(tss => tss.Value.ToUpper() == ListTypeConstants.TimeSheetStatusConstants.PENDING.ToUpper()).ValueDesc;
        }

        protected async Task SubmitTimeSheet()
        {
            ConfirmMessage = "Are you sure you want to submit?";
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
            timesheet.SubmittedDate = DateTime.Now;
            timesheet.ApprovedDate = null;
            timesheet.ClosedDate = null;
            totalhours = timesheet.TotalHours.ToString();
            /*var tempTotalHours = 0;
            TimeEntryList.ForEach(tl =>
            {
                tempTotalHours = tempTotalHours + tl.Hours;
            });
            totalhours = tempTotalHours.ToString();*/
            if (timesheet.StatusValue.ToUpper() != ListTypeConstants.TimeSheetStatusConstants.PENDING.ToUpper())
            {
                entrydisabledvalue = true;
                buttonsdisabledvalue = true;
            }
            else
            {
                entrydisabledvalue = false;
                buttonsdisabledvalue = false;
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
            isfirstElementFocus = true;
            ShowDialog = true;
            StateHasChanged();
        }

        public void Cancel()
        {
            TimeEntryList.Clear();
            TimeSheetId = -1;
            ShowDialog = false;
            StateHasChanged();
        }
        public async void Show(int Id)
        {
            ErrorMessage = "";
            TimeSheetId = Id;
            ResetDialog();
            if (user.EmployeeID != null)
            {
                timesheet.EmployeeID = (int)user.EmployeeID;
                buttonsdisabledvalue = false;
                file = null;
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
                    disabledvalue = false;
                    entrydisabledvalue = false;
                    await GetEmployeeDetails((int)user.EmployeeID);
                    Title = "Add";
                    isfirstElementFocus = true;
                    isMonthly = false;
                    ShowDialog = true;
                    StateHasChanged();
                }
            }
            else
            {
                toastService.ShowError("Timesheet can't be created for non-Employees", "");
            }
        }
        protected async Task LoadWorkDates(ChangeEventArgs e)
        {
            ErrorMessage = "";
            TimeEntryList.Clear();
            var dateexists = false;
            var weekendingdate = Convert.ToDateTime(e.Value);
            if (EmployeeAssignment != null && EmployeeAssignment.TimeSheetType.ToUpper() == TimeSheetType.MONTHLY)
            {
                weekendingdate = new DateTime(weekendingdate.Year, weekendingdate.Month, DateTime.DaysInMonth(weekendingdate.Year, weekendingdate.Month));
            }            
            var respTimeSheetRequest = (await TimeSheetService.GetTimeSheets((int)timesheet.EmployeeID, 0));
            if (respTimeSheetRequest.MessageType == MessageType.Success)
            {
                TimeSheetsList = respTimeSheetRequest.Data.ToList().FindAll(tsl => tsl.EmployeeID == timesheet.EmployeeID && tsl.ClientID == timesheet.ClientID);

                dateexists = TimeSheetsList.FindIndex(ts => ts.WeekEnding.Value.Date == weekendingdate.Date &&
                ts.StatusValue.ToUpper() != ListTypeConstants.TimeSheetStatusConstants.REJECTED.ToUpper() && ts.StatusValue.ToUpper() != ListTypeConstants.TimeSheetStatusConstants.VOID.ToUpper()) > -1;
            }


            if (EmployeeAssignment !=null && EmployeeAssignment.EndDate != null && (timesheet.WeekEnding < EmployeeAssignment.StartDate || timesheet.WeekEnding > EmployeeAssignment.EndDate))
            {
                ErrorMessage = "Please Check Assignment dates and enter valid week ending";
            }
            else if (EmployeeAssignment != null &&  EmployeeAssignment.EndDate == null && timesheet.WeekEnding < EmployeeAssignment.StartDate)
            {
                ErrorMessage = "Please Check Assignment dates and enter valid week ending";
            }
            else if(dateexists)
            {
                if (EmployeeAssignment != null &&  EmployeeAssignment.TimeSheetType.ToUpper() == TimeSheetType.MONTHLY)
                {
                    ErrorMessage = "Timesheet already exists for the selected month ending";
                }
                else
                {
                    ErrorMessage = "Timesheet already exists for the selected week ending";
                }
            }
            else
            {
                DateTime currentStartDate = weekendingdate.AddDays(-7);

                if (EmployeeAssignment != null)
                {
                    var timesheettype = EmployeeAssignment.TimeSheetType;
                    TimeEntryList.Clear();
                    if (timesheettype == null)
                    {
                        ErrorMessage = "Timesheet Type not defined";
                    }
                    else if (timesheettype.ToUpper() == TimeSheetType.MONTHLY)
                    {
                        ErrorMessage = "";
                        var date = new DateTime(weekendingdate.Year, weekendingdate.Month, 1);
                        timesheet.WeekEnding = weekendingdate.AddDays(1 - weekendingdate.Day).AddMonths(1).AddDays(-1).Date;
                        int daysInMonth = DateTime.DaysInMonth(weekendingdate.Year, weekendingdate.Month);
                        for (int i = 0; i < daysInMonth; i++)
                        {
                            DTO.TimeEntry temptimeentry = new DTO.TimeEntry { };
                            temptimeentry.WorkDate = date.AddDays(i);
                            TimeEntryList.Add(temptimeentry);
                        }
                    }
                    else if (timesheettype.ToUpper() == TimeSheetType.FRIDAY)
                    {
                        if (currentStartDate.ToString("dddd").ToUpper() != "FRIDAY")
                        {
                            ErrorMessage = "Selected Date must be Friday";
                        }
                        else
                        {
                            ErrorMessage = "";
                            var date = currentStartDate;
                            for (int i = 0; i < 7; i++)
                            {
                                DTO.TimeEntry temptimeentry = new DTO.TimeEntry { };
                                temptimeentry.WorkDate = date.AddDays(i + 1);
                                TimeEntryList.Add(temptimeentry);
                            }
                        }

                    }
                    else if (timesheettype.ToUpper() == TimeSheetType.SATURDAY)
                    {
                        if (currentStartDate.ToString("dddd").ToUpper() != "SATURDAY")
                        {
                            ErrorMessage = "Selected Date must be Saturday";
                        }
                        else
                        {
                            ErrorMessage = "";
                            var date = currentStartDate;
                            for (int i = 0; i < 7; i++)
                            {
                                DTO.TimeEntry temptimeentry = new DTO.TimeEntry { };
                                temptimeentry.WorkDate = date.AddDays(i + 1);
                                TimeEntryList.Add(temptimeentry);
                            }
                        }
                    }
                    else if (timesheettype.ToUpper() == TimeSheetType.SUNDAY)
                    {
                        if (currentStartDate.ToString("dddd").ToUpper() != "SUNDAY")
                        {
                            ErrorMessage = "Selected Date must be Sunday";
                        }
                        else
                        {
                            ErrorMessage = "";
                            var date = currentStartDate;
                            for (int i = 0; i < 7; i++)
                            {
                                DTO.TimeEntry temptimeentry = new DTO.TimeEntry { };
                                temptimeentry.WorkDate = date.AddDays(i + 1);
                                TimeEntryList.Add(temptimeentry);
                            }
                        }
                    }
                }
                else
                {
                    timesheet.TimeSheetTypeID = 0;
                    timesheet.TimeSheetType = "";
                }
            }

        }

        protected async Task LoadProject(string projectvalue, string workdate)
        {
            TimeEntryList.ForEach(te =>
            {
                if (Convert.ToDateTime(workdate).ToString("ddd").ToUpper() != "SAT" && Convert.ToDateTime(workdate).ToString("ddd").ToUpper() != "SUN")
                {
                    if (timesheet.TimeSheetType.ToUpper() == "MONTHLY")
                    {
                        for (int i = 0; i < 30; i++)
                        {
                            if (te.WorkDate == Convert.ToDateTime(workdate))
                            {
                                if (projectvalue != null && (projectvalue.ToUpper() == "CLIENT HOLIDAY" || projectvalue.ToUpper() == "PERSONAL LEAVE" || projectvalue.ToUpper() == "NON-BILLABLE"))
                                {
                                    if (te.Hours > 0)
                                    {
                                        int tempTotalHours = Convert.ToInt32(totalhours) - te.Hours;
                                        totalhours = tempTotalHours.ToString();
                                    }
                                    te.Hours = 0;
                                    te.Activity = "";
                                }
                            }
                            if (te.WorkDate == Convert.ToDateTime(workdate).AddDays(i + 1))
                            {
                                if ((te.Project == null || te.Project == "") && (Convert.ToDateTime(te.WorkDate).ToString("ddd").ToUpper() != "SAT" && Convert.ToDateTime(te.WorkDate).ToString("ddd").ToUpper() != "SUN"))
                                {
                                    te.Project = projectvalue;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            if (te.WorkDate == Convert.ToDateTime(workdate))
                            {
                                if (projectvalue != null && (projectvalue.ToUpper() == "CLIENT HOLIDAY" || projectvalue.ToUpper() == "PERSONAL LEAVE" || projectvalue.ToUpper() == "NON-BILLABLE"))
                                {
                                    if (te.Hours > 0)
                                    {
                                        int tempTotalHours = Convert.ToInt32(totalhours) - te.Hours;
                                        totalhours = tempTotalHours.ToString();
                                    }
                                    te.Hours = 0;
                                    te.Activity = "";
                                }
                            }
                            if (te.WorkDate == Convert.ToDateTime(workdate).AddDays(i + 1))
                            {
                                if ((te.Project == null || te.Project == "") && (Convert.ToDateTime(te.WorkDate).ToString("ddd").ToUpper() != "SAT" && Convert.ToDateTime(te.WorkDate).ToString("ddd").ToUpper() != "SUN"))
                                {
                                    te.Project = projectvalue;
                                }
                            }
                        }
                    }
                }
            });
        }

        protected async Task LoadActivity(string activityvalue, string workdate)
        {
            TimeEntryList.ForEach(te =>
            {
                if (Convert.ToDateTime(workdate).ToString("ddd").ToUpper() != "SAT" && Convert.ToDateTime(workdate).ToString("ddd").ToUpper() != "SUN")
                {
                    if (timesheet.TimeSheetType.ToUpper() == "MONTHLY")
                    {
                        for (int i = 0; i < 30; i++)
                        {
                            if (te.WorkDate == Convert.ToDateTime(workdate).AddDays(i + 1))
                            {
                                if ((te.Activity == null || te.Activity == "") && (Convert.ToDateTime(te.WorkDate).ToString("ddd").ToUpper() != "SAT" && Convert.ToDateTime(te.WorkDate).ToString("ddd").ToUpper() != "SUN"))
                                {
                                    te.Activity = activityvalue;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            if (te.WorkDate == Convert.ToDateTime(workdate).AddDays(i + 1))
                            {
                                if ((te.Activity == null || te.Activity == "") && (Convert.ToDateTime(te.WorkDate).ToString("ddd").ToUpper() != "SAT" && Convert.ToDateTime(te.WorkDate).ToString("ddd").ToUpper() != "SUN"))
                                {
                                    te.Activity = activityvalue;
                                }
                            }
                        }
                    }
                }
            });
        }

        protected async Task TotalHours(string hoursvalue, string workdate)
       {
            ErrorMessage = "";
            int i = 0;
            bool result = int.TryParse(hoursvalue, out i);
            if (result)
            {
                TimeEntryList.ForEach(te =>
                {
                    if (Convert.ToDateTime(workdate).ToString("ddd").ToUpper() != "SAT" && Convert.ToDateTime(workdate).ToString("ddd").ToUpper() != "SUN")
                    {
                        if (timesheet.TimeSheetType.ToUpper() == "MONTHLY")
                        {
                            for (int i = 0; i < 30; i++)
                            {
                                if (te.WorkDate == Convert.ToDateTime(workdate).AddDays(i + 1))
                                {
                                    if ((te.Hours == 0) && (Convert.ToDateTime(te.WorkDate).ToString("ddd").ToUpper() != "SAT" && Convert.ToDateTime(te.WorkDate).ToString("ddd").ToUpper() != "SUN"))
                                    {
                                        te.Hours = Convert.ToInt32(hoursvalue);
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 7; i++)
                            {
                                if (te.WorkDate == Convert.ToDateTime(workdate).AddDays(i + 1))
                                {
                                    if ((te.Hours == 0) && (Convert.ToDateTime(te.WorkDate).ToString("ddd").ToUpper() != "SAT" && Convert.ToDateTime(te.WorkDate).ToString("ddd").ToUpper() != "SUN"))
                                    {
                                        te.Hours = Convert.ToInt32(hoursvalue);
                                    }
                                }
                            }
                        }
                    }
                });

                int temptotalhours = 0;
                TimeEntryList.ForEach(te =>
                {
                    if (te.WorkDate == Convert.ToDateTime(workdate))
                    {
                        te.Hours = Convert.ToInt32(hoursvalue);
                    }
                    temptotalhours = temptotalhours + te.Hours;
                });
                totalhours = temptotalhours.ToString();
            }
            else
            {
                ErrorMessage = "Hours should be valid number";
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
        [Inject] IJSRuntime JSRuntime { get; set; }
        public bool isfirstElementFocus { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            JSRuntime.InvokeVoidAsync("OnlyPositiveNumber");
            if (isfirstElementFocus)
            {
                JSRuntime.InvokeVoidAsync("JSHelpers.setFocusByCSSClass");
                isfirstElementFocus = false;
            }
        }
    }
}
