using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.VirtualMeetingRoomService.Api.Messages
{
    [DataContract]
	public class EcVirtualDeleteMeetingRequest : VEISEcRequestBase
    {
        public EcVirtualDeleteMeetingRequest()
        {
            MessageId = Guid.NewGuid().ToString();
        }
         

        [DataMember]
		public Guid RelatedParentId { get; set; }

        [DataMember]
		public string RelatedParentEntityName { get; set; }

        [DataMember]
		public string RelatedParentFieldName { get; set; }
        
        [DataMember]
        public string mcs_EncounterId { get; set; }
        [DataMember]
        public bool LogTiming { get; set; }
        [DataMember]
        public bool LogSoap { get; set; }

        [DataMember]
        public string mcs_MiscData { get; set; }
        [DataMember]
        public LegacyHeaderInfo LegacyServiceHeaderInfo { get; set; }
    }
}