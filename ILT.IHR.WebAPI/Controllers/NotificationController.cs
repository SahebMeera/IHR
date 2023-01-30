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
    [Authorize]
    [Route("[controller]")]
    public class NotificationController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public NotificationController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new NotificationFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }

        [HttpGet]
        public Response<List<Notification>> Get()
        {
            return objFactory.GetList(new Notification());
        }

        [HttpGet("{id}")]
        public Response<Notification> Get(int id)
        {
            Notification objNotification = new Notification();
            objNotification.NotificationID = id;
            //return objFactory.GetByID(objNotification);
            return objFactory.GetRelatedObjectsByID(objNotification);
        }

        //[HttpPost]
        //public Response<Notification> Post([FromBody] Notification objNotification)
        //{
        //    return objFactory.Save(objNotification);
        //}

        [HttpPut("{id}")]
        public Response<Notification> Put(int id, [FromBody] Notification objNotification)
        {
            objNotification.UserID = this.CurrentUserID;
            return objFactory.Save(objNotification);
        }




        //[HttpDelete("{id}")]
        //public Response<Notification> Delete(int id)
        //{
        //    Notification objNotification = new Notification();
        //    objNotification.NotificationID = id;
        //    //objNotification.ModifiedBy = "Admin"; //curent user
        //    return objFactory.Delete(objNotification);
        //}
    }
}
