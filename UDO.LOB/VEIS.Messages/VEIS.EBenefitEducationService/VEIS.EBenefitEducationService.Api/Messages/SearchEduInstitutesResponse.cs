using System;
using System.Runtime.Serialization;
using VEIS.Core.Messages;

namespace VEIS.EBenefitEducationService.Api.Messages
{ 
	[DataContract]
	/// <summary>
	/// VEIS Enterprise Component for VEIS.EBenefitEducationService.Api,SearchEduInstitutes method, Response. 
	/// </summary>
	/// <param name=none></param>
	/// <returns>none</returns>
	public class VEISsrcheduinstSearchEduInstitutesResponse : VEISEcResponseBase
    {
		[DataMember]
		public VEISsrcheduinststatusMultipleResponse[] VEISsrcheduinststatusInfo { get; set; }
		[DataMember]
		public bool ExceptionOccured { get; set; }
		[DataMember]
		public string ExceptionMessage { get; set; }
	}
	[DataContract]
	public class VEISsrcheduinststatusMultipleResponse
	{
		[DataMember]
		public Int64 mcs_participantID { get; set; }
		[DataMember]
		public string mcs_instituteName { get; set; }
		[DataMember]
		public string mcs_facilityCode { get; set; }
		[DataMember]
		public DateTime mcs_statusDate { get; set; }
	}
}
