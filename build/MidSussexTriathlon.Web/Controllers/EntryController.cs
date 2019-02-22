﻿using MidSussexTriathlon.Core.Data;
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

        // GET: /umbraco/api/entry/entered
        [HttpGet]
        public IEnumerable<Entry> Entered(string eventType)
        {
            return _entryRepository.GetEntered().Where(e => e.RaceType == eventType);
        }

        // POST: /umbraco/api/entry/new
        [HttpPost]
        public string New(Entry entry)
        {
            if (entry.Cost == 0)
            {
                return "Unable to complete entry as cost can not be calculated, please contact support.";
            }

            entry.Paid = false;
            entry.DateOfBirth = DateTime.ParseExact(entry.DateOfBirthString, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            string entryString = JsonConvert.SerializeObject(entry);
            Logger.Info(this.GetType(), "New entry request: {0}", () => entryString);

            entry = _entryRepository.Create(entry);

            string apiKey = ConfigurationManager.AppSettings["stripeSecretKey"];
            StripeConfiguration.SetApiKey(apiKey);

            int cost = entry.Cost * 100; 
            var options = new StripeChargeCreateOptions
            {
                Amount = cost,
                Currency = "gbp",
                SourceTokenOrExistingSourceId = entry.TokenId,
                ReceiptEmail = entry.Email,
                Description = $"{entry.RaceType} Entry",
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

            var entryPage = Umbraco.TypedContentAtRoot().First().Children.FirstOrDefault(c => c.DocumentTypeAlias == "entry");
            var subject = (string)entryPage?.GetProperty("confirmationEmailSubject")?.Value;
            var bodyProp = entryPage?.GetProperty("confirmationEmailBody")?.Value as HtmlString;
            var body = bodyProp.ToHtmlString().Replace("/{{Domain}}", "https://midsussextriathlon.com");
            _emailService.SendConfirmationEmail(entry, subject, body);
            _emailService.SendAdminConfirmationEmail(entry);
            return "";
        }  
    }
}
