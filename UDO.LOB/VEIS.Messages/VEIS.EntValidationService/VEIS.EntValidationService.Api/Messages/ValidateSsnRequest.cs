using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.EntValidationService.Api.Messages
{
    [DataContract]
    public class VEISValidateSsnRequest : VEISEcRequestBase
    { 
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
        [DataMember]
        public string ssNumber { get; set; }
    }
}