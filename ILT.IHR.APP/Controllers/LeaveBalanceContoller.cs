using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;
using ILT.IHR.Factory;
using ITL.IHR.Factory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ILT.IHR.APP.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class LeaveBalanceController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;
        IHttpContextAccessor _contextAccessor;
        public LeaveBalanceController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            _contextAccessor = contextAccessor;
            objFactory = new LeaveBalanceFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }

        [HttpGet]
        public Response<List<LeaveBalance>> Get(int? EmployeeID)
        {
            LeaveBalance leaveBalance = new LeaveBalance();
            leaveBalance.EmployeeID = EmployeeID;
            return objFactory.GetList(leaveBalance);
        }

        //[HttpGet]
        //public Response<List<LeaveBalance>> Get()
        //{
        //    return objFactory.GetList(new LeaveBalance());
        //}

        [HttpGet("{id}")]
        public Response<LeaveBalance> Get(int id)
        {
            LeaveBalance leaveBalance = new LeaveBalance();
            leaveBalance.LeaveBalanceID = id;
            return objFactory.GetByID(leaveBalance);
        }

        [HttpPost]
        public Response<LeaveBalance> Post([FromBody] LeaveBalance Cty)
        {
            return objFactory.Save(Cty);
        }

        [HttpPut("{id}")]
        public Response<LeaveBalance> Put(int id, [FromBody] LeaveBalance Cty)
        {
            Cty.LeaveBalanceID = id;
            return objFactory.Save(Cty);
        }

        [HttpDelete("{id}")]
        public Response<LeaveBalance> Delete(int id)
        {
            LeaveBalance Cty = new LeaveBalance();
            Cty.LeaveBalanceID = id;
            Cty.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(Cty);
        }
        [HttpPost]
        [Route("GetLeavesCount", Name = "GetLeavesCount")]
        public Response<List<LeaveBalance>> GetLeavesCount(Report reportReq)
        {
            string ConString = _config.GetConnectionString(this.ClientID);
            LeaveBalanceFactory leaveFactory = new LeaveBalanceFactory(ConString, _config);
            return leaveFactory.GetLeavesCount<LeaveBalance>(reportReq);
        }
        [HttpPost]
        [Route("GetLeaveDetail", Name = "GetLeaveDetail")]
        public Response<List<LeaveBalance>> GetLeaveDetail(Report reportReq)
        {
            string ConString = _config.GetConnectionString(this.ClientID);
            LeaveBalanceFactory leaveFactory = new LeaveBalanceFactory(ConString, _config);
            
           return CalculateLWP(leaveFactory.GetLeaveDetail<LeaveBalance>(reportReq));
           
        }

        private Response<List<LeaveBalance>> CalculateLWP(Response<List<LeaveBalance>> leaveBalances)
        {
            HolidayController holiday = new HolidayController(_config, _contextAccessor);
            var lstholiday = holiday.Get();
            List<LeaveBalance> lstLeaveBalance = new List<LeaveBalance>();
            List<LeaveBalance> lstUnpaidLeaveBalance = new List<LeaveBalance>();
            List<LeaveBalance> lstFinalLeaveBalance = new List<LeaveBalance>();
            Response<List<LeaveBalance>> response = new Response<List<LeaveBalance>>();
            if (leaveBalances.Data.Count > 0)
            {
                foreach (var leave in leaveBalances.Data)
                {
                    if(leave.VacationBalance < 0 && leave.LeaveType != "Unpaid Leave")
                    {
                        var lb = new LeaveBalance()
                        {
                            EmployeeCode = leave.EmployeeCode,
                            EmployeeName = leave.EmployeeName,
                            StartDate = leave.StartDate,
                            EndDate = leave.EndDate,
                            LeaveType = leave.LeaveType,
                            LeaveInRange = leave.LeaveInRange,
                            VacationBalance = leave.VacationBalance,
                            VacationTotal = leave.VacationTotal,
                            VacationUsed = leave.VacationUsed,
                            LWPAccounted = leave.LWPAccounted,
                        };
                        lstLeaveBalance.Add(lb);
                    }
                    else if (leave.LeaveType == "Unpaid Leave")
                    {
                        var lb = new LeaveBalance()
                        {
                            EmployeeCode = leave.EmployeeCode,
                            EmployeeName = leave.EmployeeName,
                            StartDate = leave.StartDate,
                            EndDate = leave.EndDate,
                            LeaveType = leave.LeaveType,
                            LeaveInRange = leave.LeaveInRange,
                            VacationBalance = leave.VacationBalance,
                            VacationTotal = leave.VacationTotal,
                            VacationUsed = leave.VacationUsed,
                            LWPAccounted = leave.LWPAccounted,
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
                                dates.AddRange(GetDateRange(x.StartDate, x.EndDate, lstholiday.Data));

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

                
            }

            response.Data = lstFinalLeaveBalance;
            response.Message = "Success";

            return response;
        }
        private List<DateTime> GetDateRange(DateTime startDate, DateTime endDate, List<Holiday> holidays)
        {


            if (endDate < startDate)
                throw new ArgumentException("endDate must be greater than or equal to startDate");

            List<DateTime> dts = new List<DateTime>();

            while (startDate <= endDate)
            {
                if (!holidays.Any(x => x.StartDate == startDate) && startDate.Date.DayOfWeek != DayOfWeek.Sunday
                    && startDate.Date.DayOfWeek != DayOfWeek.Saturday)
                {
                    dts.Add(startDate);
                }
                startDate = startDate.AddDays(1);
            }
            return dts;
        }
    }
}
