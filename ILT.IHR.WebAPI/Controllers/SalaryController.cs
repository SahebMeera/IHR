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
    public class SalaryController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public SalaryController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new SalaryFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }

        [HttpGet]
        public Response<List<Salary>> Get()
        {
            return objFactory.GetList(new Salary());
        }

        [HttpGet("{id}")]
        public Response<Salary> Get(int id)
        {
            Salary objSal = new Salary();
            objSal.SalaryID = id;
            return objFactory.GetByID(objSal);
        }

        [HttpPost]
        public Response<Salary> Post([FromBody] Salary Dept)
        {
            return objFactory.Save(Dept);
        }

        [HttpPut("{id}")]
        public Response<Salary> Put(int id, [FromBody] Salary Dept)
        {
            Dept.SalaryID = id;
            return objFactory.Save(Dept);
        }

        [HttpDelete("{id}")]
        public Response<Salary> Delete(int id)
        {
            Salary Dept = new Salary();
            Dept.SalaryID = id;
            Dept.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(Dept);
        }
    }
}
