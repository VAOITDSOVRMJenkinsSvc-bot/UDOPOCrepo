using System;

using System.Runtime.Serialization;
using VEIS.Core.Core;

namespace VEIS.Mvi.Messages
{
    /// <summary>
    /// Messaage class for executing Unattended searches: RetievePerson, GetCorrespondingIds
    /// </summary>
    [DataContract]
    public class UnattendedSearchRequest : MessageBase
    {
        public UnattendedSearchRequest()
        {
        }

        /// <summary>
        /// Constructor: UnattendedSearchRequest
        /// </summary>
        /// <param name="edipi">Veteran's EDIPI</param>
        /// <param name="userId">User's CRM ID</param>
        /// <param name="userFirstName">CRM user's first name</param>
        /// <param name="userLastName">CRM user's last name</param>
        /// <param name="organization">CRM org name</param>
        /// <param name="messageId">Message id</param>
        public UnattendedSearchRequest(string edipi, Guid userId, string userFirstName, string userLastName, string organization, string messageId)
        {
            PatientIdentifier = edipi;
            IdentifierType = "NI";
            AssigningFacility = "200DOD";
            AssigningAuthority = "USDOD";
            UserId = userId;
            UserFirstName = userFirstName;
            UserLastName = userLastName;
            OrganizationName = organization;
            MessageId = messageId;
        }

        /// <summary>
        /// Constructor: UnattendedSearchRequest
        /// </summary>
        /// <param name="rawValueFromMvi">If available, the raw id that was returned from MVI. EX: 123456789^NI^200DOD^USDOD</param>
        /// <param name="organization">The CRM  Org Name</param>
        /// <param name="userFirstName">The user's first name</param>
        /// <param name="userLastName">The user's last name</param>
        /// <param name="userId">The user's Id</param>
        /// <param name="messsageId"></param>
        public UnattendedSearchRequest(string rawValueFromMvi, string organization, string userFirstName, string userLastName, Guid userId, string messsageId)
        {
            UserId = userId;
            UserFirstName = userFirstName;
            UserLastName = userLastName;
            OrganizationName = organization;
            RawValueFromMvi = rawValueFromMvi;
            UseRawMviValue = true;
        }

        public bool UseRawMviValue { get; set; }

        /// <summary>
        /// This is the EDIPI, SSN, VA ID, etc.
        /// </summary>
        [DataMember]
        public string PatientIdentifier { get; set; }

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

        public override string ToString()
        {
            return UseRawMviValue ? RawValueFromMvi : string.Format("{0}^{1}^{2}^{3}", PatientIdentifier, IdentifierType, AssigningFacility, AssigningAuthority);
        }

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

        [DataMember]
        public string OrganizationName { get; set; }

        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public string UserFirstName { get; set; }

        [DataMember]
        public string UserLastName { get; set; }

        //[DataMember]
        //public MessageProcessType FetchMessageProcessType { get; set; }

        [DataMember(IsRequired = false)]
        public bool LogTiming { get; set; }


        [DataMember(IsRequired = false)]
        public bool LogSoap { get; set; }
    }
}
