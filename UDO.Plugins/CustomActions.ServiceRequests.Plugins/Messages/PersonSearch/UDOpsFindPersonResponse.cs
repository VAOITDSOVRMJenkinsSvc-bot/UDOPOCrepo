using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
using UDO.LOB.Core.Interfaces;
using UDO.LOB.PersonSearch.Interfaces;
using UDO.LOB.PersonSearch.Models;


namespace CustomActions.Plugins.Messages.PersonSearch
{
    public class UDOpsFindPersonResponse
    {
        
        public bool ExceptionOccured { get; set; }        
        public string ExceptionMessage { get; set; }        
        public string URL { get; set; }        
        public Guid contactId { get; set; }        
        public Guid idProofId { get; set; }        
        public PatientPerson[] Person { get; set; }        
        public string MVIMessage { get; set; }        
        public string CORPDbMessage { get; set; }        
        public string BIRLSMessage { get; set; }        
        public int BIRLSRecordCount { get; set; }        
        public string VADIRMessage { get; set; }        
        public int VADIRRecordCount { get; set; }        
        public int MVIRecordCount { get; set; }        
        public int CORPDbRecordCount { get; set; }        
        public string UDOMessage { get; set; }        
        public string RawMviExceptionMessage { get; set; }        
        public string OrganizationName { get; set; }        
        public string FetchMessageProcessType { get; set; }
    }

    public class PatientPerson
    {
        /// <summary>
        /// Gets or sets the identify value for the patient. This could be SSN, EDIPI or some other identifier.
        /// </summary>

        public string Identifier { get; set; }

        public string IdentifierType { get; set; }

        //public string SocialSecurityNumber { get; set; }
        public string SSIdString { get; set; }
        public string FileNumber { get; set; }
        public string EdiPi { get; set; }
        public string ParticipantId { get; set; }
        public string PhoneNumber { get; set; }
        public string BranchOfService { get; set; }
        public string Rank { get; set; }
        public string VeteranSensitivityLevel { get; set; }
        public string GenderCode { get; set; }
        public string BirthDate { get; set; }
        public string StatusCode { get; set; }
        public string IsDeceased { get; set; }
        public string DeceasedDate { get; set; }
        public string IdentifyTheft { get; set; }
        public string ServiceNumber { get; set; }
        public string InsuranceNumber { get; set; }
        public string EnteredOnDutyDate { get; set; }
        public string ReleasedActiveDutyDate { get; set; }
        public string PayeeNumber { get; set; }
        public string FolderLocation { get; set; }
        public PatientAddress Address { get; set; }
        public Name[] NameList { get; set; }

        //
        public CorrespondingIDs[] CorrespondingIdList { get; set; }


        public string FullName
        {
            get
            {
                const string nameFormat = "{0} {1} {2}";

                if (NameList != null && NameList.Any())
                {
                    return string.Format(nameFormat, NameList[0].GivenName, NameList[0].MiddleName, NameList[0].FamilyName);
                }

                return string.Empty;
            }
        }


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


        public string RecordSource { get; set; }


        public string Url { get; set; }
    }

    public class CorrespondingIDs
    {
        /// <summary>
        /// This is the EDIPI, SSN, VA ID, etc.
        /// </summary>

        public string PatientIdentifier { get; set; }

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

        public override string ToString()
        {
            return string.Format("{0}^{1}^{2}^{3}", PatientIdentifier, IdentifierType, AssigningFacility,
                AssigningAuthority);
        }

        /// <summary>
        /// This is the raw value retrieved from MVI.
        /// </summary>

        public string RawValueFromMvi { get; set; }

        /// <summary>
        /// This is the Assigning Authority OID.
        /// Based on MVI SSD, this value is not supported by the VA; however, it could be in the future.
        /// </summary>

        public string AuthorityOid { get; set; }


        public string OrganizationName { get; set; }


        public Guid UserId { get; set; }


        public string UserFirstName { get; set; }


        public string UserLastName { get; set; }


        public string FetchMessageProcessType { get; set; }
    }

    public class Name
    {

        public string GivenName { get; set; }


        public string FamilyName { get; set; }


        public string MiddleName { get; set; }


        public string NamePrefix { get; set; }


        public string NameSuffix { get; set; }

        public string NameType { get; set; }
    }

    public class PatientAddress
    {

        public string Use { get; set; }


        public string StreetAddressLine { get; set; }


        public string City { get; set; }


        public string PostalCode { get; set; }


        public string Country { get; set; }


        public string State { get; set; }
    }
}
