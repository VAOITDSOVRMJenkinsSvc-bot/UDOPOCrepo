using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;

namespace UDO.LOB.eMIS.Messages
{
    [DataContract]
    public class UDOgetMilitaryInformationResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccurred { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public UDOMilitaryServiceInfo udo_MostRecentService { get; set; }

        [DataMember(IsRequired=false)]
        public UDOMilitaryServiceInfo[] udo_MilitaryServiceHistory { get; set; }
    }

    [DataContract]
    public class UDOMilitaryServiceInfo
    {
        [DataMember]
        //public DateTime? StartDate;
        public string StartDate;

        [DataMember]
        //public DateTime? EndDate;
        public string EndDate;

        [DataMember]
        public string RankCode;

        [DataMember]
        public string RankName;
        
        [DataMember]
        public string BranchCode;

        [DataMember]
        public string PayPlanCode;

        [DataMember]
        public string PayGradeCode;
    }
}