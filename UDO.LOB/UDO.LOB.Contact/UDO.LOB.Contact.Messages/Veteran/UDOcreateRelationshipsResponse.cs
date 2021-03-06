using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
//using UDO.LOB.Extensions;
using VEIS.Core.Messages;

//using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;

/// <summary>
/// VIMT LOB Component for UDOcreateRelationships,createRelationships method, Response.
/// Code Generated by IMS on: 5/8/2015 2:30:16 PM
/// Version: 2015.05.05
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace UDO.LOB.Contact.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOcreateRelationshipsResponse)]
    [DataContract]
    public class UDOcreateRelationshipsResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccured { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public UDOcreateRelationshipsMultipleResponse[] UDOcreateRelationshipsInfo { get; set; }
    }
    [DataContract]
    public class UDOcreateRelationshipsMultipleResponse
    {
        [DataMember]
        public Guid newUDOcreateRelationshipsId { get; set; }
    }
}
