using System;
using System.Runtime.Serialization;

namespace VRM.Integration.Servicebus.AddDependent.Messages
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