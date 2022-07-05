using System;
using System.Runtime.Serialization;

namespace VRM.Integration.Servicebus.AddDependent.Messages
{
    [DataContract]
    public class PhoneNumber : IPhoneNumber
    {
        [DataMember]
        public DateTime EffectiveDate { get; set; }
        [DataMember]
        public string Number { get; set; }
        [DataMember]
        public string PhoneTypeName { get; set; }
        [DataMember]
        public string AreaCode { get; set; }
    }
}
