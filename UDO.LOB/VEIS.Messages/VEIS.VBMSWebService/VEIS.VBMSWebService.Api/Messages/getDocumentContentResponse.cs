using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.VBMSWebService.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.VBMSWebService,getDocumentContent method, Response.
	/// Code Generated by IMS on: 1/11/2019 11:07:27 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISVBMSgetDocgetDocumentContentResponse : VEISEcResponseBase
	{
		[DataMember]
		public VEISVBMSgetDocDocumentContent VEISVBMSgetDocDocumentContentInfo { get; set; }
	}
	[DataContract]
	public class VEISVBMSgetDocDocumentContent
	{
		[DataMember]
		public Byte[] mcs_bytes { get; set; }
		[DataMember]
		public string mcs_documentVersionReferenceId { get; set; }
	}
}