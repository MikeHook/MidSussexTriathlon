using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MidSussexTriathlon.Web.Models
{
    public class PaymentSessionRequest
    {
        public AmountModel amount { get; set; }
        public string reference { get; set; }
        public string merchantAccount { get; set; }
        public string sdkVersion { get; set; }
        public string shopperReference { get; set; }
        public string channel { get; set; }
        public bool html { get; set; }
        public string origin { get; set; }
        public string returnUrl { get; set; }
        public string countryCode { get; set; }
        public string shopperLocale { get; set; }

        public class AmountModel
        {
            public string currency { get; set; }
            public string value { get; set; }
        }
    }
}