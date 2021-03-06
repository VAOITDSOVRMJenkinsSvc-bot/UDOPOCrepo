using System;
using Microsoft.Xrm.Sdk;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Messages;

/// <summary>
/// VIMT LOB Component for UDOcreateUDOTrackedItems,createUDOTrackedItems method, Response.
/// Code Generated by IMS on: 5/29/2015 2:35:54 PM
/// Version: 2015.05.05
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace VRM.Integration.UDO.Claims.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOcreateUDOTrackedItemsResponse)]
    [DataContract]
    public class UDOcreateUDOTrackedItemsResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccured { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public UDOcreateUDOTrackedItemsMultipleResponse[] UDOcreateUDOTrackedItemsInfo { get; set; }
    }
    [DataContract]
    public class UDOcreateUDOTrackedItemsMultipleResponse
    {
        [DataMember]
        public Guid newUDOcreateUDOTrackedItemsId { get; set; }
    }
}