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
    public class AssignmentRateController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public AssignmentRateController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new AssignmentRateFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }

        [HttpGet]
        public Response<List<AssignmentRate>> Get()
        {
            return objFactory.GetList(new AssignmentRate());
        }

        [HttpGet("{id}")]
        public Response<AssignmentRate> Get(int id)
        {
            AssignmentRate Dept = new AssignmentRate();
            Dept.AssignmentRateID = id;
            return objFactory.GetByID(Dept);
        }

        [HttpPost]
        public Response<AssignmentRate> Post([FromBody] AssignmentRate Dept)
        {
            return objFactory.Save(Dept);
        }

        [HttpPut("{id}")]
        public Response<AssignmentRate> Put(int id, [FromBody] AssignmentRate Dept)
        {
            Dept.AssignmentRateID = id;
            return objFactory.Save(Dept);
        }

        [HttpDelete("{id}")]
        public Response<AssignmentRate> Delete(int id)
        {
            AssignmentRate Dept = new AssignmentRate();
            Dept.AssignmentRateID = id;
            Dept.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(Dept);
        }
    }
}
