using System;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
using VEIS.Messages.ClaimantService;
// using VEIS.BenefitClaimServiceV2.Api.Messages;

using VEIS.Messages.BenefitClaimService;

//using Microsoft.Xrm.Sdk;
//using VIMT.ClaimantWebService.Messages;
//using VRM.Integration.Servicebus.Core;
//using VIMT.BenefitClaimService.Messages;
//using VRMMessages = VRM.Integration.UDO.Messages;

/// <summary>
/// VIMT LOB Component for UDOcreateAwardLines,createAwardLines method, Request.
/// Code Generated by IMS on: 4/29/2015 4:07:49 PM
/// Version: 2015.19.01
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace UDO.LOB.IDProofOrchestration.Messages
{

    public class UDOIDProofOrchestrationMessages : MessageBase
    {
        public string OrganizationName { get; set; }
        public Guid UserId { get; set; }
        //public Guid RelatedParentId { get; set; }
        //public string RelatedParentEntityName { get; set; }
        //public string RelatedParentFieldName { get; set; }
        public bool LogTiming { get; set; }
        public bool LogSoap { get; set; }
        public bool Debug { get; set; }
        public Guid ownerId { get; set; }
        public string ownerType { get; set; }
        // Replaced? HeaderInfo = LegacyHeaderInfo
        public UDOHeaderInfo LegacyServiceHeaderInfo { get; set; }
        public string ptcpntVetId { get; set; }
        public string ptcpntBeneId { get; set; }
        public string ptcpntRecipId { get; set; }
        public string awardTypeCd { get; set; }
        // Replaced: VIMTfgenFNfindGeneralInformationByFileNumberResponse = VEISfgenFNfindGeneralInformationByFileNumberResponse
        public VEISfgenFNfindGeneralInformationByFileNumberResponse findGeneralResponse { get; set; }
        // Replaced: VIMTfindBenefitClaimResponse = VEISfindBenefitClaimResponse
        public VEISfindBenefitClaimResponse findBenefitClaimResponse { get; set; }
    }
}
