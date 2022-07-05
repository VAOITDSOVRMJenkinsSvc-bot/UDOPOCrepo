using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
using UDO.LOB.DependentMaintenance.Messages;
//using VRM.Integration.Servicebus.Core;

//CSDEv
//namespace VRM.Integration.Servicebus.Bgs.Messages
namespace UDO.LOB.DependentMaintenance.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.SearchBGSSchoolInfoRequest)]
    [DataContract]
    public class SearchSchoolInfoRequest : MessageBase
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
        public UDOHeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public VEISsrcheduinstsearcheduinstitutessearchstringPlugin searcheduinstitutessearchstringInfo { get; set; }
        [DataMember]
        public VEISsrcheduinstedustatePlugin edustateInfo { get; set; }

        public class VEISsrcheduinstsearcheduinstitutessearchstringPlugin
        {
            [DataMember]
            public string mcs_instituteName { get; set; }
            [DataMember]
            public string mcs_facilityCode { get; set; }
        }
        public class VEISsrcheduinstedustatePlugin
        {
            [DataMember]
            public string mcs_stateCodeORForeignCountry { get; set; }
            [DataMember]
            public string mcs_stateNumber { get; set; }
        }
    }
}
