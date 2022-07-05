﻿using System;
using System.ComponentModel.Composition;
using Microsoft.Xrm.Sdk;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;

/// <summary>
/// VIMT LOB Component for UDOUDOcreateDeathRating,UDOcreateDeathRating method, Request.
/// Code Generated by IMS on: 6/15/2015 11:09:42 AM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace VRM.Integration.UDO.Ratings.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOcreateDeathRatingRequest)]
    [DataContract]
    public class UDOcreateDeathRatingRequest : MessageBase
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
        public Guid udo_ratingId { get; set; }
        [DataMember]
        public string fileNumber { get; set; }
        [DataMember]
        public UDOUDOcreateDeathRatingRelatedEntitiesMultipleRequest[] UDOUDOcreateDeathRatingRelatedEntitiesInfo { get; set; }
    }
    [DataContract]
    public class UDOUDOcreateDeathRatingRelatedEntitiesMultipleRequest
    {
        [DataMember]
        public string RelatedEntityName { get; set; }
        [DataMember]
        public Guid RelatedEntityId { get; set; }
        [DataMember]
        public string RelatedEntityFieldName { get; set; }
    }
}