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
    public class AssignmentController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        //public AssignmentController(IConfiguration config)
        //{
        //    _config = config;
        //    objFactory = new AssignmentFactory(_config.GetConnectionString("DEV"), config);
        //}

        public AssignmentController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new AssignmentFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }

        [HttpGet]
        public Response<List<Assignment>> Get()
        {
            return objFactory.GetList(new Assignment());
        }

        [HttpGet("{id}")]
        public Response<Assignment> Get(int id)
        {
            Assignment Dept = new Assignment();
            Dept.AssignmentID = id;
            //return objFactory.GetByID(Dept);
            return objFactory.GetRelatedObjectsByID(Dept);
        }

        [HttpPost]
        public Response<Assignment> Post([FromBody] Assignment Dept)
        {
            return objFactory.Save(Dept);
        }

        [HttpPut("{id}")]
        public Response<Assignment> Put(int id, [FromBody] Assignment Dept)
        {
            Dept.AssignmentID = id;
            return objFactory.Save(Dept);
        }

        [HttpDelete("{id}")]
        public Response<Assignment> Delete(int id)
        {
            Assignment Dept = new Assignment();
            Dept.AssignmentID = id;
            Dept.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(Dept);
        }
    }
}
