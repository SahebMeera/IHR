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
    public class HolidayController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public HolidayController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new HolidayFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }

        [HttpGet]
        public Response<List<Holiday>> Get()
        {
            return objFactory.GetList(new Holiday());
        }

        [HttpGet("{id}")]
        public Response<Holiday> Get(int id)
        {
            Holiday Cty = new Holiday();
            Cty.HolidayID = id;
            return objFactory.GetByID(Cty);
        }

        [HttpPost]
        public Response<Holiday> Post([FromBody] Holiday Cty)
        {
            return objFactory.Save(Cty);
        }

        [HttpPut("{id}")]
        public Response<Holiday> Put(int id, [FromBody] Holiday Cty)
        {
            Cty.HolidayID = id;
            return objFactory.Save(Cty);
        }

        [HttpDelete("{id}")]
        public Response<Holiday> Delete(int id)
        {
            Holiday Cty = new Holiday();
            Cty.HolidayID = id;
            Cty.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(Cty);
        }
    }
}
