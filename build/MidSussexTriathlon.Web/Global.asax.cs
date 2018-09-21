using MidSussexTriathlon.Web.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace MidSussexTriathlon.Web
{
    public class MidSussexTriathlonApplication : Umbraco.Web.UmbracoApplication
    {
        protected override void OnApplicationStarted(object sender, EventArgs e)
        {
            base.OnApplicationStarted(sender, e);

            GlobalConfiguration.Configuration.Services.Add(typeof(IExceptionLogger), new TraceExceptionLogger());
        }
    }
}