using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MidSussexTriathlon.Core.Services
{
    public class ZohoSmtpClient : SmtpClient
    {
        public ZohoSmtpClient(string username, string password)
        {
            DeliveryMethod = SmtpDeliveryMethod.Network;
            Host = "smtp.zoho.com";
            Port = 587;
            UseDefaultCredentials = false;
            EnableSsl = true;
            Credentials = new NetworkCredential(username, password);
        }
    }
}
