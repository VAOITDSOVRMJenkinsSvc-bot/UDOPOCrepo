using System;
using System.Runtime.Serialization;

namespace UDO.LOB.DependentMaintenance.Messages
{
    [DataContract]
    public class Address : IAddress
    {
        [DataMember]
        public DateTime EffectiveDate { get; set; }
        [DataMember]
        public string AddressLine1 { get; set; }
        [DataMember]
        public string AddressLine2 { get; set; }
        [DataMember]
        public string AddressLine3 { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string State { get; set; }
        [DataMember]
        public string Country { get; set; }
        [DataMember]
        public string County { get; set; }
        [DataMember]
        public string ZipCode { get; set; }
        [DataMember]
        public string ZipPlus4 { get; set; }
        [DataMember]
        public string AddressTypeName { get; set; }
        [DataMember]
        public bool SharedAddressIndicator { get; set; }
        [DataMember]
        public string Title { get; set; }
    }
}
