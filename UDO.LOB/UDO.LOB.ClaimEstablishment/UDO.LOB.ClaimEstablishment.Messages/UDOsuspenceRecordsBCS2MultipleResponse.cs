using System.Runtime.Serialization;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace UDO.LOB.ClaimEstablishment.Messages
{
    [DataContract]
    public class UDOsuspenceRecordsBCS2MultipleResponse
    {
        [DataMember]
        public string claimSuspenceDate
        {
            get;
            set;
        }

        [DataMember]
        public string firstName
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
        public string lastName
        {
            get;
            set;
        }

        [DataMember]
        public string middleName
        {
            get;
            set;
        }

        [DataMember]
        public string suffix
        {
            get;
            set;
        }

        [DataMember]
        public string suspenceActionDate
        {
            get;
            set;
        }

        [DataMember]
        public string suspenceCode
        {
            get;
            set;
        }

        [DataMember]
        public string suspenceReasonText
        {
            get;
            set;
        }
    }
}
