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

namespace WebAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class LeaveBalanceController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public LeaveBalanceController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
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
            return leaveFactory.GetLeaveDetail<LeaveBalance>(reportReq);
        }
    }
}
