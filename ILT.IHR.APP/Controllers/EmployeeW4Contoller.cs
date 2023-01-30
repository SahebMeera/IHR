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
    public class EmployeeW4Controller : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public EmployeeW4Controller(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new EmployeeW4Factory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }



        [HttpGet]
        public Response<List<EmployeeW4>> Get(int EmployeeID, int? EmployeeW4Id)
        {
            EmployeeW4 employeeW4 = new EmployeeW4();
            employeeW4.EmployeeID = EmployeeID;
            return objFactory.GetList(employeeW4);
        }

      

        [HttpGet("{id}")]
        public Response<EmployeeW4> Get(int id)
        {
            EmployeeW4 Cty = new EmployeeW4();
            Cty.EmployeeW4ID = id;
            return objFactory.GetByID(Cty);
        }

        [HttpPost]
        public Response<EmployeeW4> Post([FromBody] EmployeeW4 Cty)
        {
            return objFactory.Save(Cty);
        }

        [HttpPut("{id}")]
        public Response<EmployeeW4> Put(int id, [FromBody] EmployeeW4 Cty)
        {
            Cty.EmployeeW4ID = id;
            return objFactory.Save(Cty);
        }

        [HttpDelete("{id}")]
        public Response<EmployeeW4> Delete(int id)
        {
            EmployeeW4 Cty = new EmployeeW4();
            Cty.EmployeeW4ID = id;
            Cty.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(Cty);
        }
    }
}
