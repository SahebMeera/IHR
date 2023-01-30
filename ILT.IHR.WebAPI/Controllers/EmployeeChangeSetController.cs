using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;
using ILT.IHR.Factory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebAPI.Controllers;

namespace ILT.IHR.WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmployeeChangeSetController : BaseController
    {
        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public EmployeeChangeSetController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new EmployeeChangeSetFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }

        [HttpGet("{employeeid}")]
        public Response<List<EmployeeChangeSet>> Get(int employeeid)
        {
            EmployeeChangeSet employeeChangeSet = new EmployeeChangeSet();
            employeeChangeSet.EmployeeID = employeeid;
            employeeChangeSet.UserID = this.CurrentUserID;
            return objFactory.GetList(employeeChangeSet);
        }

    }
}