using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;
using ILT.IHR.Factory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ContactController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public ContactController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new ContactFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }

        [HttpGet]
        public Response<List<Contact>> Get()
        {
            return objFactory.GetList(new Contact());
        }

        [HttpGet("{id}")]
        public Response<Contact> Get(int id, int employeeId)
        {
            Contact con = new Contact();
            con.ContactID = id;
            con.EmployeeID = employeeId;
            return objFactory.GetByID(con);
        }

        [HttpPost]
        public Response<Contact> Post([FromBody] Contact con)
        {
            return objFactory.Save(con);
        }

        [HttpPut("{id}")]
        public Response<Contact> Put(int id, [FromBody] Contact con)
        {
            con.ContactID = id;
            return objFactory.Save(con);
        }

        [HttpDelete("{id}")]
        public Response<Contact> Delete(int id)
        {
            Contact con = new Contact();
            con.ContactID = id;
            con.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(con);
        }
    }
}