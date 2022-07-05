using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Contact.Messages;
using VRM.Integration.UDO.Messages;

namespace VRM.Integration.UDO.Contact.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOupdateHasBenefitsResponse)]
    [DataContract]
    public class UDOupdateHasBenefitsResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccurred { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public string Edipi { get; set; }
    }
}

