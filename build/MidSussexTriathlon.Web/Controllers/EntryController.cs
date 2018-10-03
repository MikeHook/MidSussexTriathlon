using MidSussexTriathlon.Core.Data;
using MidSussexTriathlon.Core.Domain;
using Stripe;
using System.Configuration;
using System.Linq;
using Umbraco.Web.WebApi;
using System.Web.Http;
using System;
using System.Globalization;
using MidSussexTriathlon.Core.Services;
using System.Web;

namespace MidSussexTriathlon.Web.Controllers
{
    public class EntryController : UmbracoApiController
    {
        IEntryRepository _entryRepository;
        IEmailService _emailService;

        public EntryController()
        {
            _entryRepository = new EntryRepository(new DataConnection());
            _emailService = new EmailService();
        }

        // GET: /umbraco/api/entry/placesleft
        [HttpGet]
        public int PlacesLeft()
        {
            var entryPage = Umbraco.TypedContentAtRoot().First().Children.FirstOrDefault(c => c.DocumentTypeAlias == "entry");
            int totalPlaces = 350;
            if (entryPage != null)
            {
                totalPlaces = (int)entryPage.GetProperty("totalPlaces").Value;
            }

            int entered = _entryRepository.Entered();

            return totalPlaces - entered;
        }

        // POST: /umbraco/api/entry/new
        [HttpPost]
        public string New(Entry entry)
        {
            entry.Paid = false;
            entry.DateOfBirth = DateTime.ParseExact(entry.DateOfBirthString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            entry = _entryRepository.Create(entry);

            string apiKey = ConfigurationManager.AppSettings["stripeSecretKey"];
            StripeConfiguration.SetApiKey(apiKey);

            int cost = 4000;
            var entryPage = Umbraco.TypedContentAtRoot().First().Children.FirstOrDefault(c => c.DocumentTypeAlias == "entry");
            if (entryPage != null) {
                var btfCost = (decimal)entryPage?.GetProperty("bTFCost")?.Value;
                var nonBtfCost = (decimal)entryPage?.GetProperty("nonBTFCost")?.Value;
                cost = string.IsNullOrEmpty(entry.BtfNumber) ? (int)(nonBtfCost * 100) : (int)(btfCost * 100);
            }

            var options = new StripeChargeCreateOptions
            {
                Amount = cost,
                Currency = "gbp",
                SourceTokenOrExistingSourceId = entry.TokenId,
                ReceiptEmail = entry.Email,
            };
            var service = new StripeChargeService();
            StripeCharge charge = service.Create(options);
            entry.OrderReference = charge.Id;
            entry.Paid = charge.Paid;

            if (!entry.Paid)
            {
                entry.PaymentFailureMessage = charge.FailureMessage;
                _entryRepository.Update(entry);
                return charge.FailureMessage;
            }
       
            _entryRepository.Update(entry);

            var subject = (string)entryPage?.GetProperty("confirmationEmailSubject")?.Value;
            var body = entryPage?.GetProperty("confirmationEmailBody")?.Value as HtmlString;
            _emailService.SendConfirmationEmail(entry, subject, body.ToHtmlString());
            _emailService.SendAdminConfirmationEmail(entry);
            return "";
        }  
    }
}
