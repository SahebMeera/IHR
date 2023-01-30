using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.UI.Service;
using ILT.IHR.DTO;
using Blazored.Toast.Services;
using Blazored.SessionStorage;
using Microsoft.JSInterop;

namespace ILT.IHR.UI.Pages.Employee.EmployeeAssignment
{
    public class AddEditEmployeeAssignmentBase: ComponentBase
    {
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service    
        [Inject]
        public ICompanyService CompanyService { get; set; } //Service
        [Inject]
        public ICountryService CountryService { get; set; } //Service
        [Inject]
        public IEmployeeService EmployeeService { get; set; }
        [Inject]
        public IEndClientService EndClientService { get; set; } //Service
        [Inject]
        public IAssignmentService AssignmentService { get; set; } //Service
        public bool isSaveButtonDisabled { get; set; } = false;
        public bool isSaveButtonDisabledForRate { get; set; } = false;
        protected string Title = "Add";
        public bool ShowDialog { get; set; }
        public bool ShowChildDialog { get; set; }
        public string subClient { get; set; }
        private int AssignmentId { get; set; }
        private int AssignmentRateId { get; set; }
        [Parameter]
        public List<ListValue> PaymentTypeList { get; set; } 
        public List<ListValue> WeekEndingDayList { get; set; }
        public IEnumerable<ILT.IHR.DTO.Company> CompanyList { get; set; }  // Table APi Data
        public List<ILT.IHR.DTO.Company> ClientList { get; set; }  // Table APi Data
        public List<ILT.IHR.DTO.Company> VendorList { get; set; }  // Table APi Data
        [Inject]
        public IUserService UserService { get; set; } //Service
        public IEnumerable<ILT.IHR.DTO.User> UsersList { get; set; }  // Table APi Data
        //public IEnumerable<ILT.IHR.DTO.User> timesheetApproverList { get; set; }  // Table APi Data
        public List<ListValue> TitleList { get; set; }
        public List<EndClient> endClientList { get; set; }
        public List<Country> CountryList { get; set; }
        public List<State> StateList { get; set; }
        public string country { get; set; }
        public int countryId { get; set; }
        public List<SubClient> subClients { get; set; }
        public string selectedClient { get; set; }
        public string selectedEndClient { get; set; }
        public class SubClient
        {
            public string Text { get; set; }

            public bool isValidSubClient { get; set; } = false;
        }
        [Parameter]
        public EventCallback<bool> UpdateAssignments { get; set; }

        public Assignment assignment = new Assignment();
        public AssignmentRate assignmentRate = new AssignmentRate();
        [Inject]
        public ILookupService LookupService { get; set; } //Service
        [Parameter]
        public int EmployeeId { get; set; }
        [Parameter]
        public string EmployeeName { get; set; }
        public ILT.IHR.DTO.User user;
        public List<RolePermission> RolePermissions;
        public RolePermission AssigmentsRolePermission;
        public string ErrorMessage;
        protected bool isShow { get; set; }

       
        protected override async Task OnInitializedAsync()
        {

            subClients = new List<SubClient>();
            subClient = "";
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            AssigmentsRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.ASSIGNMENT);

            user = await sessionStorage.GetItemAsync<ILT.IHR.DTO.User>("User");
            await LoadDropDown();
        }
        [Inject] IJSRuntime JSRuntime { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            JSRuntime.InvokeVoidAsync("JSHelpers.setFocusByCSSClass");
        }

