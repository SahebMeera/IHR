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
    public class DirectDepositController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public DirectDepositController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        { 
            _config = config;
            objFactory = new DirectDepositFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }

        [HttpGet]
        public Response<List<DirectDeposit>> Get()
        {
            return objFactory.GetList(new DirectDeposit());
        }

        [HttpGet("{id}")]
        public Response<DirectDeposit> Get(int id)
        {
            DirectDeposit Deposit = new DirectDeposit();
            Deposit.DirectDepositID = id;
            return objFactory.GetByID(Deposit);
        }

        [HttpPost]
        public Response<DirectDeposit>  Post([FromBody] DirectDeposit Deposit)
        {
            return objFactory.Save(Deposit);
        }

        [HttpPut("{id}")]
        public Response<DirectDeposit> Put(int id, [FromBody] DirectDeposit Deposit)
        {
            Deposit.DirectDepositID = id;
            return objFactory.Save(Deposit);
        }

        [HttpDelete("{id}")]
        public Response<DirectDeposit> Delete(int id)
        {
            DirectDeposit Deposit = new DirectDeposit();
            Deposit.DirectDepositID = id;
            Deposit.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(Deposit);
        }
    }
}
