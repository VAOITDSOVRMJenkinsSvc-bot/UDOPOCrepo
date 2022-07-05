using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.PaymentInformationService.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.PaymentInformationService,retrievePaymentDetail method, Response.
	/// Code Generated by IMS on: 12/27/2018 4:55:08 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISrtrpmtdtlretrievePaymentDetailResponse : VEISEcResponseBase
	{
		[DataMember]
		public VEISrtrpmtdtlpaymentDetailResponseVO VEISrtrpmtdtlpaymentDetailResponseVOInfo { get; set; }
	}
	[DataContract]
	public class VEISrtrpmtdtlpaymentDetailResponseVO
	{
		[DataMember]
		public VEISrtrpmtdtlawardAdjustments VEISrtrpmtdtlawardAdjustmentsInfo { get; set; }
		[DataMember]
		public VEISrtrpmtdtlpaymentAdjustments VEISrtrpmtdtlpaymentAdjustmentsInfo { get; set; }
		[DataMember]
		public VEISrtrpmtdtlpaymentIdentifier VEISrtrpmtdtlpaymentIdentifierInfo { get; set; }
		[DataMember]
		public VEISrtrpmtdtlresponse VEISrtrpmtdtlresponseInfo { get; set; }
	}
	[DataContract]
	public class VEISrtrpmtdtlawardAdjustments
	{
		[DataMember]
		public DateTime mcs_awardEffectiveDate { get; set; }
		[DataMember]
		public bool mcs_awardEffectiveDateSpecified { get; set; }
		[DataMember]
		public Decimal mcs_grossAwardAmount { get; set; }
		[DataMember]
		public bool mcs_grossAwardAmountSpecified { get; set; }
		[DataMember]
		public Decimal mcs_netAwardAmount { get; set; }
		[DataMember]
		public bool mcs_netAwardAmountSpecified { get; set; }
		[DataMember]
		public VEISrtrpmtdtlawardAdjustmentListMultipleResponse[] VEISrtrpmtdtlawardAdjustmentListInfo { get; set; }
		[DataMember]
		public VEISrtrpmtdtlawardReasonListMultipleResponse[] VEISrtrpmtdtlawardReasonListInfo { get; set; }
	}
	[DataContract]
	public class VEISrtrpmtdtlawardAdjustmentListMultipleResponse
	{
		[DataMember]
		public Decimal mcs_adjustmentAmount { get; set; }
		[DataMember]
		public bool mcs_adjustmentAmountSpecified { get; set; }
		[DataMember]
		public DateTime mcs_adjustmentEffectiveDate { get; set; }
		[DataMember]
		public bool mcs_adjustmentEffectiveDateSpecified { get; set; }
		[DataMember]
		public string mcs_adjustmentOperation { get; set; }
		[DataMember]
		public string mcs_adjustmentType { get; set; }
		[DataMember]
		public string mcs_alloteeRelationship { get; set; }
		[DataMember]
		public string mcs_allotmentRecipientName { get; set; }
		[DataMember]
		public string mcs_allotmentType { get; set; }
	}
	[DataContract]
	public class VEISrtrpmtdtlawardReasonListMultipleResponse
	{
		[DataMember]
		public string mcs_awardReasonText { get; set; }
	}
	[DataContract]
	public class VEISrtrpmtdtlpaymentAdjustments
	{
		[DataMember]
		public Decimal mcs_grossPaymentAmount { get; set; }
		[DataMember]
		public bool mcs_grossPaymentAmountSpecified { get; set; }
		[DataMember]
		public Decimal mcs_netPaymentAmount { get; set; }
		[DataMember]
		public bool mcs_netPaymentAmountSpecified { get; set; }
		[DataMember]
		public VEISrtrpmtdtlpaymentAdjustmentListMultipleResponse[] VEISrtrpmtdtlpaymentAdjustmentListInfo { get; set; }
	}
	[DataContract]
	public class VEISrtrpmtdtlpaymentAdjustmentListMultipleResponse
	{
		[DataMember]
		public Decimal mcs_adjustmentAmount { get; set; }
		[DataMember]
		public bool mcs_adjustmentAmountSpecified { get; set; }
		[DataMember]
		public string mcs_adjustmentCategory { get; set; }
		[DataMember]
		public bool mcs_adjustmentDMCIndicator { get; set; }
		[DataMember]
		public string mcs_adjustmentOperation { get; set; }
		[DataMember]
		public string mcs_adjustmentReason { get; set; }
		[DataMember]
		public string mcs_adjustmentSource { get; set; }
		[DataMember]
		public string mcs_adjustmentType { get; set; }
	}
	[DataContract]
	public class VEISrtrpmtdtlpaymentIdentifier
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
	public class VEISrtrpmtdtlresponse
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