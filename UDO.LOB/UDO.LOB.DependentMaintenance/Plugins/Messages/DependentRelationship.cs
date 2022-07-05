using System;
using System.Runtime.Serialization;

namespace VRM.Integration.Servicebus.AddDependent.Messages
{
    [DataContract]
    public class DependentRelationship : IDependentRelationship
    {
        [DataMember]
        public DateTime BeginDate { get; set; }
        [DataMember]
        public DateTime? EndDate { get; set; }
        [DataMember]
        public bool ChildPreviouslyMarried { get; set; }
        [DataMember]
        public string FamilyRelationshipTypeName { get; set; }
        [DataMember]
        public bool LivesWithRelatedPerson { get; set; }
        [DataMember]
        public string MarriageCityName { get; set; }
        [DataMember]
        public string MarriageCountryName { get; set; }
        [DataMember]
        public string MarriageStateCode { get; set; }
        [DataMember]
        public string RelationshipTypeName { get; set; }
        [DataMember]
        public decimal MonthlyContributionToSpouseSupport { get; set; }
        [DataMember]
        public string MarriageTerminationCityName { get; set; }
        [DataMember]
        public string MarriageTerminationStateCode { get; set; }
        [DataMember]
        public string MarriageTerminationCountryName { get; set; }
        [DataMember]
        public string MarriageTerminationTypeCode { get; set; }

    }
}
