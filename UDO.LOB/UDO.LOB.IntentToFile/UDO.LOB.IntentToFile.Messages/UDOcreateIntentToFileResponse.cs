using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;

//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Messages;

namespace UDO.LOB.IntentToFile.Messages
{
    // [Export(typeof(IMessageBase))]
    // [ExportMetadata("MessageType", MessageRegistry.UDOcreateIntentToFileResponse)]
    [DataContract]
    public class UDOcreateIntentToFileResponse : MessageBase
    {
        [DataMember]
        public IntentToFileResponse[] IntentToFileIds { get; set; }
        [DataMember]
        public bool ExceptionOccurred { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
    }
    [DataContract]
    public class IntentToFileResponse
    {
        [DataMember]
        public Guid Id { get; set; }
    }
}
