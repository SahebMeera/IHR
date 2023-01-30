using BlazorDownloadFile;
using Blazored.SessionStorage;
using BlazorTable;
using ILT.IHR.DTO;
using ILT.IHR.UI.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.JSInterop;
using Microsoft.Reporting.NETCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazored.Toast.Services;

namespace  ILT.IHR.UI.Pages.Dashboard

{
    public class DashboardBase: ComponentBase
    {
        [Inject]
        public ISessionStorageService sessionStorage { get; set; }
        [Inject]
        public IWebHostEnvironment _webHostEnvironment { get; set; }
        [Inject]
        public ILookupService lookupService { get; set; }
        [Inject]
        public ICountryService CountryService  { get; set; }
        [Inject]
        public ILeaveBalanceService leaveBalanceService { get; set; }
        [Inject]
        public IAuditLogService auditlogService { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        [Inject] 
        public IBlazorDownloadFileService BlazorDownloadFileService { get; set; }
        public List<DTO.ListValue> reportTypes { get; set; }
        public DTO.User user { get; set; }
        public List<IMultiSelectDropDownList> lstAssetType { get; set; } //Drop Down Api Data
        public List<IMultiSelectDropDownList> lstTicketStatus { get; set; } //Drop Down Api Data
        public List<IMultiSelectDropDownList> lstAssetChangeSetsType { get; set; } //Drop Down Api Data
        public List<IMultiSelectDropDownList> lstTicketChangeSetsStatus { get; set; } //Drop Down Api Data
        public List<Country> CountryList { get; set; }
        public string pdfContent { get; set; }
        //report Parameters
        public string reportType { get; set; }
        public string country { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool isCountryHidden { get; set; }
        public bool isShowEmployeeDetail { get; set; }
        public bool isShowAssetReport { get; set; }
        public bool isDateHidden { get; set; }
        public List<ListValue> EmployMentList { get; set; }
        public List<IDropDownList> lstCountry { get; set; } //Drop Down Api Data
        public List<IDropDownList> lstStatus { get; set; } //Drop Down Api Data
        public string employeeType { get; set; }
        [Inject]
        public ILeaveService leaveService { get; set; }
        public List<IMultiSelectDropDownList> lstEmployeeType { get; set; } //Drop Down Api Data
        [Inject]
        public IEmployeeService EmployeeService { get; set; } //Service

        protected List<RolePermission> RolePermissions;
        protected RolePermission DashboardRolePermission;
        protected RolePermission TimeSheetRolePermission;
        protected RolePermission TicketRolePermission;
        protected RolePermission WizardDataRolePermission;
        protected RolePermission LeaveRolePermission;
        protected RolePermission ExpenseRolePermission;
        public RolePermission WFHRolePermission;

        private int EmployeeID { get; set; }
        [Inject]
        protected ILeaveService LeaveService { get; set; }
        [Inject]
        protected ITimeSheetService TimeSheetService { get; set; }
        [Inject]
        protected ITicketService TicketService { get; set; }
        [Inject]
        protected IExpenseService ExpenseService { get; set; }
        [Inject]
        protected IWorkFromHomeService WorkFromHomeService { get; set; }
        public List<Expense> lstExpensesList { get; set; }  // Table APi Data
        public List<Expense> ExpensesList { get; set; }  // Table APi Data
        public List<Leave> LeaveRequestList { get; set; }
        public List<Leave> lstLeaveRequest { get; set; }
        public List<ILT.IHR.DTO.TimeSheet> TimeSheetsList { get; set; }  // Table APi Data
        public List<ILT.IHR.DTO.TimeSheet> lstTimeSheetRequest { get; set; }  // Table APi Data
        public string currentLoginUserRole { get; set; }
        public List<ILT.IHR.DTO.Ticket> TicketsList { get; set; }  // Table APi Data
        public List<ILT.IHR.DTO.Ticket> lstTicketsList { get; set; }  // Table APi Data
        [Inject]
        public IToastService toastService { get; set; } //Service
        public List<WFH> WFHList { get; set; }  // Table APi Data
        public List<WFH> lstOfWFH { get; set; }  // Table APi Data
        protected string UserName { get; set; }
        public List<Dashboard> lstDashboardRequest { get; set; }
        public class Dashboard
        {
            public bool dashboardViewPermission { get; set; }
            public string dashboardCardColor { get; set; }
            public int dashboardListLength { get; set; }
            public string dashboardHeading { get; set; }
            public string dashboardSubHeading { get; set; }
        }
        protected async override Task OnInitializedAsync()
        {
            LeaveRequestList = new List<Leave> { };
            lstLeaveRequest = new List<Leave> { };
            TicketsList = new List<ILT.IHR.DTO.Ticket> { };
            WFHList = new List<WFH> { };
            user = await sessionStorage.GetItemAsync<DTO.User>("User");
            if (user != null)
            {
                UserName = user.FirstName + " " + user.LastName;
            }
            currentLoginUserRole = await sessionStorage.GetItemAsync<string>("RoleName");
            EmployeeID = Convert.ToInt32(user.EmployeeID);
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            TimeSheetRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.TIMESHEET);
            DashboardRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.DASHBOARD);
            TicketRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.TICKET);
            WizardDataRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.PROCESSDATA);
            LeaveRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.LEAVEREQUEST);
            ExpenseRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.EXPENSES);
            WFHRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.WFHREQUEST);
            lstDashboardRequest = new List<Dashboard> { };
             await LoadList();
             await LoadLeaveRequest();
             await LoadWFHRequest();
             await LoadTimeSheetRequest();
             await LoadExpenses();
             await LoadProcessList();
        }
        protected async Task LoadLeaveRequest()
        {
            LeaveRequestList = new List<Leave> { };
            var respLeaveRequest = (await LeaveService.GetLeave("ApproverID", EmployeeID));
            if (respLeaveRequest.MessageType == MessageType.Success)
            {
                LeaveRequestList = respLeaveRequest.Data.ToList();
                loadList("PENDING");
            }
            else
            {
                LeaveRequestList = new List<Leave> { };
                lstLeaveRequest = LeaveRequestList;
            }
            StateHasChanged();
        }
        protected void loadList(string Status)
        {
            
            Dashboard ListItem = new Dashboard();
            lstLeaveRequest = LeaveRequestList.Where(x => x.Status.ToUpper() == Status.ToUpper()).ToList();
            if(LeaveRolePermission != null && LeaveRolePermission.View == true)
            {
                ListItem.dashboardViewPermission = LeaveRolePermission.View;
                ListItem.dashboardCardColor = "#FCB711";
                ListItem.dashboardHeading = "Leaves";
                ListItem.dashboardSubHeading = "Leaves Pending Approval";
                ListItem.dashboardListLength = lstLeaveRequest == null ? 0 : lstLeaveRequest.Count;
                lstDashboardRequest.Add(ListItem);
            }
            StateHasChanged();
        }

        protected async Task LoadTimeSheetRequest()
        {
            var respTimeSheetRequest = (await TimeSheetService.GetTimeSheets(0, user.UserID));
            if (respTimeSheetRequest.MessageType == MessageType.Success)
            {
                TimeSheetsList = respTimeSheetRequest.Data.Where(x => x.TSApproverEmail == user.Email).ToList();
                loadListTimesheet(ListTypeConstants.TimeSheetStatusConstants.SUBMITTED.ToUpper());
                StateHasChanged();
            }
            else
            {
                TimeSheetsList = new List<ILT.IHR.DTO.TimeSheet> { };
            }
        }

        protected void loadListTimesheet(string Status)
        {
            Dashboard ListItem = new Dashboard();
            lstTimeSheetRequest = TimeSheetsList.Where(x => x.Status.ToUpper() == Status.ToUpper()).ToList();
            if (TimeSheetRolePermission != null && TimeSheetRolePermission.View == true)
            {
                ListItem.dashboardViewPermission = TimeSheetRolePermission.View;
                ListItem.dashboardCardColor = "#F37021";
                ListItem.dashboardHeading = "Timesheets";
                ListItem.dashboardSubHeading = "Timesheets Pending Approval";
                ListItem.dashboardListLength = lstTimeSheetRequest == null ? 0 : lstTimeSheetRequest.Count;
                if(lstDashboardRequest != null)
                {
                    lstDashboardRequest.Add(ListItem);
                }
               // lstDashboardRequest.Add(ListItem);
            }
            StateHasChanged();
        }


        protected async Task LoadList()
        {
            Response<IEnumerable<DTO.Ticket>> reponses = new Response<IEnumerable<ILT.IHR.DTO.Ticket>> { };
            if (currentLoginUserRole.ToUpper() == UserRole.ADMIN)
            {
                reponses = (await TicketService.GetTickets());
            }
            else
            {
                reponses = (await TicketService.GetTicketsList(EmployeeID, EmployeeID));
            }

            if (reponses.MessageType == MessageType.Success)
            {
                TicketsList = reponses.Data.ToList();
                if (TicketsList != null)
                {
                    loadTicketList();
                }
                else
                {
                    lstTicketsList = TicketsList;
                }
            }
            else
            {
                toastService.ShowError(ErrorMsg.ERRORMSG);
            }
        }
        public void loadTicketList()
        {
            if (TicketsList !=  null)
            {
                Dashboard ListItem = new Dashboard();
                lstTicketsList = TicketsList.Where(x => (x.Status.ToUpper() == "NEW" || x.Status.ToUpper() == "Assigned".ToUpper()) && x.AssignedToID == user.EmployeeID).ToList();
                if (TicketRolePermission != null && TicketRolePermission.View == true)
                {
                    ListItem.dashboardViewPermission = TicketRolePermission.View;
                    ListItem.dashboardCardColor = "#CC004C";
                    ListItem.dashboardHeading = "Tickets";
                    ListItem.dashboardSubHeading = "Pending Tickets";
                    ListItem.dashboardListLength = lstTicketsList == null ? 0 : lstTicketsList.Count;
                    lstDashboardRequest.Add(ListItem);
                }
            }
            else
            {
                lstTicketsList = TicketsList;
            }
            StateHasChanged();
        }

        [Inject]
        public IWizardDataService WizardDataService { get; set; } //Service
        public List<ProcessData> WizardDataList { get; set; }  // Table APi Data
        public List<DTO.ProcessData> lstWizardDataList { get; set; }  // Table APi Data
        protected async Task LoadProcessList()
        {
            
            var resp = (await WizardDataService.GetWizardDatas());
            if (resp.MessageType == MessageType.Success)
            {
                WizardDataList = resp.Data.ToList();
                Dashboard ListItem = new Dashboard();
                lstWizardDataList = WizardDataList.Where(x => x.StatusId == 143 || x.StatusId == 142).ToList();
                if (WizardDataRolePermission != null && WizardDataRolePermission.View == true)
                {
                    ListItem.dashboardViewPermission = WizardDataRolePermission.View;
                    ListItem.dashboardCardColor = "#6460AA";
                    ListItem.dashboardHeading = "Process Data";
                    ListItem.dashboardSubHeading = "Process Data Count";
                    ListItem.dashboardListLength = lstWizardDataList == null ? 0 : lstWizardDataList.Count;
                    lstDashboardRequest.Add(ListItem);
                }
                StateHasChanged();
                //WizardDataDisplay();
            }
            else
            {
                lstWizardDataList = new List<ProcessData> { };
            }
            StateHasChanged();
        }
        protected async Task LoadExpenses()
        {
            string RoleShort = await sessionStorage.GetItemAsync<string>("RoleShort");
            Dashboard ListItem = new Dashboard();
            var resp = (await ExpenseService.GetExpenses());
            if (resp.MessageType == MessageType.Success)
            {
                if (resp.Data != null && (RoleShort.ToUpper() == UserRole.EMP || RoleShort.ToUpper() == UserRole.CONTRACTOR))
                {
                    ExpensesList = resp.Data.Where(x => x.EmployeeID == user.EmployeeID).ToList();
                }
                else
                {
                    ExpensesList = resp.Data.ToList();
                }
                lstExpensesList = ExpensesList.Where(x => x.Status.ToUpper() == "Submitted".ToUpper()).ToList();
                if (ExpenseRolePermission != null && ExpenseRolePermission.View == true)
                {
                    ListItem.dashboardViewPermission = ExpenseRolePermission.View;
                    ListItem.dashboardCardColor = "#0089D0";
                    ListItem.dashboardHeading = "Expenses";
                    ListItem.dashboardSubHeading = "Expenses Pending Approval";
                    ListItem.dashboardListLength = lstExpensesList == null ? 0 : lstExpensesList.Count;
                    lstDashboardRequest.Add(ListItem);
                }
                StateHasChanged();
            }
            else
            {
                toastService.ShowError(ErrorMsg.ERRORMSG);
            }
        }
        protected async Task LoadWFHRequest()
        {
                lstOfWFH = new List<WFH> { };
                var respWFHRequest = (await WorkFromHomeService.GetWFH("ApproverID", EmployeeID));
                if (respWFHRequest.MessageType == MessageType.Success)
                {
                     WFHList = respWFHRequest.Data.ToList();
                    loadListWFH("PENDING");
                }
                else
                {
                WFHList = new List<WFH> { };
                }
            StateHasChanged();
        }
        protected void loadListWFH(string Status)
        {
            Dashboard ListItem = new Dashboard();
            lstOfWFH = WFHList.Where(x => x.Status.ToUpper() == Status.ToUpper()).ToList();
            if (WFHRolePermission != null && WFHRolePermission.View == true)
            {
                ListItem.dashboardViewPermission = WFHRolePermission.View;
                ListItem.dashboardCardColor = "#0DB14B";
                ListItem.dashboardHeading = "WFH Requests";
                ListItem.dashboardSubHeading = "WFH Requests Pending Approval";
                ListItem.dashboardListLength = lstOfWFH == null ? 0 : lstOfWFH.Count;
                lstDashboardRequest.Add(ListItem);
            }
            StateHasChanged();
        }


        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public DataProvider dataProvider { get; set; } //Service
        public async Task redirectToPendingLeave()
        {
            dataProvider.TabIndex = 1;
            NavigationManager.NavigateTo($"/leaverequests");
        }
        public async Task redirectToTimesheet()
        {
            dataProvider.TabIndex = 1;
            NavigationManager.NavigateTo($"/timesheet");
        }
        public async Task redirectToTicket()
        {
            NavigationManager.NavigateTo($"/ticket");
        }
        public async Task redirectToProcessData()
        {
            NavigationManager.NavigateTo($"/processdatas");
        }
        public async Task redirectToExpenses()
        {
            NavigationManager.NavigateTo($"/expenses");
        }
        public async Task redirectToWFH()
        {
            dataProvider.TabIndex = 1;
            NavigationManager.NavigateTo($"/wfhrequests");
        }

        public async Task redirectToPages(string pageName)
        {
            if (!string.IsNullOrEmpty(pageName))
            {
                if(pageName.ToUpper() == "LEAVES")
                {
                    dataProvider.TabIndex = 1;
                    NavigationManager.NavigateTo($"/leaverequests");
                }
                if (pageName.ToUpper() == "TIMESHEETS")
                {
                    dataProvider.TabIndex = 1;
                    NavigationManager.NavigateTo($"/timesheet");
                }
                if (pageName.ToUpper() == "TICKETS")
                {
                    NavigationManager.NavigateTo($"/ticket");
                }
                if (pageName.ToUpper() == "PROCESS DATA")
                {
                    NavigationManager.NavigateTo($"/processdatas");
                }
                if (pageName.ToUpper() == "EXPENSES")
                {
                    NavigationManager.NavigateTo($"/expenses");
                }
                if (pageName.ToUpper() == "WFH REQUESTS")
                {
                    dataProvider.TabIndex = 1;
                    NavigationManager.NavigateTo($"/wfhrequests");
                }
            }
        }
    }
}