using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.Messages.ClaimantService
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.ClaimantService,findGeneralInformationByFileNumber method, Response.
	/// Code Generated by IMS on: 12/19/2018 9:34:26 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISfgenFNfindGeneralInformationByFileNumberResponse : VEISEcResponseBase
	{
		[DataMember]
		public VEISfgenFNreturn VEISfgenFNreturnInfo { get; set; }
	}
	[DataContract]
	public class VEISfgenFNreturn
	{
		[DataMember]
		public string mcs_additionalServiceIndicator { get; set; }
		[DataMember]
		public string mcs_authznChangeClmantAddrsInd { get; set; }
		[DataMember]
		public string mcs_authznPoaAccessInd { get; set; }
		[DataMember]
		public string mcs_awardTypeCode { get; set; }
		[DataMember]
		public string mcs_benefitTypeCode { get; set; }
		[DataMember]
		public string mcs_benefitTypeName { get; set; }
		[DataMember]
		public string mcs_clothingAllowanceTypeCode { get; set; }
		[DataMember]
		public string mcs_clothingAllowanceTypeName { get; set; }
		[DataMember]
		public string mcs_cnsldtPymtCd { get; set; }
		[DataMember]
		public string mcs_cnsldtPymtNm { get; set; }
		[DataMember]
		public string mcs_competencyDecisionTypeCode { get; set; }
		[DataMember]
		public string mcs_competencyDecisionTypeName { get; set; }
		[DataMember]
		public string mcs_convertedCaseIndicator { get; set; }
		[DataMember]
		public string mcs_currentMonthlyRate { get; set; }
		[DataMember]
		public string mcs_desertShieldIndicator { get; set; }
		[DataMember]
		public string mcs_directDepositAccountID { get; set; }
		[DataMember]
		public string mcs_enteredOnDutyDate { get; set; }
		[DataMember]
		public string mcs_fiduciaryDecisionTypeCode { get; set; }
		[DataMember]
		public string mcs_fiduciaryDecisionTypeName { get; set; }
		[DataMember]
		public string mcs_fundsDueIncompetentBalance { get; set; }
		[DataMember]
		public string mcs_guardianFolderLocation { get; set; }
		[DataMember]
		public string mcs_gulfWarRegistryIndicator { get; set; }
		[DataMember]
		public string mcs_mailingAddressID { get; set; }
		[DataMember]
		public string mcs_militaryBranch { get; set; }
		[DataMember]
		public string mcs_numberOfAwardBenes { get; set; }
		[DataMember]
		public string mcs_numberOfDiaries { get; set; }
		[DataMember]
		public string mcs_numberOfEvrs { get; set; }
		[DataMember]
		public string mcs_numberOfFlashes { get; set; }
		[DataMember]
		public string mcs_nursingHomeIndicator { get; set; }
		[DataMember]
		public string mcs_nursingHomeName { get; set; }
		[DataMember]
		public string mcs_paidThroughDate { get; set; }
		[DataMember]
		public string mcs_paraplegicHousingNumber { get; set; }
		[DataMember]
		public string mcs_payStatusTypeCode { get; set; }
		[DataMember]
		public string mcs_payStatusTypeName { get; set; }
		[DataMember]
		public string mcs_payeeBirthDate { get; set; }
		[DataMember]
		public string mcs_payeeName { get; set; }
		[DataMember]
		public string mcs_payeeSSN { get; set; }
		[DataMember]
		public string mcs_payeeSex { get; set; }
		[DataMember]
		public string mcs_payeeTypeCode { get; set; }
		[DataMember]
		public string mcs_payeeTypeIndicator { get; set; }
		[DataMember]
		public string mcs_payeeTypeName { get; set; }
		[DataMember]
		public string mcs_paymentAddressID { get; set; }
		[DataMember]
		public string mcs_personalFundsOfPatientBalance { get; set; }
		[DataMember]
		public string mcs_powerOfAttorney { get; set; }
		[DataMember]
		public string mcs_ptcpntBeneID { get; set; }
		[DataMember]
		public string mcs_ptcpntRecipID { get; set; }
		[DataMember]
		public string mcs_ptcpntVetID { get; set; }
		[DataMember]
		public string mcs_releasedActiveDutyDate { get; set; }
		[DataMember]
		public string mcs_returnCode { get; set; }
		[DataMember]
		public string mcs_returnMessage { get; set; }
		[DataMember]
		public string mcs_stationOfJurisdiction { get; set; }
		[DataMember]
		public string mcs_statusReasonDate { get; set; }
		[DataMember]
		public string mcs_statusReasonTypeCode { get; set; }
		[DataMember]
		public string mcs_statusReasonTypeName { get; set; }
		[DataMember]
		public string mcs_vetBirthDate { get; set; }
		[DataMember]
		public string mcs_vetDeathDate { get; set; }
		[DataMember]
		public string mcs_vetFirstName { get; set; }
		[DataMember]
		public string mcs_vetLastName { get; set; }
		[DataMember]
		public string mcs_vetMiddleName { get; set; }
		[DataMember]
		public string mcs_vetSSN { get; set; }
		[DataMember]
		public string mcs_vetSex { get; set; }
		[DataMember]
		public VEISfgenFNawardBenesMultipleResponse[] VEISfgenFNawardBenesInfo { get; set; }
		[DataMember]
		public VEISfgenFNdiariesMultipleResponse[] VEISfgenFNdiariesInfo { get; set; }
		[DataMember]
		public VEISfgenFNevrsMultipleResponse[] VEISfgenFNevrsInfo { get; set; }
		[DataMember]
		public VEISfgenFNflashesMultipleResponse[] VEISfgenFNflashesInfo { get; set; }
	}
	[DataContract]
	public class VEISfgenFNawardBenesMultipleResponse
	{
		[DataMember]
		public string mcs_awardBeneTypeCd { get; set; }
		[DataMember]
		public string mcs_awardBeneTypeName { get; set; }
		[DataMember]
		public string mcs_awardTypeCd { get; set; }
		[DataMember]
		public string mcs_awardTypeName { get; set; }
		[DataMember]
		public string mcs_beneName { get; set; }
		[DataMember]
		public string mcs_payeeCd { get; set; }
		[DataMember]
		public string mcs_ptcpntBeneId { get; set; }
		[DataMember]
		public string mcs_ptcpntRecipId { get; set; }
		[DataMember]
		public string mcs_ptcpntVetId { get; set; }
		[DataMember]
		public string mcs_recipName { get; set; }
		[DataMember]
		public string mcs_vetName { get; set; }
		[DataMember]
		public VEISfgenFNawardBenePK VEISfgenFNawardBenePKInfo { get; set; }
	}
	[DataContract]
	public class VEISfgenFNawardBenePK
	{
		[DataMember]
		public string mcs_awardTypeCd { get; set; }
		[DataMember]
		public string mcs_ptcpntBeneId { get; set; }
		[DataMember]
		public string mcs_ptcpntRecipId { get; set; }
		[DataMember]
		public string mcs_ptcpntVetId { get; set; }
	}
	[DataContract]
	public class VEISfgenFNdiariesMultipleResponse
	{
		[DataMember]
		public string mcs_date { get; set; }
		[DataMember]
		public string mcs_description { get; set; }
		[DataMember]
		public string mcs_id { get; set; }
		[DataMember]
		public string mcs_reasonCd { get; set; }
		[DataMember]
		public string mcs_reasonName { get; set; }
	}
	[DataContract]
	public class VEISfgenFNevrsMultipleResponse
	{
		[DataMember]
		public string mcs_control { get; set; }
		[DataMember]
		public string mcs_exempt { get; set; }
		[DataMember]
		public string mcs_lastReported { get; set; }
		[DataMember]
		public string mcs_status { get; set; }
		[DataMember]
		public string mcs_type { get; set; }
	}
	[DataContract]
	public class VEISfgenFNflashesMultipleResponse
	{
		[DataMember]
		public string mcs_assignedIndicator { get; set; }
		[DataMember]
		public string mcs_flashName { get; set; }
		[DataMember]
		public string mcs_flashType { get; set; }
	}
}
