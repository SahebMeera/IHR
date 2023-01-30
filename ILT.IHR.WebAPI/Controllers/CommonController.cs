using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.DTO;
using ILT.IHR.Factory;
using ITL.IHR.Factory;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;

namespace WebAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class CommonController : BaseController
    {

        private IConfiguration _config;
        AbstractFactory1 emailJobFactory;
        public CommonController(IConfiguration config, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _config = config;
            string connectionString = _config.GetConnectionString(string.IsNullOrEmpty(this.ClientID) ? "IHRConString" : this.ClientID);
            emailJobFactory = new EmailJobFactory(connectionString, config);
        }
      
        [HttpPost]
        public Common Post([FromBody] Common Common)
        {
            try
            {
                var EmailFrom = _config["EmailNotifications:" + this.ClientID + ":FromEmail"];
                var UserId = _config["AppAccount:" + this.ClientID + ":UserID"];
                var password = _config["AppAccount:" + this.ClientID + ":Password"];
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
                    email.To.Add(MailboxAddress.Parse(!string.IsNullOrEmpty(Common.EmailTo) ? Common.EmailTo : _config["EmailNotifications:" + this.ClientID + ":HREmail"]));
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


                //// send email
                using var smtp = new SmtpClient();
                smtp.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate(UserId, password);
                smtp.Send(email);
                smtp.Disconnect(true);

                //EmailJob emailJob = new EmailJob();
                //emailJob.From = EmailFrom;
                //emailJob.To = Common.EmailTo; // Common.EmailToList ?
                //emailJob.EmailToList = Common.EmailToList;
                //emailJob.CC = Common.EmailCC; // Common.EmailCCList ?
                //emailJob.EmailCCList = Common.EmailCCList;
                //// emailJob.BCC = Common.bcc
                //emailJob.Subject = Common.EmailSubject;
                //emailJob.Body = Common.EmailBody;
                //emailJob.CreatedBy = Common.CreatedBy;
                
                //emailJobFactory.Save(email);


            }
            catch (Exception ex)
            {

            }
            finally
            { 

            }

            return Common;

        }
    }
}
