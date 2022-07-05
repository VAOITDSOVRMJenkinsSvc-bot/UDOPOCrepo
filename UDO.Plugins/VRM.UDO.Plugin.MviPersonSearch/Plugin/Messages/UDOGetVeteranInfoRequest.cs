using System;

namespace VRM.Integration.UDO.MVI.Messages
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Reviewed By Brian Greig - 4/30/15
    /// </remarks>
    
    public class UDOGetVeteranInfoRequest
    {
        public string MessageId { get; set; }

        public string crme_OrganizationName { get; set; }

        
        public Guid crme_UserId { get; set; }

        
        public string crme_SSN { get; set; }
    }
}
