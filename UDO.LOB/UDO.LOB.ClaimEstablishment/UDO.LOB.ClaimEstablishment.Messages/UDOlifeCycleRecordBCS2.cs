using System.Runtime.Serialization;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//using System.Text;
//using System.Threading.Tasks;

namespace UDO.LOB.ClaimEstablishment.Messages
{
    [DataContract]
    public class UDOlifeCycleRecordBCS2
    {
        [DataMember]
        public string numberOfRecords{ get; set; }

        [DataMember]
        public string returnCode{ get; set; }

        [DataMember]
        public string returnMessage{ get; set; }

        [DataMember]
        public UDOlifeCycleRecordsBCS2MultipleResponse[] UDOlifeCycleRecordsBCS2Info{ get; set; }
    }
}
