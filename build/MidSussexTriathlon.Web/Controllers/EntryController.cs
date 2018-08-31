using MidSussexTriathlon.Core.Data;
using MidSussexTriathlon.Core.Domain;
using MidSussexTriathlon.Web.Models;
using MidSussexTriathlon.Web.Models.Payment;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
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
            entry.OrderReference = Guid.NewGuid().ToString();

            _entryRepository.Create(entry);

            string merchantAccount = ConfigurationManager.AppSettings["adyenMerchantAccount"];

            var url = HttpContext.Current.Request.Url;
            string originUrl = string.Format("{0}://{1}{2}", url.Scheme, url.Host, url.Port == 80 ? string.Empty : ":" + url.Port);
            string returnUrl = $"{originUrl}/entry/result";

            var paymentSession = new PaymentSessionRequest()
            {
                amount = new PaymentSessionRequest.AmountModel()
                {
                    currency = "GBP",
                    value = string.IsNullOrEmpty(entry.BtfNumber) ? "4000" : "3700" //TODO - Move prices into Umbraco
                },
                reference = entry.OrderReference.ToString(),
                merchantAccount = merchantAccount,
                sdkVersion = "1.3.2",
                shopperReference = entry.Name,
                channel = "Web",
                html = true,
                origin = originUrl,
                returnUrl = returnUrl,
                countryCode = "GB",
                shopperLocale = "gb_GB"
            };

            string paymentSessionUrl = "https://checkout-test.adyen.com/v32/paymentSession";    //TODO - Move to config

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            using (var client = new HttpClient())
            {
                var adyenApiKey = ConfigurationManager.AppSettings["adyenApiKey"];
                client.DefaultRequestHeaders.Add("X-API-Key", adyenApiKey);
                var response = await client.PostAsJsonAsync(paymentSessionUrl, paymentSession);

                var responseString = await response.Content.ReadAsStringAsync();
                var paymentSessionResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<PaymentSessionResponse>(responseString);
                return paymentSessionResponse.paymentSession;
            }
        }

        // POST: api/entry/ConfirmPayment
        public async Task<VerifyPaymentResponse> ConfirmPayment([FromBody]VerifyPaymentRequest request)
        {
            string paymentResultUrl = "https://checkout-test.adyen.com/v32/payments/result"; //TODO - Move to config

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            using (var client = new HttpClient())
            {
                var adyenApiKey = ConfigurationManager.AppSettings["adyenApiKey"];
                client.DefaultRequestHeaders.Add("X-API-Key", adyenApiKey);
                var response = await client.PostAsJsonAsync(paymentResultUrl, request);

                var responseString = await response.Content.ReadAsStringAsync();
                var verifyPaymentResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<VerifyPaymentResponse>(responseString);
                return verifyPaymentResponse;     
            }
        }
    }
}
