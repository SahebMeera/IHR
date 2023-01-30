using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ILT.IHR.Factory;
using ITL.IHR.Factory;
using ILT.IHR.DTO;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace EmailJobWorker
{
    public class EmailJob : BackgroundService
    {
        private readonly ILogger<EmailJob> _logger;
        private IConfiguration _config;
        int ReminderDurationInDays = 1; // in days
        List<string> Clients;

        public EmailJob(ILogger<EmailJob> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            Clients = _config["ClientID"].Split(',').ToList(); 
            this.ReminderDurationInDays = Convert.ToInt32(_config["ReminderDurationInDays"]);
        }



        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Email job running at: {time}", DateTimeOffset.Now);
                Parallel.ForEach(Clients, ClientID => this.processEmailThread(ClientID) );
                int JobDuration = Convert.ToInt32(_config["JobDuration"]);
                await Task.Delay(JobDuration, stoppingToken);
            }
        }


        private void processEmailThread(string ClientID) {
            try
            {
                AbstractFactory1 objFactory;
                EmailApprovalFactory emailApprovalFactory;
                objFactory = new EmailApprovalFactory(_config.GetConnectionString(ClientID), _config);
                emailApprovalFactory = new EmailApprovalFactory(_config.GetConnectionString(ClientID), _config);
                List<EmailApproval> emailApprovalList = objFactory.GetList(new EmailApproval()).Data;
                var emailsToSend = emailApprovalList.Where(x => x.IsActive == true
                && !String.IsNullOrEmpty(x.EmailBody) && !String.IsNullOrEmpty(x.EmailSubject)
                && isReminder(x)).ToList();
                sendMail(ClientID, emailsToSend, emailApprovalFactory);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message);
            }
            finally
            {

            }
        }
        

        private bool isReminder(EmailApproval emailApproval) 
        {
            return (emailApproval.SentCount == 0) ||
                 (emailApproval.SendDate.AddDays(ReminderDurationInDays * emailApproval.SentCount) < DateTime.Now && emailApproval.LinkID != Guid.Empty);
        }


        private int sendMail(string ClientID, List<EmailApproval> emailsToSend, EmailApprovalFactory emailApprovalFactory) {

            foreach (EmailApproval emailtoSend in emailsToSend) {
                try
                {
                    _logger.LogInformation("Email is being sent for Client {ClientID} at {time}", ClientID, DateTime.Now);
                    var emailFrom = _config["FromEmail:" + ClientID];
                    var userId = _config["AppAccount:" + ClientID + ":UserID"];
                    var password = _config["AppAccount:" + ClientID + ":Password"];
                    var email = new MimeMessage();
                    email.From.Add(MailboxAddress.Parse(emailFrom));
                    // email.To.Add(MailboxAddress.Parse(!string.IsNullOrEmpty(emailtoSend.EmailTo) ? emailtoSend.EmailTo : _config["EmailNotifications:" + ClientID + ":HREmail"]));
                    email.To.Add(MailboxAddress.Parse(emailtoSend.EmailTo));

                    if (!string.IsNullOrEmpty(emailtoSend.EmailCC))
                    {
                        List<string> ccList = emailtoSend.EmailCC.Split(';').ToList();
                        foreach (string cc in ccList)
                        {
                            email.Cc.Add(MailboxAddress.Parse(cc));
                        }
                    }
                    bool isReminder = emailtoSend.SentCount != 0 && (emailtoSend.SendDate.AddDays(ReminderDurationInDays * emailtoSend.SentCount) < DateTime.Now && emailtoSend.LinkID != Guid.Empty);
                    if (isReminder)
                    {
                        email.Subject = "Reminder : "+emailtoSend.EmailSubject;
                    }
                    else
                    {
                        email.Subject = emailtoSend.EmailSubject;
                    }
                    
                    email.Body = new TextPart(TextFormat.Html)
                    {
                        Text = emailtoSend.EmailBody
                    };


                    //// send email
                    using var smtp = new SmtpClient();
                    smtp.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);
                    smtp.Authenticate(userId, password);
                    smtp.Send(email);
                    smtp.Disconnect(true);

                    if (emailtoSend.LinkID == Guid.Empty) 
                    {
                        emailtoSend.IsActive = false;
                    }

                    emailApprovalFactory.OnEmailSent(emailtoSend);
                    _logger.LogInformation("Email sent for EmailApprovalID : {EmailApprovalID} and  Client : {ClientID} at {time}", emailtoSend.EmailApprovalID, ClientID, DateTime.Now);

                }
                catch (Exception ex)
                {
                    _logger.LogError("Error ocurred for EmailApprovalID : {EmailApprovalID} and  Client : {ClientID} at {time}", emailtoSend.EmailApprovalID, ClientID, DateTime.Now);
                }
                finally
                {

                }
            }
            
            return 0;

        }


    }
}
