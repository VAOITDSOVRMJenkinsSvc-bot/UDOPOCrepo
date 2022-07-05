using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Crm.Plugins.IntentToFile
{
    public class Claimant
    {
        public string ClaimantParticipantId { get; set; }
        public string VeteranParticipantId { get; set; }
        public string CompensationType { get; set; }
        public string VeteranFirstName { get; set; }
        public string VeteranLastName { get; set; }
        public string VeteranMiddleInitial { get; set; }
        public SecureString VeteranSsn { get; set; }
        public string VeteranFileNumber { get; set; }
        public DateTime? VeteranBirthDate { get; set; }
        public string VeteranGender { get; set; }
        public string ClaimantFirstName { get; set; }
        public string ClaimantLastName { get; set; }
        public string ClaimantMiddleInitial { get; set; }
        public SecureString ClaimantSsn { get; set; }
        public string PhoneAreaCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string UserId { get; set; }
        public string StationLocation { get; set; }
    }
}
