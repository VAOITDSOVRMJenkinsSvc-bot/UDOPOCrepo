using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.BeneficiaryAdapterService.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.BeneficiaryAdapterService,getHouseholdData method, Response.
	/// Code Generated by IMS on: 1/11/2019 8:29:47 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISgCPEHDatgetHouseholdDataResponse : VEISEcResponseBase
	{
		[DataMember]
		public VEISgCPEHDatGetHouseholdDataResponseMessage_v1 VEISgCPEHDatGetHouseholdDataResponseMessage_v1Info { get; set; }
	}
	[DataContract]
	public class VEISgCPEHDatGetHouseholdDataResponseMessage_v1
	{
		[DataMember]
		public VEISgCPEHDathouseholdDataMultipleResponse[] VEISgCPEHDathouseholdDataInfo { get; set; }
		[DataMember]
		public VEISgCPEHDatfault VEISgCPEHDatfaultInfo { get; set; }
	}
	[DataContract]
	public class VEISgCPEHDathouseholdDataMultipleResponse
	{
		[DataMember]
		public string mcs_dfn { get; set; }
		[DataMember]
		public string mcs_ssn { get; set; }
		[DataMember]
		public string mcs_fileNumber { get; set; }
		[DataMember]
		public string mcs_gender { get; set; }
		[DataMember]
		public DateTime mcs_dateOfBirth { get; set; }
		[DataMember]
		public bool mcs_dateOfBirthSpecified { get; set; }
		[DataMember]
		public string mcs_verified { get; set; }
		[DataMember]
		public VEISgCPEHDatname VEISgCPEHDatnameInfo { get; set; }
		[DataMember]
		public VEISgCPEHDataddress VEISgCPEHDataddressInfo { get; set; }
		[DataMember]
		public VEISgCPEHDatchampVAStatus VEISgCPEHDatchampVAStatusInfo { get; set; }
		[DataMember]
		public VEISgCPEHDatbeneficiaryMultipleResponse[] VEISgCPEHDatbeneficiaryInfo { get; set; }
	}
	[DataContract]
	public class VEISgCPEHDatname
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
	public class VEISgCPEHDataddress
	{
		[DataMember]
		public bool mcs_typeSpecified { get; set; }
		[DataMember]
		public string mcs_addressLine1 { get; set; }
		[DataMember]
		public string mcs_addressLine2 { get; set; }
		[DataMember]
		public string mcs_addressCityName { get; set; }
		[DataMember]
		public string mcs_addressUSStateCode { get; set; }
		[DataMember]
		public string mcs_addressZip5PostalCode { get; set; }
		[DataMember]
		public string mcs_country { get; set; }
		[DataMember]
		public bool mcs_foreignAddressFlag { get; set; }
		[DataMember]
		public bool mcs_foreignAddressFlagSpecified { get; set; }
		[DataMember]
		public VEISgCPEHDattype VEISgCPEHDattypeInfo { get; set; }
	}
	[DataContract]
	public enum VEISgCPEHDattype
	{
		SponsorRemitto,
		SponsorCorrespondence,
		BeneficiaryRemitto,
		BeneficiaryCorrespondence,
		ProviderRemitto,
		ProviderBilling,
		ProviderProvider,
		ContactContact,
	}
	[DataContract]
	public class VEISgCPEHDatchampVAStatus
	{
		[DataMember]
		public string mcs_code { get; set; }
		[DataMember]
		public string mcs_description { get; set; }
	}
	[DataContract]
	public class VEISgCPEHDatbeneficiaryMultipleResponse
	{
		[DataMember]
		public string mcs_dfn { get; set; }
		[DataMember]
		public string mcs_bfn { get; set; }
		[DataMember]
		public string mcs_ssn { get; set; }
		[DataMember]
		public string mcs_gender { get; set; }
		[DataMember]
		public DateTime mcs_dateOfBirth { get; set; }
		[DataMember]
		public bool mcs_dateOfBirthSpecified { get; set; }
		[DataMember]
		public string mcs_isSponsor { get; set; }
		[DataMember]
		public DateTime mcs_champVAStatusDate { get; set; }
		[DataMember]
		public bool mcs_champVAStatusDateSpecified { get; set; }
		[DataMember]
		public string mcs_commentFlag { get; set; }
		[DataMember]
		public string mcs_rocFlag { get; set; }
		[DataMember]
		public VEISgCPEHDatname1 VEISgCPEHDatname1Info { get; set; }
		[DataMember]
		public VEISgCPEHDataddress1 VEISgCPEHDataddress1Info { get; set; }
		[DataMember]
		public VEISgCPEHDatrelationToSponsor VEISgCPEHDatrelationToSponsorInfo { get; set; }
		[DataMember]
		public VEISgCPEHDatchampVAStatus1 VEISgCPEHDatchampVAStatus1Info { get; set; }
		[DataMember]
		public VEISgCPEHDatchampVAStatusReason VEISgCPEHDatchampVAStatusReasonInfo { get; set; }
	}
	[DataContract]
	public class VEISgCPEHDatname1
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
	public class VEISgCPEHDataddress1
	{
		[DataMember]
		public bool mcs_typeSpecified { get; set; }
		[DataMember]
		public string mcs_addressLine1 { get; set; }
		[DataMember]
		public string mcs_addressLine2 { get; set; }
		[DataMember]
		public string mcs_addressCityName { get; set; }
		[DataMember]
		public string mcs_addressUSStateCode { get; set; }
		[DataMember]
		public string mcs_addressZip5PostalCode { get; set; }
		[DataMember]
		public string mcs_country { get; set; }
		[DataMember]
		public bool mcs_foreignAddressFlag { get; set; }
		[DataMember]
		public bool mcs_foreignAddressFlagSpecified { get; set; }
		[DataMember]
		public VEISgCPEHDattype1 VEISgCPEHDattype1Info { get; set; }
	}
	[DataContract]
	public enum VEISgCPEHDattype1
	{
		SponsorRemitto,
		SponsorCorrespondence,
		BeneficiaryRemitto,
		BeneficiaryCorrespondence,
		ProviderRemitto,
		ProviderBilling,
		ProviderProvider,
		ContactContact,
	}
	[DataContract]
	public class VEISgCPEHDatrelationToSponsor
	{
		[DataMember]
		public string mcs_code { get; set; }
		[DataMember]
		public string mcs_description { get; set; }
	}
	[DataContract]
	public class VEISgCPEHDatchampVAStatus1
	{
		[DataMember]
		public string mcs_code { get; set; }
		[DataMember]
		public string mcs_description { get; set; }
	}
	[DataContract]
	public class VEISgCPEHDatchampVAStatusReason
	{
		[DataMember]
		public string mcs_code { get; set; }
		[DataMember]
		public string mcs_description { get; set; }
	}
	[DataContract]
	public class VEISgCPEHDatfault
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
		public VEISgCPEHDatessCode VEISgCPEHDatessCodeInfo { get; set; }
		[DataMember]
		public VEISgCPEHDatnestedFault VEISgCPEHDatnestedFaultInfo { get; set; }
	}
	[DataContract]
	public enum VEISgCPEHDatessCode
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
	public class VEISgCPEHDatnestedFault
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
		public VEISgCPEHDatessCode1 VEISgCPEHDatessCode1Info { get; set; }
	}
	[DataContract]
	public enum VEISgCPEHDatessCode1
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