using System;
using UDO.LOB.Core;
namespace VRM.Integration.UDO.MVI.Messages
{
    public class UDOgetVBMSDocumentContentRequest
    {
        public string MessageId { get; set; }
        public string OrganizationName { get; set; }
        public Guid UserId { get; set; }
        public Guid RelatedParentId { get; set; }
        public string RelatedParentEntityName { get; set; }
        public string RelatedParentFieldName { get; set; }
        public bool LogTiming { get; set; }
        public bool LogSoap { get; set; }
        public bool Debug { get; set; }
        public HeaderInfo LegacyServiceHeaderInfo { get; set; }
        public string udo_DocumentVersionRefId { get; set; }
        public Guid udo_VBMSeFolderId { get; set; }
    }
}