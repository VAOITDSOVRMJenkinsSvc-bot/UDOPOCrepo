using System;
using System.Runtime.Serialization;
using UDO.LOB.Core;

namespace Va.Udo.Crm.Documents.Plugins
{
    [DataContract]
    public class VEISDocGenRequest : MessageBase
    {
        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public Guid UserId { get; set; }
        [DataMember]
        public bool LogTiming { get; set; }
        [DataMember]
        public bool LogSoap { get; set; }
        [DataMember]
        public bool Debug { get; set; }
        [DataMember]
        public string PrimaryEntityName { get; set; }
        [DataMember]
        public Guid PrimaryEntityId { get; set; }
        [DataMember]
        public string DocumentTemplate { get; set; }
        [DataMember]
        public bool UploadAttachment { get; set; }
        [DataMember]
        public string AttachmentEntityName { get; set; }
        [DataMember]
        public Guid AttachmentEntityId { get; set; }
        [DataMember]
        public bool ConvertToPdf { get; set; }

    }

}
