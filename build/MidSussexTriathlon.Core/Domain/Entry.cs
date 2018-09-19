using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace MidSussexTriathlon.Core.Domain
{
    public class Entry
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name => $"{FirstName} {LastName}";
        public string DateOfBirthString { get; set; }
        public DateTime DateOfBirth => DateTime.ParseExact(DateOfBirthString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
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
        public string BtfNumber { get; set; }
        public string ClubName { get; set; }
        public bool TermsAccepted { get; set; }
        public bool Paid { get; set; }
        public string PaymentFailureMessage { get; set; }
        public string OrderReference { get; set; }
        public string TokenId { get; set; }
    }
}
