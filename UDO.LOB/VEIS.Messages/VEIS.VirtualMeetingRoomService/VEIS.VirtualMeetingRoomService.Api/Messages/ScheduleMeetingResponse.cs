using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.VirtualMeetingRoomService.Api.Messages
{
    [DataContract]
	public class EcVyoptaSMScheduleMeetingResponse : VEISEcResponseBase
    {
        public EcVyoptaSMScheduleMeetingResponse()
        {
            MessageId = Guid.NewGuid().ToString();
        } 

        [DataMember]
        public string mcs_DialingAlias { get; set; }

        [DataMember]
        public string mcs_EncounterId { get; set; }

        [DataMember]
        public string mcs_MiscData { get; set; }

        [DataMember]
		public bool ExceptionOccured { get; set; } 
	}
}