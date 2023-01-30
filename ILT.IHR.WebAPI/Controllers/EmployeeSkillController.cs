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
    public class EmployeeSkillController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public EmployeeSkillController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new EmployeeSkillFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), _config);
        }

        [HttpGet]
        public Response<List<EmployeeSkill>> Get(int? EmployeeID)
        {
            EmployeeSkill employeeSkill = new EmployeeSkill();
            if (EmployeeID != null && EmployeeID != 0)
                employeeSkill.EmployeeID = EmployeeID;
            return objFactory.GetList(employeeSkill);
        }

        [HttpGet("{id}")]
        public Response<EmployeeSkill> Get(int id)
        {
            EmployeeSkill employeeSkill = new EmployeeSkill();
            employeeSkill.EmployeeSkillID = id;
            return objFactory.GetByID(employeeSkill);
        }

        [HttpPost]
        public Response<EmployeeSkill> Post([FromBody] EmployeeSkill employeeSkill)
        {
            return objFactory.Save(employeeSkill);
        }

        [HttpPut("{id}")]
        public Response<EmployeeSkill> Put(int id, [FromBody] EmployeeSkill employeeSkill)
        {
            employeeSkill.EmployeeSkillID = id;
            return objFactory.Save(employeeSkill);
        }

        [HttpDelete("{id}")]
        public Response<EmployeeSkill> Delete(int id)
        {
            EmployeeSkill employeeSkill = new EmployeeSkill();
            employeeSkill.EmployeeSkillID = id;
            employeeSkill.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(employeeSkill);
        }
    }
}
