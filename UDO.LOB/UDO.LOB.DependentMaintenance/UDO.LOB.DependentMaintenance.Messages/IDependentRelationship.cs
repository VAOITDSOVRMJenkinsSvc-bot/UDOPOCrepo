using System;
using System.Runtime.Serialization;

namespace UDO.LOB.DependentMaintenance.Messages
{
    public interface IDependentRelationship
    {
        [DataMember]
        DateTime BeginDate { get; set; }

        [DataMember]
        DateTime? EndDate { get; set; }

        [DataMember]
        bool ChildPreviouslyMarried { get; set; }

        [DataMember]
        string FamilyRelationshipTypeName { get; set; }

        [DataMember]
        bool LivesWithRelatedPerson { get; set; }

        [DataMember]
        string MarriageCityName { get; set; }

        [DataMember]
        string MarriageCountryName { get; set; }

        [DataMember]
        string MarriageStateCode { get; set; }

        [DataMember]
        string RelationshipTypeName { get; set; }

        [DataMember]
        decimal MonthlyContributionToSpouseSupport { get; set; }
    }
}