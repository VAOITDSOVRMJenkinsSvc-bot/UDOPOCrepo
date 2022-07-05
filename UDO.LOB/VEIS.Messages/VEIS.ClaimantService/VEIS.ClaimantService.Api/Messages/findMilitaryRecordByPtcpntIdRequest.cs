using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.ClaimantService.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.ClaimantService,findMilitaryRecordByPtcpntId method, Request.
	/// Code Generated by IMS on: 12/19/2018 9:47:50 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISfmirecpicfindMilitaryRecordByPtcpntIdRequest : VEISEcRequestBase
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
		public string mcs_ptcpntid { get; set; }
	}
}
