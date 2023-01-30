using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;
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
    public class ModuleController : BaseController
    {

        AbstractFactory objFactory;
        private IConfiguration _config;

        public ModuleController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new ModuleFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID));
        }

        [HttpGet]
        public List<Module> Get()
        {
            return objFactory.GetList(new Module());
        }

        [HttpGet("{id}")]
        public Module Get(int id)
        {
            Module Cty = new Module();
            Cty.ModuleID = id;
            //return objFactory.GetByID(Cty);
            return objFactory.GetRelatedObjectsByID(Cty);
        }

        [HttpPost]
        public bool Post([FromBody] Module Cty)
        {
            return objFactory.Save(Cty);
        }

        [HttpPut("{id}")]
        public bool Put(int id, [FromBody] Module Cty)
        {
            Cty.ModuleID = id;
            return objFactory.Save(Cty);
        }

        [HttpDelete("{id}")]
        public bool Delete(int id)
        {
            Module Cty = new Module();
            Cty.ModuleID = id;
            Cty.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(Cty);
        }
    }
}
