using System;
using Microsoft.Xrm.Sdk;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Messages;
/// <summary>
/// VIMT LOB Component for UDOcreateUDOAppealIssues,createUDOAppealIssues method, Response.
/// Code Generated by IMS on: 7/8/2015 5:14:06 PM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace VRM.Integration.UDO.Appeals.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOcreateUDOAppealIssuesResponse)]
    [DataContract]
    public class UDOcreateUDOAppealIssuesResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccured { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public UDOcreateUDOAppealIssuesMultipleResponse[] UDOcreateUDOAppealIssuesInfo { get; set; }
    }
    [DataContract]
    public class UDOcreateUDOAppealIssuesMultipleResponse
    {
        [DataMember]
        public int udo_SequenceNumber { get; set; }
        [DataMember]
        public string udo_ProgramDescription { get; set; }
        [DataMember]
        public string udo_Level3Description { get; set; }
        [DataMember]
        public string udo_Level2 { get; set; }
        [DataMember]
        public string udo_Level1Description { get; set; }
        [DataMember]
        public string udo_IssueDescription { get; set; }
        [DataMember]
        public string udo_IssueCodeDescription { get; set; }
        [DataMember]
        public string udo_DispositionDescription { get; set; }
        [DataMember]
        public DateTime udo_DispositionDate { get; set; }
    }
}