        public void Show(int Id)
        {
            AssignmentId = Id;
            ResetDialog();
            if (AssignmentId != 0)
            {
                isShow = AssigmentsRolePermission.Update;
                GetDetails(AssignmentId);
            }
            else
            {
                isShow = AssigmentsRolePermission.Add;
                Title = "Add";
                subClients = new List<SubClient>();
                selectedClient = "";
                selectedEndClient = "";
                ShowChildDialog = false;
                assignment.StartDate = DateTime.Now;
                loadEmployeeData(EmployeeId);
                ShowDialog = true;
                StateHasChanged();
            }
        }
        public void ShowChild(int assignmentId, int assignmentRateId)
        {
            AssignmentId = assignmentId;
            AssignmentRateId = assignmentRateId;
            ResetDialog();
            GetAssignmentDetails();
            GetAssignmentRates(AssignmentId);
            if (AssignmentRateId != 0)
            {
                isShow = AssigmentsRolePermission.Update;
                GetAssignmentRateDetails(AssignmentRateId);
            }
            else
            {
                isShow = AssigmentsRolePermission.Add;
                Title = "Add";
                ShowChildDialog = true;
                assignmentRate.StartDate = DateTime.Now;
                ShowDialog = false;
                StateHasChanged();
            }
        }
        protected ILT.IHR.DTO.Employee Employee { get; set; }
        protected async Task loadEmployeeData(int employeeId)
        {
            if (employeeId != 0)
            {
                Response<ILT.IHR.DTO.Employee> resp = new Response<ILT.IHR.DTO.Employee>();
                resp = await EmployeeService.GetEmployeeByIdAsync(employeeId) as Response<ILT.IHR.DTO.Employee>;
                if (resp.MessageType == MessageType.Success)
                {
                    if (resp.Data.EmployeeID != 0)
                    {
                        Employee = resp.Data;
                        assignment.Country = resp.Data.Country;
                        if (assignment.Country != null)
                        {
                            await GetStates(assignment.Country);
                        }
                        else
                        {
                            StateList = new List<State>();
                        }
                    }
                }
                StateHasChanged();
            }
        }


