using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;

//using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;

/// <summary>
/// VIMT LOB Component for UDOcreateUDOVirtualVA,createUDOVirtualVA method, Response.
/// Code Generated by IMS on: 7/22/2015 4:09:38 PM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace UDO.LOB.VirtualVA.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOcreateUDOVirtualVAResponse)]
    [DataContract]
    public class UDOcreateUDOVirtualVAResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccurred { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public UDOcreateUDOVirtualVAMultipleResponse[] UDOcreateUDOVirtualVAInfo { get; set; }
    }
    [DataContract]
    public class UDOcreateUDOVirtualVAMultipleResponse
    {
        [DataMember]
        public Guid newUDOcreateUDOVirtualVAId { get; set; }
    }
}