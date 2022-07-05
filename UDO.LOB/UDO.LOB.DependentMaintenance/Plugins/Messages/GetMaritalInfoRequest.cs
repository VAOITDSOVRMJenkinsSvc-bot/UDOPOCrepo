using System;

namespace VRM.CRME.Plugin.DependentMaintenance.Messages
{
    public class GetMaritalInfoRequest
    {
        public string MessageId { get; set; }
        
        public string crme_OrganizationName { get; set; }
        
        public Guid crme_UserId { get; set; }
        
        public string crme_SSN { get; set; }
        
        public string crme_ParticipantId { get; set; }
    }
}
