﻿using System;
using System.Runtime.Serialization;
using UDO.LOB.Core;


//using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Messages;
//using System.ComponentModel.Composition;
//using VEIS.Core.Messages;

/// <summary>
/// VIMT LOB Component for UDOcreateAwardLines,createAwardLines method, Request.
/// Code Generated by IMS on: 4/29/2015 4:07:49 PM
/// Version: 2015.19.01
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace UDO.LOB.Awards.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOcreateAwardLinesRequest)]
    [DataContract]
    public class UDOcreateAwardLinesRequest : MessageBase
    {
        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public Guid UserId { get; set; }
        [DataMember]
        //public Guid RelatedParentId { get; set; }
        //[DataMember]
        //public string RelatedParentEntityName { get; set; }
        //[DataMember]
        //public string RelatedParentFieldName { get; set; }
        //[DataMember]
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
        public string ptcpntVetId { get; set; }
        [DataMember]
        public string ptcpntBeneId { get; set; }
        [DataMember]
        public string ptcpntRecipId { get; set; }
        [DataMember]
        public string awardTypeCd { get; set; }
        [DataMember]
        public Guid AwardId { get; set; }
        [DataMember]
        public UDOcreateAwardLinesRelatedEntitiesMultipleRequest[] UDOcreateAwardLinesRelatedEntitiesInfo { get; set; }
    }
    [DataContract]
    public class UDOcreateAwardLinesRelatedEntitiesMultipleRequest
    {
        [DataMember]
        public string RelatedEntityName { get; set; }
        [DataMember]
        public Guid RelatedEntityId { get; set; }
        [DataMember]
        public string RelatedEntityFieldName { get; set; }
    }
}
