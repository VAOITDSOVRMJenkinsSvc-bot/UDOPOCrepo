using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;

namespace UDO.LOB.ClaimEstablishment.Messages
{
    [DataContract]
    public class UDOInsertClaimEstablishmentRequest : UDORequestBase
    {
        [DataMember]
        public Guid ClaimEstablishmentId { get; set; }
    }
}
