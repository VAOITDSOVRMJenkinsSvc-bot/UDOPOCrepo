using System;
using UDO.LOB.Core;
using VRM.Integration.UDO.Common.Messages;

namespace VRM.Integration.UDO.Claims.Messages
{
    public class UDOcreateUDOLifecyclesRequest
    {
        public string MessageId { get; set; }
        public string OrganizationName { get; set; }
        public Guid UserId { get; set; }
        public Guid RelatedParentId { get; set; }
        public string RelatedParentEntityName { get; set; }
        public string RelatedParentFieldName { get; set; }
        public bool LogSoap { get; set; }
        public bool LogTiming { get; set; }
        public bool Debug { get; set; }
        public Guid ownerId { get; set; }
        public string ownerType { get; set; }

        public HeaderInfo LegacyServiceHeaderInfo { get; set; }
        public Guid udo_claimId { get; set; }
        public Int64 claimId { get; set; }
        public UDOcreateUDOLifecyclesRelatedEntitiesMultipleRequest[] UDOcreateUDOLifecyclesRelatedEntitiesInfo { get; set; }
    }
    public class UDOcreateUDOLifecyclesRelatedEntitiesMultipleRequest
    {
        public string RelatedEntityName { get; set; }
        public Guid RelatedEntityId { get; set; }
        public string RelatedEntityFieldName { get; set; }
    }
}