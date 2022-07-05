using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.FMCService.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.FMCService,GetPaymentHistoryDetails method, Response.
	/// Code Generated by IMS on: 1/7/2019 8:06:00 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISFMCgphdGetPaymentHistoryDetailsResponse : VEISEcResponseBase
	{
		[DataMember]
		public VEISFMCgphdPaymentHistoryMultipleResponse[] VEISFMCgphdPaymentHistoryInfo { get; set; }
	}
	[DataContract]
	public class VEISFMCgphdPaymentHistoryMultipleResponse
	{
		[DataMember]
		public Int32? mcs_PaymentHistoryId { get; set; }
		[DataMember]
		public string mcs_VendorId { get; set; }
		[DataMember]
		public string mcs_Sta { get; set; }
		[DataMember]
		public string mcs_Pat { get; set; }
		[DataMember]
		public string mcs_sp { get; set; }
		[DataMember]
		public string mcs_tc { get; set; }
		[DataMember]
		public string mcs_InvoiceNum { get; set; }
		[DataMember]
		public string mcs_Description { get; set; }
		[DataMember]
		public string mcs_Line { get; set; }
		[DataMember]
		public Decimal? mcs_Amount { get; set; }
		[DataMember]
		public bool mcs_AmountSpecified { get; set; }
		[DataMember]
		public string mcs_PCode { get; set; }
		[DataMember]
		public string mcs_ICode { get; set; }
		[DataMember]
		public string mcs_LCode { get; set; }
		[DataMember]
		public string mcs_TSCLDate { get; set; }
		[DataMember]
		public string mcs_Sched { get; set; }
		[DataMember]
		public string mcs_AccDelDte { get; set; }
		[DataMember]
		public string mcs_boc { get; set; }
		[DataMember]
		public string mcs_RefDocId { get; set; }
		[DataMember]
		public string mcs_dln { get; set; }
		[DataMember]
		public string mcs_SSta { get; set; }
		[DataMember]
		public string mcs_InvDate { get; set; }
		[DataMember]
		public string mcs_LogDate { get; set; }
		[DataMember]
		public string mcs_SchCat { get; set; }
		[DataMember]
		public string mcs_SchType { get; set; }
		[DataMember]
		public string mcs_Name { get; set; }
		[DataMember]
		public string mcs_pptype { get; set; }
		[DataMember]
		public string mcs_AccSta { get; set; }
		[DataMember]
		public string mcs_bfys { get; set; }
		[DataMember]
		public string mcs_Fund { get; set; }
		[DataMember]
		public string mcs_fcp { get; set; }
		[DataMember]
		public string mcs_cc { get; set; }
		[DataMember]
		public string mcs_pct { get; set; }
		[DataMember]
		public string mcs_Days { get; set; }
		[DataMember]
		public string mcs_VetId { get; set; }
		[DataMember]
		public string mcs_VetName { get; set; }
		[DataMember]
		public string mcs_VetIntl { get; set; }
		[DataMember]
		public string mcs_SvcDate { get; set; }
		[DataMember]
		public string mcs_Address { get; set; }
		[DataMember]
		public string mcs_ao { get; set; }
		[DataMember]
		public string mcs_LType { get; set; }
		[DataMember]
		public string mcs_CheckNumber { get; set; }
		[DataMember]
		public DateTime? mcs_CheckNumDate { get; set; }
		[DataMember]
		public bool mcs_CheckNumDateSpecified { get; set; }
		[DataMember]
		public string mcs_Sat { get; set; }
		[DataMember]
		public string mcs_tt { get; set; }
		[DataMember]
		public string mcs_po { get; set; }
		[DataMember]
		public string mcs_OverRideMsg { get; set; }
		[DataMember]
		public bool mcs_Tchk { get; set; }
		[DataMember]
		public bool mcs_TchkSpecified { get; set; }
		[DataMember]
		public string mcs_rcdln { get; set; }
		[DataMember]
		public string mcs_VoucherDate { get; set; }
		[DataMember]
		public string mcs_cxckRc { get; set; }
		[DataMember]
		public string mcs_DateCKCX { get; set; }
		[DataMember]
		public string mcs_UserID { get; set; }
		[DataMember]
		public string mcs_BeginDate { get; set; }
		[DataMember]
		public string mcs_EndDate { get; set; }
		[DataMember]
		public string mcs_FMSTSCLFileDate { get; set; }
		[DataMember]
		public string mcs_FMSChkCfrmCreationDate { get; set; }
		[DataMember]
		public string mcs_FMSChkCfrmProcessDate { get; set; }
		[DataMember]
		public string mcs_FMSChkCncCfrmDate { get; set; }
		[DataMember]
		public string mcs_FMSChkCncCode { get; set; }
		[DataMember]
		public string mcs_FMSChkCncRsnCode { get; set; }
		[DataMember]
		public string mcs_FMSChkCncCreationDate { get; set; }
		[DataMember]
		public string mcs_FMSChkCncProcessDate { get; set; }
		[DataMember]
		public Int32? mcs_FiscalYear { get; set; }
		[DataMember]
		public bool mcs_FiscalYearSpecified { get; set; }
		[DataMember]
		public DateTime? mcs_CyTSCLDATE { get; set; }
		[DataMember]
		public bool mcs_CyTSCLDATESpecified { get; set; }
		[DataMember]
		public string mcs_PoNum { get; set; }
		[DataMember]
		public Int32? mcs_ind { get; set; }
		[DataMember]
		public bool mcs_indSpecified { get; set; }
		[DataMember]
		public DateTime? mcs_ImportDate { get; set; }
		[DataMember]
		public bool mcs_ImportDateSpecified { get; set; }
		[DataMember]
		public DateTime? mcs_ModifiedDate { get; set; }
		[DataMember]
		public bool mcs_ModifiedDateSpecified { get; set; }
	}
}
