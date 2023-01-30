using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;
using ILT.IHR.Factory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebAPI.Controllers;

namespace ILT.IHR.WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FormI9ChangeSetController : BaseController
    {
        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public FormI9ChangeSetController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new FormI9ChangeSetFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }

        [HttpGet("{formi9id}")]
        public Response<List<FormI9ChangeSet>> Get(int formi9id)
        {
            FormI9ChangeSet formi9ChangeSet = new FormI9ChangeSet();
            formi9ChangeSet.FormI9ID = formi9id;            
            return objFactory.GetList(formi9ChangeSet);
        }

    }
}