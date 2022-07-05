
using System.Linq;
using System.Runtime.Serialization;

using System;


namespace VEIS.Mvi.Messages
{
    [DataContract]
    public class PatientPerson
    {
        /// <summary>
        /// Gets or sets the identify value for the patient. This could be SSN, EDIPI or some other identifier.
        /// </summary>
        [DataMember]
        public string Identifier { get; set; }

        [DataMember]
        public string IdentifierType { get; set; }

        public string SocialSecurityNumber { get; set; }

        public string EdiPi { get; set; }

        public string ParticipantId { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

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
        public UnattendedSearchRequest[] CorrespondingIdList { get; set; }
        [DataMember]
        public UnattendedSearchRequest[] ReplacementIdList { get; set; }
        //        [DataMember]
        //        public string FullName { get; set; }
        //        //{
        //        //    get
        //        //    {
        //        //        const string nameFormat = "{0} {1}";

        //        //        if (NameList != null && NameList.Any())
        //        //        {
        //        //            return string.Format(nameFormat, NameList[0].GivenName, NameList[0].FamilyName);
        //        //        }

        //        //        return string.Empty;
        //        //    }
        //        //}

        //        [DataMember]
        //        public string FullAddress { get; set; }
        //       // {
        //            //get
        //            //{
        //            //    if (Address == null) return string.Empty;

        //            //    const string addressFormat = "{0} {1} {2} {3}";
        //            //    object[] addressArrary =
        //            //    {
        //            //        Address.StreetAddressLine, Address.City, Address.State, Address.PostalCode
        //            //    };
        //            //    var address = string.Format(addressFormat, addressArrary);
        //            //    return address.Trim();
        //            //}
        //            //set;
        ////        }

        [DataMember]
        public string RecordSource { get; set; }

        [DataMember]
        public string Url { get; set; }

    }
}
