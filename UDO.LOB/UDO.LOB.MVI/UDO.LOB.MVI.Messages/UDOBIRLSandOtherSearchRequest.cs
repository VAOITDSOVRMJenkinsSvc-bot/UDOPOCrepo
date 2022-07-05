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
    public class UDOBIRLSandOtherSearchRequest : MessageBase
    {
        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public Guid UserId { get; set; }
        
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
        public bool BypassMvi { get; set; }
        [DataMember]
        public bool MVICheck { get; set; }
        [DataMember]
        public bool IsAttended { get; set; }
        [DataMember]
        public UDOHeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public string interactionId { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string MiddleName { get; set; }
        [DataMember]
        public string FamilyName { get; set; }
        [DataMember]
        public string SSIdString { get; set; }
        [DataMember]
        public string BirthDate { get; set; }
        [DataMember]
        public string BranchofService { get; set; }
        [DataMember]
        public string ServiceNumber { get; set; }
        [DataMember]
        public string InsuranceNumber { get; set; }
        [DataMember]
        public string DeceasedDate { get; set; }
        [DataMember]
        public string EnteredonDutyDate { get; set; }
        [DataMember]
        public string ReleasedActiveDutyDate { get; set; }
        [DataMember]
        public string Suffix { get; set; }
        [DataMember]
        public string PayeeNumber { get; set; }
        [DataMember]
        public string FolderLocation { get; set; }
        [DataMember]
        public string UserFirstName { get; set; }
        [DataMember]
        public string UserLastName { get; set; }
        [DataMember]
        public SecureString SSId { get; set; }
    }
}
