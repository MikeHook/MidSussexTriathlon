using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ExceptionHandling;
using Umbraco.Core.Logging;

namespace MidSussexTriathlon.Web.Logger
{
    public class TraceExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            LogHelper.Error(context.ExceptionContext.ControllerContext.Controller.GetType(), "WebApi Exception", context.Exception);
        }
    }
}