using System;
using System.ComponentModel.Composition;
using Microsoft.Xrm.Sdk;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Messages;

namespace VRM.Integration.UDO.IntentToFile.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOInitiateITFRequest)]
    [DataContract]
    public class UDOInitiateITFRequest : MessageBase
    {
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
        public Guid udo_veteranId { get; set; }
        [DataMember]
        public string fileNumber { get; set; }
        [DataMember]
        public string SSN { get; set; }
        [DataMember]
        public Int64 ptcpntId { get; set; }
        [DataMember]
        public string vetfileNumber { get; set; }
        [DataMember]
        public string vetSSN { get; set; }
        [DataMember]
        public Int64 vetptcpntId { get; set; }
        [DataMember]
        public string vetFirstName { get; set; }
        [DataMember]
        public string vetLastName { get; set; }
        [DataMember]
        public string vetMiddleInitial { get; set; }
        [DataMember]
        public string vetDOB { get; set; }
        [DataMember]
        public string vetGender    { get; set; }

        [DataMember]
        public HeaderInfo LegacyServiceHeaderInfo { get; set; }
    }
}
