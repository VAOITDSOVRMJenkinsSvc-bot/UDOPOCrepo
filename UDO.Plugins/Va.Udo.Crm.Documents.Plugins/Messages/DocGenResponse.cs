using System;
using System.Runtime.Serialization;

namespace Va.Udo.Crm.Documents.Plugins
{
    [DataContract]
    public class VEISDocGenResponse
    {
        //[DataMember(IsRequired = false, EmitDefaultValue = false)]
        //public string MessageId { get; set; }
        [DataMember]
        public string WordBytesasBase64 { get; set; }
        [DataMember]
        public Guid AttachmentId { get; set; }
        [DataMember]
        public bool ExceptionOccured { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }
    }
}
