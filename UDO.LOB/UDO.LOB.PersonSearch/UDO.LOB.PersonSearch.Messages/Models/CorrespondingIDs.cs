using System;
using System.Runtime.Serialization;
using UDO.LOB.PersonSearch.Messages;
using UDO.LOB.Core;
using VEIS.Mvi.Messages;

namespace UDO.LOB.PersonSearch.Models
{
    [DataContract]
    public class CorrespondingIDs
    {
        public CorrespondingIDs()
        {
            
        }

        public CorrespondingIDs(UnattendedSearchRequest request)
        {
            if (request != null)
            {
                PatientIdentifier = (request.PatientIdentifier ?? "").Trim();
                AssigningAuthority = (request.AssigningAuthority ?? "").Trim();
                AssigningFacility = (request.AssigningFacility ?? "").Trim();
                AuthorityOid = (request.AuthorityOid ?? "").Trim();
                ExtensionData = request.ExtensionData;
                //TODO: Review need for this property FetchMessageProcessType = request.FetchMessageProcessType;
                IdentifierType = (request.IdentifierType ?? "").Trim();
                OrganizationName = (request.OrganizationName ?? "").Trim();
                RawValueFromMvi = (request.RawValueFromMvi ?? "").Trim();
                UseRawMviValue = request.UseRawMviValue;
                UserFirstName = (request.UserFirstName ?? "").Trim();
                UserLastName = (request.UserLastName ?? "").Trim();
                UserId = request.UserId;
            }
        }

        [IgnoreDataMember]
        public bool UseRawMviValue { get; set; }

        [IgnoreDataMember]
        public ExtensionDataObject ExtensionData { get; set; }

        public UnattendedSearchRequest ConvertToUnattendedSearchRequest()
        {
            return new UnattendedSearchRequest()
            {
                PatientIdentifier = PatientIdentifier,
                AssigningAuthority = AssigningAuthority,
                AssigningFacility = AssigningFacility,
                AuthorityOid = AuthorityOid,
                ExtensionData = ExtensionData,
                // TODO: Review the need for - FetchMessageProcessType = FetchMessageProcessType,
                IdentifierType = IdentifierType,
                OrganizationName = OrganizationName,
                RawValueFromMvi = RawValueFromMvi,
                UseRawMviValue = UseRawMviValue,
                UserFirstName = UserFirstName,
                UserLastName = UserLastName,
                UserId = UserId
            };
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

        //TODO: Review need for this property
        //[DataMember]
        //public MessageProcessType FetchMessageProcessType { get; set; }
    }
}