using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;

//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Messages;

namespace UDO.LOB.IntentToFile.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOfindZipCodeResponse)]
    [DataContract]
    public class UDOInitiateITFResponse : MessageBase
    {
        [DataMember]
        public string parameter { get; set; }
        [DataMember]
        public Guid udo_veteranId { get; set; }
        [DataMember]
        public bool ExceptionOccurred { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
    }
}