using System;
using System.ComponentModel.Composition;
using Microsoft.Xrm.Sdk;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;

namespace VRM.Integration.UDO.Claims.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOcreateUDOMAPDLettersRequest)]
    [DataContract]
    public class UDOcreateUDOMAPDLettersRequest : MessageBase
    {
        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public Guid UserId { get; set; }
        [DataMember]
        public Guid RelatedParentId { get; set; }
        [DataMember]
        public string RelatedParentEntityName { get; set; }
        [DataMember]
        public string RelatedParentFieldName { get; set; }
        [DataMember]
        public bool LogSoap { get; set; }
        [DataMember]
        public bool LogTiming { get; set; }
        [DataMember]
        public bool Debug { get; set; }
        [DataMember]
        public Int64 claimId { get; set; }
        [DataMember]
        public HeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public UDOcreateUDOMAPDLettersRelatedEntitiesMultipleRequest[] UDOcreateUDOMAPDLettersRelatedEntitiesInfo { get; set; }
    }
    [DataContract]
    public class UDOcreateUDOMAPDLettersRelatedEntitiesMultipleRequest
    {
        [DataMember]
        public string RelatedEntityName { get; set; }
        [DataMember]
        public Guid RelatedEntityId { get; set; }
        [DataMember]
        public string RelatedEntityFieldName { get; set; }
    }
}