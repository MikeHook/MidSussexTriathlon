using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MidSussexTriathlon.Web.Models.Payment
{
    public class VerifyPaymentRequest
    {
        public string payload { get; set; }
        public string resultCode { get; set; }
        public string resultText { get; set; }
    }
}