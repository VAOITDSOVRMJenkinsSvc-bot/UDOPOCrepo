using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDO.LOB.Core;
namespace VRM.Integration.UDO.MVI.Messages
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
        public Guid? OwnerId { get; set; }
        public string OwnerType { get; set; }
        public HeaderInfo LegacyServiceHeaderInfo { get; set; }
        //public Guid udo_personId { get; set; }
        //public Guid udo_veteranId { get; set; }
        public Int64 ptcpntId { get; set; }
        public Int64 claimid { get; set; }
        public int LoadSize { get; set; }
        //public EntityReference owner { get; set; }
        //public Guid ownerId { get; set; }
        //public string ownerType { get; set; }
        public UDORelatedEntity[] RelatedEntities { get; set; }
        //public int Start { get; set; }
        //public int End { get; set; }
    }
}
