using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;
using ILT.IHR.Factory;
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
    public class CompanyController: BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public CompanyController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new CompanyFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }

        [HttpGet]
        public Response<List<Company>> Get()
        {
            return objFactory.GetList(new Company());
        }

        [HttpGet("{id}")]
        public Response<Company> Get(int id)
        {
            Company Comp = new Company();
            Comp.CompanyID = id;
            return objFactory.GetByID(Comp);
        }

        [HttpPost]
        public Response<Company> Post([FromBody] Company Comp)
        {
            return objFactory.Save(Comp);
        }

        [HttpPut("{id}")]
        public Response<Company> Put(int id, [FromBody] Company Comp)
        {
            Comp.CompanyID = id;
            return objFactory.Save(Comp);
        }

        [HttpDelete("{id}")]
        public Response<Company> Delete(int id)
        {
            Company Comp = new Company();
            Comp.CompanyID = id;
            Comp.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(Comp);
        }
    }
}