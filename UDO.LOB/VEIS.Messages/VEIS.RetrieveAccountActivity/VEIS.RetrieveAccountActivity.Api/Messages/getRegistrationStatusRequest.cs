using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.RetrieveAccountActivity.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.RetrieveAccountActivity,getRegistrationStatus method, Request.
	/// Code Generated by IMS on: 1/11/2019 10:57:17 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISgetRegStatgetRegistrationStatusRequest : VEISEcRequestBase
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
		public string mcs_edipi { get; set; }
	}
}
