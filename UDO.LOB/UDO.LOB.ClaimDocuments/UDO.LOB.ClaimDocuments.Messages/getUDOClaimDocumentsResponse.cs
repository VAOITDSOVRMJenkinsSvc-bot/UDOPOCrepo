using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;

//using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Messages;

namespace UDO.LOB.ClaimDocuments.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.getUDOClaimDocumentsResponse)]
    [DataContract]
    public class getUDOClaimDocumentsResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccured { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public getUDOClaimDocumentsResponseData[] getUDOClaimDocumentsResponseInfo { get; set; }
    }
    [DataContract]
    public class getUDOClaimDocumentsResponseData
    {
        [DataMember]
        public Guid udo_attachmentId { get; set; }
        [DataMember]
        public byte[] udo_lettertxt { get; set; }

    }
}