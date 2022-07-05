﻿using System;
using System.ComponentModel.Composition;
using Microsoft.Xrm.Sdk;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Messages;

/// <summary>
/// VIMT LOB Component for UDOcreateUDOStatus,createUDOStatus method, Request.
/// Code Generated by IMS on: 9/3/2015 2:11:24 PM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace VRM.Integration.UDO.Claims.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOcreateUDOStatusRequest)]
    [DataContract]
    public class UDOcreateUDOStatusRequest : MessageBase
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
        public Guid ownerId { get; set; }
        [DataMember]
        public string ownerType { get; set; }
        [DataMember]
        public HeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public Guid udo_claimId { get; set; }
        [DataMember]
        public Int64 claimId { get; set; }
        [DataMember]
        public UDOcreateUDOStatusRelatedEntitiesMultipleRequest[] UDOcreateUDOStatusRelatedEntitiesInfo { get; set; }
    }
    [DataContract]
    public class UDOcreateUDOStatusRelatedEntitiesMultipleRequest
    {
        [DataMember]
        public string RelatedEntityName { get; set; }
        [DataMember]
        public Guid RelatedEntityId { get; set; }
        [DataMember]
        public string RelatedEntityFieldName { get; set; }
    }
}