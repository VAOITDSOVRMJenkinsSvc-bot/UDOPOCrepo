using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using System;
using System.Security;
namespace VRM.Integration.UDO.MVI.Messages
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Reviewed By Brian Greig - 4/30/15
    /// </remarks>
    [Export(typeof (IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOPersonSearchResponse)]
    [DataContract]
    public class UDOPersonSearchResponse : MessageBase
    {
        [DataMember]
        public PatientPerson[] Person { get; set; }

        [DataMember]
        public bool ExceptionOccured { get; set; }

        [DataMember]
        public string MVIMessage { get; set; }
        
        [DataMember]
        public string CORPDbMessage { get; set; }

        [DataMember]
        public int MVIRecordCount { get; set; }

        [DataMember]
        public int CORPDbRecordCount { get; set; }

        [DataMember]
        public string UDOMessage { get; set; }
        
        [DataMember]
        public string RawMviExceptionMessage { get; set; }

        [DataMember]
        public string OrganizationName { get; set; }

        [DataMember]
        public MessageProcessType FetchMessageProcessType { get; set; }
    }

  
    public class PatientPerson
    {
        /// <summary>
        /// Gets or sets the identify value for the patient. 
        /// </summary>
        [DataMember]
        public string Identifier { get; set; }

        [DataMember]
        public string IdentifierType { get; set; }
        [DataMember]
        public string SSIdString { get; set; }
        [DataMember]
        public SecureString SSId { get; set; }
        [DataMember]
        public string FileNumber { get; set; }
        [DataMember]
        public string EdiPi { get; set; }
        [DataMember]
        public string ParticipantId { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }
        
        [DataMember]
        public string BranchOfService { get; set; }
        
        [DataMember]
        public string VeteranSensitivityLevel { get; set; }
        
        [DataMember]
        public string GenderCode { get; set; }

        [DataMember]
        public string BirthDate { get; set; }

        [DataMember]
        public string StatusCode { get; set; }

        [DataMember]
        public string IsDeceased { get; set; }

        [DataMember]
        public string DeceasedDate { get; set; }

        [DataMember]
        public string IdentifyTheft { get; set; }

        [DataMember]
        public PatientAddress Address { get; set; }

        [DataMember]
        public Name[] NameList { get; set; }

        [DataMember]
        public CorrespondingIDs[] CorrespondingIdList { get; set; }

        [DataMember]
        public string FullName
        {
            get
            {
                const string nameFormat = "{0} {1}";

                if (NameList != null && NameList.Any())
                {
                    return string.Format(nameFormat, NameList[0].GivenName, NameList[0].FamilyName);
                }

                return string.Empty;
            }
        }

        [DataMember]
        public string FullAddress
        {
            get
            {
                if (Address == null) return string.Empty;

                const string addressFormat = "{0} {1} {2} {3}";
                object[] addressArrary =
                {
                    Address.StreetAddressLine, Address.City, Address.State, Address.PostalCode
                };
                var address = string.Format(addressFormat, addressArrary);

                return address;
            }
        }

        [DataMember]
        public string RecordSource { get; set; }

        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public string crme_CauseOfDeath { get; set; }
        [DataMember]
        public string crme_stationofJurisdiction { get; set; }
        [DataMember]
        public string crme_CharacterofDishcarge { get; set; }

        [DataMember]
        public string Rank { get; set; }
    }
    [DataContract]
    public class CorrespondingIDs
    {
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
        /// This is the organizationn identifier -- similar to the identifier for UDO, which is "200CMRE"
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
            return string.Format("{0}^{1}^{2}^{3}", PatientIdentifier, IdentifierType, AssigningFacility,
                AssigningAuthority);
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

        [DataMember]
        public MessageProcessType FetchMessageProcessType { get; set; }
    }
    [DataContract]
    public class Name
    {
        [DataMember]
        public string GivenName { get; set; }

        [DataMember]
        public string FamilyName { get; set; }

        [DataMember]
        public string MiddleName { get; set; }

        [DataMember]
        public string NamePrefix { get; set; }

        [DataMember]
        public string NameSuffix { get; set; }

        /// <summary>
        /// Legal, Alias, Maiden
        /// </summary>
        /// 
        [DataMember]
        public string NameType { get; set; }
    }
    [DataContract]
    public class PatientAddress
    {
        [DataMember]
        public string Use { get; set; }

        [DataMember]
        public string StreetAddressLine { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string PostalCode { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string State { get; set; }
    }
}
