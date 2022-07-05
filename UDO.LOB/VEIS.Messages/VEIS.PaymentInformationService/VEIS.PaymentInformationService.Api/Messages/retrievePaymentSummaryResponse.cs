using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.PaymentInformationService.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.PaymentInformationService,retrievePaymentSummary method, Response.
	/// Code Generated by IMS on: 12/27/2018 4:58:32 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISrtrpmtsumretrievePaymentSummaryResponse : VEISEcResponseBase
	{
		[DataMember]
		public VEISrtrpmtsumpaymentSummaryResponseVO VEISrtrpmtsumpaymentSummaryResponseVOInfo { get; set; }
	}
	[DataContract]
	public class VEISrtrpmtsumpaymentSummaryResponseVO
	{
		[DataMember]
		public VEISrtrpmtsumpaymentsMultipleResponse[] VEISrtrpmtsumpaymentsInfo { get; set; }
		[DataMember]
		public VEISrtrpmtsumresponse VEISrtrpmtsumresponseInfo { get; set; }
	}
	[DataContract]
	public class VEISrtrpmtsumpaymentsMultipleResponse
	{
		[DataMember]
		public DateTime mcs_authorizedDate { get; set; }
		[DataMember]
		public bool mcs_authorizedDateSpecified { get; set; }
		[DataMember]
		public string mcs_bdnRecordType { get; set; }
		[DataMember]
		public string mcs_beneficiaryName { get; set; }
		[DataMember]
		public Int64 mcs_beneficiaryParticipantID { get; set; }
		[DataMember]
		public bool mcs_beneficiaryParticipantIDSpecified { get; set; }
		[DataMember]
		public string mcs_fileNumber { get; set; }
		[DataMember]
		public string mcs_payeeType { get; set; }
		[DataMember]
		public Decimal mcs_paymentAmount { get; set; }
		[DataMember]
		public bool mcs_paymentAmountSpecified { get; set; }
		[DataMember]
		public DateTime mcs_paymentDate { get; set; }
		[DataMember]
		public bool mcs_paymentDateSpecified { get; set; }
		[DataMember]
		public string mcs_paymentStatus { get; set; }
		[DataMember]
		public string mcs_paymentType { get; set; }
		[DataMember]
		public string mcs_paymentTypeCode { get; set; }
		[DataMember]
		public string mcs_programType { get; set; }
		[DataMember]
		public string mcs_recipientName { get; set; }
		[DataMember]
		public Int64 mcs_recipientParticipantID { get; set; }
		[DataMember]
		public bool mcs_recipientParticipantIDSpecified { get; set; }
		[DataMember]
		public DateTime mcs_scheduledDate { get; set; }
		[DataMember]
		public bool mcs_scheduledDateSpecified { get; set; }
		[DataMember]
		public string mcs_veteranName { get; set; }
		[DataMember]
		public Int64 mcs_veteranParticipantID { get; set; }
		[DataMember]
		public bool mcs_veteranParticipantIDSpecified { get; set; }
		[DataMember]
		public VEISrtrpmtsumaddressEFT VEISrtrpmtsumaddressEFTInfo { get; set; }
		[DataMember]
		public VEISrtrpmtsumcheckAddress VEISrtrpmtsumcheckAddressInfo { get; set; }
		[DataMember]
		public VEISrtrpmtsumpaymentRecordIdentifier VEISrtrpmtsumpaymentRecordIdentifierInfo { get; set; }
		[DataMember]
		public VEISrtrpmtsumreturnPayment VEISrtrpmtsumreturnPaymentInfo { get; set; }
	}
	[DataContract]
	public class VEISrtrpmtsumaddressEFT
	{
		[DataMember]
		public string mcs_accountNumber { get; set; }
		[DataMember]
		public string mcs_accountType { get; set; }
		[DataMember]
		public string mcs_bankName { get; set; }
		[DataMember]
		public string mcs_routingNumber { get; set; }
	}
	[DataContract]
	public class VEISrtrpmtsumcheckAddress
	{
		[DataMember]
		public string mcs_addressLine1 { get; set; }
		[DataMember]
		public string mcs_addressLine2 { get; set; }
		[DataMember]
		public string mcs_addressLine3 { get; set; }
		[DataMember]
		public string mcs_addressLine4 { get; set; }
		[DataMember]
		public string mcs_addressLine5 { get; set; }
		[DataMember]
		public string mcs_addressLine6 { get; set; }
		[DataMember]
		public string mcs_addressLine7 { get; set; }
		[DataMember]
		public string mcs_zipCode { get; set; }
	}
	[DataContract]
	public class VEISrtrpmtsumpaymentRecordIdentifier
	{
		[DataMember]
		public Int64 mcs_paymentID { get; set; }
		[DataMember]
		public bool mcs_paymentIDSpecified { get; set; }
		[DataMember]
		public Int64 mcs_transactionID { get; set; }
		[DataMember]
		public bool mcs_transactionIDSpecified { get; set; }
	}
	[DataContract]
	public class VEISrtrpmtsumreturnPayment
	{
		[DataMember]
		public string mcs_checkTraceNumber { get; set; }
		[DataMember]
		public DateTime mcs_returnDate { get; set; }
		[DataMember]
		public bool mcs_returnDateSpecified { get; set; }
		[DataMember]
		public string mcs_returnReason { get; set; }
	}
	[DataContract]
	public class VEISrtrpmtsumresponse
	{
		[DataMember]
		public Int32 mcs_responseCode { get; set; }
		[DataMember]
		public bool mcs_responseCodeSpecified { get; set; }
		[DataMember]
		public Byte[] mcs_responseDebug { get; set; }
		[DataMember]
		public string mcs_responseText { get; set; }
	}
}
