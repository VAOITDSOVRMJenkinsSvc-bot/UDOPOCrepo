using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.EBenefitEducationService.Api.Messages
{ 
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.EBenefitEducationService.Api,SearchEduCourses method, Request. 
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISsrcheducrsSearchEduCoursesRequest : VEISEcRequestBase
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
		public VEISsrcheducrssearcheducoursessearchstring searcheducoursessearchstringInfo { get; set; }
		[DataMember]
		public VEISsrcheducrstrainingtype trainingtypeInfo { get; set; }
		[DataMember]
		public string mcs_coursecode { get; set; }
		[DataMember]
		public string mcs_courseid { get; set; }
	}
	public class VEISsrcheducrssearcheducoursessearchstring
	{
		[DataMember]
		public string mcs_facilityCode { get; set; }
	}
	public class VEISsrcheducrstrainingtype
	{
	}
}
