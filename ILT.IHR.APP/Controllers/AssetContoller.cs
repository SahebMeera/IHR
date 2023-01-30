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
    public class AssetController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public AssetController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new AssetFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }



        [HttpGet]
        public Response<List<Asset>> Get()
        {
            return objFactory.GetList(new Asset());
        }

        [HttpGet("{id}")]
        public Response<Asset> Get(int id)
        {
            Asset Cty = new Asset();
            Cty.AssetID = id;
            return objFactory.GetByID(Cty);
        }

        [HttpPost]
        public Response<Asset> Post([FromBody] Asset Cty)
        {
            return objFactory.Save(Cty);
        }

        [HttpPut("{id}")]
        public Response<Asset> Put(int id, [FromBody] Asset Cty)
        {
            Cty.AssetID = id;
            return objFactory.Save(Cty);
        }

        [HttpDelete("{id}")]
        public Response<Asset> Delete(int id)
        {
            Asset Cty = new Asset();
            Cty.AssetID = id;
            Cty.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(Cty);
        }
    }
}
