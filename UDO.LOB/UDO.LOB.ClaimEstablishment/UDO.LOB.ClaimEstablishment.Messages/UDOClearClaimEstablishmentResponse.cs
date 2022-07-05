using System;
using System.Runtime.Serialization;
using UDO.LOB.Core;
//using System.ComponentModel.Composition;
//using UDO.LOB.Extensions;
//using VEIS.Core.Messages;

//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Common.Messages;

namespace UDO.LOB.ClaimEstablishment.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOClearClaimEstablishmentResponse)]
    [DataContract]
    public class UDOClearClaimEstablishmentResponse : UDOResponseBase
    {
        [DataMember]
        public Guid ClaimEstablishmentId { get; set; }
        [DataMember]
        public string StackTrace { get; set; }
        [DataMember]
        public UDObenefitClaimRecordBCS2 UDObenefitClaimRecordBCS2Info { get; set; }
    }
}
