using System.Runtime.Serialization;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace UDO.LOB.ClaimEstablishment.Messages
{
    [DataContract]
    public class UDOparticipantRecordBCS2
    {
        [DataMember]
        public string bddSiteName
        {
            get;
            set;
        }

        [DataMember]
        public string benefitClaimID
        {
            get;
            set;
        }

        [DataMember]
        public string benefitClaimReturnLabel
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
        public string claimantPersonOrOrganizationIndicator
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
        public string claimPriorityIndicator
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
        public string claimStationOfJurisdiction
        {
            get;
            set;
        }

        [DataMember]
        public string claimTemporaryStationOfJurisdiction
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
        public string cpBenefitClaimID
        {
            get;
            set;
        }

        [DataMember]
        public string cpClaimID
        {
            get;
            set;
        }

        [DataMember]
        public string cpClaimReturnLabel
        {
            get;
            set;
        }

        [DataMember]
        public string cpLocationID
        {
            get;
            set;
        }

        [DataMember]
        public string directDepositAccountID
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
        public string informalIndicator
        {
            get;
            set;
        }

        [DataMember]
        public string journalDate
        {
            get;
            set;
        }

        [DataMember]
        public string journalObjectID
        {
            get;
            set;
        }

        [DataMember]
        public string journalStation
        {
            get;
            set;
        }

        [DataMember]
        public string journalStatusTypeCode
        {
            get;
            set;
        }

        [DataMember]
        public string journalUserId
        {
            get;
            set;
        }

        [DataMember]
        public string lastPaidDate
        {
            get;
            set;
        }

        [DataMember]
        public string mailingAddressID
        {
            get;
            set;
        }

        [DataMember]
        public string numberOfBenefitClaimRecords
        {
            get;
            set;
        }

        [DataMember]
        public string numberOfCPClaimRecords
        {
            get;
            set;
        }

        [DataMember]
        public string numberOfRecords
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
        public string participantClaimantID
        {
            get;
            set;
        }

        [DataMember]
        public string participantVetID
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
        public string paymentAddressID
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
        public string returnCode
        {
            get;
            set;
        }

        [DataMember]
        public string returnMessage
        {
            get;
            set;
        }

        [DataMember]
        public string serviceTypeCode
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
        public string vetFirstName
        {
            get;
            set;
        }

        [DataMember]
        public string vetLastName
        {
            get;
            set;
        }

        [DataMember]
        public string vetMiddleName
        {
            get;
            set;
        }

        [DataMember]
        public string vetSuffix
        {
            get;
            set;
        }

        [DataMember]
        public UDOselectionBCS2MultipleResponse[] UDOselectionBCS2Info
        {
            get;
            set;
        }

    }
}
