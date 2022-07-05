﻿using System;
using System.ComponentModel.Composition;
using Microsoft.Xrm.Sdk;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Messages;

/// <summary>
/// VIMT LOB Component for UDOcreateUDOAppealDiaries,createUDOAppealDiaries method, Request.
/// Code Generated by IMS on: 7/8/2015 6:07:06 PM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace VRM.Integration.UDO.Appeals.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOcreateUDOAppealDiariesRequest)]
    [DataContract]
    public class UDOcreateUDOAppealDiariesRequest : MessageBase
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
        public HeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public Guid udo_appealId { get; set; }
        [DataMember]
        public UDOcreateUDOAppealDiariesRelatedEntitiesMultipleRequest[] UDOcreateUDOAppealDiariesRelatedEntitiesInfo { get; set; }
    }
    [DataContract]
    public class UDOcreateUDOAppealDiariesRelatedEntitiesMultipleRequest
    {
        [DataMember]
        public string RelatedEntityName { get; set; }
        [DataMember]
        public Guid RelatedEntityId { get; set; }
        [DataMember]
        public string RelatedEntityFieldName { get; set; }
    }
}
