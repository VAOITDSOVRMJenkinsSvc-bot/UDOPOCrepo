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
    //[ExportMetadata("MessageType", MessageRegistry.UDOClearClaimEstablishmentRequest)]
    [DataContract]
    public class UDOClearClaimEstablishmentRequest : UDORequestBase
    {
        [DataMember]
        public Guid ClaimEstablishmentId { get; set; }
    }
}
