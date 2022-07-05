using System;
using System.Runtime.Serialization;

namespace VRM.Integration.Servicebus.AddDependent.Messages
{
    public interface IParticipant
    {
        [DataMember]
        string FileNumber { get; set; }

        [DataMember]
        string Ssn { get; set; }

        [DataMember]
        string FirstName { get; set; }

        [DataMember]
        string MiddleName { get; set; }

        [DataMember]
        string LastName { get; set; }

        [DataMember]
        DateTime BirthDate { get; set; }

        [DataMember]
        string BirthCityName { get; set; }

        [DataMember]
        string BirthStateCode { get; set; }

        [DataMember]
        string BirthCountryName { get; set; }

        [DataMember]
        bool IsVetInd { get; set; }

        [DataMember]
        bool IsSeriouslyDisabled { get; set; }

        [DataMember]
        bool IsScholdChild { get; set; }

        [DataMember]
        Address[] Addresses { get; set; }

        [DataMember]
        PhoneNumber[] PhoneNumbers { get; set; }

        [DataMember]
        string AddresssType { get; set; }
        [DataMember]
        string EmailAddress { get; set; }
        [DataMember]
        string postalCodePlus4 { get; set; }
        [DataMember]
        string SuffixName { get; set; }
        [DataMember]
        string TitleName { get; set; }
        [DataMember]
        string NoSssnReasonTypeCd { get; set; }
    }
}