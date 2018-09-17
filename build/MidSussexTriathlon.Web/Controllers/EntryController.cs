using MidSussexTriathlon.Core.Data;
using MidSussexTriathlon.Core.Domain;
using MidSussexTriathlon.Web.Models.Payment;
using Stripe;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Web.WebApi;

namespace MidSussexTriathlon.Web.Controllers
{
    public class EntryController : UmbracoApiController
    {
        IEntryRepository _entryRepository;

        public EntryController()
        {
            //TODO - Add IoC
            _entryRepository = new EntryRepository(new DataConnection());
        }

        // POST: api/Entry/New
        public async Task<string> New(Entry entry)
        {
            _entryRepository.Create(entry);

            string apiKey = ConfigurationManager.AppSettings["stripeApiKey"];
            StripeConfiguration.SetApiKey(apiKey);

            var options = new StripeChargeCreateOptions
            {
                Amount = 999,
                Currency = "gbp",
                SourceTokenOrExistingSourceId = entry.TokenId,
                ReceiptEmail = entry.Email,
            };
            var service = new StripeChargeService();
            StripeCharge charge = service.Create(options);
            entry.OrderReference = charge.Id;
            entry.Paid = charge.Paid;

            //TODO - Update entry in the DB
            //_entryRepository.Create(entry);


            return "";
        }

        // POST: api/entry/ConfirmPayment
        public async Task<VerifyPaymentResponse> ConfirmPayment([FromBody]VerifyPaymentRequest request)
        {
            return new VerifyPaymentResponse();
        }
    }
}
