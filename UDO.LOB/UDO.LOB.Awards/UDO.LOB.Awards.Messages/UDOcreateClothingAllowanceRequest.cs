﻿using System;
using System.ComponentModel.Composition;
//using Microsoft.Xrm.Sdk;
using System.Runtime.Serialization;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Messages;
using UDO.LOB.Core;
/// <summary>
/// VIMT LOB Component for UDOcreateClothingAllowance,createClothingAllowance method, Request.
/// Code Generated by IMS on: 6/2/2015 9:41:07 AM
/// Version: 2015.05.05
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace UDO.LOB.Awards.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOcreateClothingAllowanceRequest)]
    [DataContract]
    public class UDOcreateClothingAllowanceRequest : MessageBase
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
        public UDOHeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public Guid ownerId { get; set; }
        [DataMember]
        public string ownerType { get; set; }
        [DataMember]
        public Guid AwardId { get; set; }
        [DataMember]
        public string ptcpntVetId { get; set; }
        [DataMember]
        public string ptcpntBeneId { get; set; }
        [DataMember]
        public string ptcpntRecipId { get; set; }
        [DataMember]
        public string awardTypeCd { get; set; }
        [DataMember]
        public UDOcreateClothingAllowanceRelatedEntitiesMultipleRequest[] UDOcreateClothingAllowanceRelatedEntitiesInfo { get; set; }
    }
    [DataContract]
    public class UDOcreateClothingAllowanceRelatedEntitiesMultipleRequest
    {
        [DataMember]
        public string RelatedEntityName { get; set; }
        [DataMember]
        public Guid RelatedEntityId { get; set; }
        [DataMember]
        public string RelatedEntityFieldName { get; set; }
    }
}
