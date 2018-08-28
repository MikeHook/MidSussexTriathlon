using MidSussexTriathlon.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace MidSussexTriathlon.Web.Controllers
{
    public class ContactController : SurfaceController
    {     

        [HttpPost]
        public ActionResult Index(ContactModel model)
        {
            if (!ModelState.IsValid)
                return CurrentUmbracoPage();

            /// Work with form data here

            return RedirectToCurrentUmbracoPage();
        }
    }
}