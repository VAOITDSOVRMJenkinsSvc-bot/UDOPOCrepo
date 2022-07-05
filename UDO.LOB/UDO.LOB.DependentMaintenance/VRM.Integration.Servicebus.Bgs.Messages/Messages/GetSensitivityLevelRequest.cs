using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;


namespace VRM.Integration.Servicebus.Bgs.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.GetSensitivityLevelRequest)]
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
