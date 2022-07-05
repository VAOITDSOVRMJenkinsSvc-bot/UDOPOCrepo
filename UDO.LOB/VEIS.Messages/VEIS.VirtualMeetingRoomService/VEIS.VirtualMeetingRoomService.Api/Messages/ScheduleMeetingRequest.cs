using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.VirtualMeetingRoomService.Api.Messages
{
    [DataContract]
    public class EcVyoptaSMScheduleMeetingRequest : VEISEcRequestBase
    {
        public EcVyoptaSMScheduleMeetingRequest()
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
        public DateTime mcs_EndTime { get; set; }

        [DataMember]
        public string mcs_GuestName { get; set; }

        [DataMember]
        public string mcs_GuestPin { get; set; }

        [DataMember]
        public string mcs_HostName { get; set; }

        [DataMember]
        public string mcs_HostPin { get; set; }

        [DataMember]
        public string mcs_MeetingRoomName { get; set; }

        [DataMember]
        public string mcs_MiscData { get; set; }

        [DataMember]
        public DateTime mcs_StartTime { get; set; }
        [DataMember]
        public LegacyHeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public bool LogTiming { get; set; }
        [DataMember]
        public bool LogSoap { get; set; }
    }
}