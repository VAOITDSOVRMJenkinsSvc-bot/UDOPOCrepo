using System;
using UDO.LOB.Core;
namespace VRM.Integration.UDO.MVI.Messages
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Reviewed By Brian Greig - 4/30/15
    /// </remarks>
 
    public class UDOCHATPersonSearchRequest
    {

        public string MessageId { get; set; }
        public string OrganizationName { get; set; }
        public Guid UserId { get; set; }
        public Guid RelatedParentId { get; set; }
        public string RelatedParentEntityName { get; set; }
        public string RelatedParentFieldName { get; set; }
        public bool LogTiming { get; set; }
        public bool LogSoap { get; set; }
        public bool noAddPerson { get; set; }
        public bool Debug { get; set; }
        public string interactionId { get; set; }
        public bool MVICheck { get; set; }
        public int userSL { get; set; }
        public bool BypassMvi { get; set; }
        public HeaderInfo LegacyServiceHeaderInfo { get; set; }
        public string Edipi { get; set; }
        //public string SocialSecurityNumber { get; set; }
        public string SSIdString { get; set; }
        public string ParticipantId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        /// <summary>
        /// Gets or sets whether the search should be treated as an Attended search. This overrides the unattended search functionalities.
        /// </summary>
        
        public bool IsAttended { get; set; }

        
    }
}
