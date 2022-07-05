using System.Runtime.Serialization;
using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UDO.LOB.Core;
//using UDO.LOB.Extensions;
//using VEIS.Core.Messages;

namespace UDO.LOB.ClaimEstablishment.Messages
{
    [DataContract]
    public class UDObenefitClaimRecordBCS2
    {
        [DataMember]
        public string fiduciaryInd{ get; set; }

        [DataMember]
        public string gulfWarRegistryPermit{ get; set; }

        [DataMember]
        public string homelessIndicator{ get; set; }

        [DataMember]
        public string powNumberOfDays{ get; set; }

        [DataMember]
        public string returnCode{ get; set; }

        [DataMember]
        public string returnMessage{ get; set; }

        [DataMember]
        public UDObenefitClaimRecord1BCS2 UDObenefitClaimRecord1BCS2Info
        {
            get;
            set;
        }

        [DataMember]
        public UDOlifeCycleRecordBCS2 UDOlifeCycleRecordBCS2Info
        {
            get;
            set;
        }

        [DataMember]
        public UDOparticipantRecordBCS2 UDOparticipantRecordBCS2Info
        {
            get;
            set;
        }

        [DataMember]
        public UDOsuspenceRecordBCS2 UDOsuspenceRecordBCS2Info
        {
            get;
            set;
        }
    }
}
