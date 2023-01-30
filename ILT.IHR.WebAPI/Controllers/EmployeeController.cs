using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ILT.IHR.DTO;
using ITL.IHR.Factory;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Microsoft.AspNetCore.Authorization;
using ILT.IHR.Factory;
using WebAPI.Controllers;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace ILT.IHR.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class EmployeeController : BaseController
    {
        AbstractFactory1 objFactory;
        AbstractFactory1 notificationFactory;
        private IConfiguration _config;

        public EmployeeController(IConfiguration config, IHttpContextAccessor contextAccessor): base(contextAccessor)
        {
            _config = config;
            string connectionString = _config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID);
            objFactory = new EmployeeFactory(connectionString, _config);
            notificationFactory = new NotificationFactory(connectionString, config);
        }

        [HttpGet]
        public Response<List<Employee>> Get()
        {
            Notification notification = new Notification();
            notification.TableName = "Employee";
            notification.UserID = this.CurrentUserID;
            List<Notification> empNotificationList = notificationFactory.GetList(notification).Data;

            Response<List<Employee>> empResponse = objFactory.GetList(new Employee());
            //empResponse = null;
            List<Employee> empList = empResponse.Data;

            if (empList != null && empList.Count > 0 && empNotificationList != null && empNotificationList.Count > 0)
            {
                empList.ForEach(
                    x =>
                        x.HasChange = empNotificationList.FirstOrDefault(y => y.RecordID == x.RecordID) != null ?
                            true : false);
                empResponse.Data = empList;
            }
            return empResponse;
        }

        [HttpGet("{id}")]
        public Response<Employee> Get(int id)
        {
            Employee Emp = new Employee();
            Emp.EmployeeID = id;
            //return objFactory.GetByID(Emp);
            return objFactory.GetRelatedObjectsByID(Emp);
        }

        [HttpPost]
        public Response<Employee> Post([FromBody] Employee Emp)
        {
            return objFactory.Save(Emp);
        }

        [HttpPut("{id}")]
        public Response<Employee> Put(int id, [FromBody] Employee Emp)
        {
            Emp.EmployeeID = id;
            return objFactory.Save(Emp);
        }

        [HttpDelete("{id}")]
        public Response<Employee> Delete(int id)
        {
            Employee Emp = new Employee();
            Emp.EmployeeID = id;
            Emp.ModifiedBy = "Admin"; //curent user
            return objFactory.Delete(Emp);
        }
        [HttpGet]
        [Route("GetEmployeeInfo", Name = "GetEmployeeInfo")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public Response<List<Employee>> GetEmployeeInfo()
        {
            string ConString = _config.GetConnectionString(this.ClientID);
            EmployeeFactory employeeFactory = new EmployeeFactory(ConString, _config);
            return employeeFactory.GetEmployeeInfo(new Employee());
        }

    }
}