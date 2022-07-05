using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
using VEIS.Core.Messages;

//using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Messages;

namespace UDO.LOB.Claims.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOcreateUDOMAPDLettersResponse)]
    [DataContract]
    public class UDOcreateUDOMAPDLettersResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccured { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public UDOcreateUDOMAPDLettersMultipleResponse[] UDOcreateUDOMAPDLettersInfo { get; set; }
    }
    [DataContract]
    public class UDOcreateUDOMAPDLettersMultipleResponse
    {
        [DataMember]
        public Guid newUDOcreateUDOMAPDLettersId { get; set; }
    }
}