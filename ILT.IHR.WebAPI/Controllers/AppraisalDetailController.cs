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
    public class AppraisalDetailController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public AppraisalDetailController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new AppraisalDetailFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }

        [HttpGet]
        public Response<List<AppraisalDetail>> Get()
        {
            return objFactory.GetList(new AppraisalDetail());
        }

        [HttpGet("{id}")]
        public Response<AppraisalDetail> Get(int id)
        {
            AppraisalDetail Dept = new AppraisalDetail();
            Dept.AppraisalDetailID = id;
            return objFactory.GetByID(Dept);
        }

        [HttpPost]
        public Response<AppraisalDetail> Post([FromBody] AppraisalDetail Dept)
        {
            return objFactory.Save(Dept);
        }

        [HttpPut("{id}")]
        public Response<AppraisalDetail> Put(int id, [FromBody] AppraisalDetail Dept)
        {
            Dept.AppraisalDetailID = id;
            return objFactory.Save(Dept);
        }

        [HttpDelete("{id}")]
        public Response<AppraisalDetail> Delete(int id)
        {
            AppraisalDetail Dept = new AppraisalDetail();
            Dept.AppraisalDetailID = id;
            Dept.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(Dept);
        }
    }
}
