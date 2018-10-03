using MidSussexTriathlon.Core.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MidSussexTriathlon.Core.Services
{
    public interface IEmailService
    {
        void SendConfirmationEmail(Entry entry, string subject, string body);
        void SendAdminConfirmationEmail(Entry entry);
    }

    public class EmailService : IEmailService
    {
        bool TestMode => bool.Parse(ConfigurationManager.AppSettings["emailTestMode"]);

        public void SendConfirmationEmail(Entry entry, string subject, string body)
        {
            string fromAddress = ConfigurationManager.AppSettings["entryEmailUserName"];
            string password = ConfigurationManager.AppSettings["entryEmailPassword"];
            string toAddress = TestMode ? ConfigurationManager.AppSettings["emailTestAddress"] : entry.Email;

            SendEmail(toAddress, fromAddress, password, subject, body);
        }

        public void SendAdminConfirmationEmail(Entry entry)
        {
            string subject = "Mid Sussex Triathlon - New Entry";
            string content = $"<p>{entry.FirstName} {entry.LastName} has entered the event.</p><p>" +
                            $"<a href='https://midsussextriathlon.com/umbraco' target='_blank'>Check all entries in the Mid Sussex Triathlon CMS.</a></p>"; ;

            string fromAddress = ConfigurationManager.AppSettings["emailUserName"];
            string password = ConfigurationManager.AppSettings["emailPassword"];
            string toAddress = TestMode ? ConfigurationManager.AppSettings["emailTestAddress"] : ConfigurationManager.AppSettings["entryEmailUserName"];           

            SendEmail(toAddress, fromAddress, password, subject, content);
        }

        private void SendEmail(string toAddress, string fromAddress, string password, string subject, string htmlContent)
        { 
            var message = new MailMessage();
            message.To.Add(toAddress);
            message.From = new MailAddress(fromAddress, "Mid Sussex Triathlon");
            message.Subject = subject;
            message.IsBodyHtml = true;
            message.Body = htmlContent;

            var smtpClient = new ZohoSmtpClient(fromAddress, password);
            smtpClient.Send(message);            
        }

    }
}
