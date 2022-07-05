﻿using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
//using UDO.LOB.Extensions;
using VEIS.Core.Messages;

//using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;

/// <summary>
/// VIMT LOB Component for UDOcreatePastPOA,createPastPOA method, Request.
/// Code Generated by IMS on: 5/13/2015 1:04:01 PM
/// Version: 2015.05.05
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace UDO.LOB.Contact.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOcreatePastPOARequest)]
    [DataContract]
    public class UDOcreatePastPOARequest : MessageBase
    {
        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public Guid UserId { get; set; }
        //[DataMember]
        //public Guid RelatedParentId { get; set; }
        //[DataMember]
        //public string RelatedParentEntityName { get; set; }
        //[DataMember]
        //public string RelatedParentFieldName { get; set; }
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
        public UDOHeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public Guid VeteranId { get; set; }
        [DataMember]
        public Guid DependentId { get; set; }

        [DataMember]
        public string fileNumber { get; set; }
        [DataMember]
        public UDOcreatePastPOARelatedEntitiesMultipleRequest[] UDOcreatePastPOARelatedEntitiesInfo { get; set; }
    }
    [DataContract]
    public class UDOcreatePastPOARelatedEntitiesMultipleRequest
    {
        [DataMember]
        public string RelatedEntityName { get; set; }
        [DataMember]
        public Guid RelatedEntityId { get; set; }
        [DataMember]
        public string RelatedEntityFieldName { get; set; }
    }
}