        private async Task LoadDropDown()
        {
            List<ListValue> lstValues = new List<ListValue>();
            Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());
            if (resp.MessageType == MessageType.Success)
            {
                WeekEndingDayList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.TIMESHEETTYPE).ToList();
                TitleList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.TITLE).ToList();
                PaymentTypeList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.PAYMENTTYPE).ToList();
            }
            Response<IEnumerable<Country>> response = (await CountryService.GetCountries());
            if (response.MessageType == MessageType.Success)
            {
                CountryList = response.Data.ToList();
            }
            var reponses = (await CompanyService.GetCompanies());
            if (reponses.MessageType == MessageType.Success)
            {
                CompanyList = reponses.Data;
                VendorList = CompanyList.Where(x => x.CompanyType.ToUpper() == "VENDOR" || x.CompanyType.ToUpper() == "SELF" || x.CompanyType.ToUpper() == "CLIENT/VENDOR").ToList();
                ClientList = CompanyList.Where(x => x.CompanyType.ToUpper() == "CLIENT" || x.CompanyType.ToUpper() == "SELF" || x.CompanyType.ToUpper() == "CLIENT/VENDOR").ToList();
            }
            var userReponse = (await UserService.GetUsers());
            if (userReponse.MessageType == MessageType.Success)
            {
                UsersList = userReponse.Data;
            }
            else
            {
                toastService.ShowError(ErrorMsg.ERRORMSG);
            }
            var endClientReponse = (await EndClientService.GetEndClients());
            if (endClientReponse.MessageType == MessageType.Success)
            {
                endClientList = endClientReponse.Data.ToList();
            }
            else
            {
                toastService.ShowError(ErrorMsg.ERRORMSG);
            }
        }
        protected async Task onChangeClient(ChangeEventArgs e)
        {
            int client = Convert.ToInt32(e.Value);
            subClients = new List<SubClient>();
            selectedClient = "";
            selectedEndClient = "";
            //assignment.TimesheetApproverID = null;
            if (client != 0 )
            {
                var endClient = ClientList.Find(x => x.IsEndClient == true && x.CompanyID == client);
                
                if (endClient != null)
                {
                    selectedClient = endClient.Name;
                    assignment.Client = selectedClient;
                    var isCheck = endClientList.Find(x => x.Name.ToUpper() == endClient.Name.ToUpper());
                    selectedEndClient = isCheck.Name;
                    if (isCheck != null)
                    {
                        assignment.EndClientID = isCheck.EndClientID;
                        assignment.SubClient = "";
                    }
                } else
                {
                    var clientObj = ClientList.Find(x => x.CompanyID == client);
                    selectedClient = clientObj.Name;
                    assignment.Client = selectedClient;
                    assignment.EndClientID = 0;
                    assignment.SubClient = "";
                }
              

                //await GetTimeSheetApproverList(client);
            } else {
                //timesheetApproverList = new List<DTO.User> { };
              }
        }

        protected async Task onChangeEndClient(ChangeEventArgs e)
        {
            int endclient = Convert.ToInt32(e.Value);
            if (endclient != 0)
            {
                    var isCheck = endClientList.Find(x => x.EndClientID == endclient);
                    selectedEndClient = isCheck.Name;
            }
        }

        private async Task GetTimeSheetApproverList(int client)
        {
            //timesheetApproverList = UsersList.Where(x => x.CompanyID == client || x.RoleShort.ToUpper().Contains(UserRole.FINADMIN.ToUpper()) || x.RoleShort.ToUpper().Contains(UserRole.FINUSER.ToUpper())).ToList();
        }
        protected async Task onChangeCountry(ChangeEventArgs e)
        {

            country = Convert.ToString(e.Value);
            await GetStates(country);
        }
        private async Task GetStates(string country)
        {
            countryId = CountryList.Find(x => x.CountryDesc.ToUpper() == country.ToUpper()).CountryID;
            if (countryId != 0 && countryId != null)
            {
                var resps = (await CountryService.GetCountryByIdAsync(countryId)); if (resps.MessageType == MessageType.Success)
                {
                    StateList = resps.Data.States;
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
            }
        }
        public List<AssignmentRate> EmployeeAssignmentRates { get; set; }  // Table APi Data
        private async Task GetDetails(int Id)
        {
            selectedClient = "";
            selectedEndClient = "";
            subClients = new List<SubClient>();
            EmployeeAssignmentRates = new List<AssignmentRate> { };
            Response<Assignment> resp = new Response<Assignment>();
            resp = await AssignmentService.GetAssignmentById(Id) as Response<Assignment>;
            if (resp.MessageType == MessageType.Success)
            assignment = resp.Data;
            EmployeeAssignmentRates = resp.Data.AssignmentRates.ToList();
            var client = ClientList != null ? ClientList.Find(x => x.CompanyID == assignment.ClientID) : null;
            selectedClient = client != null ? client.Name : "";
            var endClient = endClientList != null ? endClientList.Find(x => x.EndClientID == assignment.EndClientID) : null;
            selectedEndClient = endClient != null ? endClient.Name : "";
            if (!string.IsNullOrEmpty(assignment.SubClient))
            {
                var subClientList = assignment.SubClient.Split(',');
                subClients = (from SubClient in subClientList
                              select new SubClient { Text = SubClient, isValidSubClient = false }).ToList();
            }
            //if (assignment.ClientID != null)
            //{
            //    await GetTimeSheetApproverList(Convert.ToInt32(assignment.ClientID));
            //}
            //else
            //{
            //    //timesheetApproverList = new List<DTO.User> { };
            //}
            if (assignment.Country != null)
            {
                await GetStates(assignment.Country);
            }
            else
            {
                StateList = new List<State>();
            }
            Title = "Edit";
            ShowDialog = true;
            ShowChildDialog = false;
            StateHasChanged();
        }
        private async Task GetAssignmentRateDetails(int Id)
        {
            Response<AssignmentRate> resp = new Response<AssignmentRate>();
            resp = await AssignmentService.GetAssignmentRateById(Id) as Response<AssignmentRate>;
            if(resp.MessageType == MessageType.Success)
            assignmentRate = resp.Data;
            Title = "Edit";
            ShowDialog = false;
            ShowChildDialog = true;
            StateHasChanged();
        }
        public bool AEndDateCheck { get; set; }
        public void onchangeAssignmentEndDate(ChangeEventArgs e)
        {
            AEndDateCheck = false;
            ErrorMessage = "";
            if (Employee != null)
            {
                if (!string.IsNullOrEmpty(e.Value.ToString()))
                {
                    var Etd = Employee.TermDate;
                    var Aed = Convert.ToDateTime(e.Value);
                    if(Aed != null && Aed >= Etd)
                    {
                        AEndDateCheck = true;
                        ErrorMessage = "Employee has active Assignments. Please terminate Assignments before terminating Employee.";
                        assignment.EndDate = null;
                    } else
                    {
                        AEndDateCheck = false;
                        ErrorMessage = "";
                    }
                      //  isSaveButtonDisabled = true;
                }
                else
                {
                    AEndDateCheck = false;
                    ErrorMessage = "";
                }
            }
            StateHasChanged();
        }

        protected void onInvalidSubClient()
        {
            isCheckSubClient(); 
        }

        protected bool isCheckSubClient()
        {
            bool isInvalid = false;
            if (subClients != null && subClients.Count != 0)
            {
                subClients.ForEach(x =>
                {
                    if (string.IsNullOrEmpty(x.Text))
                    {
                        x.isValidSubClient = true;
                        isInvalid = true;
                    }
                });
            }
            return isInvalid;
        }

        protected async Task SaveAssignment()
        {
            ErrorMessage = "";
            assignment.EmployeeName = EmployeeName;
            if (!isCheckSubClient())
            {
                if (subClients != null && subClients.Count != 0)
                {
                    assignment.SubClient = string.Join(",", subClients.Select(x => x.Text));
                }
                if (isSaveButtonDisabled)
                    return;
                isSaveButtonDisabled = true;
                if (assignment.EndDate != null && AEndDateCheck == false)
                {
                    ErrorMessage = "";
                    var EmployeeAssignmentRate = EmployeeAssignmentRates.Find(x => x.EndDate == null);
                    if (EmployeeAssignmentRate != null)
                    {
                        ErrorMessage = "Assignment has active AssignmentRates. Please terminate AssignmentRates before terminating Assignment.";
                    }
                    else
                    {
                        var IscheckAssignmentRate = EmployeeAssignmentRates.Find(x =>  x.EndDate > assignment.EndDate);
                        if (IscheckAssignmentRate != null)
                        {
                            ErrorMessage = "End date must be greater than or equal to end dates of AssignmentRate.";
                            
                        } else
                        {
                            AssignmentSave();
                        }
                    }
                }
                else
                {
                    AssignmentSave();
                }
                isSaveButtonDisabled = false;
            }
        }

        protected async Task AssignmentSave()
        {
            if (assignment.EndDate == null || assignment.StartDate <= assignment.EndDate)
            {
                if (AssignmentId == 0)
                {
                    assignment.CreatedBy = user.FirstName + " " + user.LastName;
                    assignment.EmployeeID = EmployeeId;
                    var result = await AssignmentService.SaveAssignment(assignment);
                    if (result.MessageType == MessageType.Success)
                    {
                        toastService.ShowSuccess("Assignment saved successfully", "");
                        UpdateAssignments.InvokeAsync(true);
                        Cancel();
                    }
                    else
                    {
                        toastService.ShowError(ErrorMsg.ERRORMSG);
                        Cancel();
                    }
                }
                else if (AssignmentId != 0)
                {
                    assignment.ModifiedBy = user.FirstName + " " + user.LastName;
                    var result = await AssignmentService.UpdateAssignment(AssignmentId, assignment);
                    if (result.MessageType == MessageType.Success)
                    {
                        toastService.ShowSuccess("Assignment saved successfully", "");
                        UpdateAssignments.InvokeAsync(true);
                        Cancel();
                    }
                    else
                    {
                        toastService.ShowError(ErrorMsg.ERRORMSG);
                    }
                }
            } else
            {
                ErrorMessage = "End date must be greater than start date";
               // toastService.ShowError("End date must be greater than start date", "");
            }
        }
            private async void GetAssignmentRates(int AssignmentID)
        {
            EmployeeAssignmentRates = new List<AssignmentRate> { };
            Response<Assignment> resp = new Response<Assignment>();
            resp = (await AssignmentService.GetAssignmentById(AssignmentID));
            if (resp.MessageType == MessageType.Success)
            {
                EmployeeAssignmentRates = resp.Data.AssignmentRates.ToList();
            }
            StateHasChanged();
        }
        private async Task GetAssignmentDetails()
        {
            selectedClient = "";
            selectedEndClient = "";
            subClients = new List<SubClient>();
            //EmployeeAssignmentRates = new List<AssignmentRate> { };
            Response<Assignment> resp = new Response<Assignment>();
            if(AssignmentId != 0)
            {
                resp = await AssignmentService.GetAssignmentById(AssignmentId) as Response<Assignment>;
                if (resp.MessageType == MessageType.Success)
                {
                    assignment = resp.Data;
                    //EmployeeAssignmentRates = resp.Data.AssignmentRates.ToList();
                }
            }
        }
        protected async Task AssignmentRateValidation()
        {
            EmployeeAssignmentRates.ForEach(x =>
            {
                  x.EndDate = Convert.ToDateTime("9999-12-31");
            });
            ErrorMessage = "";
            if (isSaveButtonDisabledForRate)
                return;
            isSaveButtonDisabledForRate = true;
            if (assignment.EndDate != null && assignment.StartDate.Date <= assignmentRate.StartDate.Date)
            {
                if (assignmentRate.EndDate == null && assignment.EndDate >= assignmentRate.StartDate)
                {
                    isCheckAssignmentRateValid();
                } else
                {
                    if (assignmentRate.EndDate != null && assignmentRate.EndDate <= assignment.EndDate)
                    {
                        isCheckAssignmentRateValid();
                    }
                    else
                    {
                        if(assignmentRate.EndDate != null)
                        {
                            ErrorMessage = "End date must be less than or equal to assignment end date";
                        }
                        else
                        {
                            ErrorMessage = "Start date must be less than or equal to assignment end date";
                        }
                        // toastService.ShowError("AssignmentRate end date must be less than or equal to assignment end date", "");
                    }
                   
                }
                
            }
            else
            {
                if (assignment.EndDate == null && assignment.StartDate.Date <= assignmentRate.StartDate.Date)
                { 

                    isCheckAssignmentRateValid();
                } else
                {
                    ErrorMessage = "Start date must be greater than or equal to assignment start date";
                    //toastService.ShowError("AssignmentRate start date must be greater than or equal to assignment start date", "");
                }
            }
            isSaveButtonDisabledForRate = false;
        }
        protected async Task isCheckAssignmentRateValid()
        {
            if (EmployeeAssignmentRates == null || EmployeeAssignmentRates.Count == 0)
            {
                await SaveAssignmentRate();
            }
            else
            {
                if (assignmentRate.EndDate == null || assignmentRate.StartDate <= assignmentRate.EndDate)
                {
                    var currentRec = EmployeeAssignmentRates.Where(x => x.AssignmentRateID != AssignmentRateId).FirstOrDefault();
                    if (currentRec == null || currentRec.EndDate != null)
                    {
                        bool assignmentRateAlreadyexist = EmployeeAssignmentRates.FindIndex(x => x.AssignmentRateID != AssignmentRateId && x.EndDate != null && ((x.StartDate <= assignmentRate.StartDate && x.EndDate == null) ||
                                                    ((x.StartDate <= assignmentRate.EndDate || assignmentRate.EndDate == null) && assignmentRate.StartDate <= x.EndDate) ||
                                                    (x.StartDate >= assignmentRate.StartDate && (x.EndDate <= assignmentRate.EndDate || assignmentRate.EndDate == null)) ||
                                                    (x.StartDate >= assignmentRate.StartDate && x.EndDate == null) ||
                                                    (x.StartDate <= assignmentRate.StartDate && x.EndDate >= assignmentRate.StartDate))) > -1;
                        if (assignmentRateAlreadyexist)
                        {
                            ErrorMessage = "AssignmentRate already exists for this period";
                            if (assignmentRate.EndDate == Convert.ToDateTime("9999-12-31"))
                            {
                                assignmentRate.EndDate = null;
                            }
                        }
                        else
                        {
                            await SaveAssignmentRate();
                        }
                    }
                    else
                    {
                        await SaveAssignmentRate();
                    }
                }
                else
                {
                    ErrorMessage = "End date must be greater than start date";
                    //toastService.ShowError("End date must be greater than start date", "");
                }
            }
        }

            protected async Task SaveAssignmentRate()
        {
            if (AssignmentRateId == 0)
            {
                assignmentRate.AssignmentID = AssignmentId;
                assignmentRate.CreatedBy = user.FirstName + " " + user.LastName;
                var result = await AssignmentService.SaveAssignmentRate(assignmentRate);
                if (result.MessageType == MessageType.Success)
                {
                    toastService.ShowSuccess("Assignment Rate saved successfully", "");
                    UpdateAssignments.InvokeAsync(true);
                    Cancel();
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                    Cancel();
                }

            }
            else if (AssignmentId != 0)
            {
                assignmentRate.ModifiedBy = user.FirstName + " " + user.LastName;
                var result = await AssignmentService.UpdateAssignmentRate(AssignmentRateId, assignmentRate);
                if (result.MessageType == MessageType.Success)
                {
                    toastService.ShowSuccess("Assignment Rate saved successfully", "");
                    UpdateAssignments.InvokeAsync(true);
                    Cancel();
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
            }
            isSaveButtonDisabledForRate = false;
        }
        public void startDateChange(ChangeEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Value.ToString()))
            {
                assignmentRate.StartDate = Convert.ToDateTime(e.Value);
                onDateChange();
            }
        }
        public void endDateChange(ChangeEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Value.ToString()))
                assignmentRate.EndDate = Convert.ToDateTime(e.Value);
               onDateChange();
        }
        protected async Task onDateChange()
        {
            if (assignmentRate.EndDate != null)
            {
                if(assignmentRate.StartDate >= assignmentRate.EndDate)
                {
                    toastService.ShowError("End date must be greater than start date", "");
                }
            }
        }
        public void Cancel()
        {
            ShowDialog = false;
            ShowChildDialog = false;
            StateHasChanged();
        }
        public void Close()
        {
            ShowDialog = false;
            ShowChildDialog = false;
            StateHasChanged();
        }

        private void ResetDialog()
        {
            ErrorMessage = string.Empty;
            assignment = new Assignment { };
            assignmentRate = new AssignmentRate { };
        }
        protected void addSubClient(bool isInsertAtTop,SubClient subClient)
        {
            SubClient sub = new SubClient();
            
            if (isInsertAtTop)
            {
                if (selectedClient != "" && selectedEndClient != "")
                {
                    //if(selectedClient.ToUpper() != selectedEndClient.ToUpper())
                    //{
                        subClients.Insert(0, sub);
                    //}
                   
                }
            }else
            {
                var index = subClients.FindIndex(x => x.Text == subClient.Text);
                subClients.Insert(index + 1, sub);
            }
           
        }
        protected void removeSubClient(SubClient subClient)
        {
            var index = subClients.FindIndex(x => x.Text == subClient.Text);
            subClients.RemoveAt(index);
        }

        protected bool isEndClient()
        {
            var client = ClientList.Find(x => x.IsEndClient == true && x.CompanyID == assignment.ClientID);
            if (client != null)
            {
                var isCheck = endClientList.Find(x => x.Name.ToUpper() == selectedEndClient.ToUpper() && x.CompanyID != 0 && x.CompanyID == client.CompanyID);
                if (isCheck != null)
                {
                    return false;
                } else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        protected async Task onChangeVendor(ChangeEventArgs e)
        {
            if (Convert.ToInt32(e.Value) != 0 && e.Value != null && VendorList != null)
            {
                var vendor = Convert.ToInt32(e.Value);
                assignment.Vendor = VendorList.Find(x => x.CompanyID == vendor).Name;
            }
        }

        protected async Task onChangePayment(ChangeEventArgs e)
        {
            if (Convert.ToInt32(e.Value) != 0 && e.Value != null && PaymentTypeList != null)
            {
                var paymentType = Convert.ToInt32(e.Value);
                 assignment.PaymentType = PaymentTypeList.Find(x => x.ListValueID == paymentType).ValueDesc;
            }
        }

        protected async Task onChangeTimeSheetType(ChangeEventArgs e)
        {
            if (Convert.ToInt32(e.Value) != 0 && e.Value != null && WeekEndingDayList != null)
            {
                var timeSheetType = Convert.ToInt32(e.Value);
                assignment.TimeSheetType = WeekEndingDayList.Find(x => x.ListValueID == timeSheetType).ValueDesc;
            }
        }
    }
}
