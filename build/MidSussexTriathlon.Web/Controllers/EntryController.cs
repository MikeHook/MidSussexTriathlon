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
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MidSussexTriathlon.Web.Controllers
{
    public class EntryController : UmbracoApiController
    {
        IEntryRepository _entryRepository;

        public EntryController()
        {
            _entryRepository = new EntryRepository(new DataConnection());
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

        // GET: /umbraco/api/entry/entered
        [HttpGet]
        public IEnumerable<Entry> Entered(string eventType)
        {
            return _entryRepository.GetEntered().Where(e => e.RaceType == eventType);
        }

        // POST: /umbraco/api/entry/init
        [HttpPost]
        public string Init(Entry entry)
        {
            if (entry.Cost == 0)
            {
                return "Unable to complete entry as cost can not be calculated, please contact support.";
            }

            entry.Paid = false;
            entry.DateOfBirth = DateTime.ParseExact(entry.DateOfBirthString, "dd/MM/yyyy", CultureInfo.InvariantCulture);        

            string apiKey = ConfigurationManager.AppSettings["stripeSecretKey"];
            StripeConfiguration.ApiKey = apiKey;

            int cost = entry.Cost * 100;

            var options = new PaymentIntentCreateOptions
            {
                Amount = cost,
                Currency = "gbp",
                ReceiptEmail = entry.Email,
                Description = $"{entry.RaceType} Entry",
                // Verify your integration in this guide by including this parameter
                Metadata = new Dictionary<string, string>
                {
                  { "integration_check", "accept_a_payment" },
                },
            };
            
            var service = new PaymentIntentService();
            PaymentIntent paymentIntent = service.Create(options);

            entry.ClientSecret = paymentIntent.ClientSecret;
            string entryString = JsonConvert.SerializeObject(entry);
            Logger.Info(this.GetType(), "New entry request: {0}", () => entryString);

            entry = _entryRepository.Create(entry);

            return paymentIntent.ClientSecret;
        }        
    }
}
