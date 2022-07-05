using System;
using System.Runtime.Serialization;
using UDO.LOB.Core;

//using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Messages;

namespace UDO.LOB.Claims.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOUpdateUDOClaimsRequest)]
    [DataContract]
    public class UDOUpdateUDOClaimsRequest : MessageBase
    {
        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public Guid UserId { get; set; }
        [DataMember]
        //public Guid RelatedParentId { get; set; }
        //[DataMember]
        //public string RelatedParentEntityName { get; set; }
        //[DataMember]
        //public string RelatedParentFieldName { get; set; }
        //[DataMember]
        public bool LogSoap { get; set; }
        [DataMember]
        public bool LogTiming { get; set; }
        [DataMember]
        public bool Debug { get; set; }
        [DataMember]
        public string udo_benefitClaimID { get; set; }
        [DataMember]
        public Guid ownerId { get; set; }
        [DataMember]
        public string ownerType { get; set; }
        [DataMember]
        public Guid udo_ClaimID { get; set; }
        [DataMember]
        public UDOHeaderInfo LegacyServiceHeaderInfo { get; set; }
    }
}