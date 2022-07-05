using System;
using System.Runtime.Serialization;

namespace VRM.Integration.Servicebus.AddDependent.Messages
{
    [DataContract]
    public class MaritalHistory : IMaritalHistory
    {
        [DataMember]
        public string State { get; set; }
        [DataMember]
        public string SpouseSsn { get; set; }
        [DataMember]
        public DateTime MarriageStartDate { get; set; }
        [DataMember]
        public DateTime MarriageEndDate { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public DateTime DOB { get; set; }
        [DataMember]
        public string Country { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string HowMarriageWasTerminated { get; set; }
    }
}