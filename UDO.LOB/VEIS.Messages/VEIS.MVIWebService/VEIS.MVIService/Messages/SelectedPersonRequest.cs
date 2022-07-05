using System;
using System.Runtime.Serialization;
using VEIS.Core.Core;

namespace VEIS.Mvi.Messages
{
    [DataContract]
    public class SelectedPersonRequest : MessageBase
    {
        public SelectedPersonRequest()
        {
        }

        /// <summary>
        /// Constructor: SelectedPersonRequest
        /// </summary>
        /// <param name="edipi">Veteran's EDIPI</param>
        /// <param name="userId">User's CRM ID</param>
        /// <param name="userFirstName">User's first name</param>
        /// <param name="userLastName">User's last name</param>
        /// <param name="organization">CRM org name</param>
        /// <param name="messsageId"></param>
        public SelectedPersonRequest(string edipi, Guid userId, string userFirstName, string userLastName, string organization, string messsageId, string recordSource = "MVI")
        {
            PatientSearchIdentifier = edipi;
            IdentifierType = "NI";
            AssigningFacility = "200DOD";
            AssigningAuthority = "USDOD";
            UserId = userId;
            UserFirstName = userFirstName;
            UserLastName = userLastName;
            OrganizationName = organization;
            MessageId = messsageId;
            RecordSource = recordSource;
        }

        /// <summary>
        /// Constructor: SelectedPersonRequest
        /// </summary>
        /// <param name="patientId">Veteran's patient Id</param>
        /// <param name="userFirstName">User's first name</param>
        /// <param name="userLastName">User's last name</param>
        /// <param name="userId">User's CRM ID</param>
        /// <param name="organization">CRM org name</param>
        /// <param name="messsageId"></param>
        public SelectedPersonRequest(string patientId, string userFirstName, string userLastName, Guid userId, string organization, string messsageId, string recordSource = "MVI")
        {
            PatientSearchIdentifier = patientId;
            IdentifierType = "PI";
            AssigningFacility = "200CORP";
            AssigningAuthority = "USVBA^A";
            UserId = userId;
            UserFirstName = userFirstName;
            UserLastName = userLastName;
            OrganizationName = organization;
            MessageId = messsageId;
            RecordSource = recordSource;
        }

        /// <summary>
        /// Constructor: SelectedPersonRequest
        /// </summary>
        /// <param name="rawValueFromMvi">If available, the raw id that was returned from MVI. EX: 123456789^NI^200DOD^USDOD</param>
        /// <param name="organization">The CRM  Org Name</param>
        /// <param name="userFirstName">The user's first name</param>
        /// <param name="userLastName">The user's last name</param>
        /// <param name="userId">The user's Id</param>
        /// <param name="messsageId"></param>
        public SelectedPersonRequest(string rawValueFromMvi, string organization, string userFirstName, string userLastName, Guid userId, string messsageId, string recordSource = "MVI")
        {
            UserId = userId;
            UserFirstName = userFirstName;
            UserLastName = userLastName;
            OrganizationName = organization;
            RawValueFromMvi = rawValueFromMvi;
            UseRawMviValue = true;
            RecordSource = recordSource;
        }

        public bool UseRawMviValue { get; set; }

        /// <summary>
        /// Gets or sets the search identifier to use when the user clicks a record from the search results grid.
        /// </summary>
        [DataMember]
        public string PatientSearchIdentifier { get; set; }

        /// <summary>
        /// Gets or sets a value to determine what type of PatientSearchIdentifier is being used for the seacrch. Ex: If EDIPI is used, IdentifierClassCode = 'MIL'
        /// </summary>
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
        /// This is the organizationn identifier -- similar to the identifier for CRMe, which is "200CMRE"
        /// </summary>
        [DataMember]
        public string AssigningFacility { get; set; }

        /// <summary>
        /// If the search is with SSN, the authority is SSA, if it's with the VA then the value is VHA, etc.
        /// </summary>
        [DataMember]
        public string AssigningAuthority { get; set; }

        [DataMember(IsRequired = false)]
        public bool LogTiming { get; set; }


        [DataMember(IsRequired = false)]
        public bool LogSoap { get; set; }

        /// <summary>
        /// Returns the Source ID for the MVI search. Source Id is based on the combination of the
        /// "PatientSearchIdentifier^IdentifierType^AssigningFacility^AssigningAuthority". Not setting
        /// the values for IdentifierType, AssigningFacility and AssigningAuthority returns the DOD Source Id as default.
        /// </summary>
        public override string ToString()
        {
            return UseRawMviValue ? RawValueFromMvi :
                string.Format("{0}^{1}^{2}^{3}", PatientSearchIdentifier,
                string.IsNullOrEmpty(IdentifierType) ? "NI" : IdentifierType,
                string.IsNullOrEmpty(AssigningFacility) ? "200DOD" : AssigningFacility,
                string.IsNullOrEmpty(AssigningAuthority) ? "USDOD" : AssigningAuthority);
        }

        /// <summary>
        /// Gets or sets veteran's first mame
        /// </summary>
        [DataMember]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets veteran's middle name
        /// </summary>
        [DataMember]
        public string MiddleName { get; set; }

        /// <summary>
        /// Gets or sets veteran's last name
        /// </summary>
        [DataMember]
        public string FamilyName { get; set; }

        /// <summary>
        /// Gets or sets veteran's full address
        /// </summary>
        [DataMember]
        public string FullAddress { get; set; }

        /// <summary>
        /// Gets or sets veteran's full name
        /// </summary>
        [DataMember]
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets veteran's date of birth
        /// </summary>
        [DataMember]
        public string DateofBirth { get; set; }

        /// <summary>
        /// Gets or sets veteran's social security number
        /// </summary>
        [DataMember]
        public string SocialSecurityNumber { get; set; }

        /// <summary>
        /// Gets or sets veteran's EDIPI
        /// </summary>
        [DataMember]
        public string Edipi { get; set; }

        /// <summary>
        /// This is the raw value retrieved from MVI.
        /// </summary>
        [DataMember]
        public string RawValueFromMvi { get; set; }

        //[DataMember]
        //public MessageProcessType FetchMessageProcessType { get; set; }

        /// <summary>
        /// Gets or sets the source of the record search: MVI, CORPDB, etc.
        /// </summary>
        [DataMember]
        public string RecordSource { get; set; }

        /// <summary>
        /// Gets or sets the CRM org name
        /// </summary>
        [DataMember]
        public string OrganizationName { get; set; }

        /// <summary>
        /// Gets or sets the CRM user ID
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the CRM user's first name
        /// </summary>
        [DataMember]
        public string UserFirstName { get; set; }

        /// <summary>
        /// Gets or sets the CRM user's last name
        /// </summary>
        [DataMember]
        public string UserLastName { get; set; }

    }
}
