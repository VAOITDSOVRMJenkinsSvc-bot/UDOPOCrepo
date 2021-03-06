using System;
using Microsoft.Xrm.Sdk;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Messages;
/// <summary>
/// VIMT LOB Component for UDOcreateAwardAdjustment,createAwardAdjustment method, Response.
/// Code Generated by IMS on: 6/4/2015 9:39:25 AM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace VRM.Integration.UDO.Payments.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOcreateAwardAdjustmentResponse)]
    [DataContract]
    public class UDOcreateAwardAdjustmentResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccured { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public UDOcreateAwardAdjustmentMultipleResponse[] UDOcreateAwardAdjustmentInfo { get; set; }
    }
    [DataContract]
    public class UDOcreateAwardAdjustmentMultipleResponse
    {
        [DataMember]
        public Guid newUDOcreateAwardAdjustmentId { get; set; }
    }
}