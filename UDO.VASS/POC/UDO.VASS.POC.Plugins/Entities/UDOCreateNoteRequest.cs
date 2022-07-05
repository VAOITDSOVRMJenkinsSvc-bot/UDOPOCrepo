using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using UDO.VASS.POC.Plugins.Entities;

namespace UDO.VASS.POC.Plugins.NotesApi
{
    public class UDOCreateNoteRequest
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
        public Guid RelatedParentId { get; set; }
        [DataMember]
        public string RelatedParentEntityName { get; set; }
        [DataMember]
        public string RelatedParentFieldName { get; set; }
        [DataMember]
        public bool LogTiming { get; set; }
        [DataMember]
        public bool LogSoap { get; set; }
        [DataMember]
        public bool Debug { get; set; }
        //public Guid OwnerId { get; set; }
        //public string OwnerType { get; set; }
        [DataMember]
        public UDOHeader LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public string udo_ClaimId { get; set; }
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
