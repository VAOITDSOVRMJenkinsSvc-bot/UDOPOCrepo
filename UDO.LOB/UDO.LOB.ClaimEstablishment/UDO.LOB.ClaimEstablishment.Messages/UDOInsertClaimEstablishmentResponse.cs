using System;
using System.Runtime.Serialization;
using UDO.LOB.Core;

//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Common.Messages;

namespace UDO.LOB.ClaimEstablishment.Messages
{
    [DataContract]
    public class UDOInsertClaimEstablishmentResponse : UDOResponseBase
    {
        [DataMember]
        public Guid ClaimEstablishmentId { get; set; }
        [DataMember]
        public string StackTrace { get; set; }
        [DataMember]
        public UDObenefitClaimRecordBCS2 UDObenefitClaimRecordBCS2Info { get; set; }

    }
}
