using System;

using System.Runtime.Serialization;
using VEIS.Core.Core;

namespace VEIS.Mvi.Messages
{
    /// <summary>
    /// Messaage class for executing Unattended search: RetrieveWithOrchestration
    /// </summary>

    [DataContract]
    public class RetrieveWithOrchestrationRequest : MessageBase
    {
        public RetrieveWithOrchestrationRequest()
        {
        }

        /// <summary>
        /// Constructor: RetrieveWithOrchestrationRequest
        /// </summary>
        /// <param name="icn">Veteran's ICN</param>        
        /// <param name="userFirstName">CRM user's first name</param>
        /// <param name="userLastName">CRM user's last name</param>
        /// <param name="organization">CRM org name</param>
        /// <param name="userId">User's CRM ID</param>
        public RetrieveWithOrchestrationRequest(string icn, string userFirstName, string userLastName, string organization, Guid userId)
        {
            PatientIdentifier = icn;
            IdentifierType = "NI";
            AssigningAuthority = "USVHA";
            AssigningFacility = "200M";
            UserId = userId;
            UserFirstName = userFirstName;
            UserLastName = userLastName;
            OrganizationName = organization;
        }

        /// <summary>
        /// Constructor: RetrieveWithOrchestrationRequest
        /// </summary>
        /// <param name="edipi">Veteran's EDIPI</param>    
        /// <param name="userId">User's CRM ID</param>
        /// <param name="userFirstName">CRM user's first name</param>
        /// <param name="userLastName">CRM user's last name</param>
        /// <param name="organization">CRM org name</param>        
        public RetrieveWithOrchestrationRequest(string edipi, Guid userId, string userFirstName, string userLastName, string organization)
        {
            PatientIdentifier = edipi;
            IdentifierType = "NI";
            AssigningAuthority = "USDOD";
            AssigningFacility = "200DOD";
            UserId = userId;
            UserFirstName = userFirstName;
            UserLastName = userLastName;
            OrganizationName = organization;
        }

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
            return string.Format("{0}^{1}^{2}^{3}", PatientIdentifier, IdentifierType, AssigningFacility, AssigningAuthority);
        }

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
