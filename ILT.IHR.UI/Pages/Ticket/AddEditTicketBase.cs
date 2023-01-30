using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;
using ILT.IHR.UI.Service;
using Blazored.Toast.Services;
using Blazored.SessionStorage;
using Microsoft.JSInterop;
using Microsoft.Extensions.Configuration;


namespace ILT.IHR.UI.Pages.Ticket
{
    public class AddEditTicketBase : ComponentBase
    {
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service    
        [Inject]
        public ITicketService TicketService { get; set; } //Service
        [Inject]
        public ICountryService CountryService { get; set; } //Service
        public IEnumerable<ILT.IHR.DTO.Ticket> Tickets { get; set; } //Drop Down Api Data       
        private int TicketId { get; set; }
        [Parameter]
        public EventCallback<bool> TicketUpdated { get; set; }
        [Parameter]
        public EventCallback<int> WizadTicketUpdatedList { get; set; }
        [Inject]
        public NavigationManager UrlNavigationManager { get; set; }
        [Inject]
        public IRoleService RoleService { get; set; } //Service
        [Inject]
        public IUserService UserService { get; set; } //Service
        [Inject]
        public IEmailApprovalService EmailApprovalService { get; set; } //Service   
        [Inject]
        public ICommonService CommonService { get; set; } //Service

        protected string Title = "Add";
        public ILT.IHR.DTO.Ticket Ticket = new ILT.IHR.DTO.Ticket();
        public bool ShowDialog { get; set; }
        public bool disabledvalue { get; set; }
        public int EmployeeID { get; set; }

        public List<Country> CountryList { get; set; }
        public ILT.IHR.DTO.User user;
        [Inject]
        public ILookupService LookupService { get; set; } //Service
        public List<ListValue> TicketTypeList { get; set; }  // Table APi Data
        public List<ListValue> TicketStatusList { get; set; }  // Table APi Data
        public List<ListValue> TicketStatus2List { get; set; }  // Table APi Data
        public List<ListValue> TicketEmailMapList { get; set; }  // Table APi Data
        [Inject]
        public IEmployeeService EmployeeService { get; set; } //Service 
        [Inject]
        public IConfiguration Configuration { get; set; } //configuration  
        public IEnumerable<ILT.IHR.DTO.Employee> Employees { get; set; } //Drop Down Api Data  
        public IEnumerable<ILT.IHR.DTO.Employee> lstAssignedList { get; set; } //Drop Down Api Data  
        protected IEnumerable<DTO.Employee> lstEmployees { get; set; }

        public List<Module> Modules { get; set; } //Drop Down Api Data
        public bool isCommentExist { get; set; }
        public bool isCommentDisable { get; set; }
        public bool isResolvedDate { get; set; }
        public bool isAssignedDisable { get; set; }
        public bool isTicketTypeDescription { get; set; }
        public bool isSaveVisable { get; set; }
        public bool isResolveButtonDisabled { get; set; } = false;
        public bool isSaveButtonDisabled { get; set; } = false;
        ILT.IHR.DTO.EmailApproval emailapproval = new ILT.IHR.DTO.EmailApproval();
        public int WizardDataID { get; set; }




        protected override async Task OnInitializedAsync()
        {
            user = await sessionStorage.GetItemAsync<ILT.IHR.DTO.User>("User");
            await LoadDropDown();
            await LoadUserList();
        }

