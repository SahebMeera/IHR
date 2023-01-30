using System;
using System.Collections.Generic;
using ILT.IHR.DTO;
using ILT.IHR.Factory;
using ITL.IHR.Factory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace ILT.IHR.APP.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ListTypeController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public ListTypeController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new ListTypeFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }

        [HttpGet]
        public Response<List<ListType>> Get()
        {
            return objFactory.GetList(new ListType());
        }

        [HttpGet("{id}")]
        public Response<ListType> Get(int id)
        {
            ListType LstType = new ListType();
            LstType.ListTypeID = id;
            return objFactory.GetRelatedObjectsByID(LstType);
        }

        [HttpPost]
        public Response<ListType> Post([FromBody] ListType LstType)
        {
            return objFactory.Save(LstType);
        }

        [HttpPut("{id}")]
        public Response<ListType> Put(int id, [FromBody] ListType LstType)
        {
            LstType.ListTypeID = id;
            return objFactory.Save(LstType);
        }

        [HttpDelete("{id}")]
        public Response<ListType> Delete(int id)
        {
            ListType LstType = new ListType();
            LstType.ListTypeID = id;
            LstType.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(LstType);
        }
    }
}
