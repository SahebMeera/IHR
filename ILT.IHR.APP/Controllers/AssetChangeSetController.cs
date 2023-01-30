using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;
using ILT.IHR.Factory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ILT.IHR.APP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetChangeSetController : BaseController
    {
        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public AssetChangeSetController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new AssetChangeSetFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }

        [HttpGet("{Assetid}")]
        public Response<List<AssetChangeSet>> Get(int Assetid)
        {
            AssetChangeSet AssetChangeSet = new AssetChangeSet();
            AssetChangeSet.AssetID = Assetid;
            return objFactory.GetList(AssetChangeSet);
        }

    }
}