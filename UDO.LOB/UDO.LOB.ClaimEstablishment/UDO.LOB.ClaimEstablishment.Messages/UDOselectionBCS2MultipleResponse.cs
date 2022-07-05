using System;
using System.Runtime.Serialization;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace UDO.LOB.ClaimEstablishment.Messages
{
    [DataContract]
    public class UDOselectionBCS2MultipleResponse
    {
        [DataMember]
        public string benefitClaimID
        {
            get;
            set;
        }

        [DataMember]
        public string claimantFirstName
        {
            get;
            set;
        }

        [DataMember]
        public string claimantLastName
        {
            get;
            set;
        }

        [DataMember]
        public string claimantMiddleName
        {
            get;
            set;
        }

        [DataMember]
        public string claimantSuffix
        {
            get;
            set;
        }

        [DataMember]
        public string claimReceiveDate
        {
            get;
            set;
        }

        [DataMember]
        public string claimTypeCode
        {
            get;
            set;
        }

        [DataMember]
        public string claimTypeName
        {
            get;
            set;
        }

        [DataMember]
        public string endProductTypeCode
        {
            get;
            set;
        }

        [DataMember]
        public string lastActionDate
        {
            get;
            set;
        }

        [DataMember]
        public string organizationName
        {
            get;
            set;
        }

        [DataMember]
        public string organizationTitleTypeName
        {
            get;
            set;
        }

        [DataMember]
        public string payeeTypeCode
        {
            get;
            set;
        }

        [DataMember]
        public string personOrOrganizationIndicator
        {
            get;
            set;
        }

        [DataMember]
        public string programTypeCode
        {
            get;
            set;
        }

        [DataMember]
        public string statusTypeCode
        {
            get;
            set;
        }

        [DataMember]
        public Guid CRMClaimEstablishmentId{ get; set; }
    }
}
