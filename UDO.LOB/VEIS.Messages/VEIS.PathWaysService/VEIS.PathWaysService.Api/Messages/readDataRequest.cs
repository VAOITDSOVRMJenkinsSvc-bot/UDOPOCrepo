using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.PathWaysService.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.PathWaysService,readData method, Request.
	/// Code Generated by IMS on: 12/17/2018 2:40:39 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISpwsreadDataRequest : VEISEcRequestBase
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
		public string mcs_in0 { get; set; }
		[DataMember]
		public string mcs_in1 { get; set; }
		[DataMember]
		public string mcs_in2 { get; set; }
		[DataMember]
		public string mcs_in3 { get; set; }
		[DataMember]
		public VEISpwsReqreadDataResponse VEISpwsReqreadDataResponseInfo { get; set; }
	}
	[DataContract]
	public class VEISpwsReqreadDataResponse
	{
		[DataMember]
		public string mcs_stringResponse { get; set; }
	}
}