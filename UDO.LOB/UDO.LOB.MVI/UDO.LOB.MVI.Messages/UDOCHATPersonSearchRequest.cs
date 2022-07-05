using System;
using System.Security;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;

namespace UDO.LOB.MVI.Messages
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// </remarks>
    [DataContract]
    public class UDOCHATPersonSearchRequest : MessageBase
    {
        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public Guid UserId { get; set; }
        [DataMember]
        public Guid RelatedParentId { get; set; }
        [DataMember]
        public string RelatedParentEntityName { get; set; }
        [DataMember]
        public string RelatedParentFieldName { get; set; }
        [DataMember]
        public bool LogTiming { get; set; }
        [DataMember]
        public bool LogSoap { get; set; }
        [DataMember]
        public bool noAddPerson { get; set; }
        [DataMember]
        public bool Debug { get; set; }
        [DataMember]
        public int userSL { get; set; }
        [DataMember]
        public bool MVICheck { get; set; }
        [DataMember]
        public bool BypassMvi { get; set; }
        [DataMember]
        public UDOHeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public string interactionId { get; set; }
        [DataMember]
        public string Edipi { get; set; }
        [DataMember]
        public string SSIdString { get; set; }
        [DataMember]
        public string ParticipantId { get; set; }
        [DataMember]
        public string UserFirstName { get; set; }

        [DataMember]
        public string UserLastName { get; set; }
        [DataMember]
        public SecureString SSId { get; set; }
       
        /// <summary>
        /// Gets or sets whether the search should be treated as an Attended search. This overrides the unattended search functionalities.
        /// </summary>
        [DataMember]
        public bool IsAttended { get; set; }

    }
}
