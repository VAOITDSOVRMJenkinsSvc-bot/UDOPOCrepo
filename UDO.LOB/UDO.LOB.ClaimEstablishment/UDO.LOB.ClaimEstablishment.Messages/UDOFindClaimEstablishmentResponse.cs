using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
//using System;
//using Microsoft.Xrm.Sdk;
//using UDO.LOB.Extensions;
//using VEIS.Core.Messages;

//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Common.Messages;

namespace UDO.LOB.ClaimEstablishment.Messages
{
    [Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOFindClaimEstablishmentResponse)]
    [DataContract]
    public class UDOFindClaimEstablishmentResponse : UDOResponseBase
    {
        [DataMember]
        public string StackTrace { get; set; }
        [DataMember]
        public UDObenefitClaimRecordBCS2 UDObenefitClaimRecordBCS2Info{ get; set; }
    }
}
