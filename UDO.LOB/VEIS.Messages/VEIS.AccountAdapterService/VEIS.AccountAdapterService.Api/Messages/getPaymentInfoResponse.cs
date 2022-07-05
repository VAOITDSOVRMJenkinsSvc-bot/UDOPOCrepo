using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.AccountAdapterService.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.AccountAdapterService,getPaymentInfo method, Response.
	/// Code Generated by IMS on: 1/11/2019 10:16:24 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISgpinfogetPaymentInfoResponse : VEISEcResponseBase
	{
		[DataMember]
		public VEISgpinfoPayment_v1MultipleResponse[] VEISgpinfoPayment_v1Info { get; set; }
	}
	[DataContract]
	public class VEISgpinfoPayment_v1MultipleResponse
	{
		[DataMember]
		public string mcs_checkNumber { get; set; }
		[DataMember]
		public string mcs_claimNumber { get; set; }
		[DataMember]
		public string mcs_vendorAmount { get; set; }
		[DataMember]
		public Decimal mcs_beneficiaryAmount { get; set; }
		[DataMember]
		public bool mcs_beneficiaryAmountSpecified { get; set; }
		[DataMember]
		public DateTime mcs_submitDate { get; set; }
		[DataMember]
		public bool mcs_submitDateSpecified { get; set; }
		[DataMember]
		public string mcs_fmsDocumentID { get; set; }
		[DataMember]
		public VEISgpinfostatusMultipleResponse[] VEISgpinfostatusInfo { get; set; }
		[DataMember]
		public VEISgpinfoproviderIdentifier VEISgpinfoproviderIdentifierInfo { get; set; }
		[DataMember]
		public VEISgpinfoclaimCheck_v1 VEISgpinfoclaimCheck_v1Info { get; set; }
	}
	[DataContract]
	public class VEISgpinfostatusMultipleResponse
	{
		[DataMember]
		public string mcs_code { get; set; }
		[DataMember]
		public string mcs_description { get; set; }
	}
	[DataContract]
	public class VEISgpinfoproviderIdentifier
	{
		[DataMember]
		public string mcs_ien { get; set; }
		[DataMember]
		public string mcs_ein { get; set; }
	}
	[DataContract]
	public class VEISgpinfoclaimCheck_v1
	{
		[DataMember]
		public String[] mcs_pdiNumbers { get; set; }
		[DataMember]
		public string mcs_providerName { get; set; }
		[DataMember]
		public string mcs_claimNumber { get; set; }
		[DataMember]
		public VEISgpinfoserviceType VEISgpinfoserviceTypeInfo { get; set; }
		[DataMember]
		public VEISgpinfoproviderIdentifier1 VEISgpinfoproviderIdentifier1Info { get; set; }
		[DataMember]
		public VEISgpinfoaddressesMultipleResponse[] VEISgpinfoaddressesInfo { get; set; }
		[DataMember]
		public VEISgpinfobeneficiaryCheck VEISgpinfobeneficiaryCheckInfo { get; set; }
		[DataMember]
		public VEISgpinfovendorCheck VEISgpinfovendorCheckInfo { get; set; }
	}
	[DataContract]
	public class VEISgpinfoserviceType
	{
		[DataMember]
		public string mcs_code { get; set; }
		[DataMember]
		public string mcs_description { get; set; }
	}
	[DataContract]
	public class VEISgpinfoproviderIdentifier1
	{
		[DataMember]
		public string mcs_ien { get; set; }
		[DataMember]
		public string mcs_ein { get; set; }
	}
	[DataContract]
	public class VEISgpinfoaddressesMultipleResponse
	{
		[DataMember]
		public string mcs_type { get; set; }
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
	}
	[DataContract]
	public class VEISgpinfobeneficiaryCheck
	{
		[DataMember]
		public DateTime mcs_completedDate { get; set; }
		[DataMember]
		public string mcs_fmsDocumentID { get; set; }
		[DataMember]
		public string mcs_checkNumber { get; set; }
		[DataMember]
		public DateTime mcs_checkDate { get; set; }
		[DataMember]
		public string mcs_amount { get; set; }
	}
	[DataContract]
	public class VEISgpinfovendorCheck
	{
		[DataMember]
		public DateTime mcs_completedDate { get; set; }
		[DataMember]
		public string mcs_fmsDocumentID { get; set; }
		[DataMember]
		public string mcs_checkNumber { get; set; }
		[DataMember]
		public DateTime mcs_checkDate { get; set; }
		[DataMember]
		public string mcs_amount { get; set; }
	}
}
