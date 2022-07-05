using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.AddressWebService.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.AddressWebService,findPtcpntAddrs method, Request.
	/// Code Generated by IMS on: 1/3/2019 4:25:07 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISfpidaddfindPtcpntAddrsRequest : VEISEcRequestBase
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
		public Int64 mcs_ptcpntid { get; set; }
		[DataMember]
		public string mcs_ptcpntaddrstypenm { get; set; }
	}
}