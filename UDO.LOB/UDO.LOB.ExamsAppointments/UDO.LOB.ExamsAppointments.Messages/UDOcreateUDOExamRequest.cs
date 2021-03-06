using System;
using System.Runtime.Serialization;
using UDO.LOB.Core;

//using System.ComponentModel.Composition;
//using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Messages;

/// <summary>
/// VIMT LOB Component for UDOcreateUDOVirtualVA,createUDOVirtualVA method, Request.
/// Code Generated by IMS on: 7/20/2015 3:41:31 PM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace UDO.LOB.ExamsAppointments.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOcreateUDOExamRequest)]
    [DataContract]
    public class UDOcreateUDOExamRequest : MessageBase
    {
        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public Guid UserId { get; set; }
        // Hidden as these are part of UDO.LOB.Core.MessageBase
        // Use 'new' if hidden intentionally
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
        public string ICN { get; set; }
        [DataMember]
        public string transactionId { get; set; }
        [DataMember]
        public UDOHeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public Guid udo_examId { get; set; }
        [DataMember]
        public UDOcreateUDOExamsRelatedEntitiesMultipleRequest[] UDOcreateUDOExamRelatedEntitiesInfo { get; set; }
    }
    [DataContract]
    public class UDOcreateUDOExamsRelatedEntitiesMultipleRequest
    {
        [DataMember]
        public string RelatedEntityName { get; set; }
        [DataMember]
        public Guid RelatedEntityId { get; set; }
        [DataMember]
        public string RelatedEntityFieldName { get; set; }
    }
}