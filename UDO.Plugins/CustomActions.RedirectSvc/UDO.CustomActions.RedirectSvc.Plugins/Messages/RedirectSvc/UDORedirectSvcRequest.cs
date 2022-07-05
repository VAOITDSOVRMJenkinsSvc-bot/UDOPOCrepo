using System;
using System.Runtime.Serialization;
using UDO.LOB.Core;

namespace UDO.LOB.RedirectSvc.Messages
{
    [DataContract]
    public class UDORedirectSvcRequest : MessageBase
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
        public UDORedirectSvcInput mcs_RedirectSvcInput { get; set; }
    }

    [DataContract]
    public class UDORedirectSvcInput
    {
        [DataMember] public string mcs_soapRequest { get; set; }
    }

}
