using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.CpeForeignMedicalProgramAdapterServiceV3.CRMOL.Messages
{ 
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.CpeForeignMedicalProgramAdapterServiceV3.CRMOL,getFMPPPR method, Request.
	/// Code Generated by IMS on: 6/1/2018 11:16:16 AM
	/// Version: 2018.05.09.05
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISgFMPPPPRgetFMPPPRRequest : VEISEcRequestBase
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
		public VEISgFMPPPPRReqCPEForeignMedicalProgramAdapterv3getFMPPPRRequest VEISgFMPPPPRReqCPEForeignMedicalProgramAdapterv3getFMPPPRRequestInfo { get; set; }
	}
	[DataContract]
	public class VEISgFMPPPPRReqCPEForeignMedicalProgramAdapterv3getFMPPPRRequest
	{
		[DataMember]
		public string mcs_claimNumber { get; set; }
	}
}
