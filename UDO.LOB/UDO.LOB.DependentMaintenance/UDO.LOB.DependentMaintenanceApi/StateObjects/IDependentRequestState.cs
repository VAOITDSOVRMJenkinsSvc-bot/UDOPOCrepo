using UDO.LOB.DependentMaintenance.Messages;
//using VRM.Integration.Servicebus.AddDependent.Messages;

namespace UDO.LOB.DependentMaintenance
{
    public interface IDependentRequestState
    {
        long DependentParticipantId { get; set; }
        IDependentParticipant DependentParticipant { get; set; }
    }
}