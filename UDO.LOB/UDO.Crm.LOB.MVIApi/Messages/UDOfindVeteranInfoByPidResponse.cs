using System;
using VIMT.VeteranWebService.Messages;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;

namespace VRM.Integration.UDO.MVI.Messages
{

    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOfindVeteranInfoResponse)]
    [DataContract]
    public class UDOfindVeteranInfoByPidResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccured { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public UDOfindVeteranInfo UDOfindVeteranInfoInfo { get; set; }
    }
}
