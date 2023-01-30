using System.Collections.Generic;
using ILT.IHR.DTO;
using ILT.IHR.Factory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace WebAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class ProcessWizardController : BaseController
    {
        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public ProcessWizardController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new ProcessWizardFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }



        [HttpGet]
        public Response<List<ProcessWizard>> Get()
        {
            return objFactory.GetList(new ProcessWizard());
        }

        [HttpGet("{id}")]
        public Response<ProcessWizard> Get(int id)
        {
            ProcessWizard Cty = new ProcessWizard();
            Cty.ProcessWizardID = id;
            //return objFactory.GetByID(Cty);
            return objFactory.GetRelatedObjectsByID(Cty);
        }

        [HttpPost]
        public Response<ProcessWizard> Post([FromBody] ProcessWizard Cty)
        {
            return objFactory.Save(Cty);
        }

        [HttpPut("{id}")]
        public Response<ProcessWizard> Put(int id, [FromBody] ProcessWizard Cty)
        {
            Cty.ProcessWizardID = id;
            return objFactory.Save(Cty);
        }

        [HttpDelete("{id}")]
        public Response<ProcessWizard> Delete(int id)
        {
            ProcessWizard Cty = new ProcessWizard();
            Cty.ProcessWizardID = id;
            return objFactory.Delete(Cty);
        }
    }
}
