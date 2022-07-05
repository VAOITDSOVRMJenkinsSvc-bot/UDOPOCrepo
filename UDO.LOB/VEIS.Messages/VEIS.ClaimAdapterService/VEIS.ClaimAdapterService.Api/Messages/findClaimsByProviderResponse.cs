using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.ClaimAdapterService.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.ClaimAdapterService,findClaimsByProvider method, Response.
	/// Code Generated by IMS on: 1/11/2019 8:43:52 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISfclmbprovfindClaimsByProviderResponse : VEISEcResponseBase
	{
		[DataMember]
		public VEISfclmbprovCPEClaimAdapterv2findClaimsByProviderResponse1 VEISfclmbprovCPEClaimAdapterv2findClaimsByProviderResponse1Info { get; set; }
	}
	[DataContract]
	public class VEISfclmbprovCPEClaimAdapterv2findClaimsByProviderResponse1
	{
		[DataMember]
		public VEISfclmbprovclaimsMultipleResponse[] VEISfclmbprovclaimsInfo { get; set; }
		[DataMember]
		public VEISfclmbprovfault VEISfclmbprovfaultInfo { get; set; }
		[DataMember]
		public VEISfclmbprovresponseStats VEISfclmbprovresponseStatsInfo { get; set; }
	}
	[DataContract]
	public class VEISfclmbprovclaimsMultipleResponse
	{
		[DataMember]
		public string mcs_claimNumber { get; set; }
		[DataMember]
		public String[] mcs_pdiNumbers { get; set; }
		[DataMember]
		public DateTime mcs_dateOfService { get; set; }
		[DataMember]
		public bool mcs_dateOfServiceSpecified { get; set; }
		[DataMember]
		public string mcs_providerName { get; set; }
		[DataMember]
		public string mcs_billCharged { get; set; }
		[DataMember]
		public string mcs_allowedAmount { get; set; }
		[DataMember]
		public string mcs_primaryOHI { get; set; }
		[DataMember]
		public string mcs_secondaryOHI { get; set; }
		[DataMember]
		public string mcs_champVAPayment { get; set; }
		[DataMember]
		public string mcs_deductibleAmount { get; set; }
		[DataMember]
		public string mcs_costShare { get; set; }
		[DataMember]
		public string mcs_assignBenefitsFlag { get; set; }
		[DataMember]
		public VEISfclmbprovclaimStatus VEISfclmbprovclaimStatusInfo { get; set; }
		[DataMember]
		public VEISfclmbprovplaceOfService VEISfclmbprovplaceOfServiceInfo { get; set; }
		[DataMember]
		public VEISfclmbprovrejectReasonsMultipleResponse[] VEISfclmbprovrejectReasonsInfo { get; set; }
		[DataMember]
		public VEISfclmbprovclaimCommentMultipleResponse[] VEISfclmbprovclaimCommentInfo { get; set; }
	}
	[DataContract]
	public class VEISfclmbprovclaimStatus
	{
		[DataMember]
		public string mcs_code { get; set; }
		[DataMember]
		public string mcs_description { get; set; }
	}
	[DataContract]
	public class VEISfclmbprovplaceOfService
	{
		[DataMember]
		public string mcs_code { get; set; }
		[DataMember]
		public string mcs_description { get; set; }
	}
	[DataContract]
	public class VEISfclmbprovrejectReasonsMultipleResponse
	{
		[DataMember]
		public VEISfclmbprovrejectReason VEISfclmbprovrejectReasonInfo { get; set; }
	}
	[DataContract]
	public class VEISfclmbprovrejectReason
	{
		[DataMember]
		public string mcs_code { get; set; }
		[DataMember]
		public string mcs_description { get; set; }
	}
	[DataContract]
	public class VEISfclmbprovclaimCommentMultipleResponse
	{
		[DataMember]
		public DateTime mcs_dateEntered { get; set; }
		[DataMember]
		public bool mcs_dateEnteredSpecified { get; set; }
		[DataMember]
		public string mcs_source { get; set; }
		[DataMember]
		public string mcs_comment { get; set; }
		[DataMember]
		public VEISfclmbproventeredBy VEISfclmbproventeredByInfo { get; set; }
	}
	[DataContract]
	public class VEISfclmbproventeredBy
	{
		[DataMember]
		public string mcs_prefix { get; set; }
		[DataMember]
		public string mcs_firstName { get; set; }
		[DataMember]
		public string mcs_middleName { get; set; }
		[DataMember]
		public string mcs_lastName { get; set; }
		[DataMember]
		public string mcs_suffix { get; set; }
	}
	[DataContract]
	public class VEISfclmbprovfault
	{
		[DataMember]
		public bool mcs_essCodeSpecified { get; set; }
		[DataMember]
		public string mcs_essText { get; set; }
		[DataMember]
		public string mcs_code { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public VEISfclmbprovessCode VEISfclmbprovessCodeInfo { get; set; }
		[DataMember]
		public VEISfclmbprovnestedFault VEISfclmbprovnestedFaultInfo { get; set; }
	}
	[DataContract]
	public enum VEISfclmbprovessCode
	{
		SUCCESS,
		ERROR,
		WARNING,
		REFUSED,
		ACCEPT,
		REJECT,
		FAIL,
	}
	[DataContract]
	public class VEISfclmbprovnestedFault
	{
		[DataMember]
		public bool mcs_essCodeSpecified { get; set; }
		[DataMember]
		public string mcs_essText { get; set; }
		[DataMember]
		public string mcs_code { get; set; }
		[DataMember]
		public string mcs_text { get; set; }
		[DataMember]
		public VEISfclmbprovessCode1 VEISfclmbprovessCode1Info { get; set; }
	}
	[DataContract]
	public enum VEISfclmbprovessCode1
	{
		SUCCESS,
		ERROR,
		WARNING,
		REFUSED,
		ACCEPT,
		REJECT,
		FAIL,
	}
	[DataContract]
	public class VEISfclmbprovresponseStats
	{
		[DataMember]
		public Int32 mcs_recordsReturned { get; set; }
		[DataMember]
		public bool mcs_recordsReturnedSpecified { get; set; }
		[DataMember]
		public bool mcs_moreChunkRecords { get; set; }
		[DataMember]
		public bool mcs_moreChunkRecordsSpecified { get; set; }
		[DataMember]
		public bool mcs_moreDateRecords { get; set; }
		[DataMember]
		public bool mcs_moreDateRecordsSpecified { get; set; }
		[DataMember]
		public Int32 mcs_totalRecords { get; set; }
		[DataMember]
		public bool mcs_totalRecordsSpecified { get; set; }
	}
}
