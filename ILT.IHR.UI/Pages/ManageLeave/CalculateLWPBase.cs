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
using System.Data;
using System.Linq;
using System.Threading.Tasks;


namespace ILT.IHR.UI.Pages.ManageLeave
{
    public class CalculateLWPBase : ComponentBase
    {
        [Inject]
        public ISessionStorageService sessionStorage { get; set; }
        [Inject]
        public IToastService toastService { get; set; } //Service   
        
        [Inject]
        public ICountryService CountryService { get; set; }
       
        [Parameter]
        public EventCallback<bool> UpdateLeaveList { get; set; }
        public bool ShowDialog { get; set; }
        public ILT.IHR.DTO.User user;
        public List<Country> CountryList { get; set; }
        public bool isDisableLWP { get; set; }
        protected DeleteConfirmation.ConfirmBase confirmBase { get; set; }
        public bool isSaveButtonDisabled { get; set; } = false;
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; } 
        public string selectedCountry { get; set; }
        public bool isStartDateRequired { get; set; } = false;
        public bool isEndDateRequired { get; set; } = false;
        public bool isDisableProcessbtn { get; set; } = false;
        
        public bool showLWPGrid { get; set; } = false;

        public List<LeaveBalance> lstLeaveBalance = new List<LeaveBalance>();
        public List<LeaveBalance> lstUnpaidLeaveBalance = new List<LeaveBalance>();
        public List<LeaveBalance> lstFinalLeaveBalance = new List<LeaveBalance>();
        public int DefaultPageSize { get; set; }
        [Inject]
        public ILeaveBalanceService leaveBalanceService { get; set; }
        [Inject]
        public ILookupService LookupService { get; set; } //Service
        [Inject]
        public IEmployeeService EmployeeService { get; set; } //Service
        [Inject]
        public ILeaveService LeaveService { get; set; } //Service
        [Inject]
        public IHolidayService HolidayService { get; set; } //Service

        public DTO.Report report = new Report();
        public decimal usedLeaveBal { get; set; }
        public decimal sumOfLeaveInRange { get; set; }
        protected List<ListValue> VacationTypeList { get; set; }
        protected List<ListValue> StatusList { get; set; }
        protected IEnumerable<DTO.Employee> EmployeeList { get; set; }
        public IEnumerable<ILT.IHR.DTO.Holiday> Holidays { get; set; } 
        public class LeaveMonth
        {
            public int ID { get; set; }
            public DateTime Month { get; set; }
            public string MonthName { get; set; }
        }

