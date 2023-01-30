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
    public class I9DocumentController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public I9DocumentController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new I9DocumentFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }



        [HttpGet]
        public Response<List<I9Document>> Get()
        {
            return objFactory.GetList(new I9Document());
        }

        [HttpGet("{id}")]
        public Response<I9Document> Get(int id)
        {
            I9Document I9Doc = new I9Document();
            I9Doc.I9DocumentID = id;
            return objFactory.GetByID(I9Doc);
        }

        [HttpPost]
        public Response<I9Document> Post([FromBody] I9Document I9Doc)
        {
            return objFactory.Save(I9Doc);
        }

        [HttpPut("{id}")]
        public Response<I9Document> Put(int id, [FromBody] I9Document I9Doc)
        {
            I9Doc.I9DocumentID = id;
            return objFactory.Save(I9Doc);
        }

        [HttpDelete("{id}")]
        public Response<I9Document> Delete(int id)
        {
            I9Document I9Doc = new I9Document();
            I9Doc.I9DocumentID = id;
            I9Doc.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(I9Doc);
        }
    }
}
