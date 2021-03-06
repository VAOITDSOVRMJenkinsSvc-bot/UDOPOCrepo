using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.EBenefitsBnftClaimStatusService.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.EBenefitsBnftClaimStatusService,findOpenBenefitClaimsStatusBySSN method, Response.
	/// Code Generated by IMS on: 12/21/2018 1:46:03 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISfindOpenBCStatBySSN_findOpenBenefitClaimsStatusBySSNResponse : VEISEcResponseBase
	{
		[DataMember]
		public VEISfindOpenBCStatBySSN_benefitClaimDTOMultipleResponse[] VEISfindOpenBCStatBySSN_benefitClaimDTOInfo { get; set; }
	}
	[DataContract]
	public class VEISfindOpenBCStatBySSN_benefitClaimDTOMultipleResponse
	{
		[DataMember]
		public string mcs_appealPossible { get; set; }
		[DataMember]
		public string mcs_attentionNeeded { get; set; }
		[DataMember]
		public string mcs_baseEndPrdctTypeCd { get; set; }
		[DataMember]
		public Int64 mcs_benefitClaimId { get; set; }
		[DataMember]
		public string mcs_bnftClaimTypeCd { get; set; }
		[DataMember]
		public string mcs_claimCloseDt { get; set; }
		[DataMember]
		public string mcs_claimCompleteDt { get; set; }
		[DataMember]
		public string mcs_claimDt { get; set; }
		[DataMember]
		public string mcs_claimStatus { get; set; }
		[DataMember]
		public string mcs_claimStatusType { get; set; }
		[DataMember]
		public string mcs_decisionNotificationSent { get; set; }
		[DataMember]
		public string mcs_developmentLetterSent { get; set; }
		[DataMember]
		public string mcs_ealiestEvidenceDueDate { get; set; }
		[DataMember]
		public string mcs_endPrdctTypeCd { get; set; }
		[DataMember]
		public string mcs_filed5103WaiverInd { get; set; }
		[DataMember]
		public string mcs_latestEvidenceRecdDate { get; set; }
		[DataMember]
		public string mcs_maxEstClaimCompleteDt { get; set; }
		[DataMember]
		public string mcs_minEstClaimCompleteDt { get; set; }
		[DataMember]
		public string mcs_phaseChngdDt { get; set; }
		[DataMember]
		public string mcs_phaseType { get; set; }
		[DataMember]
		public string mcs_programType { get; set; }
		[DataMember]
		public Int64 mcs_ptcpntClmantId { get; set; }
		[DataMember]
		public Int64 mcs_ptcpntVetId { get; set; }
		[DataMember]
		public string mcs_submtrApplcnTypeCd { get; set; }
		[DataMember]
		public string mcs_submtrRoleTypeCd { get; set; }
	}
}
