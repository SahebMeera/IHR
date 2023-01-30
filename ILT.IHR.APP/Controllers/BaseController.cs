using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ILT.IHR.DTO;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ILT.IHR.APP.Controllers
{
    public class BaseController: ControllerBase
    {
        protected string ClientID { get; set; }

        protected int CurrentUserID { get; set; }

        public BaseController(IHttpContextAccessor contextAccessor)
        {
            var authenticatedUser = contextAccessor.HttpContext.User.Identity.Name;
            IEnumerable<Claim> claims = contextAccessor.HttpContext.User.Claims;
            if (claims != null)
            {
                this.ClientID = claims.Where(x => x.Type == "ClientID").Select(c => c.Value).SingleOrDefault();
                this.CurrentUserID =  Convert.ToInt32(claims.Where(x => x.Type == "UserID").Select(c => c.Value).SingleOrDefault());
            }
        }
    }

    public enum SessionVariable
    {
        LoggedInUserID,
        LoggedInUserName,
        IsSessionActive
    }
}
