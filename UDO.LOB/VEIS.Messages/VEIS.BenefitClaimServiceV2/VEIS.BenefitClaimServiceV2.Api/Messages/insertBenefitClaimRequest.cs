using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.BenefitClaimServiceV2.Api.Messages
{ 
	[DataContract] 
	public class VEISinsertBenefitClaimRequest : VEISEcRequestBase
    {
		[DataMember]
		public bool Debug
		{
			get;
			set;
		}

		[DataMember]
		public LegacyHeaderInfo LegacyServiceHeaderInfo
		{
			get;
			set;
		}

		[DataMember]
		public bool LogSoap
		{
			get;
			set;
		}

		[DataMember]
		public bool LogTiming
		{
			get;
			set;
		} 

		[DataMember]
		public string RelatedParentEntityName
		{
			get;
			set;
		}

		[DataMember]
		public string RelatedParentFieldName
		{
			get;
			set;
		}

		[DataMember]
		public Guid RelatedParentId
		{
			get;
			set;
		} 

		[DataMember]
		public VEISReqbenefitClaimInputBCS2 VEISReqbenefitClaimInputBCS2Info
		{
			get;
			set;
		}
         
	}
    [DataContract]
    public class VEISReqbenefitClaimInputBCS2
    {
        [DataMember]
        public string mcs_fileNumber { get; set; }
        [DataMember]
        public string mcs_ssn { get; set; }
        [DataMember]
        public string mcs_benefitClaimType { get; set; }
        [DataMember]
        public string mcs_payee { get; set; }
        [DataMember]
        public string mcs_endProduct { get; set; }
        [DataMember]
        public string mcs_endProductCode { get; set; }
        [DataMember]
        public string mcs_titleName { get; set; }
        [DataMember]
        public string mcs_firstName { get; set; }
        [DataMember]
        public string mcs_middleName { get; set; }
        [DataMember]
        public string mcs_lastName { get; set; }
        [DataMember]
        public string mcs_suffixName { get; set; }
        [DataMember]
        public string mcs_addressLine1 { get; set; }
        [DataMember]
        public string mcs_addressLine2 { get; set; }
        [DataMember]
        public string mcs_addressLine3 { get; set; }
        [DataMember]
        public string mcs_city { get; set; }
        [DataMember]
        public string mcs_state { get; set; }
        [DataMember]
        public string mcs_postalCode { get; set; }
        [DataMember]
        public string mcs_postalCodePlus4 { get; set; }
        [DataMember]
        public string mcs_country { get; set; }
        [DataMember]
        public string mcs_dayTimeAreaCode { get; set; }
        [DataMember]
        public string mcs_dayTimePhoneNumber { get; set; }
        [DataMember]
        public string mcs_nightTimeAreaCode { get; set; }
        [DataMember]
        public string mcs_nightTimePhoneNumber { get; set; }
        [DataMember]
        public string mcs_emailAddress { get; set; }
        [DataMember]
        public string mcs_dateOfClaim { get; set; }
        [DataMember]
        public string mcs_disposition { get; set; }
        [DataMember]
        public string mcs_suspenseDate { get; set; }
        [DataMember]
        public string mcs_sectionUnitNo { get; set; }
        [DataMember]
        public string mcs_folderWithClaim { get; set; }
        [DataMember]
        public string mcs_futureReason { get; set; }
        [DataMember]
        public string mcs_claimantSsn { get; set; }
        [DataMember]
        public string mcs_beneficiaryDateOfBirth { get; set; }
        [DataMember]
        public string mcs_powerOfAttorney { get; set; }
        [DataMember]
        public string mcs_gulfWarRegistryPermit { get; set; }
        [DataMember]
        public string mcs_suppressAcknowledgementLetter { get; set; }
        [DataMember]
        public string mcs_specialIssueCase { get; set; }
        [DataMember]
        public string mcs_inTakeSite { get; set; }
        [DataMember]
        public string mcs_endProductName { get; set; }
        [DataMember]
        public string mcs_soj { get; set; }
        [DataMember]
        public string mcs_mltyPostalTypeCd { get; set; }
        [DataMember]
        public string mcs_mltyPostOfficeTypeCd { get; set; }
        [DataMember]
        public string mcs_foreignMailCode { get; set; }
        [DataMember]
        public string mcs_addressType { get; set; }
        [DataMember]
        public string mcs_preDischargeIndicator { get; set; }
        [DataMember]
        public string mcs_bypassIndicator { get; set; }
        [DataMember]
        public string mcs_bnftClaimId { get; set; }
        [DataMember]
        public string mcs_allowPoaAccess { get; set; }
        [DataMember]
        public string mcs_allowPoaCadd { get; set; }
        [DataMember]
        public string mcs_submtrRoleTypeCd { get; set; }
        [DataMember]
        public string mcs_submtrApplcnTypeCd { get; set; }
        [DataMember]
        public string mcs_awardSoj { get; set; }
        [DataMember]
        public string mcs_powNumberOfDays { get; set; }
        [DataMember]
        public string mcs_homelessIndicator { get; set; }
        [DataMember]
        public string mcs_ptcpntIdClaimant { get; set; }
        [DataMember]
        public string mcs_preDschrgTypeCd { get; set; }
        [DataMember]
        public string mcs_personOrgInd { get; set; }
        [DataMember]
        public string mcs_payeeOrgName { get; set; }
        [DataMember]
        public string mcs_payeeOrgType { get; set; }
        [DataMember]
        public string mcs_payeeOrgTitle { get; set; }
        [DataMember]
        public string mcs_groupOneValidatedInd { get; set; }
        [DataMember]
        public string mcs_prvncNm { get; set; }
        [DataMember]
        public string mcs_trtryNm { get; set; }
    }

}
