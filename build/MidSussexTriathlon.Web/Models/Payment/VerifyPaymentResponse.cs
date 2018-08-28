using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MidSussexTriathlon.Web.Models.Payment
{
    public class VerifyPaymentResponse
    {
        public string pspReference { get; set; }
        public string authResponse { get; set; }
        public string merchantReference { get; set; }
        public string paymentMethod { get; set; }
        public string shopperLocale { get; set; }
    }
}