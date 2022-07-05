﻿using System;
using System.ComponentModel.Composition;
using Microsoft.Xrm.Sdk;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Messages;

namespace VRM.Integration.UDO.Awards.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOretrieveAwardRequest)]
    [DataContract]
    public class UDOretrieveAwardRequest : MessageBase
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
        public bool LogSoap { get; set; }
        [DataMember]
        public bool LogTiming { get; set; }
        [DataMember]
        public bool Debug { get; set; }
        [DataMember]
        public Guid ownerId { get; set; }
        [DataMember]
        public string ownerType { get; set; }
        [DataMember]
        public Guid AwardId { get; set; }
        [DataMember]
        public Guid vetSnapShotId { get; set; }
        [DataMember]
        public string ptcpntVetId { get; set; }
        [DataMember]
        public string ptcpntBeneId { get; set; }
        [DataMember]
        public string ptcpntRecipId { get; set; }
        [DataMember]
        public string awardTypeCd { get; set; }
        [DataMember]
        public string fileNumber { get; set; }
        [DataMember]
        public HeaderInfo LegacyServiceHeaderInfo { get; set; }
    }
}