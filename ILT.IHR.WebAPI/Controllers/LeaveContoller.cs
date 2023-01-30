using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    public class LeaveController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public LeaveController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new LeaveFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }



        [HttpGet]
        public Response<List<Leave>> Get(int? EmployeeID, int? ApproverID)
        {
            Leave leave = new Leave();
            leave.EmployeeID = EmployeeID;
            leave.ApproverID = ApproverID;
            return objFactory.GetList(leave);
        }

        [HttpGet("{id}")]
        public Response<Leave> Get(int id)
        {
            Leave leave = new Leave();
            leave.LeaveID = id;
            return objFactory.GetByID<Leave>(leave);
        }

        [HttpPost]
        public Response<Leave> Post([FromBody] Leave leave)
        {
            return objFactory.Save(leave);
        }

        [HttpPut("{id}")]
        public Response<Leave> Put(int id, [FromBody] Leave leave)
        {
            leave.LeaveID = id;
            return objFactory.Save(leave);
        }

        [HttpDelete("{id}")]
        public Response<Leave> Delete(int id)
        {
            Leave leave = new Leave();
            leave.LeaveID = id;
            leave.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(leave);
        }

        [HttpGet]
        [Route("GetLeaveDays", Name = "GetLeaveDays")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public Response<Leave> GetLeaveDays(string clientId, int employeeId, DateTime startDate, DateTime endDate, bool includesHalfDay)
        {
            string ConString = _config.GetConnectionString(clientId);
            LeaveFactory leaveFactory = new LeaveFactory(ConString, _config);
            Leave leave = new Leave();
            leave.EmployeeID = employeeId;
            leave.StartDate = startDate;
            leave.EndDate = endDate;
            leave.IncludesHalfDay = includesHalfDay;
            return leaveFactory.GetLeaveDays<Leave>(leave);
        }
    }
}
