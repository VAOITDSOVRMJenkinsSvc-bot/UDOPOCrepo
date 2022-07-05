using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.VeteranWebService.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.VeteranWebService,findVeteranByFileNumber method, Request.
	/// Code Generated by IMS on: 12/12/2018 4:37:38 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISvetFNfindVeteranByFileNumberRequest : VEISEcRequestBase
	{
		private string crme_SSN;

		public VEISvetFNfindVeteranByFileNumberRequest(string crme_SSN)
		{
			this.crme_SSN = crme_SSN;
		}

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
		public string mcs_filenumber { get; set; }
	}
}