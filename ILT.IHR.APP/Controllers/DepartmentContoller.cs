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
    public class DepartmentController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public DepartmentController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new DepartmentFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }

        [HttpGet]
        public Response<List<Department>> Get()
        {
            return objFactory.GetList(new Department());
        }

        [HttpGet("{id}")]
        public Response<Department> Get(int id)
        {
            Department Dept = new Department();
            Dept.DepartmentID = id;
            return objFactory.GetByID(Dept);
        }

        [HttpPost]
        public Response<Department> Post([FromBody] Department Dept)
        {
            return objFactory.Save(Dept);
        }

        [HttpPut("{id}")]
        public Response<Department> Put(int id, [FromBody] Department Dept)
        {
            Dept.DepartmentID = id;
            return objFactory.Save(Dept);
        }

        [HttpDelete("{id}")]
        public Response<Department> Delete(int id)
        {
            Department Dept = new Department();
            Dept.DepartmentID = id;
            Dept.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(Dept);
        }
    }
}
