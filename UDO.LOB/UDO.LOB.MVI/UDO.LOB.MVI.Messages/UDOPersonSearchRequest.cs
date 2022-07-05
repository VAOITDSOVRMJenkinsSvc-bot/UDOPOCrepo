using System;
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
    public class UDOPersonSearchRequest : MessageBase
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
        public bool Debug { get; set; }
        [DataMember]
        public UDOHeaderInfo LegacyServiceHeaderInfo { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string MiddleName { get; set; }

        [DataMember]
        public string FamilyName { get; set; }

        [DataMember]
        public string Edipi { get; set; }

        [DataMember]
        public string SSIdString { get; set; }
        [DataMember]
        public byte[] SSId { get; set; }
        [DataMember]
        public string BirthDate { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the search identifier to use when the user clicks a record from search results grid.
        /// </summary>
        [DataMember]
        public string PatientSearchIdentifier { get; set; }

        [DataMember]
        public string IdentifierClassCode { get; set; }

        [DataMember]
        public string SearchUse { get; set; }

        /// <summary>
        /// NI - National Identifier 
        /// PI - Patient Identifier
        /// EI - Employee Identifier
        /// PN - Patient Number 
        /// SS – Social Security
        /// </summary>
        [DataMember]
        public string IdentifierType { get; set; }

        /// <summary>
        /// This is the organizationn identifier -- similar to the identifier for UDO, which is "200CMRE"
        /// </summary>
        [DataMember]
        public string AssigningFacility { get; set; }

        /// <summary>
        /// SSN:USSSA, VA Patient Id:UAVHA, Military: USDOD etc.
        /// </summary>
        [DataMember]
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


        [DataMember]
        public string UserFirstName { get; set; }

        [DataMember]
        public string UserLastName { get; set; }


        /// <summary>
        /// Gets or sets whether the search should be treated as an Attended search. This overrides the unattended search functionalities.
        /// </summary>
        [DataMember]
        public bool IsAttended { get; set; }


        // TODO: Commented to check the need in new remediated code
        //[DataMember]
        //public MessageProcessType FetchMessageProcessType { get; set; }

        /// <summary>
        /// This is the raw value retrieved from MVI.
        /// </summary>
        [DataMember]
        public string RawValueFromMvi { get; set; }

        /// <summary>
        /// This is the Assigning Authority OID.
        /// Based on MVI SSD, this value is not supported by the VA; however, it could be in the future.
        /// </summary>
        [DataMember]
        public string AuthorityOid { get; set; }
        /// <summary>
        /// This is the EDIPI, SSN, VA ID, etc.
        /// </summary>
        [DataMember]
        public string PatientIdentifier { get; set; }

        [DataMember]
        public string BranchOfService { get; set; }
        [DataMember]
        public string interactionId { get; set; }
        [DataMember]
        public bool AlreadyQueriedMvi { get; set; }
    }
}
