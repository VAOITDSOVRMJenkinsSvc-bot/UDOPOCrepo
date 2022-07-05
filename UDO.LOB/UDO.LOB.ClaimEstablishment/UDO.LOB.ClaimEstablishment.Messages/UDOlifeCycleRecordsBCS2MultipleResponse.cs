using System.Runtime.Serialization;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace UDO.LOB.ClaimEstablishment.Messages
{
    [DataContract]
    public class UDOlifeCycleRecordsBCS2MultipleResponse
    {
        [DataMember]
        public string actionFirstName
        {
            get;
            set;
        }

        [DataMember]
        public string actionLastName
        {
            get;
            set;
        }

        [DataMember]
        public string actionMiddleName
        {
            get;
            set;
        }

        [DataMember]
        public string actionStationNumber
        {
            get;
            set;
        }

        [DataMember]
        public string actionSuffix
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
        public string caseAssignmentLocationID
        {
            get;
            set;
        }

        [DataMember]
        public string caseAssignmentStatusNumber
        {
            get;
            set;
        }

        [DataMember]
        public string caseID
        {
            get;
            set;
        }

        [DataMember]
        public string changedDate
        {
            get;
            set;
        }

        [DataMember]
        public string closedDate
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
        public string journalUserID
        {
            get;
            set;
        }

        [DataMember]
        public string lifeCycleStatusID
        {
            get;
            set;
        }

        [DataMember]
        public string lifeCycleStatusTypeName
        {
            get;
            set;
        }

        [DataMember]
        public string reasonText
        {
            get;
            set;
        }

        [DataMember]
        public string stationofJurisdiction
        {
            get;
            set;
        }

        [DataMember]
        public string statusReasonTypeCode
        {
            get;
            set;
        }

        [DataMember]
        public string statusReasonTypeName
        {
            get;
            set;
        }
    }
}
