using MidSussexTriathlon.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace MidSussexTriathlon.Web.Controllers
{
    public class EntryController : SurfaceController
    {     

        [HttpPost]
        public async Task<ActionResult> Index(EntryModel model)
        {
            if (!ModelState.IsValid)
                return CurrentUmbracoPage();

            string merchantAccount = ConfigurationManager.AppSettings["adyenMerchantAccount"];

            string originUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Host, Request.Url.Port == 80 ? string.Empty : ":" + Request.Url.Port);
            string returnUrl = $"{originUrl}/entry/result";

            var paymentSession = new PaymentSessionRequest()
            {
                amount = new PaymentSessionRequest.AmountModel()
                {
                    currency = "GBP",
                    value = "3500"
                },
                reference = "Order123",
                merchantAccount = merchantAccount,
                sdkVersion = "1.3.2",
                shopperReference = "entrant_Order123",
                channel = "Web",
                html = true,
                origin = originUrl,
                returnUrl = returnUrl,
                countryCode = "GB",
                shopperLocale = "gb_GB"
            };

            string paymentSessionUrl = "https://checkout-test.adyen.com/v32/paymentSession";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            using (var client = new HttpClient())
            {
                var adyenApiKey = ConfigurationManager.AppSettings["adyenApiKey"];                
                client.DefaultRequestHeaders.Add("X-API-Key", adyenApiKey);                       
                var response = await client.PostAsJsonAsync(paymentSessionUrl, paymentSession);

                var responseString = await response.Content.ReadAsStringAsync();
                var paymentSessionResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<PaymentSessionResponse>(responseString);             
            }

            return RedirectToCurrentUmbracoPage();
        }
    }
}