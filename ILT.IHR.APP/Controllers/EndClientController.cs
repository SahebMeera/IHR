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
    public class EndClientController : BaseController
    {
        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public EndClientController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new EndClientFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }

        [HttpGet]
        public Response<List<EndClient>> Get()
        {
            return objFactory.GetList(new EndClient());
        }

        [HttpGet("{id}")]
        public Response<EndClient> Get(int id)
        {
            EndClient endclient = new EndClient();
            endclient.EndClientID = id;
            return objFactory.GetByID(endclient);
        }

        [HttpPost]
        public Response<EndClient> Post([FromBody] EndClient endclient)
        {
            return objFactory.Save(endclient);
        }

        [HttpPut("{id}")]
        public Response<EndClient> Put(int id, [FromBody] EndClient endclient)
        {
            endclient.EndClientID = id;
            return objFactory.Save(endclient);
        }

        [HttpDelete("{id}")]
        public Response<EndClient> Delete(int id)
        {
            EndClient endclient = new EndClient();
            endclient.EndClientID = id;
            endclient.ModifiedBy = endclient.ModifiedBy; //curent user 
            return objFactory.Delete(endclient);
        }
    }
}
