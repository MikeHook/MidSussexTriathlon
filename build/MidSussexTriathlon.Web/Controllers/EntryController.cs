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

       

            return RedirectToCurrentUmbracoPage();
        }
    }
}