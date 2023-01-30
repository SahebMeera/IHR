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
    public class TicketController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public TicketController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new TicketFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), _config);
        }


        [HttpGet]
        public Response<List<Ticket>> Get(int RequestedByID, int? AssignedToID)
        {
            Ticket objTicket = new Ticket();
            objTicket.RequestedByID = RequestedByID;
            objTicket.AssignedToID = AssignedToID;
            return objFactory.GetList(objTicket);
        }



        [HttpGet("{id}")]
        public Response<Ticket> Get(int id)
        {
            Ticket Cty = new Ticket();
            Cty.TicketID = id;
            return objFactory.GetByID(Cty);
        }

        [HttpPost]
        public Response<Ticket> Post([FromBody] Ticket Cty)
        {
            return objFactory.Save(Cty);
        }

        [HttpPut("{id}")]
        public Response<Ticket> Put(int id, [FromBody] Ticket Cty)
        {
            Cty.TicketID = id;
            return objFactory.Save(Cty);
        }

        [HttpDelete("{id}")]
        public Response<Ticket> Delete(int id)
        {
            Ticket Cty = new Ticket();
            Cty.TicketID = id;
            Cty.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(Cty);
        }
    }
}
