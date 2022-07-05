using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;


namespace VEIS.EBenefitEducationService.Api.Messages
{ 
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.EBenefitEducationService.Api,GetEduInstituteDetail method, Request. 
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISgteduinstdtlGetEduInstituteDetailRequest : VEISEcRequestBase
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
		public string mcs_fullFacilityCode { get; set; }
	}
}
