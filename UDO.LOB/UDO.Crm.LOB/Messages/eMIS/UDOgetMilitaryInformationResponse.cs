using System;
using Microsoft.Xrm.Sdk;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Common.Messages;
using VRM.Integration.UDO.Messages;

namespace VRM.Integration.UDO.eMIS.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOgetMilitaryInformationResponse)]
    [DataContract]
    public class UDOgetMilitaryInformationResponse : UDOResponseBase
    {
        [DataMember]
        public UDOMilitaryServiceInfo udo_MostRecentService { get; set; }

        [DataMember(IsRequired=false)]
        public UDOMilitaryServiceInfo[] udo_MilitaryServiceHistory { get; set; }
    }

    [DataContract]
    public class UDOMilitaryServiceInfo
    {
        [DataMember]
        public DateTime? StartDate;

        [DataMember]
        public DateTime? EndDate;

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