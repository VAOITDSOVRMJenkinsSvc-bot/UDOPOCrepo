using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.VBMSWebService.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.VBMSWebService,getDocumentContent method, Request.
	/// Code Generated by IMS on: 1/11/2019 11:07:27 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISVBMSgetDocgetDocumentContentRequest : VEISEcRequestBase
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
		public string mcs_documentversionrefid { get; set; }
	}
}
