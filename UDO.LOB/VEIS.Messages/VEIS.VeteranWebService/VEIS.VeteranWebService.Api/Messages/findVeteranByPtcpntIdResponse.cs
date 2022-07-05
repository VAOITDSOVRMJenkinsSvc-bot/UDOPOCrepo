using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.VeteranWebService.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.VeteranWebService,findVeteranByPtcpntId method, Response.
	/// Code Generated by IMS on: 12/12/2018 11:33:15 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISvetPctfindVeteranByPtcpntIdResponse : VEISEcResponseBase
	{
		[DataMember]
		public VEISvetPctreturn VEISvetPctreturnInfo { get; set; }
	}
	[DataContract]
	public class VEISvetPctreturn
	{
		[DataMember]
		public VEISvetPctvetBirlsRecord VEISvetPctvetBirlsRecordInfo { get; set; }
		[DataMember]
		public VEISvetPctvetCorpRecord VEISvetPctvetCorpRecordInfo { get; set; }
	}
	[DataContract]
	public class VEISvetPctvetBirlsRecord
	{
		[DataMember]
		public string mcs_RETURN_CODE { get; set; }
		[DataMember]
		public string mcs_RETURN_MESSAGE { get; set; }
		[DataMember]
		public string mcs_SPACE { get; set; }
		[DataMember]
		public string mcs_CLAIM_NUMBER { get; set; }
		[DataMember]
		public string mcs_SOC_SEC_NUMBER { get; set; }
		[DataMember]
		public string mcs_INS_PREFIX { get; set; }
		[DataMember]
		public string mcs_INS_NUMBER { get; set; }
		[DataMember]
		public string mcs_LAST_NAME { get; set; }
		[DataMember]
		public string mcs_FIRST_NAME { get; set; }
		[DataMember]
		public string mcs_MIDDLE_NAME { get; set; }
		[DataMember]
		public string mcs_NAME_SUFFIX { get; set; }
		[DataMember]
		public string mcs_BIRTH_MONTH { get; set; }
		[DataMember]
		public string mcs_BIRTH_DAY { get; set; }
		[DataMember]
		public string mcs_BIRTH_CENTURY { get; set; }
		[DataMember]
		public string mcs_BIRTH_YEAR { get; set; }
		[DataMember]
		public string mcs_DATE_OF_BIRTH { get; set; }
		[DataMember]
		public string mcs_DEATH_MONTH { get; set; }
		[DataMember]
		public string mcs_DEATH_DAY { get; set; }
		[DataMember]
		public string mcs_DEATH_CENTURY { get; set; }
		[DataMember]
		public string mcs_DEATH_YEAR { get; set; }
		[DataMember]
		public string mcs_DATE_OF_DEATH { get; set; }
		[DataMember]
		public string mcs_POW_NUMBER_OF_DAYS { get; set; }
		[DataMember]
		public string mcs_TOTAL_ACTIVE_SERVICE_YEARS { get; set; }
		[DataMember]
		public string mcs_TOTAL_ACTIVE_SERVICE_MONTHS { get; set; }
		[DataMember]
		public string mcs_TOTAL_ACTIVE_SERVICE_DAYS { get; set; }
		[DataMember]
		public string mcs_DISABILITY_SEVERANCE_PAY { get; set; }
		[DataMember]
		public string mcs_LUMP_SUM_READJUSTMENT_PAY { get; set; }
		[DataMember]
		public string mcs_SEPARATION_PAY { get; set; }
		[DataMember]
		public string mcs_CLAIM_FOLDER_LOCATION { get; set; }
		[DataMember]
		public string mcs_VET_HAS_BENE_IND { get; set; }
		[DataMember]
		public string mcs_VET_IS_BENE_IND { get; set; }
		[DataMember]
		public string mcs_PURPLE_HEART_IND { get; set; }
		[DataMember]
		public string mcs_VERIFIED_SOC_SEC_IND { get; set; }
		[DataMember]
		public string mcs_VA_EMPLOYEE_IND { get; set; }
		[DataMember]
		public string mcs_VIETNAM_SERVICE_IND { get; set; }
		[DataMember]
		public string mcs_DISABILITY_IND { get; set; }
		[DataMember]
		public string mcs_MEDAL_OF_HONOR_IND { get; set; }
		[DataMember]
		public string mcs_TRANSFER_TO_RESERVES_IND { get; set; }
		[DataMember]
		public string mcs_ACTIVE_DUTY_TRAINING_IND { get; set; }
		[DataMember]
		public string mcs_REENLISTED_IND { get; set; }
		[DataMember]
		public string mcs_BURIAL_FLAG_ISSUE_IND { get; set; }
		[DataMember]
		public string mcs_SEX_CODE { get; set; }
		[DataMember]
		public string mcs_CONTESTED_DATA_IND { get; set; }
		[DataMember]
		public string mcs_GUARDIANSHIP_CASE_IND { get; set; }
		[DataMember]
		public string mcs_INCOMPETENT_IND { get; set; }
		[DataMember]
		public string mcs_CP_VET_CP_BENE_IND { get; set; }
		[DataMember]
		public string mcs_VADS_IND { get; set; }
		[DataMember]
		public string mcs_VERIFIED_SVC_DATA_IND { get; set; }
		[DataMember]
		public string mcs_CH30_IND { get; set; }
		[DataMember]
		public string mcs_CH32_BANK_IND { get; set; }
		[DataMember]
		public string mcs_CH32_BEN_IND { get; set; }
		[DataMember]
		public string mcs_CH34_IND { get; set; }
		[DataMember]
		public string mcs_CH106_IND { get; set; }
		[DataMember]
		public string mcs_CH31_IND { get; set; }
		[DataMember]
		public string mcs_CH32_903_IND { get; set; }
		[DataMember]
		public string mcs_IND_901 { get; set; }
		[DataMember]
		public string mcs_JOBS_IND { get; set; }
		[DataMember]
		public string mcs_VARMS_IND { get; set; }
		[DataMember]
		public string mcs_DIAGS_VERIFIED_IND { get; set; }
		[DataMember]
		public string mcs_HOMELESS_VET_IND { get; set; }
		[DataMember]
		public string mcs_RET_SVR_IND { get; set; }
		[DataMember]
		public string mcs_PERSIAN_GULF_SVC_IND { get; set; }
		[DataMember]
		public string mcs_SVC_MED_RECORD_IND { get; set; }
		[DataMember]
		public string mcs_BANKRUPTCY_IND { get; set; }
		[DataMember]
		public string mcs_CAUSE_OF_DEATH { get; set; }
		[DataMember]
		public string mcs_DEATH_IN_SVC { get; set; }
		[DataMember]
		public string mcs_POWER_OF_ATTY_CODE1 { get; set; }
		[DataMember]
		public string mcs_POWER_OF_ATTY_CODE2 { get; set; }
		[DataMember]
		public string mcs_CLOTHING_ALLOWANCE { get; set; }
		[DataMember]
		public string mcs_NUM_OF_SVC_CON_DIS { get; set; }
		[DataMember]
		public string mcs_BURIAL_AWARD_PLOT { get; set; }
		[DataMember]
		public string mcs_BURIAL_AWARD_TRANSPORT { get; set; }
		[DataMember]
		public string mcs_HEADSTONE { get; set; }
		[DataMember]
		public string mcs_PAYMENT { get; set; }
		[DataMember]
		public string mcs_APPLICATION_FOR_PLOT { get; set; }
		[DataMember]
		public string mcs_ADAPTIVE_EQUIPMENT { get; set; }
		[DataMember]
		public string mcs_SPECIAL_ADAPTIVE_HOUSING { get; set; }
		[DataMember]
		public string mcs_REASON_FOR_TERM_DISALLOW { get; set; }
		[DataMember]
		public string mcs_ENTITLEMENT_CODE { get; set; }
		[DataMember]
		public string mcs_SPECIAL_LAW_CODE { get; set; }
		[DataMember]
		public string mcs_CP_EFFCTVE_DATE_OF_TERM { get; set; }
		[DataMember]
		public string mcs_BURIAL_AWD_SVC_CONNECT { get; set; }
		[DataMember]
		public string mcs_BURIAL_AWD_NONSVC_CON { get; set; }
		[DataMember]
		public string mcs_AUTOMOBILE_ALLOWANCE { get; set; }
		[DataMember]
		public string mcs_COMBINED_DEGREE { get; set; }
		[DataMember]
		public string mcs_ADD_DIA_IND { get; set; }
		[DataMember]
		public string mcs_EMPLOYEE_NUMBER { get; set; }
		[DataMember]
		public string mcs_EMPLOYEE_STATION_NUMBER { get; set; }
		[DataMember]
		public string mcs_UPDATE_MONTH { get; set; }
		[DataMember]
		public string mcs_UPDATE_DAY { get; set; }
		[DataMember]
		public string mcs_UPDATE_CENTURY { get; set; }
		[DataMember]
		public string mcs_UPDATE_YEAR { get; set; }
		[DataMember]
		public string mcs_DATE_OF_UPDATE { get; set; }
		[DataMember]
		public string mcs_NUMBER_OF_DISCLOSURES { get; set; }
		[DataMember]
		public string mcs_INSURANCE_JURIS { get; set; }
		[DataMember]
		public string mcs_INS_LAPSED_PURGE_MONTH { get; set; }
		[DataMember]
		public string mcs_INS_LAPSED_PURGE_CENTURY { get; set; }
		[DataMember]
		public string mcs_INS_LAPSED_PURGE_YEAR { get; set; }
		[DataMember]
		public string mcs_DATE_OF_INS_LAPSED_PURGE { get; set; }
		[DataMember]
		public string mcs_CH30_OVERPAYMENT { get; set; }
		[DataMember]
		public string mcs_DMDC_RETIRE_PAY_SBP_AMT_C { get; set; }
		[DataMember]
		public string mcs_DMDC_RETIRE_PAY_SBP_MM_C { get; set; }
		[DataMember]
		public string mcs_DMDC_RETIRE_PAY_SBP_YEAR_C { get; set; }
		[DataMember]
		public string mcs_DATE_OF_DMDC_RETIRE_PAY_C { get; set; }
		[DataMember]
		public string mcs_DMDC_RETIRE_PAY_SBP_AMT_P { get; set; }
		[DataMember]
		public string mcs_DMDC_RETIRE_PAY_SBP_MM_P { get; set; }
		[DataMember]
		public string mcs_DMDC_RETIRE_PAY_SBP_YEAR_P { get; set; }
		[DataMember]
		public string mcs_DATE_OF_DMDC_RETIRE_PAY_P { get; set; }
		[DataMember]
		public string mcs_VADS_IND2 { get; set; }
		[DataMember]
		public string mcs_VADS_IND3 { get; set; }
		[DataMember]
		public string mcs_VERIFIED_SVC_DATA_IND2 { get; set; }
		[DataMember]
		public string mcs_VERIFIED_SVC_DATA_IND3 { get; set; }
		[DataMember]
		public string mcs_SVC_NUM_EDIT_FILLER { get; set; }
		[DataMember]
		public string mcs_PVR_MONTH { get; set; }
		[DataMember]
		public string mcs_PVR_DAY { get; set; }
		[DataMember]
		public string mcs_PVR_CENTURY { get; set; }
		[DataMember]
		public string mcs_PVR_YEAR { get; set; }
		[DataMember]
		public string mcs_PVR_FILLER1 { get; set; }
		[DataMember]
		public string mcs_APPEALS_IND { get; set; }
		[DataMember]
		public string mcs_IN_THEATER_START_DATE { get; set; }
		[DataMember]
		public string mcs_IN_THEATER_END_DATE { get; set; }
		[DataMember]
		public string mcs_IN_THEATER_DAYS { get; set; }
		[DataMember]
		public string mcs_NUMBER_OF_RECORDS { get; set; }
		[DataMember]
		public VEISvetPctINSURANCE_POLICYMultipleResponse[] VEISvetPctINSURANCE_POLICYInfo { get; set; }
		[DataMember]
		public VEISvetPctSERVICEMultipleResponse[] VEISvetPctSERVICEInfo { get; set; }
		[DataMember]
		public VEISvetPctALTERNATE_NAMEMultipleResponse[] VEISvetPctALTERNATE_NAMEInfo { get; set; }
		[DataMember]
		public VEISvetPctFOLDERMultipleResponse[] VEISvetPctFOLDERInfo { get; set; }
		[DataMember]
		public VEISvetPctFLASHMultipleResponse[] VEISvetPctFLASHInfo { get; set; }
		[DataMember]
		public VEISvetPctSERVICEDIAGNOSTICSMultipleResponse[] VEISvetPctSERVICEDIAGNOSTICSInfo { get; set; }
		[DataMember]
		public VEISvetPctRECURING_DISCLOSUREMultipleResponse[] VEISvetPctRECURING_DISCLOSUREInfo { get; set; }
		[DataMember]
		public VEISvetPctBIRLS_SELECTIONMultipleResponse[] VEISvetPctBIRLS_SELECTIONInfo { get; set; }
		[DataMember]
		public VEISvetPctBENE_RECORD VEISvetPctBENE_RECORDInfo { get; set; }
	}
	[DataContract]
	public class VEISvetPctINSURANCE_POLICYMultipleResponse
	{
		[DataMember]
		public string mcs_INS_POL_PREFIX { get; set; }
		[DataMember]
		public string mcs_INS_POL_NUMBER { get; set; }
	}
	[DataContract]
	public class VEISvetPctSERVICEMultipleResponse
	{
		[DataMember]
		public string mcs_SHORT_SERVICE_NUMBER { get; set; }
		[DataMember]
		public string mcs_SERVICE_NUMBER_FILL { get; set; }
		[DataMember]
		public string mcs_BRANCH_OF_SERVICE { get; set; }
		[DataMember]
		public string mcs_ENTERED_ON_DUTY_DATE { get; set; }
		[DataMember]
		public string mcs_RELEASED_ACTIVE_DUTY_DATE { get; set; }
		[DataMember]
		public string mcs_SEPARATION_REASON_CODE { get; set; }
		[DataMember]
		public string mcs_NONPAY_DAYS { get; set; }
		[DataMember]
		public string mcs_PAY_GRADE { get; set; }
		[DataMember]
		public string mcs_CHAR_OF_SVC_CODE { get; set; }
	}
	[DataContract]
	public class VEISvetPctALTERNATE_NAMEMultipleResponse
	{
		[DataMember]
		public string mcs_ALT_LAST_NAME { get; set; }
		[DataMember]
		public string mcs_ALT_FIRST_NAME { get; set; }
		[DataMember]
		public string mcs_ALT_MIDDLE_NAME { get; set; }
		[DataMember]
		public string mcs_ALT_NAME_SUFFIX { get; set; }
	}
	[DataContract]
	public class VEISvetPctFOLDERMultipleResponse
	{
		[DataMember]
		public string mcs_FOLDER_TYPE { get; set; }
		[DataMember]
		public string mcs_FOLDER_CURRENT_LOCATION { get; set; }
		[DataMember]
		public string mcs_TRANSFER_MONTH { get; set; }
		[DataMember]
		public string mcs_TRANSFER_DAY { get; set; }
		[DataMember]
		public string mcs_TRANSFER_YEAR { get; set; }
		[DataMember]
		public string mcs_DATE_OF_TRANSFER { get; set; }
		[DataMember]
		public string mcs_FOLDER_PRIOR_LOCATION { get; set; }
		[DataMember]
		public string mcs_IN_TRANSIT_TO_STATION { get; set; }
		[DataMember]
		public string mcs_IN_TRANSIT_MONTH { get; set; }
		[DataMember]
		public string mcs_IN_TRANSIT_DAY { get; set; }
		[DataMember]
		public string mcs_IN_TRANSIT_YEAR { get; set; }
		[DataMember]
		public string mcs_DATE_OF_TRANSIT { get; set; }
		[DataMember]
		public string mcs_RELOCATION_INDICATOR { get; set; }
		[DataMember]
		public string mcs_FARC_ACCESSION_NUM { get; set; }
		[DataMember]
		public string mcs_NO_FOLDER_EST_REASON { get; set; }
		[DataMember]
		public string mcs_FOLDER_DESTROYED_IND { get; set; }
		[DataMember]
		public string mcs_FOLDER_REBUILT_IND { get; set; }
		[DataMember]
		public string mcs_NO_RECORD_IND { get; set; }
		[DataMember]
		public string mcs_FLDR_RETIRE_MONTH { get; set; }
		[DataMember]
		public string mcs_FLDR_RETIRE_DAY { get; set; }
		[DataMember]
		public string mcs_FLDR_RETIRE_YEAR { get; set; }
		[DataMember]
		public string mcs_DATE_OF_FLDR_RETIRE { get; set; }
		[DataMember]
		public string mcs_BOX_SEQUENCE_NUMBER { get; set; }
		[DataMember]
		public string mcs_LOCATION_NUMBER { get; set; }
		[DataMember]
		public string mcs_INSURANCE_FOLDER_TYPE { get; set; }
	}
	[DataContract]
	public class VEISvetPctFLASHMultipleResponse
	{
		[DataMember]
		public string mcs_FLASH_CODE { get; set; }
		[DataMember]
		public string mcs_FLASH_STATION { get; set; }
		[DataMember]
		public string mcs_FLASH_ROUTING_SYMBOL { get; set; }
	}
	[DataContract]
	public class VEISvetPctSERVICEDIAGNOSTICSMultipleResponse
	{
		[DataMember]
		public string mcs_SERVICE_DIAGNOSTICS { get; set; }
		[DataMember]
		public string mcs_SERVICE_PERCENT1 { get; set; }
		[DataMember]
		public string mcs_SERVICE_PERCENT2 { get; set; }
		[DataMember]
		public string mcs_RECUR_ANALOGUS_CODE { get; set; }
		[DataMember]
		public string mcs_RECUR_SVC_CON_DISABILITY { get; set; }
	}
	[DataContract]
	public class VEISvetPctRECURING_DISCLOSUREMultipleResponse
	{
		[DataMember]
		public string mcs_RECUR_DISCLOSURE_NUM { get; set; }
		[DataMember]
		public string mcs_RECUR_DISCLOSURE_MONTH { get; set; }
		[DataMember]
		public string mcs_RECUR_DISCLOSURE_YEAR { get; set; }
		[DataMember]
		public string mcs_DATE_OF_DISCLOSURE { get; set; }
	}
	[DataContract]
	public class VEISvetPctBIRLS_SELECTIONMultipleResponse
	{
		[DataMember]
		public string mcs_VET_IND { get; set; }
		[DataMember]
		public string mcs_LAST_NAME { get; set; }
		[DataMember]
		public string mcs_FIRST_NAME { get; set; }
		[DataMember]
		public string mcs_MIDDLE_NAME { get; set; }
		[DataMember]
		public string mcs_SUFFIX { get; set; }
		[DataMember]
		public string mcs_FILE_NUMBER { get; set; }
		[DataMember]
		public string mcs_PAYEE_CODE { get; set; }
		[DataMember]
		public string mcs_CURRENT_LOCATION { get; set; }
		[DataMember]
		public string mcs_ENTERED_ON_DUTY_DATE { get; set; }
		[DataMember]
		public string mcs_RELEASED_ACTIVE_DUTY_DATE { get; set; }
		[DataMember]
		public string mcs_DATE_OF_BIRTH { get; set; }
		[DataMember]
		public string mcs_DATE_OF_DEATH { get; set; }
		[DataMember]
		public string mcs_SSN_VERIFIED { get; set; }
		[DataMember]
		public string mcs_SSN { get; set; }
		[DataMember]
		public string mcs_SERVICE_NUMBER { get; set; }
		[DataMember]
		public string mcs_BRANCH_OF_SERVICE { get; set; }
	}
	[DataContract]
	public class VEISvetPctBENE_RECORD
	{
		[DataMember]
		public string mcs_BENE_IS_VET_IND { get; set; }
		[DataMember]
		public string mcs_BIRTH_CENTURY { get; set; }
		[DataMember]
		public string mcs_BIRTH_DAY { get; set; }
		[DataMember]
		public string mcs_BIRTH_MONTH { get; set; }
		[DataMember]
		public string mcs_BIRTH_YEAR { get; set; }
		[DataMember]
		public string mcs_CH35_CURRENT_LOCATION { get; set; }
		[DataMember]
		public string mcs_CH35_FARC_ACCESSION_NUM { get; set; }
		[DataMember]
		public string mcs_CH35_FOLDER_DESTROYED_IND { get; set; }
		[DataMember]
		public string mcs_CH35_FOLDER_REBUILT_IND { get; set; }
		[DataMember]
		public string mcs_CH35_FOLDER_RELOCATION_IND { get; set; }
		[DataMember]
		public string mcs_CH35_IN_TRANSIT_DAY { get; set; }
		[DataMember]
		public string mcs_CH35_IN_TRANSIT_MONTH { get; set; }
		[DataMember]
		public string mcs_CH35_IN_TRANSIT_TO_STATION { get; set; }
		[DataMember]
		public string mcs_CH35_IN_TRANSIT_YEAR { get; set; }
		[DataMember]
		public string mcs_CH35_NO_FOLDER_ESTAB { get; set; }
		[DataMember]
		public string mcs_CH35_NO_RECORD_IND { get; set; }
		[DataMember]
		public string mcs_CH35_PRIOR_LOCATION { get; set; }
		[DataMember]
		public string mcs_CH35_RETIRE_DAY { get; set; }
		[DataMember]
		public string mcs_CH35_RETIRE_MONTH { get; set; }
		[DataMember]
		public string mcs_CH35_RETIRE_YEAR { get; set; }
		[DataMember]
		public string mcs_CH35_TRANSFER_DAY { get; set; }
		[DataMember]
		public string mcs_CH35_TRANSFER_MONTH { get; set; }
		[DataMember]
		public string mcs_CH35_TRANSFER_YEAR { get; set; }
		[DataMember]
		public string mcs_CLAIM_NUMBER { get; set; }
		[DataMember]
		public string mcs_FIRST_NAME { get; set; }
		[DataMember]
		public string mcs_LAST_NAME { get; set; }
		[DataMember]
		public string mcs_MIDDLE_NAME { get; set; }
		[DataMember]
		public string mcs_NAME_SUFFIX { get; set; }
		[DataMember]
		public string mcs_PAYEE_NUMBER { get; set; }
		[DataMember]
		public string mcs_SEX_CODE { get; set; }
		[DataMember]
		public string mcs_SOC_SEC_NUMBER { get; set; }
		[DataMember]
		public VEISvetPctALTERNATE_NAME1MultipleResponse[] VEISvetPctALTERNATE_NAME1Info { get; set; }
	}
	[DataContract]
	public class VEISvetPctALTERNATE_NAME1MultipleResponse
	{
		[DataMember]
		public string mcs_ALT_LAST_NAME { get; set; }
		[DataMember]
		public string mcs_ALT_FIRST_NAME { get; set; }
		[DataMember]
		public string mcs_ALT_MIDDLE_NAME { get; set; }
		[DataMember]
		public string mcs_ALT_NAME_SUFFIX { get; set; }
	}
	[DataContract]
	public class VEISvetPctvetCorpRecord
	{
		[DataMember]
		public string mcs_addressLine1 { get; set; }
		[DataMember]
		public string mcs_addressLine2 { get; set; }
		[DataMember]
		public string mcs_addressLine3 { get; set; }
		[DataMember]
		public string mcs_areaNumberOne { get; set; }
		[DataMember]
		public string mcs_areaNumberTwo { get; set; }
		[DataMember]
		public string mcs_blockCaddInd { get; set; }
		[DataMember]
		public string mcs_city { get; set; }
		[DataMember]
		public string mcs_cnsldtPymtTypeCd { get; set; }
		[DataMember]
		public string mcs_competencyDecisionTypeCode { get; set; }
		[DataMember]
		public string mcs_country { get; set; }
		[DataMember]
		public string mcs_cpPaymentAddressLine1 { get; set; }
		[DataMember]
		public string mcs_cpPaymentAddressLine2 { get; set; }
		[DataMember]
		public string mcs_cpPaymentAddressLine3 { get; set; }
		[DataMember]
		public string mcs_cpPaymentCity { get; set; }
		[DataMember]
		public string mcs_cpPaymentCountry { get; set; }
		[DataMember]
		public string mcs_cpPaymentForeignZip { get; set; }
		[DataMember]
		public string mcs_cpPaymentPostOfficeTypeCode { get; set; }
		[DataMember]
		public string mcs_cpPaymentPostalTypeCode { get; set; }
		[DataMember]
		public string mcs_cpPaymentState { get; set; }
		[DataMember]
		public string mcs_cpPaymentZipCode { get; set; }
		[DataMember]
		public string mcs_dateOfBirth { get; set; }
		[DataMember]
		public string mcs_debitCardInd { get; set; }
		[DataMember]
		public string mcs_eftAccountNumber { get; set; }
		[DataMember]
		public string mcs_eftAccountType { get; set; }
		[DataMember]
		public string mcs_eftRoutingNumber { get; set; }
		[DataMember]
		public string mcs_emailAddress { get; set; }
		[DataMember]
		public string mcs_fiduciaryDecisionCategoryTypeCode { get; set; }
		[DataMember]
		public string mcs_fiduciaryFolderLocation { get; set; }
		[DataMember]
		public string mcs_fileNumber { get; set; }
		[DataMember]
		public string mcs_firstName { get; set; }
		[DataMember]
		public string mcs_foreignCode { get; set; }
		[DataMember]
		public string mcs_lastName { get; set; }
		[DataMember]
		public string mcs_middleName { get; set; }
		[DataMember]
		public string mcs_militaryPostOfficeTypeCode { get; set; }
		[DataMember]
		public string mcs_militaryPostalTypeCode { get; set; }
		[DataMember]
		public string mcs_orgName { get; set; }
		[DataMember]
		public string mcs_orgTitle { get; set; }
		[DataMember]
		public string mcs_orgType { get; set; }
		[DataMember]
		public string mcs_phoneNumberOne { get; set; }
		[DataMember]
		public string mcs_phoneNumberTwo { get; set; }
		[DataMember]
		public string mcs_phoneTypeNameOne { get; set; }
		[DataMember]
		public string mcs_phoneTypeNameTwo { get; set; }
		[DataMember]
		public string mcs_prepPhraseType { get; set; }
		[DataMember]
		public string mcs_provinceName { get; set; }
		[DataMember]
		public string mcs_ptcpntId { get; set; }
		[DataMember]
		public string mcs_ptcpntRelationship { get; set; }
		[DataMember]
		public string mcs_returnCode { get; set; }
		[DataMember]
		public string mcs_returnMessage { get; set; }
		[DataMember]
		public string mcs_salutationName { get; set; }
		[DataMember]
		public string mcs_sensitiveLevelOfRecord { get; set; }
		[DataMember]
		public string mcs_ssn { get; set; }
		[DataMember]
		public string mcs_state { get; set; }
		[DataMember]
		public string mcs_suffixName { get; set; }
		[DataMember]
		public string mcs_temporaryCustodianIndicator { get; set; }
		[DataMember]
		public string mcs_territoryName { get; set; }
		[DataMember]
		public string mcs_treasuryMailingAddressLine1 { get; set; }
		[DataMember]
		public string mcs_treasuryMailingAddressLine2 { get; set; }
		[DataMember]
		public string mcs_treasuryMailingAddressLine3 { get; set; }
		[DataMember]
		public string mcs_treasuryMailingAddressLine4 { get; set; }
		[DataMember]
		public string mcs_treasuryMailingAddressLine5 { get; set; }
		[DataMember]
		public string mcs_treasuryMailingAddressLine6 { get; set; }
		[DataMember]
		public string mcs_treasuryPaymentAddressLine1 { get; set; }
		[DataMember]
		public string mcs_treasuryPaymentAddressLine2 { get; set; }
		[DataMember]
		public string mcs_treasuryPaymentAddressLine3 { get; set; }
		[DataMember]
		public string mcs_treasuryPaymentAddressLine4 { get; set; }
		[DataMember]
		public string mcs_treasuryPaymentAddressLine5 { get; set; }
		[DataMember]
		public string mcs_treasuryPaymentAddressLine6 { get; set; }
		[DataMember]
		public string mcs_zipCode { get; set; }
		[DataMember]
		public VEISvetPctptcpntSearch VEISvetPctptcpntSearchInfo { get; set; }
	}
	[DataContract]
	public class VEISvetPctptcpntSearch
	{
		[DataMember]
		public string mcs_numberOfRecords { get; set; }
		[DataMember]
		public string mcs_returnCode { get; set; }
		[DataMember]
		public string mcs_returnMessage { get; set; }
		[DataMember]
		public VEISvetPctpersonsMultipleResponse[] VEISvetPctpersonsInfo { get; set; }
	}
	[DataContract]
	public class VEISvetPctpersonsMultipleResponse
	{
		[DataMember]
		public string mcs_branchOfService1 { get; set; }
		[DataMember]
		public string mcs_dateOfBirth { get; set; }
		[DataMember]
		public string mcs_dateOfDeath { get; set; }
		[DataMember]
		public string mcs_filler { get; set; }
		[DataMember]
		public string mcs_firstName { get; set; }
		[DataMember]
		public string mcs_highSecurityLevel { get; set; }
		[DataMember]
		public string mcs_lastName { get; set; }
		[DataMember]
		public string mcs_middleName { get; set; }
		[DataMember]
		public string mcs_ptcpntId { get; set; }
		[DataMember]
		public string mcs_securityIssueCount { get; set; }
		[DataMember]
		public string mcs_ssn { get; set; }
		[DataMember]
		public string mcs_suffixName { get; set; }
	}
}
