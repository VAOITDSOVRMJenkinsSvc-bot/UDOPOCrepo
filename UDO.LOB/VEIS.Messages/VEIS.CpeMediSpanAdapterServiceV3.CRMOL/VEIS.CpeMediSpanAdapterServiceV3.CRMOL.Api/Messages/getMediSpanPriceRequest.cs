using System; 
using System.Runtime.Serialization;
using VEIS.Core.Messages;
using VEIS.CpeMediSpanAdapterServiceV3.CRMOL.Messages; 

namespace VEIS.CpeMediSpanAdapterServiceV3.CRMOL.Messages
{ 
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.CpeMediSpanAdapterServiceV3.CRMOL,getMediSpanPrice method, Request.
	/// Code Generated by IMS on: 5/29/2018 2:54:07 PM
	/// Version: 2018.05.09.05
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISgMSPgetMediSpanPriceRequest : VEISEcRequestBase
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
		public string mcs_transactionid { get; set; }
		[DataMember]
		public VEISgMSPReqCPEMediSpanAdapterv3getMediSpanPriceRequest VEISgMSPReqCPEMediSpanAdapterv3getMediSpanPriceRequestInfo { get; set; }
	}
	[DataContract]
	public class VEISgMSPReqCPEMediSpanAdapterv3getMediSpanPriceRequest
	{
		[DataMember]
		public DateTime mcs_date { get; set; }
		[DataMember]
		public string mcs_vaDrugIen { get; set; }
		[DataMember]
		public string mcs_quantity { get; set; }
	}
}
