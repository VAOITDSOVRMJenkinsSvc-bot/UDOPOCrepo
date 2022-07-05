using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;

namespace VRM.Integration.Servicebus.Bgs.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.GetDependentInfoRequest)]
    [DataContract]
    public class GetDependentInfoRequest : MessageBase
    {
        [DataMember]
        public string crme_OrganizationName { get; set; }

        [DataMember]
        public Guid crme_UserId { get; set; }

        [DataMember]
        public string crme_SSN { get; set; }

        [DataMember]
        public string crme_ParticipantId { get; set; }
    }
}
