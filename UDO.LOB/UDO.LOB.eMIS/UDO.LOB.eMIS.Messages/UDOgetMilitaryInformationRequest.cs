using System;
using System.Runtime.Serialization;
using UDO.LOB.Core;

namespace UDO.LOB.eMIS.Messages
{
    [DataContract]
    public class UDOgetMilitaryInformationRequest : MessageBase
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
        public String udo_EDIPI { get; set; }

        [DataMember]
        public String udo_ICN { get; set; }

        [DataMember]
        public bool udo_MostRecentServiceOnly { get; set; }
    }
}