using System.Runtime.Serialization;

namespace UDO.LOB.PersonSearch.Models
{
    [DataContract]
    public class PatientAddress
    {
        [DataMember]
        public string Use { get; set; }

        [DataMember]
        public string StreetAddressLine { get; set; }

        [DataMember]
        public string StreetAddressLine2 { get; set; }

        [DataMember]
        public string StreetAddressLine3 { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string PostalCode { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string State { get; set; }
    }
}