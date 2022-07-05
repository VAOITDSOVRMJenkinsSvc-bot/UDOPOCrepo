using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using VEIS.Core.Messages;

//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Contact.Messages;

namespace UDO.LOB.Contact.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOupdateHasBenefitsResponse)]
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

