using System;
using System.Runtime.Serialization;

namespace UDO.LOB.Core
{
    [DataContract]
    public abstract class UDORequestBase : MessageBase
    {
        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public Guid UserId { get; set; }

        // Related Parent Information is not always required
        //[DataMember(IsRequired = false)]
        //public Guid RelatedParentId { get; set; }
        //[DataMember(IsRequired = false)]
        //public string RelatedParentEntityName { get; set; }
        //[DataMember(IsRequired = false)]
        //public string RelatedParentFieldName { get; set; }

        // If the Log Settings are not passed, they are set to false.
        [DataMember(IsRequired = false)]
        public bool LogTiming { get; set; }
        [DataMember(IsRequired = false)]
        public bool LogSoap { get; set; }
        [DataMember(IsRequired = false)]
        public bool Debug { get; set; }

        // Default is MessageProcessType.Local
        //[DataMember(IsRequired = false)]
        //public MessageProcessType ProcessType { get; set; }

        // Optional Message Paramters for Owner
        [DataMember(IsRequired = false)]
        public Guid? OwnerId { get; set; }
        [DataMember(IsRequired = false)]
        public string OwnerType { get; set; }

        // Legacy Header is not used by every LOB
        [DataMember(IsRequired = false)]
        public UDOHeaderInfo LegacyServiceHeaderInfo { get; set; }

        // Related Entities is not used by every LOB
        [DataMember(IsRequired = false)]
        public UDORelatedEntity[] RelatedEntities { get; set; }
    }
}
