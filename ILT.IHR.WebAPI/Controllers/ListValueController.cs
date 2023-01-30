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
    [Route("[controller]")]
    [Authorize]
    public class ListValueController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public ListValueController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new ListValueFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }

        [HttpGet]
        public Response<List<ListValue>>  Get()
        {
            return objFactory.GetList(new ListValue());
        }

        [HttpGet("{id}")]
        public Response<ListValue> Get(int id)
        {
            ListValue LstVal = new ListValue();
            LstVal.ListValueID = id;
            return objFactory.GetByID(LstVal);
        }

        [HttpPost]
        public Response<ListValue> Post([FromBody] ListValue LstVal)
        {
            return objFactory.Save(LstVal);
        }

        [HttpPut("{id}")]
        public Response<ListValue> Put(int id, [FromBody] ListValue LstVal)
        {
            LstVal.ListValueID = id;
            return objFactory.Save(LstVal);
        }

        [HttpDelete("{id}/{modifiedBy}")]
        public Response<ListValue> Delete(int id, string modifiedBy)
        {
            ListValue LstVal = new ListValue();
            LstVal.ListValueID = id;
            LstVal.ModifiedBy = modifiedBy; //curent user
            return objFactory.Delete(LstVal);
        }
    }
}
