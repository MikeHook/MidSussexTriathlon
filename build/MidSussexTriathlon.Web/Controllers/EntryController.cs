using MidSussexTriathlon.Core.Data;
using MidSussexTriathlon.Core.Domain;
using Stripe;
using System.Configuration;
using Umbraco.Web.WebApi;

namespace MidSussexTriathlon.Web.Controllers
{
    public class EntryController : UmbracoApiController
    {
        IEntryRepository _entryRepository;

        public EntryController()
        {
            _entryRepository = new EntryRepository(new DataConnection());
        }

        // POST: api/entry/new
        public string New(Entry entry)
        {
            entry.Paid = false;
            entry = _entryRepository.Create(entry);

            string apiKey = ConfigurationManager.AppSettings["stripeApiKey"];
            StripeConfiguration.SetApiKey(apiKey);

            var options = new StripeChargeCreateOptions
            {
                //TODO - Move costs into CMS
                Amount = string.IsNullOrEmpty(entry.BtfNumber) ? 4000 : 3700,
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
                //TODO - log failures into DB
                return charge.FailureMessage;
            }
       
            _entryRepository.Update(entry);
            return "";
        }  
    }
}
