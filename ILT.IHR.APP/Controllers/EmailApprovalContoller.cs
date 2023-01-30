using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ILT.IHR.DTO;
using ILT.IHR.Factory;
using ITL.IHR.Factory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;

namespace ILT.IHR.APP.Controllers
{
    [ApiController]
    // [Authorize]
    [Route("api/[controller]")]
    public class EmailApprovalController : BaseController
    {

        AbstractFactory1 objFactory;
        private IConfiguration _config;

        public EmailApprovalController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            objFactory = new EmailApprovalFactory(_config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID), config);
        }



        [HttpGet]
        public Response<List<EmailApproval>> Get()
        {
            return objFactory.GetList(new EmailApproval());
        }

        [HttpGet("{id}")]
        public Response<EmailApproval> Get(Guid id)
        {
            EmailApproval objEmailApproval = new EmailApproval();
            objEmailApproval.LinkID = id;
            return objFactory.GetByID(objEmailApproval);
        }

        [HttpPost]
        public Response<EmailApproval> Post([FromBody] EmailApproval objEmailApproval)
        {
            return objFactory.Save(objEmailApproval);
        }

        [HttpPut("{id}")]
        public Response<EmailApproval> Put(int id, [FromBody] EmailApproval objEmailApproval)
        {
            objEmailApproval.EmailApprovalID = id;
            return objFactory.Save(objEmailApproval);
        }

        [HttpDelete("{id}")]
        public Response<EmailApproval> Delete(int id)
        {
            EmailApproval objEmailApproval = new EmailApproval();
            objEmailApproval.EmailApprovalID = id;
            return objFactory.Delete(objEmailApproval);
        }

        [AllowAnonymous]
        [Route("EmailApproval", Name = "EmailApproval")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpGet]
        public async Task<Response<EmailApproval>> EamilApprovalAction(string ClientID, Guid LinkID, string Value)
        {
            return await approveOrDenyTimesheet(ClientID, LinkID, Value);
        }

        //[Route("ApproveEmail", Name = "ApproveEmail")]
        [HttpGet("{ClientID}/{LinkID}/{Value}/{Module}")]
        public async Task<Response<EmailApproval>> ApproveEmail(string ClientID, Guid LinkID, string Value, string Module = "Timesheet")
        {
            if (Module.ToUpper() == "TIMESHEET")
            {
                return await approveOrDenyTimesheet(ClientID, LinkID, Value);
            }
            else
            {
                return await approveOrDenyLeave(ClientID, LinkID, Value);
            }
        }

        private async Task<Response<EmailApproval>> approveOrDenyTimesheet(string ClientID, Guid LinkID, string Value) {
            EmailApproval objEmailApproval = new EmailApproval();
            objEmailApproval.LinkID = LinkID;
            objEmailApproval.Value = Value;

            string ConString = _config.GetConnectionString(ClientID);

            EmailApprovalFactory emailApprovalFactory = new EmailApprovalFactory(ConString, _config);
            Response<EmailApproval> responseEmailApproval = emailApprovalFactory.GetByID(objEmailApproval);

            if (responseEmailApproval.Data.IsActive == true || Value == "VOID")
            {
                if (DateTime.Now <= responseEmailApproval.Data.ValidTime || Value == "VOID")
                {
                    TimeSheet objTimeSheet = new TimeSheet();
                    objTimeSheet.TimeSheetID = responseEmailApproval.Data.ID;
                    TimeSheetFactory timeSheetFactory = new TimeSheetFactory(ConString, _config);
                    Response<TimeSheet> responseTS = timeSheetFactory.GetRelatedObjectsByID(objTimeSheet);

                    Response<EmailApproval> responseEA = emailApprovalFactory.EmailApprovalAction(objEmailApproval);
                    // await sendEMail(responseTS.Data, ClientID, Value);
                    EmailFields emailFields = await prepareEMail(responseTS.Data, ClientID, Value);

                    var emailApproval = responseEmailApproval.Data;
                    if (Value == "VOID")
                    {
                        emailApproval.IsActive = true;
                        emailApproval.Value = null;
                    }
                    emailApproval.EmailApprovalID = 0;
                    emailApproval.LinkID = Guid.Empty;
                    emailApproval.EmailSubject = emailFields.EmailSubject;
                    emailApproval.EmailBody = emailFields.EmailBody;
                    emailApproval.EmailCC = emailFields.EmailCC;
                    emailApproval.EmailTo = emailFields.EmailTo;
                    // emailApproval.Value = Value.ToUpper();
                    emailApprovalFactory.Save(emailApproval);

                    Response<EmailApproval> responseR = new Response<EmailApproval>();
                    responseR.Data = new EmailApproval();
                    responseR.MessageType = MessageType.Success;
                    //TimeSheet.Data.FileName = FileName;

                    return responseR;
                }
                else
                {
                    Response<EmailApproval> responseR = new Response<EmailApproval>();
                    responseR.Data = new EmailApproval();
                    responseR.MessageType = MessageType.Error;
                    return responseR;
                  //  return _config["EmailApprovalMessages:TimeSheetLinkExpired"];
                }

            }
            else
            {
                Response<EmailApproval> responseR = new Response<EmailApproval>();
                responseR.Data = new EmailApproval();
                responseR.MessageType = MessageType.Error;
                return responseR;
              //  return _config["EmailApprovalMessages:TimeSheetLink" + responseEmailApproval.Data.Value.ToLower()];
            }

        }

        private async Task<Response<EmailApproval>> approveOrDenyLeave(string ClientID, Guid LinkID, string Value)
        {
            EmailApproval objEmailApproval = new EmailApproval();
            objEmailApproval.LinkID = LinkID;
            objEmailApproval.Value = Value;
            string ConString = _config.GetConnectionString(ClientID);
            EmailApprovalFactory emailApprovalFactory = new EmailApprovalFactory(ConString, _config);
            Response<EmailApproval> responseEA = emailApprovalFactory.EmailApprovalAction(objEmailApproval);
            Response<EmailApproval> responseR = new Response<EmailApproval>();
            responseR.Data = new EmailApproval();
            responseR.MessageType = MessageType.Success;
            return responseR;
            //TimeSheet.Data.FileName = FileName;
            // return responseEA.MessageType.ToString();
        }

        private async Task<EmailFields> prepareEMail(TimeSheet timesheet, string ClientID, string Value)
        {
            Employee objEmployee = new Employee();
            objEmployee.EmployeeID = (int)timesheet.EmployeeID;
            string ConString = _config.GetConnectionString(ClientID);
            EmployeeFactory employeeFactory = new EmployeeFactory(ConString, _config);
            Response<Employee> responseEmployee = employeeFactory.GetRelatedObjectsByID(objEmployee);
            Employee employeeDetails = responseEmployee.Data;
            var tableInfo = "";
            tableInfo = tableInfo + "<tr><th style='border: 1px solid black;'>Work Date</th><th style='border: 1px solid black;'>Project</th>" +
                "<th style='border: 1px solid black;'>Activity</th><th style='border: 1px solid black;'>Hours</th>";
            timesheet.TimeEntries.ForEach(te =>
            {
                tableInfo = tableInfo + "<tr><td style='border: 1px solid black;' width='15%'>&nbsp;" + te.WorkDate.ToString("MM/dd/yyyy") + "</td>" +
                                            "<td style='border: 1px solid black;' width='35%'>&nbsp;" + te.Project + "</td>" +
                                            "<td style='border: 1px solid black;' width='40%'>&nbsp;" + te.Activity + "</td>" +
                                            "<td style='border: 1px solid black;' width='10%' align='right'>" + te.Hours + "&nbsp;</td>";
            });

            EmailFields emailFields = new EmailFields();

            emailFields.isMultipleEmail = true;
            emailFields.EmailCCList = new List<string>();
            //common.EmailTo = employeeDetails.Email;            
            emailFields.EmailTo = !string.IsNullOrEmpty(employeeDetails.LoginEmail) ? employeeDetails.LoginEmail : employeeDetails.Email;
            emailFields.EmailCCList.Add(_config["EmailNotifications:" + ClientID + ":Timesheet"]);
            emailFields.EmailCCList.Add(timesheet.TSApproverEmail);
            if (!string.IsNullOrEmpty(timesheet.ApprovedEmailTo))
            {
                List<string> ApprovedEmailTo = timesheet.ApprovedEmailTo.Split(';').ToList();
                emailFields.EmailCCList.AddRange(ApprovedEmailTo);
            }

            emailFields.EmailCC = string.Join(";", emailFields.EmailCCList);

            if (Value.ToUpper() == "APPROVE")
            {
                timesheet.Status = "Approved";
                if (timesheet.TimeSheetType.ToUpper() == "MONTHLY")
                {
                    emailFields.EmailSubject = "Timesheet approved for " + employeeDetails.EmployeeName + " for month ending " + Convert.ToDateTime(timesheet.WeekEnding).ToString("MM/dd/yyyy");
                }
                else
                {
                    emailFields.EmailSubject = "Timesheet approved for " + employeeDetails.EmployeeName + " for week ending " + Convert.ToDateTime(timesheet.WeekEnding).ToString("MM/dd/yyyy");
                }
            }
            else if (Value.ToUpper() == "REJECT")
            {
                timesheet.Status = "Rejected";
                if (timesheet.TimeSheetType.ToUpper() == "MONTHLY")
                {
                    emailFields.EmailSubject = "Timesheet rejected for " + employeeDetails.EmployeeName + " for month ending " + Convert.ToDateTime(timesheet.WeekEnding).ToString("MM/dd/yyyy");
                }
                else
                {
                    emailFields.EmailSubject = "Timesheet rejected for " + employeeDetails.EmployeeName + " for week ending " + Convert.ToDateTime(timesheet.WeekEnding).ToString("MM/dd/yyyy");
                }
            }
            else if (Value.ToUpper() == "VOID")
            {
                timesheet.Status = "Void";
                if (timesheet.TimeSheetType.ToUpper() == "MONTHLY")
                {
                    emailFields.EmailSubject = "Timesheet voided for " + employeeDetails.EmployeeName + " for month ending " + Convert.ToDateTime(timesheet.WeekEnding).ToString("MM/dd/yyyy");
                }
                else
                {
                    emailFields.EmailSubject = "Timesheet voided for " + employeeDetails.EmployeeName + " for week ending " + Convert.ToDateTime(timesheet.WeekEnding).ToString("MM/dd/yyyy");
                }
            }

            var emailBody = "<table><tr><td><b>Employee</b></td>" + "<td>: " + timesheet.EmployeeName + "</td></tr>";
            if (timesheet.TimeSheetType.ToUpper() == "MONTHLY")
            {
                emailBody = emailBody + "<tr><td><b>Month Ending</b></td>" + "<td>: " + Convert.ToDateTime(timesheet.WeekEnding).ToString("MM/dd/yyyy") + "</td></tr>";
            }
            else
            {
                emailBody = emailBody + "<tr><td><b>Week Ending</b></td>" + "<td>: " + Convert.ToDateTime(timesheet.WeekEnding).ToString("MM/dd/yyyy") + "</td></tr>";
            }
            emailBody = emailBody + "<tr><td><b>Client</b></td>" + "<td>: " + timesheet.ClientName + "</td></tr>";
            if (timesheet.ClientManager != null && timesheet.ClientManager != "")
            {
                emailBody = emailBody + "<tr><td><b>Client Manager</b></td>" + "<td>: " + timesheet.ClientManager + "</td></tr>";
            }
            if (employeeDetails.Manager != null && employeeDetails.Manager != "")
            {
                emailBody = emailBody + "<tr><td><b>Manager</b></td>" + "<td>: " + employeeDetails.Manager + "</td></tr>";
            }
            string status = (timesheet.Status.ToUpper() == "VOID" ? "Voided" : timesheet.Status);
            string modifiedBy = (timesheet.Status.ToUpper() == "VOID" ? timesheet.ModifiedBy : timesheet.TSApproverEmail);
            emailBody = emailBody + "<tr><td><b>Status</b></td>" + "<td>: " + timesheet.Status + "</td></tr>" +
            "<tr><td><b>Total Hours</b></td>" + "<td>: " + timesheet.TotalHours + "</td></tr></table><br/><br/>" +
            "<table style='border: 1px solid black; border-collapse:collapse;' width='80%'>" + tableInfo + "</table><br/>" +
            "<table><tr><td>Timesheet " + status + " by " + modifiedBy  + " on " + DateTime.Now.ToString("dd MMM yyy HH:mm:ss") + " GMT</td></tr></table>";

            emailFields.EmailBody = emailBody;
            return emailFields;
        }

        private async Task sendEMail(TimeSheet timesheet, string ClientID, string Value)
        {
            Employee objEmployee = new Employee();
            objEmployee.EmployeeID = (int)timesheet.EmployeeID;
            string ConString = _config.GetConnectionString(ClientID);
            EmployeeFactory employeeFactory = new EmployeeFactory(ConString, _config);
            Response<Employee> responseEmployee = employeeFactory.GetRelatedObjectsByID(objEmployee);
            Employee employeeDetails = responseEmployee.Data;
            //UserFactory userFactory = new UserFactory(ConString);
            //Response<List<User>> responseUserList = userFactory.GetList(new User());
            //List<User> UserList = responseUserList.Data;            
            var tableInfo = "";
            tableInfo = tableInfo + "<tr><th style='border: 1px solid black;'>Work Date</th><th style='border: 1px solid black;'>Project</th>" +
                "<th style='border: 1px solid black;'>Activity</th><th style='border: 1px solid black;'>Hours</th>";
            timesheet.TimeEntries.ForEach(te =>
            {
                tableInfo = tableInfo + "<tr><td style='border: 1px solid black;' width='15%'>&nbsp;" + te.WorkDate.ToString("MM/dd/yyyy") + "</td>" +
                                            "<td style='border: 1px solid black;' width='35%'>&nbsp;" + te.Project + "</td>" +
                                            "<td style='border: 1px solid black;' width='40%'>&nbsp;" + te.Activity + "</td>" +
                                            "<td style='border: 1px solid black;' width='10%' align='right'>" + te.Hours + "&nbsp;</td>";
            });

            Common common = new Common();
            common.isMultipleEmail = true;
            common.EmailCCList = new List<string>();
            //common.EmailTo = employeeDetails.Email;            
            common.EmailTo = !string.IsNullOrEmpty(employeeDetails.LoginEmail) ? employeeDetails.LoginEmail : employeeDetails.Email;
            common.EmailCCList.Add(_config["EmailNotifications:" + ClientID + ":Timesheet"]);
            common.EmailCCList.Add(timesheet.TSApproverEmail);
            if (!string.IsNullOrEmpty(timesheet.ApprovedEmailTo))
            {
                List<string> ApprovedEmailTo = timesheet.ApprovedEmailTo.Split(';').ToList();
                common.EmailCCList.AddRange(ApprovedEmailTo);
            }

            if (Value.ToUpper() == "APPROVE")
            {
                timesheet.Status = "Approved";
                if (timesheet.TimeSheetType.ToUpper() == "MONTHLY")
                {
                    common.EmailSubject = "Timesheet approved for " + employeeDetails.EmployeeName + " for month ending " + Convert.ToDateTime(timesheet.WeekEnding).ToString("MM/dd/yyyy");
                }
                else
                {
                    common.EmailSubject = "Timesheet approved for " + employeeDetails.EmployeeName + " for week ending " + Convert.ToDateTime(timesheet.WeekEnding).ToString("MM/dd/yyyy");
                }
            }
            else if (Value.ToUpper() == "REJECT")
            {
                timesheet.Status = "Rejected";
                if (timesheet.TimeSheetType.ToUpper() == "MONTHLY")
                {
                    common.EmailSubject = "Timesheet rejected for " + employeeDetails.EmployeeName + " for month ending " + Convert.ToDateTime(timesheet.WeekEnding).ToString("MM/dd/yyyy");
                }
                else
                {
                    common.EmailSubject = "Timesheet rejected for " + employeeDetails.EmployeeName + " for week ending " + Convert.ToDateTime(timesheet.WeekEnding).ToString("MM/dd/yyyy");
                }
            }

            var emailBody = "<table><tr><td><b>Employee</b></td>" + "<td>: " + timesheet.EmployeeName + "</td></tr>";
            if (timesheet.TimeSheetType.ToUpper() == "MONTHLY")
            {
                emailBody = emailBody + "<tr><td><b>Month Ending</b></td>" + "<td>: " + Convert.ToDateTime(timesheet.WeekEnding).ToString("MM/dd/yyyy") + "</td></tr>";
            }
            else
            {
                emailBody = emailBody + "<tr><td><b>Week Ending</b></td>" + "<td>: " + Convert.ToDateTime(timesheet.WeekEnding).ToString("MM/dd/yyyy") + "</td></tr>";
            }
            emailBody = emailBody + "<tr><td><b>Client</b></td>" + "<td>: " + timesheet.ClientName + "</td></tr>";
            if (timesheet.ClientManager != null && timesheet.ClientManager != "")
            {
                emailBody = emailBody + "<tr><td><b>Client Manager</b></td>" + "<td>: " + timesheet.ClientManager + "</td></tr>";
            }
            if (employeeDetails.Manager != null && employeeDetails.Manager != "")
            {
                emailBody = emailBody + "<tr><td><b>Manager</b></td>" + "<td>: " + employeeDetails.Manager + "</td></tr>";
            }
            emailBody = emailBody + "<tr><td><b>Status</b></td>" + "<td>: " + timesheet.Status + "</td></tr>" +
            "<tr><td><b>Total Hours</b></td>" + "<td>: " + timesheet.TotalHours + "</td></tr></table><br/><br/>" +
            "<table style='border: 1px solid black; border-collapse:collapse;' width='80%'>" + tableInfo + "</table><br/>" +
            "<table><tr><td>Timesheet " + timesheet.Status + " by " + timesheet.TSApproverEmail + " on " + DateTime.Now.ToString("dd MMM yyy HH:mm:ss") + " GMT</td></tr></table>";

            common.EmailBody = emailBody;
            var result = Post(common, ClientID);
        }

        private Common Post([FromBody] Common Common, string ClientID)
        {
            var EmailFrom = _config["EmailNotifications:" + ClientID + ":FromEmail"];
            var UserId = _config["AppAccount:" + ClientID + ":UserID"];
            var password = _config["AppAccount:" + ClientID + ":Password"];
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(EmailFrom));
            if (Common.EmailToList != null && Common.EmailToList.Count > 0)
            {
                InternetAddressList list = new InternetAddressList();
                foreach (var emailAddress in Common.EmailToList)
                {
                    list.Add(MailboxAddress.Parse(emailAddress));
                }
                email.To.AddRange(list);
            }
            else
            {
                email.To.Add(MailboxAddress.Parse(!string.IsNullOrEmpty(Common.EmailTo) ? Common.EmailTo : _config["EmailNotifications:" + ClientID + ":Timesheet"]));
            }

            if (Common.EmailCCList != null && Common.EmailCCList.Count > 0)
            {
                InternetAddressList list = new InternetAddressList();
                foreach (var emailAddress in Common.EmailCCList)
                {
                    list.Add(MailboxAddress.Parse(emailAddress));
                }
                email.Cc.AddRange(list);
            }
            else if (!string.IsNullOrEmpty(Common.EmailCC))
            {
                email.Cc.Add(MailboxAddress.Parse(Common.EmailCC));
            }

            email.Subject = Common.EmailSubject;
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = Common.EmailBody
            };
            // send email
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(UserId, password);
            smtp.Send(email);
            smtp.Disconnect(true);
            return Common;
        }

        private async void TriggerWebJob()
        {
            string ApiBaseAddress = _config["AzureWebJobSettings:ApiBaseAddress"];
            string UserId = _config["AzureWebJobSettings:UserId"];
            string Password = _config["AzureWebJobSettings:Password"];
            string WebJobPath = _config["AzureWebJobSettings:WebJobPath"];
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(ApiBaseAddress);
            var byteArray = Encoding.ASCII.GetBytes(UserId+":"+Password);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            var responseData = await client.PostAsync(WebJobPath+"run", null);
        }

    }
}