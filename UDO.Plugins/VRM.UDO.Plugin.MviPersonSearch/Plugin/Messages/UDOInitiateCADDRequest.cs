using System;
using UDO.LOB.Core;

namespace VRM.Integration.UDO.MVI.Messages
{

   
    public class UDOInitiateCADDRequest 
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

        public Guid udo_IDProofId { get; set; }
        public Guid va_bankaccountId { get; set; }
        public Guid udo_personId { get; set; }
        public Guid udo_veteranId { get; set; }
        public Guid udo_snapshotid { get; set; }
        public string awardtypecode { get; set; }
        public string vetfileNumber { get; set; }
        public string SSN { get; set; }
        public string appealFirstName { get; set; }
        public string appealLastName { get; set; }
        public Int64 ptcpntId { get; set; }
        public Int64 vetptcpntId { get; set; }
        public string PayeeCode { get; set; }
        public string RoutingNumber { get; set; }
        public UDOInitiateCADDRelatedEntitiesMultipleRequest[] UDOInitiateCADDRelatedEntitiesInfo { get; set; }
    }

    public class UDOInitiateCADDRelatedEntitiesMultipleRequest
    {
      
        public string RelatedEntityName { get; set; }
      
        public Guid RelatedEntityId { get; set; }
      
        public string RelatedEntityFieldName { get; set; }
    }
}