using System;
using System.Collections.Generic;
using System.Linq;
using ILT.IHR.DTO;
using ILT.IHR.Factory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UserRoleController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public UserRoleController(IConfiguration config, IHttpContextAccessor contextAccessor): base(contextAccessor)
        {
            _config = config;
            objFactory = new RoleFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ?"IHRConString" : this.ClientID), config);
        }

        [HttpGet]
        public Response<List<UserRole>> Get()
        {
            return objFactory.GetList(new UserRole());
        }

        [HttpGet("{UserID}/{RoleID}")]
        public Response<UserRole> Get(int UserID, int RoleID)
        {
            UserRole userRole = new UserRole();
            userRole.UserID = UserID;
            userRole.RoleID = RoleID;
            return objFactory.GetRelatedObjectsByID(userRole);
        }

        [HttpPost]
        public List<Response<UserRole>> Post([FromBody] List<UserRole> UserRoles)
        {
            List<Response<UserRole>> responses = new List<Response<UserRole>>();
            foreach (UserRole userRole in UserRoles) {
                responses.Add(objFactory.Save(userRole));
            }
            return responses;
        }

        [HttpPut]
        public List<Response<UserRole>> Put([FromBody] List<UserRole> UserRoles)
        {
            List<Response<UserRole>> responses = new List<Response<UserRole>>();
            foreach (UserRole userRole in UserRoles)
            {
                responses.Add(objFactory.Save(userRole));
            }
            return responses;
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
