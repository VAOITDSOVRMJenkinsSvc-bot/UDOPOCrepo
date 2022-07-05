using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
//using UDO.LOB.Extensions;
//using VEIS.Core.Messages;

//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Common.Messages;

namespace UDO.LOB.ClaimEstablishment.Messages
{
    [Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOFindClaimEstablishmentRequest)]
    [DataContract]
    public class UDOFindClaimEstablishmentRequest : UDORequestBase
    {
        [DataMember]
        public string FileNumber { get; set; }
        [DataMember]
        public Guid IdProofId { get; set; }
        [DataMember]
        public Guid InteractionId { get; set; }
    }
}
