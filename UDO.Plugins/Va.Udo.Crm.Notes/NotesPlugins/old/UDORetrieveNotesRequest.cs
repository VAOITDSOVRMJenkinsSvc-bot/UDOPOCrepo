using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRM.Integration.UDO.Notes.Messages
{
    public class UDORetrieveNotesAsyncRequest : UDORetrieveNotesRequest
    {
    }
    public class UDORetrieveNotesRequest
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
        public Guid ownerId { get; set; }
        public string ownerType { get; set; }
        public HeaderInfo LegacyServiceHeaderInfo { get; set; }
        public Guid udo_personId { get; set; }
        public Guid udo_veteranId { get; set; }
        public Int64 ptcpntId { get; set; }
        public Int64 claimid { get; set; }
        public EntityReference owner { get; set; }
        //public Guid ownerId { get; set; }
        //public string ownerType { get; set; }
        public UDORetrieveNotesRelatedEntitiesMultipleRequest[] UDORetrieveNotesRelatedEntitiesInfo { get; set; }
        //public int Start { get; set; }
        //public int End { get; set; }
    }
    public class UDORetrieveNotesRelatedEntitiesMultipleRequest
    {
        public string RelatedEntityName { get; set; }
        public Guid RelatedEntityId { get; set; }
        public string RelatedEntityFieldName { get; set; }
    }
}
