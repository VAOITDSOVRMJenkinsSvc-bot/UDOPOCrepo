using System;
using System.Runtime.Serialization;

namespace VRM.Integration.Servicebus.AddDependent.Messages
{
    [DataContract]
    public class Participant : IParticipant
    {
        [DataMember]
        public string FileNumber { get; set; }
        [DataMember]
        public string Ssn { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string MiddleName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public DateTime BirthDate { get; set; }
        [DataMember]
        public string BirthCityName { get; set; }
        [DataMember]
        public string BirthStateCode { get; set; }
        [DataMember]
        public string BirthCountryName { get; set; }
        [DataMember]
        public bool IsVetInd { get; set; }
        [DataMember]
        public bool IsSeriouslyDisabled { get; set; }
        [DataMember]
        public bool IsScholdChild { get; set; }
        [DataMember]
        public Address[] Addresses { get; set; }
        [DataMember]
        public PhoneNumber[] PhoneNumbers { get; set; }
        [DataMember]
        public string AddresssType { get; set; }
        [DataMember]
        public string EmailAddress { get; set; }
        [DataMember]
        public string postalCodePlus4 { get; set; }
        [DataMember]
        public string SuffixName { get; set; }
        [DataMember]
        public string TitleName { get; set; }
        [DataMember]
        public string NoSssnReasonTypeCd { get; set; }

    }
}
