using System;
using System.Runtime.Serialization;


namespace UDO.LOB.DependentMaintenance.Messages
{
    [DataContract]
    public class UdoEcSearchEduInstitutesResponse : UdoEcResponseBase
    {
        [DataMember]
        public UdoEcSrcheduinststatusMultipleResponse[] VEISsrcheduinststatusInfo { get; set; }
        [DataMember]
        public bool ExceptionOccured { get; set; }
    }
    [DataContract]
    public class UdoEcSrcheduinststatusMultipleResponse
    {
        [DataMember]
        public Int64 mcs_participantID { get; set; }
        [DataMember]
        public string mcs_instituteName { get; set; }
        [DataMember]
        public string mcs_facilityCode { get; set; }
        [DataMember]
        public string mcs_statusDate { get; set; }
    }

}
