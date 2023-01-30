using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    public class FormI9Controller : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public FormI9Controller(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new FormI9Factory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }



        [HttpGet]
        public Response<List<FormI9>> Get(int EmployeeID, int? FormI9Id)
        {
            FormI9 formI9 = new FormI9();
            formI9.EmployeeID = EmployeeID;
            return objFactory.GetList(formI9);
        }



        [HttpGet("{id}")]
        public Response<FormI9> Get(int id)
        {
            FormI9 formI9 = new FormI9();
            formI9.FormI9ID = id;
            return objFactory.GetByID(formI9);
        }

        [HttpPost]
        public Response<FormI9> Post([FromBody] FormI9 formI9)
        {
            return objFactory.Save(formI9);
        }

        [HttpPut("{id}")]
        public Response<FormI9> Put(int id, [FromBody] FormI9 formI9)
        {
            formI9.FormI9ID = id;
            return objFactory.Save(formI9);
        }

        [HttpDelete("{id}")]
        public Response<FormI9> Delete(int id)
        {
            FormI9 formI9 = new FormI9();
            formI9.FormI9ID = id;
            formI9.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(formI9);
        }
        [HttpGet]
        [Route("GetI9ExpiryForm", Name = "GetI9ExpiryForm")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public Response<List<FormI9>> GetI9ExpiryForm(DateTime expirydate)
        {
            string ConString = _config.GetConnectionString("DEV");
            FormI9Factory formI9Factory = new FormI9Factory(ConString, _config);
            FormI9 formI9 = new FormI9();
            formI9.I94ExpiryDate = expirydate;
            return formI9Factory.GetI9ExpiryForm<FormI9>(formI9);
        }
    }
}
