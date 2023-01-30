using Blazored.SessionStorage;
using Blazored.Toast.Services;
using ILT.IHR.DTO;
using ILT.IHR.UI.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ILT.IHR.UI.Pages.ManageLeave
{
    public class AddEditLWPBase: ComponentBase
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
        protected DTO.User user { get; set; }
        [Parameter]
        public EventCallback<bool> UpdateLeaveList { get; set; }
        protected IEnumerable<DTO.Employee> EmployeeList { get; set; }
        protected IEnumerable<DTO.Employee> lstEmployees { get; set; }
        protected List<ListValue> VacationTypeList { get; set; }
        protected List<ListValue> StatusList { get; set; }
        public bool ShowDialog { get; set; }
        private int LeaveID { get; set; }
        private int EmployeeID { get; set; }
        protected bool isDisabled { get; set; }
        protected bool isCancel { get; set; }
        protected bool isShowComments { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string ClientID { get; set; }
        public decimal BalanceLeaves { get; set; }
        public bool isSaveButtonDisabled { get; set; } = false;

        protected DTO.Employee employeeDetails { get; set; }
        public Leave Leave = new Leave();
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
               var  vacList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.VACATIONTYPE).ToList();
                VacationTypeList = vacList.Where(x => x.Value.ToUpper() == ListTypeConstants.LWP.ToUpper()).ToList();
                StatusList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.LEAVEREQUESTSTATUS).ToList();
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
        public void Show(int Id, int employeeId)
        {
            
            LeaveID = Id;
            EmployeeID = employeeId;
            isDisabled = false;
            isShowComments = false;
            lstEmployees = EmployeeList.Where(x => x.EmployeeID == employeeId);
            ResetDialog();
            
             if (employeeId != 0)
            {
                if (VacationTypeList != null && VacationTypeList.Count == 1)
                {
                    Leave.LeaveTypeID = VacationTypeList[0].ListValueID;
                }
                Leave.EmployeeID = employeeId;
                Leave.RequesterID = employeeId;
                Leave.StartDate = DateTime.Now;
                Leave.EndDate = DateTime.Now;
                Leave.Title = "Unpaid Leave";
                Leave.Detail = "Unpaid Leave";
                Leave.StatusID = StatusList.Find(x => x.ValueDesc.ToUpper() == "Approved".ToUpper()).ListValueID;
                duration();
                isfirstElementFocus = true;
                ShowDialog = true;
                StateHasChanged();
            }
            else
            {
                toastService.ShowError("Leave request can't be created for non-Employees", "");
            }
        }
       
        protected async Task SaveLWP()
        {
            if (isSaveButtonDisabled)
                return;
            isSaveButtonDisabled = true;
            if (LeaveID == 0)
            {
                if (Leave.StartDate.Date <= Leave.EndDate.Date)
                {
                    Leave.CreatedBy = user.FirstName + " " + user.LastName;
                   // Leave.ApproverID = employeeDetails.ManagerID;
                    Leave.ApproverID = user.EmployeeID;
                    //Leave.Approver = employeeDetails.Manager;
                    Leave.Approver = user.FirstName + " " + user.LastName;
                  
                    Leave.LeaveType = VacationTypeList.Find(x => x.ListValueID == Leave.LeaveTypeID).ValueDesc;
                  
                    var result = await LeaveService.SaveLeave(Leave);
                    if (result.MessageType == MessageType.Success)
                    {
                        toastService.ShowSuccess("Unpaid Leave Save successfully", "");
                        await UpdateLeaveList.InvokeAsync(true);
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
            } else
                {
                    toastService.ShowSuccess("Error occured", "");
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

        public void startDateChange(ChangeEventArgs e)
        {
            Leave.EndDate = Convert.ToDateTime(e.Value);
            duration();
        }

        public void endDateChange(ChangeEventArgs e)
        {
            Leave.EndDate = Convert.ToDateTime(e.Value);
            duration();
        }  
        public void onCheckChange(ChangeEventArgs e)
        {
            Leave.IncludesHalfDay = Convert.ToBoolean(e.Value);
            duration();
            StateHasChanged();
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
