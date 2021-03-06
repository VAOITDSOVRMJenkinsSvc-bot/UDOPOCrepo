using System;
using Microsoft.Xrm.Sdk;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Messages;

/// <summary>
/// VIMT LOB Component for UDOcreateUDOVirtualVA,createUDOVirtualVA method, Response.
/// Code Generated by IMS on: 7/20/2015 3:41:31 PM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace VRM.Integration.UDO.ExamsAppointments.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOcreateUDOExamRequestResponse)]
    [DataContract]
    public class UDOcreateUDOExamRequestResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccured { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public UDOcreateUDOExamRequestMultipleResponse[] UDOcreateUDOExamsAppointmentsInfo { get; set; }
    }
    [DataContract]
    public class UDOcreateUDOExamRequestMultipleResponse
    {
        [DataMember]
        public Guid newUDOcreateUDOExamAppointmentsId { get; set; }
    }
}