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
        void SendConfirmationEmail(Entry entry);
    }

    public class EmailService : IEmailService
    {
        public void SendConfirmationEmail(Entry entry)
        {
            string subject = "Mid Sussex Triathlon - Entry Received";
            string content = "<p>Congratulations! You are now registered for the Mid Sussex Triathlon/Aquabike. " +
                 "Your entry will only be confirmed on the website when we have received your payment " +
                 "and your name has been entered into the '<a href='http://www.midsussextriclub.com/the-mid-sussex-triathlon/enter-race/entries-so-far.aspx'>Entries so far</a>' list. " +
                 "Please allow up to 48 hours for this. Please familiarise yourself with the <a href='http://www.midsussextriclub.com/the-mid-sussex-triathlon/race-info/race-instructions.aspx'>race instructions</a>.</p>" +
                 "<p>We do not post anything out so you will pick your number up at registration on the Saturday from 17:00 until 18:30 " +
                 "or on race day before 06:45 when registration closes.</p>" +
                 "<p>We are unable to run the course familiarisation day this year due to Wineham Lane being closed  until 25th May 2018.</p>" +                
                 "<p>Thanks</p>" +
                 "<p>Steve Mac<br/>Event Director</p>";

            SendEmail(entry.Email, "support@midsussextriathlon.com", subject, content);
        }

        private void SendEmail(string toAddress, string fromAddress, string subject, string htmlContent)
        {
            MailMessage objMail = new MailMessage();

            objMail.To.Add(toAddress);
            objMail.From = new MailAddress(fromAddress);
            objMail.Subject = subject;
            objMail.IsBodyHtml = true;
            objMail.Body = htmlContent;

            string emailUserName = ConfigurationManager.AppSettings["emailUserName"];
            if (string.IsNullOrWhiteSpace(emailUserName) == false)
            {
                var smtpClient = new ZohoSmtpClient();
                smtpClient.Send(objMail);
            }
        }

    }
}
