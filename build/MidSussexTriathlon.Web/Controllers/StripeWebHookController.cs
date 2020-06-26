using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using MidSussexTriathlon.Core.Data;
using MidSussexTriathlon.Core.Services;
using Stripe;
using Umbraco.Web.WebApi;

namespace MidSussexTriathlon.Web.Controllers
{
    public class StripeWebHookController : UmbracoApiController
    {
        IEntryRepository _entryRepository;
        IEmailService _emailService;

        public StripeWebHookController()
        {
            _entryRepository = new EntryRepository(new DataConnection());
            _emailService = new EmailService();
        }


        // POST: /umbraco/api/StripeWebHook/Post
        [HttpPost]
        public async Task<IHttpActionResult> Post()
        {
            string apiKey = ConfigurationManager.AppSettings["stripeSecretKey"];
            StripeConfiguration.ApiKey = apiKey;            
            string endpointSecret = ConfigurationManager.AppSettings["stripeEndpointSecret"];

            var json = await Request.Content.ReadAsStringAsync();

            try
            {
                IEnumerable<string> stripesignatures = new List<string>();
                Request.Headers.TryGetValues("Stripe-Signature", out stripesignatures);

                var stripeEvent = EventUtility.ConstructEvent(json, stripesignatures.FirstOrDefault(), endpointSecret, throwOnApiVersionMismatch: false);

                // Handle the event
                if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    Logger.Info(this.GetType(), $"PaymentIntent was successful! {paymentIntent.ToString()}");
                    return ConfirmEntry(paymentIntent);
                }
                else
                {
                    // Unexpected event type
                    Logger.Info(this.GetType(), $"Unexpected stripe event type: {stripeEvent.Type}");
                    return BadRequest();
                }
            }
            catch (StripeException ex)
            {
                Logger.Info(this.GetType(), $"Stripe exception: {ex.ToString()}");

                return BadRequest();
            }
        }


        private IHttpActionResult ConfirmEntry(PaymentIntent paymentIntent)
        {
            var entry = _entryRepository.Get(paymentIntent.ClientSecret);
            if (entry == null)
            {
                Logger.Warn(this.GetType(), $"Unable to locate entry with client secret: {paymentIntent.ClientSecret}");
                return BadRequest();
            }

            entry.OrderReference = paymentIntent.Id;
            entry.Paid = true;           

            _entryRepository.Update(entry);

            var entryPage = Umbraco.TypedContentAtRoot().First().Children.FirstOrDefault(c => c.DocumentTypeAlias == "entry");
            var subject = (string)entryPage?.GetProperty("confirmationEmailSubject")?.Value;
            var bodyProp = entryPage?.GetProperty("confirmationEmailBody")?.Value as HtmlString;
            var body = bodyProp.ToHtmlString().Replace("/{{Domain}}", "https://midsussextriathlon.com");
            _emailService.SendConfirmationEmail(entry, subject, body);
            _emailService.SendAdminConfirmationEmail(entry);

            return Ok();
        }
    }
}
