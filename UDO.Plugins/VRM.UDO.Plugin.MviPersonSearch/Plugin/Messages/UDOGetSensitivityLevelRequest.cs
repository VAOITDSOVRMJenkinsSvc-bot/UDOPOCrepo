using System;

namespace VRM.Integration.UDO.MVI.Messages
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Reviewed By Brian Greig - 4/30/15
    /// </remarks>
 
    public class UDOGetSensitivityLevelRequest 
    {

        public string MessageId { get; set; }

        public string OrganizationName { get; set; }

        
        public Guid UserId { get; set; }

        
        public string SSN { get; set; }

        
        public long? ParticipantId { get; set; }

    }
}