        protected async Task SaveTicket()
        {
            if (isSaveButtonDisabled)
                return;
            isSaveButtonDisabled = true;
            if (TicketId == 0)
            {
                Ticket.CreatedBy = user.FirstName + " " + user.LastName;
                var result = await TicketService.SaveTicket(Ticket);
                if (result.MessageType == MessageType.Success)
                {
                    var ticketResp = await TicketService.GetTicketByIdAsync(result.Data.RecordID) as Response<ILT.IHR.DTO.Ticket>;
                    if (ticketResp.MessageType == MessageType.Success)
                    {
                        //DTO.EmailApproval emailapproval = new DTO.EmailApproval();
                        Ticket = ticketResp.Data;
                        emailapproval.EmailApprovalID = 0;
                        emailapproval.ModuleID = Modules.Find(m => m.ModuleName.ToUpper() == "TICKET").ModuleID;
                        emailapproval.ID = Ticket.TicketID;
                        emailapproval.ValidTime = DateTime.Now.AddDays(Configuration["EmailApprovalValidity"] == null ? 1 : Convert.ToInt32(Configuration["EmailApprovalValidity"]));
                        emailapproval.Value = Ticket.Status;
                        emailapproval.LinkID = Guid.NewGuid();
                        // emailapproval.ApproverEmail = EmployeeAssignment.TSApproverEmail;
                        emailapproval.CreatedBy = user.FirstName + " " + user.LastName;
                        EmailFields emailFields = await prepareTicketMail();
                        emailapproval.EmailSubject = emailFields.EmailSubject;
                        emailapproval.EmailBody = emailFields.EmailBody;
                        emailapproval.EmailFrom = emailFields.EmailFrom;
                        emailapproval.EmailTo = emailFields.EmailTo;
                        emailapproval.EmailCC = emailFields.EmailCC;
                        emailapproval.IsActive = true;
                        var resultEmailApproval = await EmailApprovalService.SaveEmailApproval(emailapproval);
                        toastService.ShowSuccess("Ticket saved successfully", "");
                        TicketUpdated.InvokeAsync(true);
                        //await sendTicketMail();
                    }
                    Cancel();
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
            }
            else if (TicketId > 0)
            {
                Ticket.ModifiedBy = user.FirstName + " " + user.LastName;
                var result = await TicketService.UpdateTicket(TicketId, Ticket);
                if (result.MessageType == MessageType.Success)
                {
                    var ticketResp = await TicketService.GetTicketByIdAsync(result.Data.RecordID) as Response<ILT.IHR.DTO.Ticket>;
                    if (ticketResp.MessageType == MessageType.Success)
                    {
                        Ticket = ticketResp.Data;
                        EmailFields emailFields = new EmailFields();
                        Response<ILT.IHR.DTO.EmailApproval> respEmailApproval = new Response<ILT.IHR.DTO.EmailApproval>();
                        if (Ticket.LinkID != Guid.Empty)
                        {
                            respEmailApproval = await EmailApprovalService.GetEmailApprovalByIdAsync(Ticket.LinkID) as Response<ILT.IHR.DTO.EmailApproval>;
                            emailapproval = respEmailApproval.Data;
                            emailapproval.ModifiedBy = user.FirstName + " " + user.LastName;
                            emailapproval.SentCount = emailapproval.SentCount > 0 ? emailapproval.SentCount - 1 : 0;
                        }
                        else
                        {
                            emailapproval.EmailApprovalID = 0;
                            emailapproval.ModuleID = Modules.Find(m => m.ModuleShort.ToUpper() == "TICKET").ModuleID;
                            emailapproval.ID = Ticket.TicketID;
                            emailapproval.IsActive = true;
                            emailapproval.LinkID = Guid.NewGuid();
                            // emailapproval.ApproverEmail = employeeDetails.ManagerEmail;
                            emailapproval.CreatedBy = user.FirstName + " " + user.LastName;
                        }
                        emailapproval.ValidTime = DateTime.Now.AddDays(Configuration["EmailApprovalValidity"] == null ? 1 : Convert.ToInt32(Configuration["EmailApprovalValidity"]));
                        emailFields = await prepareMail();
                        emailapproval.EmailSubject = emailFields.EmailSubject;
                        emailapproval.EmailBody = emailFields.EmailBody;
                        emailapproval.EmailFrom = emailFields.EmailFrom;
                        emailapproval.EmailTo = emailFields.EmailTo;
                        emailapproval.EmailCC = emailFields.EmailCC;
                        var resultEmailApproval = await EmailApprovalService.SaveEmailApproval(emailapproval);
                        toastService.ShowSuccess("Ticket saved successfully", "");
                        await TicketUpdated.InvokeAsync(true);
                        if (WizardDataID != null && WizardDataID != 0)
                        {
                            await WizadTicketUpdatedList.InvokeAsync(WizardDataID);
                        }
                        Cancel();
                }
                else
                {
                    Cancel();
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }

            }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
            }

            isSaveButtonDisabled = false;


        }



        protected async Task resolve()
        {
            if (isResolveButtonDisabled)
                return;
            isResolveButtonDisabled = true;
            string ClientID = await sessionStorage.GetItemAsync<string>("ClientID");
            if (TicketId > 0 && !String.IsNullOrEmpty(Ticket.Comment))
            {
                isCommentExist = false;
                isResolvedDate = false;
                Ticket.StatusID = TicketStatusList.Find(x => x.ValueDesc.ToUpper() == TicketStatusConstants.RESOLVED).ListValueID;
                Ticket.ResolvedDate = DateTime.Now;
                Ticket.ModifiedBy = user.FirstName + " " + user.LastName;
                var result = await TicketService.UpdateTicket(TicketId, Ticket);
                if (result.MessageType == MessageType.Success)
                {
                    var ticketResp = await TicketService.GetTicketByIdAsync(TicketId) as Response<ILT.IHR.DTO.Ticket>;
                    if (ticketResp.MessageType == MessageType.Success) {
                        Ticket = ticketResp.Data;
                        Response<ILT.IHR.DTO.EmailApproval> respEmailApproval = new Response<ILT.IHR.DTO.EmailApproval>();
                        DTO.EmailApproval emailApproval = new DTO.EmailApproval();
                        if (Ticket.LinkID != Guid.Empty)
                        {
                            respEmailApproval = await EmailApprovalService.GetEmailApprovalByIdAsync(Ticket.LinkID) as Response<ILT.IHR.DTO.EmailApproval>;
                            emailApproval = respEmailApproval.Data;
                            emailApproval.ModifiedBy = user.FirstName + " " + user.LastName;
                            await EmailApprovalService.EamilApprovalAction(ClientID, Ticket.LinkID, "RESOLVED", "TICKET");
                        }
                        else
                        {
                            emailApproval.ModuleID = Modules.Find(m => m.ModuleShort.ToUpper() == "TICKET").ModuleID;
                            emailApproval.ID = Ticket.TicketID;
                            emailApproval.ValidTime = DateTime.Now.AddDays(Configuration["EmailApprovalValidity"] == null ? 1 : Convert.ToInt32(Configuration["EmailApprovalValidity"]));
                            emailApproval.IsActive = true;
                            emailApproval.LinkID = Guid.NewGuid();
                            //emailApproval.ApproverEmail = await GetEmail(WFH.ApproverID);
                            emailApproval.CreatedBy = user.FirstName + " " + user.LastName;
                        }
                        EmailFields emailFields = await prepareResolveMail();
                        emailApproval.EmailApprovalID = 0;
                        emailApproval.LinkID = Guid.Empty;
                        emailApproval.EmailSubject = emailFields.EmailSubject;
                        emailApproval.EmailBody = emailFields.EmailBody;
                        emailapproval.EmailFrom = emailFields.EmailFrom;
                        //emailApproval.EmailCC = emailFields.EmailCC;
                        emailApproval.EmailTo = emailFields.EmailTo;
                        emailApproval.Value = null;
                        emailApproval.IsActive = true;
                        await EmailApprovalService.SaveEmailApproval(emailApproval);
                        toastService.ShowSuccess("Ticket saved successfully", "");
                        await TicketUpdated.InvokeAsync(true);
                        if (WizardDataID != null && WizardDataID != 0)
                        {
                            await WizadTicketUpdatedList.InvokeAsync(WizardDataID);
                        }
                        Cancel();
                    } else
                    {
                        toastService.ShowError(ErrorMsg.ERRORMSG);
                    }
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
            } else
            {
                isResolvedDate = true;
                isCommentExist = true;
            }
            isResolveButtonDisabled = false;
        }

        protected string TicketEmail { get; set; }
        private async Task LoadDropDown()
        {
            TicketEmail = Configuration["EmailNotifications:" + this.user.ClientID.ToUpper() + ":Ticket"];
            List<ListValue> lstValues = new List<ListValue>();
            Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());

            if (resp.MessageType == MessageType.Success)
            {
                TicketEmailMapList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.TICKETEMAILMAP).ToList();
                TicketTypeList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.TICKETTYPE).ToList();
                TicketStatusList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.TICKETSTATUS).ToList();
                // TicketStatus2List = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.ASSETSTATUS).ToList();
            }
            var respEmployees = (await EmployeeService.GetEmployees());
            if (respEmployees.MessageType == MessageType.Success)
            {
                Employees = respEmployees.Data;
            }
            Modules = (await RoleService.GetModules()).ToList();
        }
        public IEnumerable<ILT.IHR.DTO.User> UsersList { get; set; }  // Table APi Data
        protected async Task LoadUserList()
        {

            var reponses = (await UserService.GetUsers());
            if (reponses.MessageType == MessageType.Success)
            {
                UsersList = reponses.Data;
            }
            else
            {
                toastService.ShowError(ErrorMsg.ERRORMSG);
            }
        }
        private async Task GetDetails(int Id)
        {
            Response<ILT.IHR.DTO.Ticket> resp = new Response<ILT.IHR.DTO.Ticket>();
            resp = await TicketService.GetTicketByIdAsync(Id) as Response<ILT.IHR.DTO.Ticket>;
            if (resp.MessageType == MessageType.Success)
            {
                Ticket = resp.Data;
                var Roleshort = await sessionStorage.GetItemAsync<string>("RoleShort");
                if (Roleshort.ToUpper() == UserRole.ADMIN)
                {
                    isTicketTypeDescription = false;
                    lstAssignedList = Employees;
                }
                else if (Ticket.AssignedToID != null)
                {
                    isTicketTypeDescription = true;
                    lstAssignedList = Employees.Where(x => x.ManagerID == EmployeeID || x.EmployeeID == EmployeeID || x.EmployeeID == Ticket.AssignedToID);
                }
                else
                {
                    isTicketTypeDescription = false;
                    lstAssignedList = Employees.Where(x => x.ManagerID == EmployeeID || x.EmployeeID == EmployeeID);
                }
                if (Ticket.Status != null && Ticket.Status.ToUpper() == TicketStatusConstants.RESOLVED) {
                    isSaveVisable = true;
                    isAssignedDisable = true;
                } else
                {
                    isSaveVisable = false;
                    isAssignedDisable = false;
                }
                // onTicketStatus(Ticket.AssignedToID);
            }
            Title = "Edit";
            isfirstElementFocus = true;
            isCommentExist = false;
            isCommentDisable = false;
            ShowDialog = true;
            StateHasChanged();
        }

        public void Cancel()
        {
            TicketId = -1;
            ShowDialog = false;
            // UserUpdated.InvokeAsync(true);
            StateHasChanged();

        }
        
        public async void Show(int Id, int employeeId, int WizardDataId)
        {
            TicketId = Id;
            EmployeeID = employeeId;
            WizardDataID = WizardDataId;
            ResetDialog();
            if (TicketId != 0)
            {
                disabledvalue = true;
                isAssignedDisable = false;
                GetDetails(TicketId);
                lstEmployees = Employees;

            }
            else
            {
                isTicketTypeDescription = false;
                lstEmployees = Employees.Where(x => x.ManagerID == employeeId || x.EmployeeID == employeeId);
                Title = "Add";
                disabledvalue = false;
                Ticket.RequestedByID = employeeId;
                // Ticket.ResolvedDate = DateTime.Now;
                Ticket.CreatedDate = DateTime.Now;
                Ticket.StatusID = TicketStatusList.Find(x => x.ValueDesc.ToUpper() == TicketStatusConstants.NEW).ListValueID;
                isfirstElementFocus = true;
                isAssignedDisable = true;
                isCommentDisable = true;
                isSaveVisable = false;
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
            Ticket = new ILT.IHR.DTO.Ticket { };
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
        protected async Task onAssignedChange(ChangeEventArgs e)
        {
            if (Convert.ToInt32(e.Value) != 0 && e.Value != null)
            {
                Ticket.StatusID = TicketStatusList.Find(x => x.ValueDesc.ToUpper() == TicketStatusConstants.ASSIGNED).ListValueID;
                isTicketTypeDescription = true;
            }
            else
            {
                isTicketTypeDescription = false;
                Ticket.StatusID = TicketStatusList.Find(x => x.ValueDesc.ToUpper() == TicketStatusConstants.NEW).ListValueID;
            }
        }

        public async Task<EmailFields> prepareTicketMail()
        {
            EmailFields emailFields = new EmailFields();
            emailFields.EmailCCList = new List<string>();
            string RequesterEmail = await GetEmail(Ticket.RequestedByID);
            await GetAdminEmail(Ticket, emailFields);
            emailFields.EmailTo = TicketEmail;
            emailFields.EmailCCList.Add(RequesterEmail);
            emailFields.EmailCC = emailFields.EmailCCList.Distinct().Aggregate((x, y) => x + ";" + y);
            emailFields.EmailSubject = "IHR Ticket created for " + Ticket.RequestedBy;
            emailFields.EmailBody = "Ticket #" + Ticket.TicketID + " has been Created " +
                "<br/>" +
                "<ul style='margin-bottom: 0px;'><li>Type: " + Ticket.TicketType + "<br/>" +
                "</li><li>Requester: " + Ticket.RequestedBy +
                "</li><li>Title: " + Ticket.Title +
                "</li><li>Description: " + Ticket.Description +
                "</li><li>Submitted Date: " + FormatDate(Ticket.CreatedDate) +
                "</li>" +
                "<li>Comments: " + Ticket.Comment +
                "</li></ul>";
            return emailFields;
        }

        public async Task<EmailFields> prepareMail()
        {

            EmailFields emailFields = new EmailFields();
            emailFields.isMultipleEmail = true;
            emailFields.EmailCCList = new List<string>();
            string RequesterEmail = await GetEmail(Ticket.RequestedByID);
            await GetAdminEmail(Ticket, emailFields);
            if (Ticket.AssignedToID != null)
            {
                string ApproverEmail = await GetEmail(Ticket.AssignedToID);
                emailFields.EmailTo = ApproverEmail;
            }
            if (Ticket.AssignedToID != null && lstAssignedList != null)
            {
                Ticket.AssignedTo = lstAssignedList.First(x => x.EmployeeID == Ticket.AssignedToID).EmployeeName;
            }
            emailFields.EmailCCList.Add(RequesterEmail);
            emailFields.EmailCC = emailFields.EmailCCList.Distinct().Aggregate((x, y) => x + ";" + y);
            emailFields.EmailSubject = "IHR Ticket for " + Ticket.RequestedBy + " assigned to " + Ticket.AssignedTo;
            emailFields.EmailBody = "Ticket #" + Ticket.TicketID + " has been " + Ticket.Status + " To " + Ticket.AssignedTo +
                "<br/>" +
                "<ul style='margin-bottom: 0px;'><li>Type: " + Ticket.TicketType + "<br/>" +
                "</li><li>Requester: " + Ticket.RequestedBy +
                "</li><li>Assigned To: " + Ticket.AssignedTo +
                "</li><li>Title: " + Ticket.Title +
                "</li><li>Description: " + Ticket.Description +
                "</li><li>Submitted Date: " + FormatDate(Ticket.CreatedDate) +
                "</li>" +
                "<li>Comments: " + Ticket.Comment +
                "</li></ul>";
            return emailFields;
        }

        public async Task<EmailFields> prepareResolveMail()
        {
            // string uri = Configuration["EmailApprovalUrl:" + user.ClientID];
            EmailFields emailFields = new EmailFields();
            emailFields.EmailCCList = new List<string>();
            string RequesterEmail = await GetEmail(Ticket.RequestedByID);
            await GetAdminEmail(Ticket, emailFields);
            emailFields.EmailTo = RequesterEmail;
            emailFields.EmailCC = emailFields.EmailCCList.Distinct().Aggregate((x, y) => x + ";" + y);
            emailFields.EmailSubject = "IHR Ticket resolved for " + Ticket.RequestedBy;
            emailFields.EmailBody = "Ticket #" + Ticket.TicketID + " has been " + Ticket.Status +
                "<br/>" +
                "<ul style='margin-bottom: 0px;'><li>Type: " + Ticket.TicketType + "<br/>" +
                "</li><li>Requester: " + Ticket.RequestedBy +
                "</li><li>Assigned To: " + Ticket.AssignedTo +
                "</li><li>Title: " + Ticket.Title +
                "</li><li>Description: " + Ticket.Description +
                "</li><li>Submitted Date: " + FormatDate(Ticket.CreatedDate) +
                "</li><li>Resolved Date: " + FormatDate(Ticket.ResolvedDate) +
                "</li><li>Comments: " + Ticket.Comment +
                "</li>" +
                "</ul>";
            return emailFields;
        }
        protected async Task<string> GetEmail(int? EmployeeID)
        {
            DTO.Employee emp = Employees.ToList().Find(x => x.EmployeeID == EmployeeID);
            return !string.IsNullOrEmpty(emp.LoginEmail) ? emp.LoginEmail : emp.WorkEmail;
        }
        protected async Task GetAdminEmail(DTO.Ticket Ticket, EmailFields emailFields)
        {
            emailFields.EmailCCList = new List<string>();
            string AdminType = TicketEmailMapList.Where(x => x.Value == Ticket.TicketShort).Select(x => x.ValueDesc)?.FirstOrDefault();
            if (!string.IsNullOrEmpty(AdminType) && UsersList != null)
            {
                foreach (var user in UsersList)
                {
                    var exists = user.RoleShort.Trim().Split(',').Any(s => s.Trim().ToUpper() == AdminType);
                    if (exists)
                    {
                        emailFields.EmailCCList.Add(user.Email);
                    }
                }
            }
        }
        protected string FormatDate(DateTime? dateTime)
        {
            string formattedDate = "";
            if (dateTime.Value != null)
            {
                var date = dateTime.Value.ToString("dd MMM yyy HH:mm:ss") + " GMT";
                formattedDate = date;
            }
            return formattedDate;
        }

        protected async Task onCommentChange(ChangeEventArgs e)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(e.Value)))
            {
                isCommentExist = false;
            }
            else
            {
                isCommentExist = true;
            }
        }
        public void resolvedDateChange(ChangeEventArgs e)
        {
            
            if (e.Value != null)
            {
                isResolvedDate = false;
            } else
            {
                isResolvedDate = true;
            }
               
        }


    }
}
