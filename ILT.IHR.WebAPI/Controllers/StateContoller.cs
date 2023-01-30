using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;
using ITL.IHR.Factory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class StateController : BaseController
    {

        AbstractFactory objFactory;
        private IConfiguration _config;

        public StateController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new StateFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID));
        }

        [HttpGet]
        public List<State> Get()
        {
            return objFactory.GetList(new State());
        }

        [HttpGet("{id}")]
        public State Get(int id)
        {
            State objState = new State();
            objState.StateID = id;
            return objFactory.GetByID(objState);
        }

        [HttpPost]
        public bool Post([FromBody] State objState)
        {
            return objFactory.Save(objState);
        }

        [HttpPut("{id}")]
        public bool Put(int id, [FromBody] State objState)
        {
            objState.StateID = id;
            return objFactory.Save(objState);
        }

        [HttpDelete("{id}")]
        public bool Delete(int id)
        {
            State objState = new State();
            objState.StateID = id;
            objState.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(objState);
        }
    }
}
