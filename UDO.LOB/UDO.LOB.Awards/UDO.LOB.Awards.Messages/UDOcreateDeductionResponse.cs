using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Messages;
using UDO.LOB.Core;

/// <summary>
/// VIMT LOB Component for UDOcreateDeductions,createDeductions method, Response.
/// Code Generated by IMS on: 5/8/2015 1:20:26 PM
/// Version: 2015.05.05
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace UDO.LOB.Awards.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOcreateDeductionsResponse)]
    [DataContract]
    public class UDOcreateDeductionsResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccured { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public UDOcreateDeductionsMultipleResponse[] UDOcreateDeductionsInfo { get; set; }
    }
    [DataContract]
    public class UDOcreateDeductionsMultipleResponse
    {
        [DataMember]
        public Guid newUDOcreateDeductionsId { get; set; }
    }
}
