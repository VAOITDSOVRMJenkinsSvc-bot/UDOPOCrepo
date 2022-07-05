using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.VirtualMeetingRoomService.Api.Messages
{
    [DataContract]
	public class EcVirtualDeleteMeetingResponse : VEISEcResponseBase
    {
        public EcVirtualDeleteMeetingResponse()
        {
            MessageId = Guid.NewGuid().ToString();
        }
         

        [DataMember]
        public string mcs_MiscData { get; set; }

        [DataMember]
		public bool ExceptionOccured { get; set; } 
	}
}