using System;

using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
using System.Collections.Generic;
//using VRM.Integration.Servicebus.Core;


//CSDEv
//namespace VRM.Integration.Servicebus.Bgs.Messages
namespace UDO.LOB.DependentMaintenance.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.SearchBGSSchoolInfoResponse)]
    [DataContract]
    public class SearchSchoolInfoResponse : MessageBase
    {
        [DataMember]
        public List<string> facilityCode { get; set; }
        [DataMember]
        public List<long> participantID { get; set; }
        [DataMember]
        public bool ExceptionOccured { get; set; }

        public SearchSchoolInfoResponse() 
        {
            participantID = new List<long>();
            facilityCode = new List<string>();
        }


    }
    
   
}
