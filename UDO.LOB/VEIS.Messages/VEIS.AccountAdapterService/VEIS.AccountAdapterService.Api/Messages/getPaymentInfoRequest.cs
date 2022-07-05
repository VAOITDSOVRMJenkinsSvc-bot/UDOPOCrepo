using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.AccountAdapterService.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.AccountAdapterService,getPaymentInfo method, Request.
	/// Code Generated by IMS on: 1/11/2019 10:16:24 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISgpinfogetPaymentInfoRequest : VEISEcRequestBase
	{
		[DataMember]
		public Guid RelatedParentId { get; set; }
		[DataMember]
		public string RelatedParentEntityName { get; set; }
		[DataMember]
		public string RelatedParentFieldName { get; set; }
		[DataMember]
		public bool LogTiming { get; set; }
		[DataMember]
		public bool LogSoap { get; set; }
		[DataMember]
		public bool Debug { get; set; }
		[DataMember]
		public LegacyHeaderInfo LegacyServiceHeaderInfo { get; set; }
		[DataMember]
		public VEISgpinfoReqCPEAccountAdapterv2getPaymentInfoRequest1 VEISgpinfoReqCPEAccountAdapterv2getPaymentInfoRequest1Info { get; set; }
	}
	[DataContract]
	public class VEISgpinfoReqCPEAccountAdapterv2getPaymentInfoRequest1
	{
		[DataMember]
		public string mcs_fmsDocumentID { get; set; }
		[DataMember]
		public string mcs_checkNumber { get; set; }
		[DataMember]
		public string mcs_claimNumber { get; set; }
	}
}
