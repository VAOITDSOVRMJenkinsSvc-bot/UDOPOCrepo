using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;

//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Messages;

namespace UDO.LOB.IntentToFile.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOcreateIntentToFileRequest)]
    [DataContract]
    public class UDOcreateIntentToFileRequest : MessageBase
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
        // Replaced? HeaderInfo = LegacyHeaderInfo
        public UDOHeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public long PtcpntId { get; set; }
        [DataMember]
        public UDOgeneratedITFRecord[] UDOgeneratedITFRecordsInfo { get; set; }
        [DataMember]
        public UDOcreateITFRelatedEntitiesMultipleRequest[] UDOcreateITFRelatedEntitiesInfo { get; set; }
    }
    [DataContract]
    public class UDOgeneratedITFRecord
    {
        [DataMember]
        public Guid ITFCrmGuid { get; set; }
        [DataMember]
        public long ITFExternalID { get; set; }
    }
    [DataContract]
    public class UDOcreateITFRelatedEntitiesMultipleRequest
    {
        [DataMember]
        public string RelatedEntityName { get; set; }
        [DataMember]
        public Guid RelatedEntityId { get; set; }
        [DataMember]
        public string RelatedEntityFieldName { get; set; }
    }
}
