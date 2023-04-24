using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace MidSussexTriathlon.Core.Model
{
    public class EntryReport
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public string RaceType { get; set; }
        public string SwimTime { get; set; }
        public string SwimDistance { get; set; }
        public string BtfNumber { get; set; }
        public string ClubName { get; set; }
        public bool TermsAccepted { get; set; }
        public string EntryDate { get; set; }
        public bool NewToSport { get; set; }
        public string HowHeardAboutUs { get; set; }
        public bool Paid { get; set; }
        public string PaymentFailureMessage { get; set; }
        public string OrderReference { get; set; }
        public string ClientSecret { get; set; }
        public int Cost { get; set; }

        public string Relay2FirstName { get; set; }
        public string Relay2LastName { get; set; }
        public string Relay2BtfNumber { get; set; }
        public string Relay3FirstName { get; set; }
        public string Relay3LastName { get; set; }
        public string Relay3BtfNumber { get; set; }
        public int? Wave { get; set; }

        public string EmergencyContactFirstName { get; set; }
        public string EmergencyContactLastName { get; set; }
        public string EmergencyContactPhone { get; set; }
    }
}
