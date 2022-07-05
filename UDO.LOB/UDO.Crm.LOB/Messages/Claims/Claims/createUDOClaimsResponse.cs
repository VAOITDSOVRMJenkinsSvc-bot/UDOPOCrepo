using System;
using System.ComponentModel.Composition;
using Microsoft.Xrm.Sdk;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VIMT.BenefitClaimService.Messages;
using VRMMessages = VRM.Integration.UDO.Messages;

namespace VRM.Integration.UDO.Claims.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", VRMMessages.MessageRegistry.UDOcreateUDOClaimsResponse)]
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
        public VIMTfindBenefitClaimResponse VIMTfindBenefitClaimRequestData { get; set; }
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