using System;
using UDO.LOB.Core;
namespace VRM.Integration.UDO.MVI.Messages
{
    public class UDOInitiateFNODRequest
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
        public Guid udo_idproofId { get; set; }
        public Guid udo_personId { get; set; }
        public Guid udo_veteranId { get; set; }
        public string fileNumber { get; set; }
        public string SSN { get; set; }
        public Int64 ptcpntId { get; set; }
        public string vetfileNumber { get; set; }
        public string vetSSN { get; set; }
        public Int64 vetptcpntId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public HeaderInfo LegacyServiceHeaderInfo { get; set; }
        public UDOInitiateFNODRelatedEntitiesMultipleRequest[] UDOCreateFNODRelatedEntitiesInfo { get; set; }
    }

    
    public class UDOInitiateFNODRelatedEntitiesMultipleRequest
    {
        public string RelatedEntityName { get; set; }
        public Guid RelatedEntityId { get; set; }
        public string RelatedEntityFieldName { get; set; }
    }
}
