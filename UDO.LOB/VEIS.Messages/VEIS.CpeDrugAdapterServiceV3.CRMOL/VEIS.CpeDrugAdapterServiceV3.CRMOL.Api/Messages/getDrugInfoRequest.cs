using System; 
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.CpeDrugAdapterServiceV3.CRMOL.Messages
{ 
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.CpeDrugAdapterServiceV3.CRMOL,getDrugInfo method, Request.
	/// Code Generated by IMS on: 5/31/2018 2:05:54 PM
	/// Version: 2018.05.09.05
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISgDIgetDrugInfoRequest : VEISEcRequestBase
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
		public VEISgDIReqCPEDrugAdapterv3getDrugInfoRequest VEISgDIReqCPEDrugAdapterv3getDrugInfoRequestInfo { get; set; }
	}
	[DataContract]
	public class VEISgDIReqCPEDrugAdapterv3getDrugInfoRequest
	{
		[DataMember]
		public string mcs_vaDrugIen { get; set; }
	}
}
