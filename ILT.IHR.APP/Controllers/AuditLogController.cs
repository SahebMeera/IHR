using ILT.IHR.DTO;
using ILT.IHR.Factory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ILT.IHR.APP.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AuditLogController : BaseController
    {
        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public AuditLogController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new AuditLogFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }

        [HttpGet]
        public Response<List<AuditLog>> Get(int AuditLogID)
        {
            AuditLog auditLog = new AuditLog();
            auditLog.AuditLogID = AuditLogID;
            return objFactory.GetList(auditLog);
        }
        [HttpPost]
        public Response<AuditLog> Post([FromBody] AuditLog Cty)
        {
            return objFactory.Save(Cty);
        }

        [HttpPut("{id}")]
        public Response<AuditLog> Put(int id, [FromBody] AuditLog Cty)
        {
            Cty.AuditLogID = id;
            return objFactory.Save(Cty);
        }

        [HttpDelete("{id}")]
        public Response<AuditLog> Delete(int id)
        {
            AuditLog Cty = new AuditLog();
            Cty.AuditLogID = id;
            return objFactory.Delete(Cty);
        }
        [HttpPost]
        [Route("GetAuditLogInfo", Name = "GetAuditLogInfo")]
        public Response<List<AuditLog>> GetAuditLogInfo(Report reportReq)
        {
            string ConString = _config.GetConnectionString("DEV");
            AuditLogFactory auditlogFactory = new AuditLogFactory(ConString, _config);
            return auditlogFactory.GetAuditLogInfo<AuditLog>(reportReq);
        }
    }
}


    

