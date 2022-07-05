using System.Runtime.Serialization;

namespace UDO.LOB.DependentMaintenance.Messages
{
    [DataContract]
    public class DependentParticipant : Participant, IDependentParticipant
    {
        [DataMember]
        public DependentRelationship DependentRelationship { get; set; }
    }
}