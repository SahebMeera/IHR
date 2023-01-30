using Blazored.SessionStorage;
using Blazored.Toast.Services;
using BlazorTable;
using ILT.IHR.DTO;
using ILT.IHR.UI.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ILT.IHR.UI.Pages.ManageLeave
{
    public class AccrueLeaveBase : ComponentBase
    {
        [Inject]
        public IConfiguration Configuration { get; set; }
        [Inject]
        public ISessionStorageService sessionStorage { get; set; }
        [Inject]
        public IToastService toastService { get; set; } //Service   
        [Inject]
        public ILeaveAccrualService leaveAccrualService { get; set; }
        [Inject]
        public ICountryService CountryService { get; set; }
        public LeaveAccrual LeaveAccrual = new LeaveAccrual();
        [Parameter]
        public EventCallback<bool> UpdateLeaveList { get; set; }
        public bool ShowDialog { get; set; }
        public ILT.IHR.DTO.User user;
        public List<Country> CountryList { get; set; }
        public List<LeaveMonth> LeaveMonthList { get; set; }
        protected DateTime accrualDate { get; set; }       
        //protected Decimal accureCount { get; set; }
        public int MonthID { get; set; }
        public string accrualMonth { get; set; }
        protected List<IDropDownList> lstMonth { get; set; }
        public bool isDisableAccrual { get; set; }
        protected DeleteConfirmation.ConfirmBase confirmBase { get; set; }
        public bool isSaveButtonDisabled { get; set; } = false;

        public class LeaveMonth
        {
            public int ID { get; set; }
            public DateTime Month { get; set; }
            public string MonthName { get; set; }
        }

        protected override async Task OnInitializedAsync()
        {
            user = await sessionStorage.GetItemAsync<ILT.IHR.DTO.User>("User");
            isDisableAccrual = true;
            //accureCount = Convert.ToDecimal(Configuration[SessionConstants.LEAVEACCRUAL]);
            Response<IEnumerable<Country>> response = (await CountryService.GetCountries());
            if (response.MessageType == MessageType.Success)
            {
                CountryList = response.Data.ToList();
            }            
        }
        protected async void onCountryChange(ChangeEventArgs e)
        {
            string Country = e.Value.ToString();
            await LoadLeaveAccrual(Country);
        }
        public List<LeaveAccrual> LeaveAccrualList { get; set; }
        protected async Task LoadLeaveAccrual(string Country)
        {
            var respLeaveAccrual = (await leaveAccrualService.GetLeaveAccrual(Country));
            if (respLeaveAccrual.MessageType == MessageType.Success)
            {
                LeaveAccrualList = respLeaveAccrual.Data.ToList();
                loadMonthDropDown();
            }
            else
            {
                toastService.ShowError("Error occured", "");
            }
        }

        protected void loadMonthDropDown()
        {
            LeaveMonthList = new List<LeaveMonth> { };
            int i = 0;
            // MonthID = 0;
            foreach (var accrual in LeaveAccrualList)
            {
                if (accrual.LeaveAccrualID == 0)
                {
                    i = i + 1;
                    var firstDayOfMonth = new DateTime(accrual.AccruedDate.Year, accrual.AccruedDate.Month, 1);
                    LeaveMonth month = new LeaveMonth();
                    month.ID = i;
                    month.Month = accrual.AccruedDate;
                    // month.Month = firstDayOfMonth;
                    month.MonthName = accrual.AccruedDate.Year.ToString() + " - " + firstDayOfMonth.ToString("MMMM");
                    LeaveMonthList.Add(month);
                }
            }
            SetGridMonthList();
            StateHasChanged();
        }

        protected void onMonthChange(ChangeEventArgs e)
        {
            MonthID = Convert.ToInt32(e.Value);

            if (MonthID != 0)
            {
                accrualDate = LeaveMonthList.Find(x => x.ID == MonthID).Month;
                accrualMonth = accrualDate.ToString("MMMM") + " " + accrualDate.Year.ToString();
                
                isDisableAccrual = (LeaveAccrualList.Find(x => x.AccruedDate == accrualDate)) != null ? false : true;
                if(isDisableAccrual == false)
                {
                    if (LeaveAccrual.AccruedValue == 0)
                    {

                        isDisableAccrual = true;
                    }
                } 
                
               
            }
            else
            {
                isDisableAccrual = true;
            }
        }

        public bool isAccruvalValue { get; set; } = false;
        public void onAccrualValueChange()
        {
            var value = Convert.ToInt32(LeaveAccrual.AccruedValue);

            if (value < 1 )
            {
                isAccruvalValue = true;
                isDisableAccrual = true;
                
            }
            else if(MonthID == 0)
            {
                isAccruvalValue = true;
                isDisableAccrual = true;
                isMonthSelected = true;
               
            } else
            {
                isAccruvalValue = false;
                isDisableAccrual = false;
                isMonthSelected = false;
            }
        }

        public bool isMonthSelected { get; set; } = false;
        protected async Task saveLeaveAccrual()
        {
            isMonthSelected = false;
            isDisableAccrual = false;
            if (MonthID != 0)
            {
                confirmBase.Show();
            } else
            {
                isMonthSelected = true;
                isDisableAccrual = true;
            }
                       
        }

        protected async Task ConfirmLeaveAccrual_Click(bool isLeaveAccure)
        {
            if (isSaveButtonDisabled)
                return;
            isSaveButtonDisabled = true;
            if (isLeaveAccure)
            {
                //LeaveAccrual.AccruedValue = accureCount;
                LeaveAccrual.AccruedDate = accrualDate;
                LeaveAccrual.CreatedBy = user.FirstName + ' ' + user.LastName;

                var resp = (await leaveAccrualService.SaveLeaveAccrual(LeaveAccrual));
                if (resp.MessageType == MessageType.Success)
                {
                    toastService.ShowSuccess("Leave Accrued");
                    isDisableAccrual = true;
                    await UpdateLeaveList.InvokeAsync(true);
                    await LoadLeaveAccrual(LeaveAccrual.Country);
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

        protected void SetGridMonthList()
        {
            lstMonth = new List<IDropDownList> { };
            IDropDownList ListItem = new IDropDownList();
            lstMonth = (from lookupItem in LeaveMonthList
                        select new IDropDownList { ID = Convert.ToInt32(lookupItem.ID), Value = lookupItem.MonthName }).ToList();
        }

        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }

        public void Show()
        {           
            ResetDialog();
            ShowDialog = true;
            isfirstElementFocus = true;
            isAccruvalValue = false;
            StateHasChanged();
        }
       

        private void ResetDialog()
        {
            LeaveAccrual = new LeaveAccrual { };
            MonthID = 0;
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
