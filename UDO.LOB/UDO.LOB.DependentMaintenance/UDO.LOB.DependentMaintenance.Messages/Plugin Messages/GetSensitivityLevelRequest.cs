using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
//using VRM.Integration.Servicebus.Core;

//CSDEv
//namespace VRM.Integration.Servicebus.Bgs.Messages
namespace UDO.LOB.DependentMaintenance.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.GetBGSSensitivityLevelRequest)]
    [DataContract]
    public class GetSensitivityLevelRequest : MessageBase
    {
        [DataMember]
        public string crme_OrganizationName { get; set; }

        [DataMember]
        public Guid crme_UserId { get; set; }

        [DataMember]
        public string crme_SSN { get; set; }

        [DataMember]
        public long? crme_ParticipantId { get; set; }
    }
}
