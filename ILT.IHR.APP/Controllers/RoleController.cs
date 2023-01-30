using System;
using System.Collections.Generic;
using System.Linq;
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
    public class RoleController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public RoleController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new RoleFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), _config);
        }


        [HttpGet]
        public Response<List<Role>> Get()
        {
            return objFactory.GetList(new Role());
        }

        [HttpGet("{id}")]
        public Response<Role> Get(int id)
        {
            Role role = new Role();
            role.RoleID = id;
            return objFactory.GetRelatedObjectsByID(role);
        }

        [HttpPost]
        public Response<Role> Post([FromBody] Role role)
        {
            return objFactory.Save(role);
        }

        [HttpPut("{id}")]
        public Response<Role> Put(int id, [FromBody] Role role)
        {
            role.RoleID = id;
            return objFactory.Save(role);
        }

        //[HttpDelete("{id}")]
        //public Response<Role> Delete(int id)
        //{
        //    Role rol = new Role();
        //    rol.RoleID = id;
        //    rol.ModifiedBy = "Admin"; //curent user
        //    return objFactory.Delete(rol);
        //}
    }
}
