using System.Runtime.Serialization;

namespace UDO.VASS.Plugins.Models
{
    [DataContract]
    public class UDOCreateNoteResponse
    {
        [DataMember]
        public bool ExceptionOccurred { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
        [DataMember]
        public UDOCreateNoteResponseInfo UDOCreateNoteInfo { get; set; }
    }

    public class UDOCreateNoteResponseInfo
    {
        [DataMember(IsRequired =false)]
        public string udo_ClaimId { get; set; }
        [DataMember(IsRequired = false)]
        public string udo_DateTime { get; set; }
        [DataMember(IsRequired = false)]
        public string udo_Note { get; set; }
        [DataMember(IsRequired = false)]
        public string udo_ParticipantID { get; set; }
        [DataMember(IsRequired = false)]
        public string udo_RO { get; set; }
        [DataMember(IsRequired = false)]
        public string udo_SuspenseDate { get; set; }
        [DataMember(IsRequired = false)]
        public string udo_Type { get; set; }
        [DataMember(IsRequired = false)]
        public string udo_User { get; set; }
        [DataMember(IsRequired = false)]
        public string udo_UserId { get; set; }
        [DataMember(IsRequired = false)]
        public string udo_legacynoteid { get; set; }
    }
}
