using System;
using UDO.LOB.Core;

namespace VRM.Integration.UDO.Notes.Messages
{
    public class UDODeleteNoteRequest
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
        //public Guid OwnerId { get; set; }
        //public string OwnerType { get; set; }
        public HeaderInfo LegacyServiceHeaderInfo { get; set; }
        public string udo_ClaimId { get; set; }
        public string udo_LegacyNoteId { get; set; }
        public string udo_dtTime { get; set; }
        public string udo_Note { get; set; }
        public string udo_ParticipantID { get; set; }
        public string udo_RO { get; set; }
        public string udo_Type { get; set; }
        public string udo_pctptnttc { get; set; }
        public string udo_User { get; set; }
    }
}