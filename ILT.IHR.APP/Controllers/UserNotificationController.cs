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
    public class UserNotificationController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public UserNotificationController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new UserNotificationFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }

        [HttpGet("{UserID}")]
        public Response<List<UserNotification>> Get(int UserID)
        {
            UserNotification objNotification = new UserNotification();
            objNotification.UserID = UserID;
            return objFactory.GetList(objNotification);
        }


        [HttpPut("{NotificationID}")]
        public Response<UserNotification> Put(int NotificationID, [FromBody] UserNotification notification)
        {
            UserNotification objNotification = new UserNotification();
            objNotification.NotificationID = NotificationID;
            objNotification.IsAck = true;
            return objFactory.Save(objNotification);
        }

    }
}
