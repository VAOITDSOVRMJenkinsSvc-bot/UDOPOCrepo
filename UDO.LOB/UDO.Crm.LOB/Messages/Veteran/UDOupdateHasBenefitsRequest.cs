using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRM.Integration.Servicebus.Core;
using System.ComponentModel.Composition;
using VRM.Integration.UDO.Contact.Messages;
using System.Runtime.Serialization;
using VRM.Integration.UDO.Messages;

namespace VRM.Integration.UDO.Contact.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOupdateHasBenefitsRequest)]
    [DataContract]
    public class UDOupdateHasBenefitsRequest : MessageBase
    {
        [DataMember]
        public HeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public string Edipi { get; set; }

        [DataMember]
        public string ContactId { get; set; }
        //[DataMember]
        //public string MessageId { get; set; }
        [DataMember]
        public Guid UserId { get; set; }
        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public bool Debug { get; set; }
        [DataMember]
        public Guid RelatedParentId { get; set; }
        [DataMember]
        public string RelatedParentEntityName { get; set; }
        [DataMember]
        public string RelatedParentFieldName { get; set; }
        [DataMember]
        public bool LogTiming { get; set; }
        [DataMember]
        public bool LogSoap { get; set; }

    }
}
