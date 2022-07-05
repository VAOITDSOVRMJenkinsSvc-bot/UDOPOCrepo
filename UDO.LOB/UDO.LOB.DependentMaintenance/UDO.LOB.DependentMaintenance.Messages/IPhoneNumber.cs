using System;
using System.Runtime.Serialization;

namespace UDO.LOB.DependentMaintenance.Messages
{
    public interface IPhoneNumber
    {
        [DataMember]
        DateTime EffectiveDate { get; set; }

        [DataMember]
        string Number { get; set; }

        [DataMember]
        string PhoneTypeName { get; set; }

        [DataMember]
        string AreaCode { get; set; }
    }
}