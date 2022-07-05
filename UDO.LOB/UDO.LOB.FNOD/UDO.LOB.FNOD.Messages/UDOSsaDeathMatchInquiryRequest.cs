using System;
using System.Runtime.Serialization;
using UDO.LOB.Core;

namespace UDO.LOB.FNOD.Messages
{
    [DataContract]
    public class UDOSsaDeathMatchInquiryRequest : MessageBase
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
        public UDOSsaInquiryInput mcs_SsaInquiryInput { get; set; }
    }

    [DataContract]
    public class UDOSsaInquiryInput
    {
        [DataMember] public string mcs_fileNumberField { get; set; }
        [DataMember] public string mcs_dobField { get; set; }
        [DataMember] public string mcs_firstNameField { get; set; }
        [DataMember] public string mcs_lastNameField { get; set; }
        [DataMember] public string mcs_vetFileNumberField { get; set; }
    }

}
