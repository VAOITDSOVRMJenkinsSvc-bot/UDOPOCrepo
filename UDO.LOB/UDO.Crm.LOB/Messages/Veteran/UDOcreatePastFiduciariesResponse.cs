using System;
using Microsoft.Xrm.Sdk;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Messages;

/// <summary>
/// VIMT LOB Component for UDOcreatePastFiduciaries,createPastFiduciaries method, Response.
/// Code Generated by IMS on: 5/8/2015 2:31:27 PM
/// Version: 2015.05.05
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace VRM.Integration.UDO.Contact.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOcreatePastFiduciariesResponse)]
    [DataContract]
    public class UDOcreatePastFiduciariesResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccured { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public UDOcreatePastFiduciariesMultipleResponse[] UDOcreatePastFiduciariesInfo { get; set; }
        [DataMember]
        public UDOcreatePastPOAMultipleResponse[] UDOcreatePastPOAInfo { get; set; }
    }
    [DataContract]
    public class UDOcreatePastFiduciariesMultipleResponse
    {
        [DataMember]
        public Guid newUDOcreatePastFiduciariesId { get; set; }
    }
    [DataContract]
    public class UDOcreatePastPOAMultipleResponse
    {
        public Guid newUDOcreatePastPOAId { get; set; }
    }
}
