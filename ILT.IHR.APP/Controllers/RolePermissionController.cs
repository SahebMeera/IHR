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
    [Route("api/[controller]")]
    [Authorize]
    public class RolePermissionController : BaseController
    {
        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public RolePermissionController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new RolePermissionFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), _config);
        }

        [HttpGet]
        public Response<List<RolePermission>> Get()
        {
            return objFactory.GetList(new RolePermission());
        }

        [HttpGet("{id}")]
        public Response<RolePermission> Get(int id)
        {
            RolePermission rolPermission = new RolePermission();
            rolPermission.RolePermissionID = id;
            return objFactory.GetByID(rolPermission);
        }

        [HttpPost]
        public Response<RolePermission> Post([FromBody] RolePermission rol)
        {
            return objFactory.Save(rol);
        }

        [HttpPut("{id}")]
        public Response<RolePermission> Put(int id, [FromBody] RolePermission rol)
        {
            rol.RolePermissionID = id;
            return objFactory.Save(rol);
        }

        [HttpDelete("{id}")]
        public Response<RolePermission> Delete(int id)
        {
            RolePermission rol = new RolePermission();
            rol.RolePermissionID = id;
            rol.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(rol);
        }
    }
}