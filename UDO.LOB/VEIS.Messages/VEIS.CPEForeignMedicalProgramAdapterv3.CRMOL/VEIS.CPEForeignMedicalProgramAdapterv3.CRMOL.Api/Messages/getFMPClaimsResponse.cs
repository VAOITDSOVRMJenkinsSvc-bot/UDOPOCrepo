using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.CpeForeignMedicalProgramAdapterServiceV3.CRMOL.Messages
{ 
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.CpeForeignMedicalProgramAdapterServiceV3.CRMOL,getFMPClaims method, Response.
	/// Code Generated by IMS on: 6/1/2018 11:11:04 AM
	/// Version: 2018.05.09.05
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISgFMPClgetFMPClaimsResponse : VEISEcResponseBase
	{
		[DataMember]
		public VEISgFMPClCPEForeignMedicalProgramAdapterv3getFMPClaimsResponse VEISgFMPClCPEForeignMedicalProgramAdapterv3getFMPClaimsResponseInfo { get; set; }
		[DataMember]
		public bool ExceptionOccured { get; set; } 
	}
	[DataContract]
	public class VEISgFMPClCPEForeignMedicalProgramAdapterv3getFMPClaimsResponse
	{
		[DataMember]
		public VEISgFMPClcode VEISgFMPClcodeInfo { get; set; }
		[DataMember]
		public VEISgFMPClfmpClaimsMultipleResponse[] VEISgFMPClfmpClaimsInfo { get; set; }
		[DataMember]
		public VEISgFMPClfault VEISgFMPClfaultInfo { get; set; }
	}
	[DataContract]
	public enum VEISgFMPClcode
	{
		SUCCESS,
		ERROR,
	}
	[DataContract]
	public class VEISgFMPClfmpClaimsMultipleResponse
	{
		[DataMember]
		public string mcs_claimNumber { get; set; }
		[DataMember]
		public string mcs_providerName { get; set; }
		[DataMember]
		public DateTime mcs_dateOfService { get; set; }
		[DataMember]
		public bool mcs_dateOfServiceSpecified { get; set; }
		[DataMember]
		public string mcs_amountPaid { get; set; }
		[DataMember]
		public string mcs_amountBilled { get; set; }
		[DataMember]
		public string mcs_fmpBeneficiaryCheckNumber { get; set; }
		[DataMember]
		public string mcs_fmpBeneficiaryCheckDate { get; set; }
		[DataMember]
		public string mcs_fmpBeneficiaryCheckAmount { get; set; }
		[DataMember]
		public string mcs_vendorCheckNumber { get; set; }
		[DataMember]
		public string mcs_vendorCheckDate { get; set; }
		[DataMember]
		public string mcs_vendorCheckAmount { get; set; }
		[DataMember]
		public VEISgFMPClclaimStatus VEISgFMPClclaimStatusInfo { get; set; }
		[DataMember]
		public VEISgFMPCltypeOfService VEISgFMPCltypeOfServiceInfo { get; set; }
	}
	[DataContract]
	public class VEISgFMPClclaimStatus
	{
		[DataMember]
		public string mcs_code { get; set; }
		[DataMember]
		public string mcs_description { get; set; }
	}
	[DataContract]
	public class VEISgFMPCltypeOfService
	{
		[DataMember]
		public string mcs_code { get; set; }
		[DataMember]
		public string mcs_description { get; set; }
	}
	[DataContract]
	public class VEISgFMPClfault
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
		public VEISgFMPClessCode VEISgFMPClessCodeInfo { get; set; }
		[DataMember]
		public VEISgFMPClnestedFault VEISgFMPClnestedFaultInfo { get; set; }
	}
	[DataContract]
	public enum VEISgFMPClessCode
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
	public class VEISgFMPClnestedFault
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
		public VEISgFMPClessCode1 VEISgFMPClessCode1Info { get; set; }
	}
	[DataContract]
	public enum VEISgFMPClessCode1
	{
		SUCCESS,
		ERROR,
		WARNING,
		REFUSED,
		ACCEPT,
		REJECT,
		FAIL,
	}
}
