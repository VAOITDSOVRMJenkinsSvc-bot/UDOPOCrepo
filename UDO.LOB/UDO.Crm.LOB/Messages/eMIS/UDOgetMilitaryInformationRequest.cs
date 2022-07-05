using System;
using System.ComponentModel.Composition;
using Microsoft.Xrm.Sdk;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Common.Messages;
using VRM.Integration.UDO.Messages;

namespace VRM.Integration.UDO.eMIS.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOgetMilitaryInformationRequest)]
    [DataContract]
    public class UDOgetMilitaryInformationRequest : UDORequestBase
    {
        [DataMember]
        public String udo_EDIPI { get; set; }

        [DataMember]
        public String udo_ICN { get; set; }

        [DataMember]
        public bool udo_MostRecentServiceOnly { get; set; }
    }
}