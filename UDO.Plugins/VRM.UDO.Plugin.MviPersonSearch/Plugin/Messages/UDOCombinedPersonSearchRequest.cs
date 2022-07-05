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
 
    public class UDOCombinedPersonSearchRequest
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
        public string FirstName { get; set; }

        
        public string MiddleName { get; set; }

        
        public string FamilyName { get; set; }

        
        public string Edipi { get; set; }

        
        //public string SocialSecurityNumber { get; set; }
        public string SSIdString { get; set; }
        
        public string BirthDate { get; set; }

        
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the search identifier to use when the user clicks a record from search results grid.
        /// </summary>
        
        public string PatientSearchIdentifier { get; set; }

        
        public string IdentifierClassCode { get; set; }

        
        public string SearchUse { get; set; }

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
        /// SSN:USSSA, VA Patient Id:UAVHA, Military: USDOD etc.
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
                !string.IsNullOrEmpty(Edipi) || IdentifierClassCode.Equals("MIL") ? "NI" : IdentifierType,
                !string.IsNullOrEmpty(Edipi) || IdentifierClassCode.Equals("MIL") ? "200DOD" : AssigningFacility,
                !string.IsNullOrEmpty(Edipi) || IdentifierClassCode.Equals("MIL") ? "USDOD" : AssigningAuthority);
        }

        
        
        public string UserFirstName { get; set; }

        
        public string UserLastName { get; set; }


        /// <summary>
        /// Gets or sets whether the search should be treated as an Attended search. This overrides the unattended search functionalities.
        /// </summary>
        
        public bool IsAttended { get; set; }

        
        //public string FetchMessageProcessType { get; set; }
        /// <summary>
        /// This is the raw value retrieved from MVI.
        /// </summary>
        
        public string RawValueFromMvi { get; set; }

        /// <summary>
        /// This is the Assigning Authority OID.
        /// Based on MVI SSD, this value is not supported by the VA; however, it could be in the future.
        /// </summary>
        
        public string AuthorityOid { get; set; }
        /// <summary>
        /// This is the EDIPI, SSN, VA ID, etc.
        /// </summary>
        
        public string PatientIdentifier { get; set; }

        
        public string BranchOfService { get; set; }

    }
}
