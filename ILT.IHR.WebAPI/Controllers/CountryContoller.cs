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
    public class CountryController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public CountryController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new CountryFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }



        [HttpGet]
        public Response<List<Country>> Get()
        {
            return objFactory.GetList(new Country());
        }

        [HttpGet("{id}")]
        public Response<Country> Get(int id)
        {
            Country Cty = new Country();
            Cty.CountryID = id;
            //return objFactory.GetByID(Cty);
            return objFactory.GetRelatedObjectsByID(Cty);
        }

        [HttpPost]
        public Response<Country> Post([FromBody] Country Cty)
        {
            return objFactory.Save(Cty);
        }

        [HttpPut("{id}")]
        public Response<Country> Put(int id, [FromBody] Country Cty)
        {
            Cty.CountryID = id;
            return objFactory.Save(Cty);
        }

        [HttpDelete("{id}")]
        public Response<Country> Delete(int id)
        {
            Country Cty = new Country();
            Cty.CountryID = id;
            Cty.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(Cty);
        }
    }
}
