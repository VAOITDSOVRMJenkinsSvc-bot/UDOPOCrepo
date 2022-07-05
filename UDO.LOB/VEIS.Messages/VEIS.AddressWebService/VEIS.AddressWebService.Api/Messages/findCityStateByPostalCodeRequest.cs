using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.AddressWebService.Messages
{
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.AddressWebService,findCityStateByPostalCode method, Request.
	/// Code Generated by IMS on: 1/3/2019 4:21:06 PM
	/// Version: 2018.12.11.01
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISfcitystbpostfindCityStateByPostalCodeRequest : VEISEcRequestBase
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
		public string mcs_postalcode { get; set; }
	}
}