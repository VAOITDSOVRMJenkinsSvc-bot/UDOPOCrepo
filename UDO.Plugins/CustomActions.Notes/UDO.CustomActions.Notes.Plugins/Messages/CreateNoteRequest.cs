using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using UDO.LOB.Core;

namespace UDO.CustomActions.Notes.Plugins.Messages
{
    [DataContract]
    public class CreateNoteRequest
    {
        [DataMember]
        public string udo_ParticipantID { get; set; }
        [DataMember]
        public string MessageId { get; set; }
        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public Guid UserId { get; set; }
        [DataMember]
        public string PrimaryEntityName { get; set; }
        [DataMember]
        public Guid PrimaryEntityId { get; set; }
        [DataMember]
        public bool LogTiming { get; set; }
        [DataMember]
        public bool LogSoap { get; set; }
        [DataMember]
        public bool Debug { get; set; }
        //public Guid OwnerId { get; set; }
        //public string OwnerType { get; set; }
        [DataMember]
        public HeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public string udo_fromUDO { get; set; }
        [DataMember]
        public string udo_NoteText { get; set; }
        [DataMember]
        public Guid udo_VeteranId { get; set; }
        [DataMember]
        public string udo_DateTime { get; set; }
        [DataMember]
        public string udo_Note { get; set; }
        [DataMember]
        public string udo_RO { get; set; }
        [DataMember]
        public string udo_Type { get; set; }
        [DataMember]
        public string udo_pctptnttc { get; set; }
        [DataMember]
        public string udo_User { get; set; }

    }
}
