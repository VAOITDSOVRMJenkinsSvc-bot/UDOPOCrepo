using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.FMCService.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.FMCService,RequestPayment method, Response.
	/// Code Generated by IMS on: 1/7/2019 8:09:24 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISFMCRpRequestPaymentResponse : VEISEcResponseBase
	{
		[DataMember]
		public VEISFMCRpRequestTransactionResponse VEISFMCRpRequestTransactionResponseInfo { get; set; }
	}
	[DataContract]
	public class VEISFMCRpRequestTransactionResponse
	{
		[DataMember]
		public Int32 mcs_TransactionId { get; set; }
		[DataMember]
		public bool mcs_TransactionIdSpecified { get; set; }
		[DataMember]
		public bool mcs_TransactionResultCodeSpecified { get; set; }
		[DataMember]
		public VEISFMCRpErrorInfoListMultipleResponse[] VEISFMCRpErrorInfoListInfo { get; set; }
		[DataMember]
		public VEISFMCRpTransactionResultCode VEISFMCRpTransactionResultCodeInfo { get; set; }
	}
	[DataContract]
	public class VEISFMCRpErrorInfoListMultipleResponse
	{
		[DataMember]
		public string mcs_Description { get; set; }
	}
	[DataContract]
	public enum VEISFMCRpTransactionResultCode
	{
		Succeeded,
		TransactionSubmissionFailed,
		TransactionSubmissionFailedDuplicate,
	}
}
