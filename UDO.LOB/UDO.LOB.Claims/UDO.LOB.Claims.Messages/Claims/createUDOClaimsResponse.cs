using System;
using System.Runtime.Serialization;
using UDO.LOB.Core;
using VEIS.Messages.BenefitClaimService;

namespace UDO.LOB.Claims.Messages
{
    [DataContract]
    public class UDOcreateUDOClaimsResponse : MessageBase
    {
        [DataMember]
        public UDOcreateUDOClaimsMultipleResponse[] UDOcreateUDOClaimsInfo { get; set; }
        [DataMember]
        public bool ExceptionOccured { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public string newUDOcreateUDOClaimsParticipantId { get; set; }
        [DataMember]
        public VEISfindBenefitClaimResponse VEISfindBenefitClaimRequestData { get; set; }
    }
    [DataContract]
    public class UDOcreateUDOClaimsMultipleResponse
    {
        [DataMember]
        public Guid newUDOcreateUDOClaimsId { get; set; }
        [DataMember]
        public string newUDOcreateUDOClaimsParticipantId { get; set; }
        [DataMember]
        public Int64 newUDOcreateUDOClaimsIdentifier { get; set; }
    }
}