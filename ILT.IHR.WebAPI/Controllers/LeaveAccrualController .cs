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
    public class LeaveAccrualController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public LeaveAccrualController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new LeaveAccrualFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }

        [HttpGet]
        public Response<List<LeaveAccrual>> Get(string Country)
        {
            LeaveAccrual leaveAccrual = new LeaveAccrual();
            if (Country != null && Country != "")
                leaveAccrual.Country = Country;
            return objFactory.GetList(leaveAccrual);
        }

        [HttpGet("{id}")]
        public Response<LeaveAccrual> Get(int id)
        {
            LeaveAccrual leaveAccrual = new LeaveAccrual();
            leaveAccrual.LeaveAccrualID = id;
            return objFactory.GetByID(leaveAccrual);
        }

        [HttpPost]
        public Response<LeaveAccrual> Post([FromBody] LeaveAccrual leaveAccrual)
        {
            return objFactory.Save(leaveAccrual);
        }

        [HttpPut("{id}")]
        public Response<LeaveAccrual> Put(int id, [FromBody] LeaveAccrual leaveAccrual)
        {
            leaveAccrual.LeaveAccrualID = id;
            return objFactory.Save(leaveAccrual);
        }

        [HttpDelete("{id}")]
        public Response<LeaveAccrual> Delete(int id)
        {
            LeaveAccrual leaveAccrual = new LeaveAccrual();
            leaveAccrual.LeaveAccrualID = id;
            return objFactory.Delete(leaveAccrual);
        }
    }
}
