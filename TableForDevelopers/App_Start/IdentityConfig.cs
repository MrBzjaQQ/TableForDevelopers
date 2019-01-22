using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using TableForDevelopers.Models;
using Twilio.Clients;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace TableForDevelopers.App_Start
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // настройка логина, пароля отправителя
            var from = ConfigurationManager.AppSettings["GmailUserName"];
            var pass = ConfigurationManager.AppSettings["GmailPassword"];
            var host = ConfigurationManager.AppSettings["GmailHost"];
            var port = Int32.Parse(ConfigurationManager.AppSettings["GmailPort"]);


            // адрес и порт smtp-сервера, с которого мы и будем отправлять письмо
            SmtpClient client = new SmtpClient(host, port);

            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(from, pass);
            client.EnableSsl = Boolean.Parse(ConfigurationManager.AppSettings["GmailSsl"]);

            // создаем письмо: message.Destination - адрес получателя
            var mail = new MailMessage(from, message.Destination);
            mail.Subject = message.Subject;
            mail.Body = message.Body;
            mail.IsBodyHtml = true;

            return client.SendMailAsync(mail);
        }
        public static ApplicationUserManager Create(
                        IdentityFactoryOptions<ApplicationUserManager> options,
                        IOwinContext context)
        {
            var manager = new ApplicationUserManager(
                       new UserStore<ApplicationUser>(context.Get<ApplicationContext>()));
            //..........................
            manager.EmailService = new EmailService();
            //.........................
            return manager;
        }
    }
    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            string AccountSid = ConfigurationManager.AppSettings["TwilioSid"]; //TwilioSid
            string AuthToken = ConfigurationManager.AppSettings["TwilioToken"]; //TwilioToken
            string twilioPhoneNumber = ConfigurationManager.AppSettings["TwilioNumber"]; //TwilioNumber
            TwilioClient.Init(AccountSid, AuthToken);
            var messageToSend = MessageResource.Create(
                from: twilioPhoneNumber,
                to: message.Destination,
                body: message.Body);
            return Task.FromResult(0);
        }
    }
}