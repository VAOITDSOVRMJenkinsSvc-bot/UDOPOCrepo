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

    public class UDOBIRLSandOtherSearchRequest
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
        public int userSL { get; set; }
        public string interactionId { get; set; }
        public HeaderInfo LegacyServiceHeaderInfo { get; set; }

       
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string FamilyName { get; set; }
        //public string SocialSecurityNumber { get; set; }
        public string SSIdString { get; set; }
        public string BirthDate { get; set; }
        public string BranchofService { get; set; }
        public string ServiceNumber { get; set; }
        public string InsuranceNumber { get; set; }
        public string DeceasedDate { get; set; }
        public string EnteredonDutyDate { get; set; }
        public string ReleasedActiveDutyDate { get; set; }
        public string Suffix { get; set; }
        public string PayeeNumber { get; set; }
        public string FolderLocation { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
    }
}
