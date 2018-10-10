using MidSussexTriathlon.Core.Domain;
using MidSussexTriathlon.Core.Model;
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
        void SendEnquiryEmail(Contact contact);
    }

    public class EmailService : IEmailService
    {
        public Dictionary<string, string> EmailLookup = new Dictionary<string, string>()
        {
            {"race", "race@midsussextriathlon.com"},
            {"question", "questions@midsussextriathlon.com"},
            {"sponsor", "sponsorship@midsussextriclub.com"},
        };

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

        public void SendEnquiryEmail(Contact contact)
        {
            string fromAddress = ConfigurationManager.AppSettings["emailUserName"];
            string password = ConfigurationManager.AppSettings["emailPassword"];
   
            string toAddress;
            if (EmailLookup.TryGetValue(contact.Recipient, out toAddress) == false)
            {
                toAddress = ConfigurationManager.AppSettings["entryEmailUserName"];
            }      

            if (TestMode)
            {
                toAddress = ConfigurationManager.AppSettings["emailTestAddress"];
            }

            string subject = "Mid Sussex Triathlon - Enquiry";
            string content = $"<p>Name: {contact.Name}</p>" +
                $"<p>Email: {contact.Email}</p>" +
                $"<p>Message: {contact.Message}</p>";

            var replyTo = new MailAddress(contact.Email);

            SendEmail(toAddress, fromAddress, password, subject, content, replyTo);
        }

        private void SendEmail(string toAddress, string fromAddress, string password, string subject, string htmlContent, MailAddress replyTo = null)
        { 
            var message = new MailMessage();
            message.To.Add(toAddress);
            if (replyTo != null) {
                message.ReplyToList.Add(replyTo);
            }
            message.From = new MailAddress(fromAddress, "Mid Sussex Triathlon");
            message.Subject = subject;
            message.IsBodyHtml = true;
            message.Body = htmlContent;

            var smtpClient = new ZohoSmtpClient(fromAddress, password);
            smtpClient.Send(message);            
        }

    }
}
