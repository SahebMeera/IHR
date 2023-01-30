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
    public class AppraisalController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public AppraisalController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new AppraisalFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), _config);
        }

        //[HttpGet]
        //public Response<List<Appraisal>> Get()
        //{
        //    return objFactory.GetList(new Appraisal());
        //}
        [HttpGet]
        public Response<List<Appraisal>> Get(int? EmployeeID)
        {
            Appraisal appraisal = new Appraisal();

            if (EmployeeID != null && EmployeeID != 0)
                appraisal.EmployeeID = EmployeeID;
            return objFactory.GetList(appraisal);
        }

        [HttpGet("{id}")]
        public Response<Appraisal> Get(int id)
        {
            Appraisal Dept = new Appraisal();
            Dept.AppraisalID = id;
            //return objFactory.GetByID(Dept);
            return objFactory.GetRelatedObjectsByID(Dept);
        }

        [HttpPost]
        public Response<Appraisal> Post([FromBody] Appraisal Dept)
        {
            return objFactory.Save(Dept);
        }

        [HttpPut("{id}")]
        public Response<Appraisal> Put(int id, [FromBody] Appraisal Dept)
        {
            Dept.AppraisalID = id;
            return objFactory.Save(Dept);
        }

        [HttpDelete("{id}")]
        public Response<Appraisal> Delete(int id)
        {
            Appraisal Dept = new Appraisal();
            Dept.AppraisalID = id;
            Dept.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(Dept);
        }
    }
}
