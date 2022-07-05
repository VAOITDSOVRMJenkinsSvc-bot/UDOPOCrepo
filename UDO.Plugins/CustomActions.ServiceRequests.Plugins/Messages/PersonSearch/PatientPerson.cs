using System;
using System.Security;
using System.Linq;
using System.Runtime.Serialization;

namespace UDO.LOB.PersonSearch.Models
{
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
                const string NAME_FORMAT = "{0} {1}";

                if (NameList != null && NameList.Any())
                {
                    Name name =
                        NameList.FirstOrDefault(
                            n => n.NameType.Equals("Legal", StringComparison.CurrentCultureIgnoreCase));

                    if (name == null)
                    {
                        name =
                            NameList.FirstOrDefault(
                                n => n.NameType.Equals("Alias", StringComparison.CurrentCultureIgnoreCase));
                    }

                    if (name == null) name = NameList[0];

                    return string.Format(NAME_FORMAT, name.GivenName, name.FamilyName);
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

                const string ADDRESS_FORMAT = "{0} {1} {2} {3}";
                object[] addressArrary =
                {
                    Address.StreetAddressLine, Address.City, Address.State, Address.PostalCode
                };
                var address = string.Format(ADDRESS_FORMAT, addressArrary);

                return address;
            }
        }

        [DataMember]
        public string RecordSource { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public string CauseOfDeath { get; set; }

        [DataMember]
        public string StationofJurisdiction { get; set; }

        [DataMember]
        public string CharacterofDishcarge { get; set; }

        [DataMember]
        public string Rank { get; set; }

        [DataMember]
        public string DateOfDeath { get; set; }
    }
}