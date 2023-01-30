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
    public class DependentController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public DependentController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new DependentFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }

        [HttpGet]
        public Response<List<Dependent>> Get()
        {
            return objFactory.GetList(new Dependent());
        }

        [HttpGet("{id}")]
        public Response<Dependent> Get(int id)
        {
            Dependent Dep = new Dependent();
            Dep.DependentID = id;
            return objFactory.GetByID(Dep);
        }

        [HttpPost]
        public Response<Dependent> Post([FromBody] Dependent Dep)
        {
            return objFactory.Save(Dep);
        }

        [HttpPut("{id}")]
        public Response<Dependent> Put(int id, [FromBody] Dependent Dep)
        {
            Dep.DependentID = id;
            return objFactory.Save(Dep);
        }

        [HttpDelete("{id}")]
        public Response<Dependent> Delete(int id)
        {
            Dependent Dep = new Dependent();
            Dep.DependentID = id;
            Dep.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(Dep);
        }
    }
}
