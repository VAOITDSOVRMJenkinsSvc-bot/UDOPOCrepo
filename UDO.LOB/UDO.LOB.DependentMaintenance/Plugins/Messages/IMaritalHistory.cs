using System;
using System.Runtime.Serialization;

namespace VRM.Integration.Servicebus.AddDependent.Messages
{
    public interface IMaritalHistory
    {
        [DataMember]
        string State { get; set; }

        [DataMember]
        string SpouseSsn { get; set; }

        [DataMember]
        DateTime MarriageStartDate { get; set; }

        [DataMember]
        DateTime MarriageEndDate { get; set; }

        [DataMember]
        string LastName { get; set; }

        [DataMember]
        string FirstName { get; set; }

        [DataMember]
        DateTime DOB { get; set; }

        [DataMember]
        string Country { get; set; }

        [DataMember]
        string City { get; set; }
        [DataMember]
        string HowMarriageWasTerminated { get; set; }

    }
}