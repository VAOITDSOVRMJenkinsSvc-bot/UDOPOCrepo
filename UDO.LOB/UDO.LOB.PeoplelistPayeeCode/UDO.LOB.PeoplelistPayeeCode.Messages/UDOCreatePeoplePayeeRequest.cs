﻿using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using VEIS.Core.Messages;
using VEIS.Messages.BenefitClaimService;
using VEIS.Messages.ClaimantService;
// REM: TN determined not to use because items exist in ClaimantService that do not exist in V2, and Have not found it true other way around.
// using VEIS.Messages.BenefitClaimServiceV2;

//using Microsoft.Xrm.Sdk;
//using VRM.Integration.Servicebus.Core;
//using VIMT.BenefitClaimService.Messages;
//using VIMT.ClaimantWebService.Messages;

/// <summary>
/// VIMT LOB Component for UDOcreateAwardLines,createAwardLines method, Request.
/// Code Generated by IMS on: 4/29/2015 4:07:49 PM
/// Version: 2015.19.01
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>

namespace UDO.LOB.PeoplelistPayeeCode.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDOCreatePeoplePayeeRequest)]
    [DataContract]
    public class UDOCreatePeoplePayeeRequest : MessageBase
    {
        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public Guid UserId { get; set; }
        /*[DataMember]
        public Guid RelatedParentId { get; set; }
        [DataMember]
        public string RelatedParentEntityName { get; set; }
        [DataMember]
        public string RelatedParentFieldName { get; set; }*/
        [DataMember]
        public bool LogTiming { get; set; }
        [DataMember]
        public bool LogSoap { get; set; }
        [DataMember]
        public bool Debug { get; set; }
        [DataMember]
        public UDOHeaderInfo LegacyServiceHeaderInfo { get; set; }
        [DataMember]
        public Guid idProofId { get; set; }
        [DataMember]
        public Guid vetsnapshotId { get; set; }
        [DataMember]
        public Guid ownerId { get; set; }
        [DataMember]
        public string ownerType { get; set; }
        [DataMember]
        public Guid udo_contactId { get; set; }
        [DataMember]
        public Guid udo_dependentId { get; set; }
        [DataMember]
        public string fileNumber { get; set; }
        [DataMember]
        public string ptcpntVetId { get; set; }
        [DataMember]
        public string ptcpntBeneId { get; set; }
        [DataMember]
        public string ptcpntRecipId { get; set; }
        [DataMember]
        public string awardTypeCd { get; set; }
        [DataMember]
        public string udo_ssn { get; set; }
        [DataMember]
        public bool fidExists { get; set; }
        [DataMember]
        public FromUDOcreateAwardsMultipleResponse[] UDOcreateAwardsInfo { get; set; }
        [DataMember]
        public VEISfgenFNfindGeneralInformationByFileNumberResponse findGeneralResponse { get; set; }
        [DataMember]
        public VEISfindBenefitClaimResponse findBenefitClaimResponse { get; set; }
        [DataMember]
        public PeopleRelatedEntitiesMultipleRequest[] UDOcreatePeopleRelatedEntitiesInfo { get; set; }
        [DataMember]
        public PayeeCodeRelatedEntitiesMultipleRequest[] UDOcreatePayeeRelatedEntitiesInfo { get; set; }

    }
    [DataContract]
    public class FromUDOcreateAwardsMultipleResponse
    {
        [DataMember]
        public Guid newUDOcreateAwardsId { get; set; }
        [DataMember]
        public string mcs_ptcpntBeneId { get; set; }
        [DataMember]
        public string mcs_ptcpntRecipId { get; set; }
        [DataMember]
        public string mcs_ptcpntVetId { get; set; }
        [DataMember]
        public string mcs_payeeCd { get; set; }
        [DataMember]
        public string mcs_awardTypeCd { get; set; }
        [DataMember]
        public string mcs_awardBeneTypeName { get; set; }
        [DataMember]
        public string mcs_awardBeneTypeCd { get; set; }

    }
    [DataContract]
    public class PeopleRelatedEntitiesMultipleRequest
    {
        [DataMember]
        public string RelatedEntityName { get; set; }
        [DataMember]
        public Guid RelatedEntityId { get; set; }
        [DataMember]
        public string RelatedEntityFieldName { get; set; }
    }
    [DataContract]
    public class PayeeCodeRelatedEntitiesMultipleRequest
    {
        [DataMember]
        public string RelatedEntityName { get; set; }
        [DataMember]
        public Guid RelatedEntityId { get; set; }
        [DataMember]
        public string RelatedEntityFieldName { get; set; }
    }
}