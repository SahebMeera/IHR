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
    public class wFHController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public wFHController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new WFHFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), _config);
        }


        [HttpGet]
        public Response<List<WFH>> Get(int? EmployeeID, int? ApproverID)
        {
            WFH WFH = new WFH();
            WFH.EmployeeID = EmployeeID;
            WFH.ApproverID = ApproverID;
            return objFactory.GetList(WFH);
        }

        [HttpGet("{id}")]
        public Response<WFH> Get(int id)
        {
            WFH WFH = new WFH();
            WFH.WFHID = id;
            return objFactory.GetByID<WFH>(WFH);
        }

        [HttpPost]
        public Response<WFH> Post([FromBody] WFH WFH)
        {
            return objFactory.Save(WFH);
        }

        [HttpPut("{id}")]
        public Response<WFH> Put(int id, [FromBody] WFH WFH)
        {
            WFH.WFHID = id;
            return objFactory.Save(WFH);
        }

        [HttpDelete("{id}")]
        public Response<WFH> Delete(int id)
        {
            WFH WFH = new WFH();
            WFH.WFHID = id;
            WFH.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(WFH);
        }
    }
}
