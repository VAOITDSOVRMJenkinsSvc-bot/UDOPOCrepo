using System;
using System.Runtime.Serialization;
using UDO.LOB.Core;

//using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;

/// <summary>
/// VIMT LOB Component for UDOcreateDependents,createDependents method, Response.
/// Code Generated by IMS on: 5/6/2015 12:41:42 PM
/// Version: 2015.05.05
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace UDO.LOB.Contact.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOcreateDependentsResponse)]
    [DataContract]
    public class UDOcreateDependentsResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccured { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public UDOcreateDependentsMultipleResponse[] UDOcreateDependentsInfo { get; set; }
    }
    [DataContract]
    public class UDOcreateDependentsMultipleResponse
    {
        [DataMember]
        public Guid newUDOcreateDependentsId { get; set; }
    }
}
