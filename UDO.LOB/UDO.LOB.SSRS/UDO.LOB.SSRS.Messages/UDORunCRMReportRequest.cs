#region Using Directives

using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;

//using VRM.Integration.Servicebus.Core;

#endregion

namespace UDO.LOB.SSRS.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDORunCRMReportRequest)]
    [DataContract]
    public class UDORunCRMReportRequest : MessageBase
    {
        [DataMember]
        public string OrganizationName { get; set; }

        [DataMember]
        public Guid OrganizationId { get; set; }

        [DataMember]
        public Guid UserId { get; set; }

     

        [DataMember]
        public bool LogTiming { get; set; }

        [DataMember]
        public bool LogSoap { get; set; }

        [DataMember]
        public bool Debug { get; set; }

        [DataMember]
        public Guid ownerId { get; set; }

        [DataMember]
        public string ownerType { get; set; }

        [DataMember]
        public UDOHeaderInfo LegacyServiceHeaderInfo { get; set; }

        // Request  Params
        [DataMember]
        public string udo_SSRSReportName { get; set; }

        [DataMember]
        public Guid udo_ReportId { get; set; }

        [DataMember]
        public Guid udo_LetterGenerationId { get; set; }

        [DataMember]
        public Guid udo_ServiceRequestId { get; set; }
        [DataMember]
        public Guid udo_FnodId { get; set; }

        [DataMember]
        public string udo_SourceUrl { get; set; }

        [DataMember]
        public UDOReportParameterValue[] udo_ParameterValues { get; set; }

        [DataMember]
        public string udo_FormatType { get; set; }

        [DataMember]
        public bool udo_UploadToVBMS { get; set; }

        [DataMember]
        public Guid udo_PersonId { get; set; }

        [DataMember]
        public PersonInfo udo_PersonInfo { get; set; }

        [DataMember]
        public Guid? udo_vbmsdocumentid { get; set; }
    }

    [DataContract]
    public class PersonInfo
    {
        [DataMember]
        public string udo_FileNumber { get; set; }

        [DataMember]
        public string udo_FirstName { get; set; }

        [DataMember]
        public string udo_MiddleName { get; set; }

        [DataMember]
        public string udo_LastName { get; set; }
    }

    [DataContract]
    public class UDOReportParameterValue
    {
        [DataMember]
        public string Label { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Value { get; set; }
    }
}