﻿using System;
using System.ComponentModel.Composition;
using Microsoft.Xrm.Sdk;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Messages;

/// <summary>
/// VIMT LOB Component for UDOCreateFNOD,CreateFNOD method, Request.
/// Code Generated by IMS on: 10/12/2015 9:53:35 AM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace VRM.Integration.UDO.FNOD.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOInitiateFNODRequest)]
    [DataContract]
    public class UDOInitiateFNODRequest : MessageBase
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
        public Guid udo_personId { get; set; }
        [DataMember]
        public Guid udo_idproofId { get; set; }
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
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public HeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public UDOInitiateFNODRelatedEntitiesMultipleRequest[] UDOCreateFNODRelatedEntitiesInfo { get; set; }
    }
    [DataContract]
    public class UDOInitiateFNODRelatedEntitiesMultipleRequest
    {
        [DataMember]
        public string RelatedEntityName { get; set; }
        [DataMember]
        public Guid RelatedEntityId { get; set; }
        [DataMember]
        public string RelatedEntityFieldName { get; set; }
    }
}