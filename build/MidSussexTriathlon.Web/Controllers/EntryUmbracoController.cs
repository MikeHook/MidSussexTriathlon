using MidSussexTriathlon.Core.Data;
using MidSussexTriathlon.Core.Domain;
using System.Linq;
using Umbraco.Web.Editors;
using System.Collections.Generic;
using MidSussexTriathlon.Core.Model;

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
            var entries = _entryRepository.GetAll().ToList();
            return entries;
        }

        public IEnumerable<EntryReport> GetAllForCsv()
        {
            var entries = _entryRepository.GetAll().ToList();
            var entriesReport = entries.Select(e => new EntryReport()
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName =  e.LastName,
                DateOfBirth = e.DateOfBirth.ToString("yyyy/MM/dd"),
                PhoneNumber = string.IsNullOrEmpty(e.PhoneNumber) ? "" : $"'{e.PhoneNumber}'", //Add quotes to stop excel mucking up the formatting
                Email = e.Email,
                Gender = e.Gender,
                AddressLine1 = e.AddressLine1,
                AddressLine2 = e.AddressLine2,
                City = e.City,
                County = e.County,
                Postcode = e.Postcode,
                RaceType = e.RaceType,
                SwimTime = e.SwimTime,
                SwimDistance = e.SwimDistance,
                BtfNumber = e.BtfNumber,
                ClubName = e.ClubName,
                TermsAccepted = e.TermsAccepted,
                EntryDate = e.EntryDate.ToString("yyyy/MM/dd"),
                NewToSport = e.NewToSport,
                HowHeardAboutUs = e.HowHeardAboutUs,
                Paid = e.Paid,
                PaymentFailureMessage = e.PaymentFailureMessage,
                OrderReference = e.OrderReference,
                ClientSecret = e.ClientSecret,
                Cost = e.Cost,
                Relay2FirstName = e.Relay2FirstName,
                Relay2LastName = e.Relay2LastName,
                Relay2BtfNumber = e.Relay2BtfNumber,
                Relay3FirstName = e.Relay3FirstName,
                Relay3LastName = e.Relay3LastName,
                Relay3BtfNumber = e.Relay3BtfNumber,
                Wave = e.Wave
            }
               );
            return entriesReport;
        }

        public Entry Update(Entry entry)
        {
            _entryRepository.Update(entry);
            return _entryRepository.Get(entry.Id);
        }

        public void SetWaves(WaveModel model)
        {
            var entrants = _entryRepository.GetEntered();
            entrants.OrderBy(e => e.EntryDate);

            int wave = 1;
            int waveCount = 1;
            foreach(var entrant in entrants)
            {
                entrant.Wave = wave;
                _entryRepository.Update(entrant);
                if (waveCount == model.WaveSize)
                {
                    wave++;
                    waveCount = 1;
                }
                else
                {
                    waveCount++;
                }
            }
        }
    }
}
