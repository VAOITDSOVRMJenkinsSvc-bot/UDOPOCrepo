using System;
using System.Runtime.Serialization;

namespace UDO.LOB.DependentMaintenance.Messages
{
    public interface IAddress
    {
        [DataMember]
        DateTime EffectiveDate { get; set; }

        [DataMember]
        string AddressLine1 { get; set; }

        [DataMember]
        string AddressLine2 { get; set; }

        [DataMember]
        string AddressLine3 { get; set; }

        [DataMember]
        string City { get; set; }

        [DataMember]
        string State { get; set; }

        [DataMember]
        string Country { get; set; }

        [DataMember]
        string County { get; set; }

        [DataMember]
        string ZipCode { get; set; }

        [DataMember]
        string AddressTypeName { get; set; }

        [DataMember]
        bool SharedAddressIndicator { get; set; }
    }
}