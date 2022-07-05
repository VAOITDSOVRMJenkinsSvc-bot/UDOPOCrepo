using UDO.LOB.DependentMaintenance.Messages;
//using VRM.Integration.Servicebus.AddDependent.Messages;

namespace UDO.LOB.DependentMaintenance
{
    public interface IVeteranRequestState
    {
        long VeteranParticipantId { get; set; }

        IVeteranParticipant VeteranParticipant { get; set; }
    }
}