using MidSussexTriathlon.Core.Data;
using MidSussexTriathlon.Core.Domain;
using Stripe;
using System.Configuration;
using System.Linq;
using Umbraco.Web.WebApi;
using System.Web.Http;
using Umbraco.Web.Editors;
using System.Collections.Generic;

namespace MidSussexTriathlon.Web.Controllers
{
    [Umbraco.Web.Mvc.PluginController("EntriesDashboard")]
    public class EntryUmbracoController : UmbracoAuthorizedJsonController
    {
        IEntryRepository _entryRepository;
        
        public EntryUmbracoController()
        {
            _entryRepository = new EntryRepository(new DataConnection());
        }

        // GET: /umbraco/backoffice/EntriesDashboard/EntryUmbraco/GetAll

        public IEnumerable<Entry> GetAll()
        {
            return _entryRepository.GetAll().ToList();
        }      
    }
}
