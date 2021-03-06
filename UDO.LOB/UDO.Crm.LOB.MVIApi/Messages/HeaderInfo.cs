using System.Runtime.Serialization;

namespace VRM.Integration.UDO.MVI.Messages
{
    public class HeaderInfo
    {
        [DataMember]
        public string StationNumber { get; set; }
        [DataMember]
        public string LoginName { get; set; }
        [DataMember]
        public string ApplicationName { get; set; }
        [DataMember]
        public string ClientMachine { get; set; }
    }
}
