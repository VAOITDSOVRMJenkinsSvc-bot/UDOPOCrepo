using System;
using System.Runtime.Serialization;
using UDO.LOB.Core;


//using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Messages;
//using System.ComponentModel.Composition;
//using VEIS.Core.Messages;

namespace UDO.LOB.Appeals.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOcreateUDOAppealDetailsRequest)]
    [DataContract]
    public class UDOcreateUDOAppealDetailsRequest : MessageBase
    {
        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public Guid UserId { get; set; }
        //[DataMember]
        //public Guid RelatedParentId { get; set; }
        //[DataMember]
        //public string RelatedParentEntityName { get; set; }
        //[DataMember]
        //public string RelatedParentFieldName { get; set; }
        [DataMember]
        public bool LogTiming { get; set; }
        [DataMember]
        public bool LogSoap { get; set; }
        [DataMember]
        public bool Debug { get; set; }
        [DataMember]
        public Guid ownerId { get; set; }
        [DataMember]
        public string ownerType { get; set; }
        [DataMember]
        public UDOHeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public Guid udo_appealId { get; set; }
        [DataMember]
        public string AppealKey { get; set; }
        [DataMember]
        public UDOcreateUDOAppealDetailsRelatedEntitiesMultipleRequest[] UDOcreateUDOAppealDetailsRelatedEntitiesInfo { get; set; }
    }
    [DataContract]
    public class UDOcreateUDOAppealDetailsRelatedEntitiesMultipleRequest
    {
        [DataMember]
        public string RelatedEntityName { get; set; }
        [DataMember]
        public Guid RelatedEntityId { get; set; }
        [DataMember]
        public string RelatedEntityFieldName { get; set; }
    }
}