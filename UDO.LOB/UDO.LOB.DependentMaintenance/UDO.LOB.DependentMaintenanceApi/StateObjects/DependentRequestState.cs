using UDO.LOB.DependentMaintenance.Messages;
//using VRM.Integration.Servicebus.AddDependent.Messages;

namespace UDO.LOB.DependentMaintenance
{
    public class DependentRequestState : IDependentRequestState
    {
        public long DependentParticipantId { get; set; }
        public IDependentParticipant DependentParticipant { get; set; }
    }
}
