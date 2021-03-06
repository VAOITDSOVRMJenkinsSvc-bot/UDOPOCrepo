using System;
using System.ComponentModel.Composition;
using Microsoft.Xrm.Sdk;
using System.Runtime.Serialization;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Messages;

/// <summary>
/// VIMT LOB Component for UDOgetPaymentDetails,getPaymentDetails method, Request.
/// Code Generated by IMS on: 6/4/2015 1:39:18 PM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace VRM.Integration.UDO.Payments.Messages
{
    [Export(typeof(IMessageBase))]
    [ExportMetadata("MessageType", MessageRegistry.UDOgetPaymentDetailsRequest)]
    [DataContract]
    public class UDOgetPaymentDetailsRequest : MessageBase
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
        public bool latestpayment { get; set; }
        [DataMember]
        public HeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public Guid ownerId { get; set; }
        [DataMember]
        public string ownerType { get; set; }
        [DataMember]
        public Guid udo_paymentId { get; set; }
        [DataMember]
        public Int64 PaymentId { get; set; }
        [DataMember]
        public Int64 FbtId { get; set; }
        [DataMember]
        public Guid vetsnapshotId { get; set; }
        [DataMember]
        public Guid udo_personId { get; set; }
        [DataMember]
        public Guid payeecodeid { get; set; }
        [DataMember]
        public UDOcreateRelatedEntitiesMultipleRequest[] UDOcreateRelatedEntitiesInfo { get; set; }
    }
    [DataContract]
    public class UDOcreateRelatedEntitiesMultipleRequest
    {
        [DataMember]
        public string RelatedEntityName { get; set; }
        [DataMember]
        public Guid RelatedEntityId { get; set; }
        [DataMember]
        public string RelatedEntityFieldName { get; set; }
    }
}