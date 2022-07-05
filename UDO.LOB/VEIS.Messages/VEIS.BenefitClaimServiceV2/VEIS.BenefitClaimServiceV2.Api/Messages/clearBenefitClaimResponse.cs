using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.BenefitClaimServiceV2.Api.Messages
{ 
	[DataContract] 
	public class VEISclearBenefitClaimResponse : VEISEcResponseBase
    { 

		[DataMember]
		public bool ExceptionOccured
		{
			get;
			set;
		}

		[DataMember]
		public VEISbenefitClaimRecordBCS2 VEISbenefitClaimRecordBCS2Info
		{
			get;
			set;
		}

    }
    [DataContract]
    public class VEISbenefitClaimRecordBCS2
    {
        [DataMember]
        public string mcs_fiduciaryInd { get; set; }
        [DataMember]
        public string mcs_gulfWarRegistryPermit { get; set; }
        [DataMember]
        public string mcs_homelessIndicator { get; set; }
        [DataMember]
        public string mcs_powNumberOfDays { get; set; }
        [DataMember]
        public string mcs_returnCode { get; set; }
        [DataMember]
        public string mcs_returnMessage { get; set; }
        [DataMember]
        public VEISbenefitClaimRecord1BCS2 VEISbenefitClaimRecord1BCS2Info { get; set; }
        [DataMember]
        public VEISlifeCycleRecordBCS2 VEISlifeCycleRecordBCS2Info { get; set; }
        [DataMember]
        public VEISparticipantRecordBCS2 VEISparticipantRecordBCS2Info { get; set; }
        [DataMember]
        public VEISsuspenceRecordBCS2 VEISsuspenceRecordBCS2Info { get; set; }
    }
    [DataContract]
    public class VEISsuspenceRecordBCS2
    {
        [DataMember]
        public string mcs_numberOfRecords { get; set; }
        [DataMember]
        public string mcs_returnCode { get; set; }
        [DataMember]
        public string mcs_returnMessage { get; set; }
        [DataMember]
        public VEISsuspenceRecordsBCS2MultipleResponse[] VEISsuspenceRecordsBCS2Info { get; set; }
    }
    [DataContract]
    public class VEISsuspenceRecordsBCS2MultipleResponse
    {
        [DataMember]
        public string mcs_claimSuspenceDate { get; set; }
        [DataMember]
        public string mcs_firstName { get; set; }
        [DataMember]
        public string mcs_journalDate { get; set; }
        [DataMember]
        public string mcs_journalObjectID { get; set; }
        [DataMember]
        public string mcs_journalStation { get; set; }
        [DataMember]
        public string mcs_journalStatusTypeCode { get; set; }
        [DataMember]
        public string mcs_journalUserID { get; set; }
        [DataMember]
        public string mcs_lastName { get; set; }
        [DataMember]
        public string mcs_middleName { get; set; }
        [DataMember]
        public string mcs_suffix { get; set; }
        [DataMember]
        public string mcs_suspenceActionDate { get; set; }
        [DataMember]
        public string mcs_suspenceCode { get; set; }
        [DataMember]
        public string mcs_suspenceReasonText { get; set; }
    }
    [DataContract]
    public class VEISbenefitClaimRecord1BCS2
    {
        [DataMember]
        public string mcs_bddSiteName { get; set; }
        [DataMember]
        public string mcs_benefitClaimID { get; set; }
        [DataMember]
        public string mcs_benefitClaimReturnLabel { get; set; }
        [DataMember]
        public string mcs_claimPriorityIndicator { get; set; }
        [DataMember]
        public string mcs_claimReceiveDate { get; set; }
        [DataMember]
        public string mcs_claimStationOfJurisdiction { get; set; }
        [DataMember]
        public string mcs_claimTemporaryStationOfJurisdiction { get; set; }
        [DataMember]
        public string mcs_claimTypeCode { get; set; }
        [DataMember]
        public string mcs_claimTypeName { get; set; }
        [DataMember]
        public string mcs_claimantFirstName { get; set; }
        [DataMember]
        public string mcs_claimantLastName { get; set; }
        [DataMember]
        public string mcs_claimantMiddleName { get; set; }
        [DataMember]
        public string mcs_claimantPersonOrOrganizationIndicator { get; set; }
        [DataMember]
        public string mcs_claimantSuffix { get; set; }
        [DataMember]
        public string mcs_cpBenefitClaimID { get; set; }
        [DataMember]
        public string mcs_cpClaimID { get; set; }
        [DataMember]
        public string mcs_cpClaimReturnLabel { get; set; }
        [DataMember]
        public string mcs_cpLocationID { get; set; }
        [DataMember]
        public string mcs_directDepositAccountID { get; set; }
        [DataMember]
        public string mcs_endProductTypeCode { get; set; }
        [DataMember]
        public string mcs_informalIndicator { get; set; }
        [DataMember]
        public string mcs_journalDate { get; set; }
        [DataMember]
        public string mcs_journalObjectID { get; set; }
        [DataMember]
        public string mcs_journalStation { get; set; }
        [DataMember]
        public string mcs_journalStatusTypeCode { get; set; }
        [DataMember]
        public string mcs_journalUserId { get; set; }
        [DataMember]
        public string mcs_lastPaidDate { get; set; }
        [DataMember]
        public string mcs_mailingAddressID { get; set; }
        [DataMember]
        public string mcs_numberOfBenefitClaimRecords { get; set; }
        [DataMember]
        public string mcs_numberOfCPClaimRecords { get; set; }
        [DataMember]
        public string mcs_organizationName { get; set; }
        [DataMember]
        public string mcs_organizationTitleTypeName { get; set; }
        [DataMember]
        public string mcs_participantClaimantID { get; set; }
        [DataMember]
        public string mcs_participantVetID { get; set; }
        [DataMember]
        public string mcs_payeeTypeCode { get; set; }
        [DataMember]
        public string mcs_paymentAddressID { get; set; }
        [DataMember]
        public string mcs_preDischargeInd { get; set; }
        [DataMember]
        public string mcs_preDschrgTypeCd { get; set; }
        [DataMember]
        public string mcs_programTypeCode { get; set; }
        [DataMember]
        public string mcs_returnCode { get; set; }
        [DataMember]
        public string mcs_returnMessage { get; set; }
        [DataMember]
        public string mcs_serviceTypeCode { get; set; }
        [DataMember]
        public string mcs_statusTypeCode { get; set; }
        [DataMember]
        public string mcs_submtrApplcnTypeCd { get; set; }
        [DataMember]
        public string mcs_submtrRoleTypeCd { get; set; }
        [DataMember]
        public string mcs_vetFirstName { get; set; }
        [DataMember]
        public string mcs_vetLastName { get; set; }
        [DataMember]
        public string mcs_vetMiddleName { get; set; }
        [DataMember]
        public string mcs_vetSuffix { get; set; }
    }
    [DataContract]
    public class VEISlifeCycleRecordBCS2
    {
        [DataMember]
        public string mcs_numberOfRecords { get; set; }
        [DataMember]
        public string mcs_returnCode { get; set; }
        [DataMember]
        public string mcs_returnMessage { get; set; }
        [DataMember]
        public VEISlifeCycleRecordsBCS2MultipleResponse[] VEISlifeCycleRecordsBCS2Info { get; set; }
    }
    [DataContract]
    public class VEISlifeCycleRecordsBCS2MultipleResponse
    {
        [DataMember]
        public string mcs_actionFirstName { get; set; }
        [DataMember]
        public string mcs_actionLastName { get; set; }
        [DataMember]
        public string mcs_actionMiddleName { get; set; }
        [DataMember]
        public string mcs_actionStationNumber { get; set; }
        [DataMember]
        public string mcs_actionSuffix { get; set; }
        [DataMember]
        public string mcs_benefitClaimID { get; set; }
        [DataMember]
        public string mcs_caseAssignmentLocationID { get; set; }
        [DataMember]
        public string mcs_caseAssignmentStatusNumber { get; set; }
        [DataMember]
        public string mcs_caseID { get; set; }
        [DataMember]
        public string mcs_changedDate { get; set; }
        [DataMember]
        public string mcs_closedDate { get; set; }
        [DataMember]
        public string mcs_journalDate { get; set; }
        [DataMember]
        public string mcs_journalObjectID { get; set; }
        [DataMember]
        public string mcs_journalStation { get; set; }
        [DataMember]
        public string mcs_journalStatusTypeCode { get; set; }
        [DataMember]
        public string mcs_journalUserID { get; set; }
        [DataMember]
        public string mcs_lifeCycleStatusID { get; set; }
        [DataMember]
        public string mcs_lifeCycleStatusTypeName { get; set; }
        [DataMember]
        public string mcs_reasonText { get; set; }
        [DataMember]
        public string mcs_stationofJurisdiction { get; set; }
        [DataMember]
        public string mcs_statusReasonTypeCode { get; set; }
        [DataMember]
        public string mcs_statusReasonTypeName { get; set; }
    }
    [DataContract]
    public class VEISparticipantRecordBCS2
    {
        [DataMember]
        public string mcs_bddSiteName { get; set; }
        [DataMember]
        public string mcs_benefitClaimID { get; set; }
        [DataMember]
        public string mcs_benefitClaimReturnLabel { get; set; }
        [DataMember]
        public string mcs_claimPriorityIndicator { get; set; }
        [DataMember]
        public string mcs_claimReceiveDate { get; set; }
        [DataMember]
        public string mcs_claimStationOfJurisdiction { get; set; }
        [DataMember]
        public string mcs_claimTemporaryStationOfJurisdiction { get; set; }
        [DataMember]
        public string mcs_claimTypeCode { get; set; }
        [DataMember]
        public string mcs_claimTypeName { get; set; }
        [DataMember]
        public string mcs_claimantFirstName { get; set; }
        [DataMember]
        public string mcs_claimantLastName { get; set; }
        [DataMember]
        public string mcs_claimantMiddleName { get; set; }
        [DataMember]
        public string mcs_claimantPersonOrOrganizationIndicator { get; set; }
        [DataMember]
        public string mcs_claimantSuffix { get; set; }
        [DataMember]
        public string mcs_cpBenefitClaimID { get; set; }
        [DataMember]
        public string mcs_cpClaimID { get; set; }
        [DataMember]
        public string mcs_cpClaimReturnLabel { get; set; }
        [DataMember]
        public string mcs_cpLocationID { get; set; }
        [DataMember]
        public string mcs_directDepositAccountID { get; set; }
        [DataMember]
        public string mcs_endProductTypeCode { get; set; }
        [DataMember]
        public string mcs_informalIndicator { get; set; }
        [DataMember]
        public string mcs_journalDate { get; set; }
        [DataMember]
        public string mcs_journalObjectID { get; set; }
        [DataMember]
        public string mcs_journalStation { get; set; }
        [DataMember]
        public string mcs_journalStatusTypeCode { get; set; }
        [DataMember]
        public string mcs_journalUserId { get; set; }
        [DataMember]
        public string mcs_lastPaidDate { get; set; }
        [DataMember]
        public string mcs_mailingAddressID { get; set; }
        [DataMember]
        public string mcs_numberOfBenefitClaimRecords { get; set; }
        [DataMember]
        public string mcs_numberOfCPClaimRecords { get; set; }
        [DataMember]
        public string mcs_numberOfRecords { get; set; }
        [DataMember]
        public string mcs_organizationName { get; set; }
        [DataMember]
        public string mcs_organizationTitleTypeName { get; set; }
        [DataMember]
        public string mcs_participantClaimantID { get; set; }
        [DataMember]
        public string mcs_participantVetID { get; set; }
        [DataMember]
        public string mcs_payeeTypeCode { get; set; }
        [DataMember]
        public string mcs_paymentAddressID { get; set; }
        [DataMember]
        public string mcs_programTypeCode { get; set; }
        [DataMember]
        public string mcs_returnCode { get; set; }
        [DataMember]
        public string mcs_returnMessage { get; set; }
        [DataMember]
        public string mcs_serviceTypeCode { get; set; }
        [DataMember]
        public string mcs_statusTypeCode { get; set; }
        [DataMember]
        public string mcs_vetFirstName { get; set; }
        [DataMember]
        public string mcs_vetLastName { get; set; }
        [DataMember]
        public string mcs_vetMiddleName { get; set; }
        [DataMember]
        public string mcs_vetSuffix { get; set; }
        [DataMember]
        public VEISselectionBCS2MultipleResponse[] VEISselectionBCS2Info { get; set; }
    }
    public class VEISselectionBCS2MultipleResponse
    {
        [DataMember]
        public string mcs_benefitClaimID { get; set; }
        [DataMember]
        public string mcs_claimReceiveDate { get; set; }
        [DataMember]
        public string mcs_claimTypeCode { get; set; }
        [DataMember]
        public string mcs_claimTypeName { get; set; }
        [DataMember]
        public string mcs_claimantFirstName { get; set; }
        [DataMember]
        public string mcs_claimantLastName { get; set; }
        [DataMember]
        public string mcs_claimantMiddleName { get; set; }
        [DataMember]
        public string mcs_claimantSuffix { get; set; }
        [DataMember]
        public string mcs_endProductTypeCode { get; set; }
        [DataMember]
        public string mcs_lastActionDate { get; set; }
        [DataMember]
        public string mcs_organizationName { get; set; }
        [DataMember]
        public string mcs_organizationTitleTypeName { get; set; }
        [DataMember]
        public string mcs_payeeTypeCode { get; set; }
        [DataMember]
        public string mcs_personOrOrganizationIndicator { get; set; }
        [DataMember]
        public string mcs_programTypeCode { get; set; }
        [DataMember]
        public string mcs_statusTypeCode { get; set; }
    }

}
