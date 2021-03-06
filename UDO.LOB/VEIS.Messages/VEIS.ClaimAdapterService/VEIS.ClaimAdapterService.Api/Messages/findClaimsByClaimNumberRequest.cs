using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.ClaimAdapterService.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.ClaimAdapterService,findClaimsByClaimNumber method, Request.
	/// Code Generated by IMS on: 1/11/2019 8:40:48 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISclmbyclnumfindClaimsByClaimNumberRequest : VEISEcRequestBase
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
		public VEISclmbyclnumReqCPEClaimAdapterv2findClaimsByClaimNumberRequest1 VEISclmbyclnumReqCPEClaimAdapterv2findClaimsByClaimNumberRequest1Info { get; set; }
	}
	[DataContract]
	public class VEISclmbyclnumReqCPEClaimAdapterv2findClaimsByClaimNumberRequest1
	{
		[DataMember]
		public string mcs_claimNumber { get; set; }
		[DataMember]
		public VIMTclmbyclnumReqresponseFilter VIMTclmbyclnumReqresponseFilterInfo { get; set; }
	}
	[DataContract]
	public class VIMTclmbyclnumReqresponseFilter
	{
		[DataMember]
		public DateTime mcs_startDate { get; set; }
		[DataMember]
		public bool mcs_startDateSpecified { get; set; }
		[DataMember]
		public DateTime mcs_endDate { get; set; }
		[DataMember]
		public bool mcs_endDateSpecified { get; set; }
		[DataMember]
		public Int32 mcs_chunkSize { get; set; }
		[DataMember]
		public bool mcs_chunkSizeSpecified { get; set; }
		[DataMember]
		public Int32 mcs_chunkNumber { get; set; }
		[DataMember]
		public bool mcs_chunkNumberSpecified { get; set; }
	}
}
