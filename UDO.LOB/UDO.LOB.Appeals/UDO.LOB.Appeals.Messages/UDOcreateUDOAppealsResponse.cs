using System;
using System.Runtime.Serialization;
using UDO.LOB.Core;

//using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Messages;
//using System.ComponentModel.Composition;
//using VEIS.Core.Messages;

/// <summary>
/// VIMT LOB Component for UDOcreateUDOAppeals,createUDOAppeals method, Response.
/// Code Generated by IMS on: 7/8/2015 5:03:28 PM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace UDO.LOB.Appeals.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOcreateUDOAppealsResponse)]
    [DataContract]
    public class UDOcreateUDOAppealsResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccured { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public UDOcreateUDOAppealsMultipleResponse[] UDOcreateUDOAppealsInfo { get; set; }
    }
    [DataContract]
    public class UDOcreateUDOAppealsMultipleResponse
    {
        [DataMember]
        public Guid newUDOcreateUDOAppealsId { get; set; }
    }
}
