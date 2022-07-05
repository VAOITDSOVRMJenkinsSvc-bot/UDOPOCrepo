using System;
using UDO.LOB.Core;

namespace VRM.Integration.UDO.SSRS.Messages
{
    public class UDORunCRMReportRequest
    {
        public string MessageId { get; set; }
        public string OrganizationName { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid UserId { get; set; }
        public Guid RelatedParentId { get; set; }
        public string RelatedParentEntityName { get; set; }
        public string RelatedParentFieldName { get; set; }
        public bool LogTiming { get; set; }
        public bool LogSoap { get; set; }
        public bool Debug { get; set; }
        public Guid ownerId { get; set; }
        public string ownerType { get; set; }
        public HeaderInfo LegacyServiceHeaderInfo { get; set; }
        public string udo_SSRSReportName { get; set; }
        public Guid udo_ReportId { get; set; }
        public Guid udo_LetterGenerationId { get; set; }
        public Guid udo_ServiceRequestId { get; set; }
        public Guid udo_FnodId { get; set; }
        public string udo_SourceUrl { get; set; }
        public UDOReportParameterValue[] udo_ParameterValues { get; set; }
        public string udo_FormatType { get; set; }
        public SSRSConnectionInfo udo_ConnectionInfo { get; set; }
        public bool udo_UploadToVBMS { get; set; }
        public Guid udo_PersonId { get; set; }
        public PersonInfo udo_PersonInfo { get; set; }
        public string udo_ClaimNumber { get; set; }
        public Guid udo_vbmsdocumentid { get; set; }
    }

    public class PersonInfo
    {
        public string udo_FileNumber { get; set; }
        public string udo_FirstName { get; set; }
        public string udo_MiddleName { get; set; }
        public string udo_LastName { get; set; }
    }

    public class UDOReportParameterValue
    {
        public string Label { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class SSRSConnectionInfo
    {
        public string EndpointUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public string ClientCredentialType { get; set; }
    }
}