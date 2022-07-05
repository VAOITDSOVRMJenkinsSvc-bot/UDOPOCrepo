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
    public class UDOHandleDupCorpRecordResponse : MessageBase
    {
        [DataMember]
        public PatientPerson[] Person { get; set; }
        [DataMember]
        public string CORPDbMessage { get; set; }
        [DataMember]
        public string UDOMessage { get; set; }
        [DataMember]
        public bool ExceptionOccured { get; set; }
    }
}
