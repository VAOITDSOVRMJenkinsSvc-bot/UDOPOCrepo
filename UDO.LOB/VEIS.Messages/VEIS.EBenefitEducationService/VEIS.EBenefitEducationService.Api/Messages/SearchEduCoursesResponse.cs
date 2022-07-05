using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.EBenefitEducationService.Api.Messages
{ 
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.EBenefitEducationService.Api,SearchEduCourses method, Response. 
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISsrcheducrsSearchEduCoursesResponse : VEISEcResponseBase
    {
		[DataMember]
		public VEISsrcheducrstrainingTypeMultipleResponse[] VEISsrcheducrstrainingTypeInfo { get; set; }
		[DataMember]
		public bool ExceptionOccured { get; set; }
		[DataMember]
		public string ExceptionMessage { get; set; }
	}
	[DataContract]
	public class VEISsrcheducrstrainingTypeMultipleResponse
	{
	}
}
