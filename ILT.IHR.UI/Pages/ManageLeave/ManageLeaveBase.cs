using Blazored.SessionStorage;
using Blazored.Toast.Services;
using BlazorTable;
using ILT.IHR.DTO;
using ILT.IHR.UI.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ILT.IHR.UI.Pages.ManageLeave
{
    public class ManageLeaveBase: ComponentBase

    {
        [Inject]
        public ISessionStorageService sessionStorage { get; set; }
        [Inject]
        protected ILeaveBalanceService LeaveBalanceService { get; set; }
        [Inject]
        public ICountryService CountryService { get; set; } //Service
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public IConfiguration Configuration { get; set; }
        [Inject]
        public ILeaveAccrualService leaveAccrualService { get; set; }
        public List<LeaveBalance> LeaveBalanceList { get; set; }  // Table APi Data
        public List<LeaveBalance> lstManageLeave { get; set; }  // Table APi Data
        protected LeaveBalance selected { get; set; }
        public List<IRowActions> RowActions { get; set; } //Row Actions
        public IEnumerable<IHeaderActions> HeaderAction { get; set; } //Header Actions
        public AddEditManageLeaveBase AddEditManageLeaveModal { get; set; }
        public AddEditLWPBase AddEditLWPModal { get; set; }
        public AccrueLeaveBase AccrueLeaveModal { get; set; }
        public CalculateLWPBase CalculateLWPModal { get; set; }

        public List<RolePermission> RolePermissions;
        public RolePermission LeaveRolePermission;
        protected int yearId { get; set; }
        protected List<IDropDownList> lstYear { get; set; }
        protected List<IDropDownList> lstMonth { get; set; }
        public List<LeaveYear> LeaveYearList { get; set; }
        public List<LeaveMonth> LeaveMonthList { get; set; }
        public int DefaultTypeID { get; set; }
        public List<Country> CountryList { get; set; }
        public List<IDropDownList> lstCountry { get; set; } //Drop Down Api Data

        public int MonthID { get; set; }
        public int DefaultPageSize { get; set; }
        public bool isDisableAccrual { get; set; }
        protected DateTime accrualDate { get; set; }
        protected DTO.User user { get; set; }
        protected Decimal accureCount { get; set; }
        //protected DeleteConfirmation.ConfirmBase confirmBase { get; set; }
        public string accrualMonth { get; set; }
        private int EmployeeID { get; set; }

        public class LeaveYear
        {
            public string year { get; set; }
            public string text { get; set; }
        }
        public class LeaveMonth
        {
            public int ID { get; set; }
            public DateTime Month { get; set; }
            public string MonthName { get; set; }
        }

     
        protected async override Task OnInitializedAsync()
        {
            user = await sessionStorage.GetItemAsync<DTO.User>("User");
            EmployeeID = Convert.ToInt32(user.EmployeeID);
            lstCountry = new List<IDropDownList>();
            isDisableAccrual = true;
            DefaultPageSize = Convert.ToInt32(Configuration[ConfigPageSize.PAGESIZE]);
            accureCount = Convert.ToDecimal(Configuration[SessionConstants.LEAVEACCRUAL]);
            LeaveBalanceList = new List<LeaveBalance> { };
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            Response<IEnumerable<Country>> response = (await CountryService.GetCountries());
            if (response.MessageType == MessageType.Success)
            {
                CountryList = response.Data.ToList();
                setCountryList();
               
            }
            LoadTableConfig();
            loadYearDropdown();
            yearId = Convert.ToInt32(LeaveYearList.Find(x => x.text.ToLower() == "Current Year".ToLower()).year);
            await LoadLeaveBalance();
            await LoadLeaveAccrual();
           
            StateHasChanged();
        }
        private void LoadTableConfig()
        {
            LeaveRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.MANAGELEAVE);
            IRowActions m1 = new IRowActions
            {
                IconClass = "oi oi-pencil",
                ActionMethod = Edit,
                ButtonClass = "btn-primary",
                IsShow = LeaveRolePermission.Update
            };
            IRowActions m2 = new IRowActions
            {
                IconClass = "far fa-calendar-plus",
                ActionMethod = AddLWP,
                ButtonClass = "btn-primary",
                //IsShow = LeaveRolePermission.Update
            };

            RowActions = new List<IRowActions> { m1, m2 };
            IHeaderActions accrueLeave = new IHeaderActions
            {
                IconClass = "oi oi-plus",
                ButtonClass = "btn-primary btn-height",
                ActionMethod = OnLeaveAccrue,
                ActionText = "ACCRUE LEAVE",
                //IsDisabled = isDisableAccrual
            };
            IHeaderActions calculateLWPLeave = new IHeaderActions
            {
                IconClass = "oi oi-plus",
                ButtonClass = "btn-primary btn-height",
                ActionMethod = OnCalculateLWP,
                ActionText = "PENDING LWP",
                //IsDisabled = isDisableAccrual
            };
            HeaderAction = new List<IHeaderActions> { accrueLeave, calculateLWPLeave };
            lstManageLeave = LeaveBalanceList;
          //  StateHasChanged();
        }
        protected async Task LoadLeaveBalance()
        {
            var respLeaveBalance = (await LeaveBalanceService.GetLeaveBalance(0));
            if (respLeaveBalance.MessageType == MessageType.Success)
            {
                LeaveBalanceList = respLeaveBalance.Data.ToList();
                loadLeaveBalanceList();
                //lstManageLeave = LeaveBalanceList;
                //if (yearId != 0 ) {
                //    onYearChange(yearId);
                //} 
            }
            else
            {
                LeaveBalanceList = new List<LeaveBalance> { };
                lstManageLeave = LeaveBalanceList;
            }
            StateHasChanged();
        }
        public void loadLeaveBalanceList()
        {
            if (yearId != 0 && country != "All")
            {
                lstManageLeave = LeaveBalanceList.Where(x => x.LeaveYear == yearId && x.Country == country).ToList();
            }
            else if (yearId == 0 && country != "All")
            {
                lstManageLeave = LeaveBalanceList.Where(x => x.Country == country).ToList();
            }
            else if (yearId != 0 && country == "All")
            {
                 lstManageLeave = LeaveBalanceList.Where(x => x.LeaveYear == yearId).ToList();
            }
            else
            {
                lstManageLeave = LeaveBalanceList;
            }
            StateHasChanged();
        }
        protected void OnLeaveAccrue()
        {
            //confirmBase.Show();
            AccrueLeaveModal.Show();
        }

        protected void OnCalculateLWP()
        {
            CalculateLWPModal.Show();
        }

        protected async Task saveLeaveAccrual(LeaveAccrual leaveAccrual)
        {

            var resp = (await leaveAccrualService.SaveLeaveAccrual(leaveAccrual));
            if(resp.MessageType == MessageType.Success)
            {
                toastService.ShowSuccess("Leave Accrued");
                MonthID = 0;
                isDisableAccrual = true;

                LoadTableConfig();
                LoadLeaveAccrual();
                await LoadLeaveBalance();
            }
        }
        public string country { get; set; }
        public void OnCountryChange(int countryID)
        {
            DefaultTypeID = countryID;
            country = lstCountry.Find(x => x.ID == countryID).Value;
            LoadLeaveBalance();
        }
        protected void onYearChange(int Year)
        {
            yearId = Year;
            //if (Year != 0)
            //    lstManageLeave = LeaveBalanceList.Where(x => x.LeaveYear == Year).ToList();
            //else
            //    lstManageLeave = LeaveBalanceList;
            //StateHasChanged();
            LoadLeaveBalance();
        }
        private void Edit()
        {
            AddEditManageLeaveModal.Show(selected.LeaveBalanceID);
        } 
        private void AddLWP()
        {
            AddEditLWPModal.Show(0, Convert.ToInt32(selected.EmployeeID));
        }

        public void RowClick(LeaveBalance data)
        {
            selected = data;
            StateHasChanged();
        }
        
        public void loadYearDropdown()
        {
            LeaveYear m1 = new LeaveYear
            {
               year = DateTime.Now.Year.ToString(),
               text = "Current Year"
            };
            LeaveYear m2 = new LeaveYear
            {
                year = (DateTime.Now.Year - 1).ToString(),
                text = "Previous Year"
            };
            LeaveYearList = new List<LeaveYear> { m1, m2 };
            SetYearList();
           
        }

        public List<LeaveAccrual> LeaveAccrualList { get; set; }
        protected async Task LoadLeaveAccrual()
        {
            var respLeaveAccrual = (await leaveAccrualService.GetLeaveAccrual(""));
            if(respLeaveAccrual.MessageType == MessageType.Success)
            {
                LeaveAccrualList = respLeaveAccrual.Data.ToList();
                loadMonthDropDown();
            }else
            {
                toastService.ShowError(ErrorMsg.ERRORMSG);
            }
            StateHasChanged();
        }
        protected void loadMonthDropDown()
        {
            LeaveMonthList = new List<LeaveMonth> { };
            int i = 0;
            // MonthID = 0;
            foreach(var accrual in LeaveAccrualList)
            {
                if(accrual.LeaveAccrualID == 0)
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
       
        protected void SetYearList()
        {
            lstYear = new List<IDropDownList> { };
            IDropDownList ListItem = new IDropDownList();
            lstYear = (from lookupItem in LeaveYearList
                       select new IDropDownList { ID = Convert.ToInt32(lookupItem.year), Value = lookupItem.text }).ToList();
            ListItem.ID = 0;
            ListItem.Value = "Select";
            lstYear.Insert(0, ListItem);
        }

        protected void SetGridMonthList()
        {
            lstMonth = new List<IDropDownList> { };
            IDropDownList ListItem = new IDropDownList();
            lstMonth = (from lookupItem in LeaveMonthList
                        select new IDropDownList { ID = Convert.ToInt32(lookupItem.ID), Value = lookupItem.MonthName }).ToList();
            ListItem.ID = 0;
            ListItem.Value = "Select";
            lstMonth.Insert(0, ListItem);
        }

       
        protected void onMonthChange(int month)
        {
            MonthID = month;

            if (MonthID != 0)
            {
                accrualDate = LeaveMonthList.Find(x => x.ID == month).Month;
                accrualMonth = accrualDate.ToString("MMMM") + " " + accrualDate.Year.ToString();
                isDisableAccrual = (LeaveAccrualList.Find(x => x.AccruedDate == accrualDate)) != null ? false : true;
            }else
            {
                isDisableAccrual = true;
            }
            LoadTableConfig();
        }
        protected void UpdatePageSize(int pageSize)
        {
            DefaultPageSize = pageSize;
        }

        protected async Task ConfirmLeaveAccrual_Click(bool isLeaveAccure)
        {
            if (isLeaveAccure)
            {
                LeaveAccrual leaveAccrual = new LeaveAccrual();
                leaveAccrual.AccruedValue = accureCount;
                leaveAccrual.AccruedDate = accrualDate;
                leaveAccrual.CreatedBy = user.FirstName + ' ' + user.LastName;
                await saveLeaveAccrual(leaveAccrual);
                // await LookupService.DeleteListValue(selected.ListValueID);
                // await LoadList();
                // toastService.ShowSuccess("Lookup Delete successfully", "");

            }
        }
        protected void setCountryList()
        {
            lstCountry.Clear();
            IDropDownList ListItem = new IDropDownList();
            lstCountry = (from country in CountryList
                          select new IDropDownList { ID = country.CountryID, Value = country.CountryDesc }).ToList();
            ListItem.ID = 0;
            ListItem.Value = "All";
            if (country == null)
            {
                country = "United States";
            }
            lstCountry.Insert(0, ListItem);
            DefaultTypeID = lstCountry.Find(x => x.Value.ToLower() == country.ToLower()).ID;
        }


    }
}
