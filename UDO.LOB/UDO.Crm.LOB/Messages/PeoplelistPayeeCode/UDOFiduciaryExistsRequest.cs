using System;
using System.ComponentModel.Composition;
using Microsoft.Xrm.Sdk;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VIMT.ClaimantWebService.Messages;
using VRMMessages = VRM.Integration.UDO.Messages;

namespace VRM.Integration.UDO.PeoplelistPayeeeCode.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", VRMMessages.MessageRegistry.UDOFiduciaryExistsRequest)]
    [DataContract]
    public class UDOFiduciaryExistsRequest : MessageBase
    {
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
        [DataMember]
        public HeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public string fileNumber { get; set; }
        //[DataMember]
        //public string udo_ssn { get; set; }

    }    
}
