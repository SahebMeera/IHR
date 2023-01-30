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
    public class ProcessDataController : BaseController
    {
        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public ProcessDataController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new ProcessDataFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }



        [HttpGet]
        public Response<List<ProcessData>> Get()
        {
            return objFactory.GetList(new ProcessData());
        }

        [HttpGet("{id}")]
        public Response<ProcessData> Get(int id)
        {
            ProcessData wd = new ProcessData();
            wd.ProcessDataID = id;
           return objFactory.GetRelatedObjectsByID(wd);
          
        }

        [HttpPost]
        public Response<ProcessData> Post([FromBody] ProcessData Cty)
        {
            return objFactory.Save(Cty);
        }

        [HttpPut("{id}")]
        public Response<ProcessData> Put(int id, [FromBody] ProcessData Cty)
        {
            Cty.ProcessDataID = id;
            return objFactory.Save(Cty);
        }

        [HttpDelete("{id}")]
        public Response<ProcessData> Delete(int id)
        {
            ProcessData Cty = new ProcessData();
            Cty.ProcessDataID = id;
            return objFactory.Delete(Cty);
        }
    }
}
