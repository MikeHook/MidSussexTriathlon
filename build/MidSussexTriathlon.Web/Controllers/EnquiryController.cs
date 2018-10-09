using MidSussexTriathlon.Core.Model;
using MidSussexTriathlon.Core.Services;
using System.Web.Mvc;
using Umbraco.Web.WebApi;

namespace MidSussexTriathlon.Web.Controllers
{
    public class EnquiryController : UmbracoApiController
    {
        IEmailService _emailService;

        public EnquiryController()
        {
            _emailService = new EmailService();
        }        

        // POST: /umbraco/api/enquiry/send
        [HttpPost]
        public void Send(Contact contact)
        {
            _emailService.SendEnquiryEmail(contact);
        }
    }
}