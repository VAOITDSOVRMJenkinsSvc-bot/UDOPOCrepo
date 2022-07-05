using System.Runtime.Serialization;

namespace VRM.Integration.Servicebus.AddDependent.Messages
{
    [DataContract]
    public class DependentParticipant : Participant, IDependentParticipant
    {
        [DataMember]
        public DependentRelationship DependentRelationship { get; set; }
    }
}