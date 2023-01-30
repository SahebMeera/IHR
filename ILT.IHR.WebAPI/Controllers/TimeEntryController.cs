using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using WebAPI.Controllers;

namespace ILT.IHR.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class TimeEntryController : BaseController
    {
        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public TimeEntryController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new TimeEntryFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }

        [HttpGet]
        public Response<List<TimeEntry>> Get()
        {
            return objFactory.GetList(new TimeEntry());
        }

        [HttpGet("{id}")]
        public Response<TimeEntry> Get(int id)
        {
            TimeEntry timeEntry = new TimeEntry();
            timeEntry.TimeEntryID = id;
            return objFactory.GetByID(timeEntry);
        }

        [HttpPost]
        public Response<TimeEntry> Post([FromBody] TimeEntry timeEntry)
        {
            return objFactory.Save(timeEntry);
        }

        [HttpPut("{id}")]
        public Response<TimeEntry> Put(int id, [FromBody] TimeEntry timeEntry)
        {
            timeEntry.TimeEntryID = id;
            return objFactory.Save(timeEntry);
        }

        [HttpDelete("{id}")]
        public Response<TimeEntry> Delete(int id)
        {
            TimeEntry timeEntry = new TimeEntry();
            timeEntry.TimeEntryID = id;
            // timeEntry.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(timeEntry);
        }

    }
}