        protected override async Task OnInitializedAsync()
        {
            user = await sessionStorage.GetItemAsync<ILT.IHR.DTO.User>("User");
            isDisableLWP = true;
            Response<IEnumerable<Country>> response = (await CountryService.GetCountries());

            if (response.MessageType == MessageType.Success)
            {
                CountryList = response.Data.ToList();
            }
            var reponses = (await HolidayService.GetHolidays());
            if (reponses.MessageType == MessageType.Success)
            {
                Holidays = reponses.Data;
            }

            Response<IEnumerable<ListValue>> resp = (await LookupService.GetListValues());
            if (resp.MessageType == MessageType.Success)
            {
                var vacList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.VACATIONTYPE).ToList();
                VacationTypeList = vacList.Where(x => x.Value.ToUpper() == ListTypeConstants.LWP.ToUpper()).ToList();
                StatusList = resp.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.LEAVEREQUESTSTATUS).ToList();
            }
            Response<IEnumerable<ILT.IHR.DTO.Employee>> respEmployees = await EmployeeService.GetEmployees();
            if (respEmployees.MessageType == MessageType.Success)
            {
                EmployeeList = respEmployees.Data;
            }
        }

        protected async void onCountryChange(ChangeEventArgs e)
        {
            isDisableLWP = false;
        }

        protected void UpdatePageSize(int pageSize)
        {
            DefaultPageSize = pageSize;
        }
        protected async Task calculateLWP()
        {
            isStartDateRequired = false;
            isEndDateRequired = false;
            isDisableProcessbtn = false;
            if (startDate == null)
            {
                isStartDateRequired = true;
            }
            else if (endDate == null)
            {
                isEndDateRequired = true;
            }
            if(isStartDateRequired == false && isEndDateRequired == false)
            {
                showLWPGrid = true;
            }
             
            lstLeaveBalance = new List<LeaveBalance>();
            lstUnpaidLeaveBalance = new List<LeaveBalance>();

            lstFinalLeaveBalance = new List<LeaveBalance>();
            DTO.Report reportReq = new DTO.Report();
            reportReq.Country = selectedCountry;
            reportReq.StartDate = startDate;
            reportReq.EndDate = endDate;
            
            DataTable dt = (await leaveBalanceService.GetReportLeaveDetailInfo(reportReq, null));
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (Convert.ToDecimal(row["VacationBalance"]) < 0 && row["LeaveType"].ToString() != "Unpaid Leave")
                    {
                        var lb = new LeaveBalance()
                        {
                            EmployeeCode = row["EmployeeCode"].ToString(),
                            EmployeeName = row["EmployeeName"].ToString(),
                            StartDate = Convert.ToDateTime(row["StartDate"]),
                            EndDate = Convert.ToDateTime(row["EndDate"]),
                            LeaveType = row["LeaveType"].ToString(),
                            LeaveInRange = Convert.ToDecimal(row["LeaveInRange"]),
                            VacationBalance = Convert.ToDecimal(row["VacationBalance"]),
                            VacationTotal = Convert.ToDecimal(row["VacationTotal"]),
                            VacationUsed = Convert.ToDecimal(row["VacationUsed"]),
                            LWPAccounted = Convert.ToDecimal(row["LWPAccounted"]),
                        };
                        lstLeaveBalance.Add(lb);
                    }
                    else if (row["LeaveType"].ToString() == "Unpaid Leave")
                    {
                        var lb = new LeaveBalance()
                        {
                            EmployeeCode = row["EmployeeCode"].ToString(),
                            EmployeeName = row["EmployeeName"].ToString(),
                            StartDate = Convert.ToDateTime(row["StartDate"]),
                            EndDate = Convert.ToDateTime(row["EndDate"]),
                            LeaveType = row["LeaveType"].ToString(),
                            LeaveInRange = Convert.ToDecimal(row["LeaveInRange"]),
                            VacationBalance = Convert.ToDecimal(row["VacationBalance"]),
                            VacationTotal = Convert.ToDecimal(row["VacationTotal"]),
                            VacationUsed = Convert.ToDecimal(row["VacationUsed"]),
                            LWPAccounted = Convert.ToDecimal(row["LWPAccounted"]),
                        };
                        lstUnpaidLeaveBalance.Add(lb);
                    }
                }

                var distinctEmp = lstLeaveBalance.GroupBy(x => x.EmployeeCode).Select(x => new
                {
                    EmployeeCode = x.Key,
                    EmplyeeName = x.FirstOrDefault().EmployeeName,
                    LeaveType = x.FirstOrDefault().LeaveType,
                    TotalLeaves = x.Sum(x => x.LeaveInRange),
                    balance = x.FirstOrDefault().VacationBalance
                });

                foreach (var d in distinctEmp)
                {
                    decimal counter = d.balance * (-1);

                    var leaves = lstLeaveBalance.Where(x => x.EmployeeCode == d.EmployeeCode).ToList();
                    leaves = leaves.OrderByDescending(x => x.StartDate).ToList();
                    
                    leaves.ForEach(x =>
                    {
                        if (counter != 0)
                        {
                            if (counter > x.LeaveInRange)
                            {
                                x.LeaveInRange = x.LeaveInRange;
                                counter = counter - x.LeaveInRange;

                                var lb = new LeaveBalance()
                                {
                                    EmployeeCode = x.EmployeeCode,
                                    EmployeeName = x.EmployeeName,
                                    StartDate = x.StartDate,
                                    EndDate = x.EndDate,
                                    LeaveType = x.LeaveType,
                                    LeaveInRange = x.LeaveInRange,
                                    VacationBalance = x.VacationBalance,
                                };
                                lstFinalLeaveBalance.Add(lb);
                            }
                            else
                            {
                                List<DateTime> dates = new List<DateTime>();
                                dates.AddRange(GetDateRange(x.StartDate, x.EndDate));
                                
                                var lb = new LeaveBalance()
                                {
                                    EmployeeCode = x.EmployeeCode,
                                    EmployeeName = x.EmployeeName,
                                    LeaveType = x.LeaveType,
                                    LeaveInRange = counter,
                                    VacationBalance = x.VacationBalance,
                                };
                                counter = Math.Ceiling(counter);
                                
                                dates = dates.OrderByDescending(y => y.Date).
                                            Take(Convert.ToInt32(counter)).ToList();
                                lb.StartDate = dates.Last();
                                lb.EndDate = dates.First();
                                lstFinalLeaveBalance.Add(lb);
                                
                                counter = 0;
                            }
                        }
                    });
                }

                if (lstFinalLeaveBalance.Count == 0)
                {
                    isDisableProcessbtn = true;
                }
            }
            else {
                isDisableProcessbtn = true;
            }
        }

        private  List<DateTime> GetDateRange(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("endDate must be greater than or equal to startDate");

            List<DateTime> dts = new List<DateTime>();

            while (startDate <= endDate)
            {
                if (!Holidays.Any(x => x.StartDate == startDate) && startDate.Date.DayOfWeek != DayOfWeek.Sunday 
                    && startDate.Date.DayOfWeek != DayOfWeek.Saturday) 
                {
                    dts.Add(startDate);
                }
                startDate = startDate.AddDays(1);
            }
            return dts;
        }

       
        protected async Task ConfirmProcess()
        {
            confirmBase.Show();
        }

        protected async Task UpdatePendingLWP(bool isProcessLWP)
        {
            if (isProcessLWP)
            {
                isDisableProcessbtn = true;
                foreach (var lwp in lstFinalLeaveBalance)
                {
                    var leave = new Leave()
                    {
                        CreatedBy = user.FirstName + " " + user.LastName,
                        ApproverID = user.EmployeeID,
                        Approver = user.FirstName + " " + user.LastName,
                        LeaveType = VacationTypeList.Find(x => x.Value.ToUpper() == LeaveType.LWP).ValueDesc,
                        LeaveTypeID = VacationTypeList.Find(x => x.Value.ToUpper() == LeaveType.LWP).ListValueID,
                        StatusID = StatusList.Find(x => x.ValueDesc.ToUpper() == LeaveStatus.APPROVED).ListValueID,
                        EmployeeID = EmployeeList.Where(x => x.EmployeeCode == lwp.EmployeeCode)?.FirstOrDefault().EmployeeID,
                        StartDate = lwp.StartDate,
                        EndDate = lwp.EndDate,
                        Duration = lwp.LeaveInRange,
                        RequesterID = (int)EmployeeList.Where(x => x.EmployeeCode == lwp.EmployeeCode)?.FirstOrDefault().EmployeeID,
                        Title = "Unpaid Leave",
                        Detail = "Unpaid Leave",
                        IncludesHalfDay = (lwp.LeaveInRange % 1) > 0 ? true : false,
                        Comment = StatusList.Find(x => x.ValueDesc.ToUpper() == LeaveStatus.APPROVED).ValueDesc,
                    };
                    var result = await LeaveService.SaveLeave(leave);
                }
                toastService.ShowSuccess("Unpaid Leave Save successfully", "");
                await UpdateLeaveList.InvokeAsync(true);
                Cancel();
            }
        }

        public void Cancel()
        {
            ShowDialog = false;
            StateHasChanged();
            ResetDialog();
        }

        protected string FormatDate(DateTime? dateTime)
        {
            string formattedDate = "";
            if (dateTime != null)
            {
                var date = dateTime.Value.ToString("MM/dd/yyyy");
                formattedDate = date;
            }

            return formattedDate;
        }
       
        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
            ResetDialog();
        }

        public void Show()
        {
            ResetDialog();
            ShowDialog = true;
            isfirstElementFocus = true;
            StateHasChanged();
        }

        private void ResetDialog()
        {
            lstFinalLeaveBalance = new List<LeaveBalance>();
            lstLeaveBalance = new List<LeaveBalance>();
            startDate = null;
            endDate = null;
            selectedCountry = null;
            showLWPGrid = false;
            isEndDateRequired = false;
            isStartDateRequired = false;
            isDisableLWP = true;
            isDisableProcessbtn = false;
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

