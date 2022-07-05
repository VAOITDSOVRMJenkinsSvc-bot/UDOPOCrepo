using System;
using System.ComponentModel.Composition;
using Microsoft.Xrm.Sdk;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Messages;

namespace VRM.Integration.UDO.IntentToFile.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOcreateIntentToFileResponse)]
    [DataContract]
    public class UDOcreateIntentToFileResponse : MessageBase
    {
        [DataMember]
        public IntentToFileResponse[] IntentToFileIds { get; set; }
        [DataMember]
        public bool ExceptionOccured { get; set; }
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
