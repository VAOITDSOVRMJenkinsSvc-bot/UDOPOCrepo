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
    public class UDOCombinedSelectedPersonRequest
    {
        /// <summary>
        /// Gets or sets the search identifier to use when the user clicks a record from search results grid.
        /// </summary>
        public string MessageId { get; set; }
        public string OrganizationName { get; set; }
        public Guid UserId { get; set; }
        public int userSL { get; set; }
        public Guid RelatedParentId { get; set; }
        public string RelatedParentEntityName { get; set; }
        public string RelatedParentFieldName { get; set; }
        public bool LogTiming { get; set; }
        public bool LogSoap { get; set; }
        public bool Debug { get; set; }
        public string interactionId { get; set; }
        public HeaderInfo LegacyServiceHeaderInfo { get; set; }
        public bool noAddPerson { get; set; }
        public bool MVICheck { get; set; }
        public bool BypassMvi { get; set; }
        public string PatientSearchIdentifier { get; set; }
        public string ICN { get; set; }
        public string IdentifierClassCode { get; set; }

        /// <summary>
        /// NI - National Identifier 
        /// PI - Patient Identifier
        /// EI - Employee Identifier
        /// PN - Patient Number 
        /// SS – Social Security
        /// </summary>
        public string IdentifierType { get; set; }

        /// <summary>
        /// This is the organizationn identifier -- similar to the identifier for UDO, which is "200CMRE"
        /// </summary>
        public string AssigningFacility { get; set; }
     
        /// <summary>
        /// If the search is with SSN, the authority is SSA, if it's with the VA then the value is VHA, etc.
        /// </summary>
        
        public string AssigningAuthority { get; set; }

        /// <summary>
        /// Returns the Source ID for the MVI search. Source Id is based on the combination of the
        /// "PatientSearchIdentifier^IdentifierType^AssigningFacility^AssigningAuthority". Not setting
        /// the values for IdentifierType, AssigningFacility and AssigningAuthority returns the DOD Source Id as default.
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0}^{1}^{2}^{3}", PatientSearchIdentifier,
                string.IsNullOrEmpty(IdentifierType) ? "NI" : IdentifierType,
                string.IsNullOrEmpty(AssigningFacility) ? "200DOD" : AssigningFacility,
                string.IsNullOrEmpty(AssigningAuthority) ? "USDOD" : AssigningAuthority);
        }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string PrefixName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string FamilyName { get; set; }
        public string SuffixName { get; set; }
        public string AliasName { get; set; }
        public string FullAddress { get; set; }
        public string FullName { get; set; }
        public string DateofBirth { get; set; }
        public string DeceasedDate { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string ParticipantId { get; set; }
        public bool IsAttended { get; set; }
        public string IdentityTheft { get; set; }
        
        /// <summary>
        /// This is the raw value retrieved from MVI.
        /// </summary>
        public string RawValueFromMvi { get; set; }
        public string FetchMessageProcessType { get; set; }

        public string RecordSource { get; set; }
        public string FileNumber { get; set; }
        //public string SocialSecurityNumber { get; set; }
        public string SSIdString { get; set; }
        public Int64 participantID { get; set; }
        public string Edipi { get; set; }
        public int VeteranSensitivityLevel { get; set; }
    }
}
