using System;
using System.Security;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;

namespace VRM.Integration.UDO.MVI.Messages
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Reviewed By Brian Greig - 4/30/15
    /// </remarks>
    [Export(typeof (IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOCombinedSelectedPersonRequest)]
    [DataContract]
    public class UDOCombinedSelectedPersonRequest : MessageBase
    {
        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public Guid UserId { get; set; }
        [DataMember]
        public int userSL { get; set; }
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
        public bool MVICheck { get; set; }
        [DataMember]
        public HeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public string interactionId { get; set; }
        /// <summary>
        /// Gets or sets the search identifier to use when the user clicks a record from search results grid.
        /// </summary>
        [DataMember]
        public string PatientSearchIdentifier { get; set; }
        [DataMember]
        public string ICN { get; set; }
        [DataMember]
        public string IdentifierClassCode { get; set; }

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
        /// If the search is with SSN, the authority is SSA, if it's with the VA then the value is VHA, etc.
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
                string.IsNullOrEmpty(IdentifierType) ? "NI" : IdentifierType,
                string.IsNullOrEmpty(AssigningFacility) ? "200DOD" : AssigningFacility,
                string.IsNullOrEmpty(AssigningAuthority) ? "USDOD" : AssigningAuthority);
        }

        [DataMember]
        public string UserFirstName { get; set; }

        [DataMember]
        public string UserLastName { get; set; }

        [DataMember]
        public string PrefixName { get; set; }
        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string MiddleName { get; set; }

        [DataMember]
        public string FamilyName { get; set; }

        [DataMember]
        public string SuffixName { get; set; }

        [DataMember]
        public string AliasName { get; set; }

        [DataMember]
        public string FullAddress { get; set; }
        [DataMember]
        public string FullName { get; set; }
        [DataMember]
        public string DateofBirth { get; set; }
        [DataMember]
        public string DeceasedDate { get; set; }
        [DataMember]
        public string Gender { get; set; }
        [DataMember]
        public string BranchOfService { get; set; }
        [DataMember]
        public string PhoneNumber { get; set; }
        [DataMember]
        public string ParticipantId { get; set; }
        [DataMember]
        public string IdentityTheft { get; set; }
        /// <summary>
        /// This is the raw value retrieved from MVI.
        /// </summary>
        /// 
        [DataMember]
        public string RawValueFromMvi { get; set; }

        [DataMember]
        public MessageProcessType FetchMessageProcessType { get; set; }

        [DataMember]
        public string RecordSource { get; set; }

        [DataMember]
        public string SSIdString { get; set; }
        [DataMember]
        public SecureString SSId { get; set; }
        [DataMember]
        public string FileNumber { get; set; }
        [DataMember]
        public string Edipi { get; set; }
        [DataMember]
        public Int64 participantID { get; set; }
        [DataMember]
        public bool IsAttended { get; set; }
        [DataMember]
        public int VeteranSensitivityLevel { get; set; }
        //[DataMember]
        //public UnattendedSearchRequest[] CorrespondingIdList { get; set; }
        [DataMember]
        public CorrespondingIDs[] CorrespondingIdList { get; set; }
    }
}
