using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
using UDO.LOB.DependentMaintenance.Messages;

namespace UDO.LOB.DependentMaintenance.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.GetBGSSchoolInfoRequest)]
    [DataContract]
    public class GetSchoolInfoRequest : MessageBase
    {
        public string OrganizationName { get; set; }
        [DataMember]
        public Guid UserId { get; set; }
        //public Guid RelatedParentId { get; set; }
        //public string RelatedParentEntityName { get; set; }

        //public string RelatedParentFieldName { get; set; }
        [DataMember]
        public bool LogTiming { get; set; }
        [DataMember]
        public bool LogSoap { get; set; }
        [DataMember]
        public bool Debug { get; set; }
        [DataMember]
        public UDOHeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public string mcs_fullFacilityCode { get; set; }
    }
}
