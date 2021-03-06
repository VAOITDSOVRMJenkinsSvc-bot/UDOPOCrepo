using System.Runtime.Serialization;

namespace UDO.LOB.Core
{
    public class UDOHeaderInfo: ILegacyHeaderInfo
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