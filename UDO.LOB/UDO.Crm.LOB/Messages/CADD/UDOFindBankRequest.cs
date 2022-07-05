﻿using System;
using System.ComponentModel.Composition;
using Microsoft.Xrm.Sdk;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Messages;
/// <summary>
/// VIMT LOB Component for UDOInitiateCADD,InitiateCADD method, Request.
/// Code Generated by IMS on: 8/4/2015 10:18:38 AM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace VRM.Integration.UDO.CADD.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", VRM.Integration.UDO.Messages.MessageRegistry.UDOFindBankRequest)]
    [DataContract]
    public class UDOFindBankRequest : MessageBase
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
        public Guid va_bankaccountId { get; set; }
        [DataMember]
        public Guid udo_personId { get; set; }
        [DataMember]
        public Guid udo_veteranId { get; set; }
        [DataMember]
        public string bankroutingnumber { get; set; }
        [DataMember]
        public string awardtypecode { get; set; }
        [DataMember]
        public string SSN { get; set; }
        [DataMember]
        public string appealFirstName { get; set; }
        [DataMember]
        public string appealLastName { get; set; }
        [DataMember]
        public Int64 ptcpntId { get; set; }
        [DataMember]
        public string PayeeCode { get; set; }
        [DataMember]
        public string RoutingNumber { get; set; }
   
    }
}