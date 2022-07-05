using System.Runtime.Serialization;

namespace UDO.LOB.DependentMaintenance.Messages
{
    public interface IAddDependentRequest
    {
        [DataMember]
        VeteranParticipant Veteran { get; set; }

        [DataMember]
        DependentParticipant[] Dependents { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        string MessageId { get; set; }

        ExtensionDataObject ExtensionData { get; set; }
    }
}