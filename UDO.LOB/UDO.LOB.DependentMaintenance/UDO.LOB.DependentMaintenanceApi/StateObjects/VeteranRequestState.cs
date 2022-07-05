using UDO.LOB.DependentMaintenance.Messages;
//using VRM.Integration.Servicebus.AddDependent.Messages;

namespace UDO.LOB.DependentMaintenance
{
    public class VeteranRequestState : IVeteranRequestState
    {
        public long VeteranParticipantId { get; set; }
        public IVeteranParticipant VeteranParticipant { get; set; }
    }
}
