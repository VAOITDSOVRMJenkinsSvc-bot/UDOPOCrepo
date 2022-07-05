﻿using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
//using UDO.LOB.Extensions;
using VEIS.Core.Messages;

//using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;

/// <summary>
/// VIMT LOB Component for UDOcreatePayments,createPayments method, Request.
/// Code Generated by IMS on: 6/4/2015 9:18:47 AM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace UDO.LOB.Payments.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOcreatePaymentsRequest)]
    [DataContract]
    public class UDOcreatePaymentsRequest : MessageBase
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
        public Guid vetsnapshotId { get; set; }
        [DataMember]
        public Guid udo_personId { get; set; }
        [DataMember]
        public Guid udo_paymentId { get; set; }
        [DataMember]
        public Guid udo_payeecodeId { get; set; }
        [DataMember]
        public Int64 PaymentId { get; set; }
        [DataMember]
        public Int64 ParticipantId { get; set; }
        [DataMember]
        public string FileNumber { get; set; }
        [DataMember]
        public string PayeeCode { get; set; }
        [DataMember]
        public UDOcreatePaymentsRelatedEntitiesMultipleRequest[] UDOcreatePaymentsRelatedEntitiesInfo { get; set; }
        [DataMember(IsRequired = false)]
        public bool IDProofOrchestration { get; set; }
        [DataMember(IsRequired=false)] //CreatePaymentRecords is not used
        public bool CreatePaymentRecords { get; set; }
    }

    [DataContract]
    public class UDOcreatePaymentsRelatedEntitiesMultipleRequest
    {
        [DataMember]
        public string RelatedEntityName { get; set; }
        [DataMember]
        public Guid RelatedEntityId { get; set; }
        [DataMember]
        public string RelatedEntityFieldName { get; set; }
    }
}